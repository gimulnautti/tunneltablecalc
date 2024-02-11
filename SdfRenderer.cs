using System;
using System.Numerics;
using System.Threading.Channels;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using CsvHelper;

namespace calc
{
	public struct SdfFragment
	{
		public Vector2 UV;
		public int Gradient;
	}

	public class SdfRenderer
	{
		private string ObjectType;
        private string ObjectFileName;
		private Vector2 Tiling;
        private Vector2 ObjMod;
        private Vector3 ViewPoint;
        private Vector3 LookAt;
        private Vector3 ViewUp;
        private Vector3 Repeat;
        private bool FlipGradient;
        private float RenderDistance;
        private List<SdfCsvRow> CustomObjectRows;

        public SdfRenderer(string objectType, string objectFileName, Vector2 tiling, Vector2 objMod, Vector3 viewPoint, Vector3 lookAt, Vector3 viewUp, Vector3 repeat, float renderDistance, bool flipGradient)
		{
			ObjectType = objectType;
            ObjectFileName = objectFileName;
			Tiling = tiling;
			ObjMod = objMod;
			ViewPoint = viewPoint;
			LookAt = lookAt;
			ViewUp = viewUp;
			Repeat = repeat;
            RenderDistance = renderDistance;
            FlipGradient = flipGradient;

            if (objectFileName != string.Empty)
            {
                using (var reader = new StreamReader(ObjectFileName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    CustomObjectRows = new List<SdfCsvRow>(csv.GetRecords<SdfCsvRow>());
                }
            }
		}

        static Vector2 vecXZ(Vector3 p)
        {
            return new Vector2(p.X, p.Z);
        }

        static Vector3 vecRound(Vector3 p)
        {
            return new Vector3(MathF.Round(p.X), MathF.Round(p.Y), MathF.Round(p.Z));
        }

        Vector2 polarCoordinates(in Vector3 p, in Vector3 up, in Vector3 right, in Vector3 fwd)
        {
            Vector3 rfPlane = new Vector3(p.X, 0.0f, p.Z);
            Vector3 n = Vector3.Normalize(rfPlane);

            float dotnright = Vector3.Dot(n, right);
            float dotnfwd = Vector3.Dot(n, fwd);

            float angle = MathF.Acos(dotnright);
            if (dotnfwd < 0.0) angle = 6.28f - angle;
            return new Vector2(angle / 6.28f, p.Y + 0.5f);
        }

        float sdTorus(in Vector3 p, in Vector2 t)
        {
            Vector2 q = new Vector2(vecXZ(p).Length() - t.X, p.Y);
            return q.Length() - t.Y;
        }

        float sdInvTorus(in Vector3 p, in Vector2 t)
        {
            Vector2 q = new Vector2(vecXZ(p).Length() - t.X, p.Y);
            return -(q.Length() - t.Y);
        }

        Vector2 sdTorusMapped(in Vector3 p, in Vector2 t)
        {
            Vector2 q = new Vector2(vecXZ(p).Length() - t.X, p.Y);

            Vector3 right = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 fwd = new Vector3(0.0f, 0.0f, 1.0f);

            return new Vector2(polarCoordinates(p, up, right, fwd).X, polarCoordinates(new Vector3(q.X, 0.0f, q.Y), up, right, fwd).X);
        }

        float sdSphere(in Vector3 p, in float s)
        {
            return p.Length() - s;
        }

        Vector2 sdSphereMapped(in Vector3 p, in float s)
        {
            return polarCoordinates(p, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
        }

        // TODO implement cylinder

        Vector3 repeat(in Vector3 pos, in Vector3 s)
        {
            return pos - s * vecRound(pos / s);
        }

        float sdOpUnion(in float d1, in float d2)
        {
            return MathF.Min(d1, d2);
        }

        float opSubtraction(in float d1, in float d2)
        {
            return MathF.Max(d1,-d2);
        }

        float opIntersection(in float d1,in float d2)
        {
            return MathF.Max(d1,d2);
        }
        
        float opXor(in float d1, in float d2)
        {
            return MathF.Max(MathF.Min(d1,d2),-MathF.Max(d1,d2));
        }

        Vector3 transformToObjDomain(in Vector3 p, SdfCsvRow row)
        {
            Vector3 objDomainP = (p - row.GetTranslate);
            Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(-row.GetRotate.Y, -row.GetRotate.X, -row.GetRotate.Z);
            objDomainP = Vector3.Transform(objDomainP, rotation);
            objDomainP /= row.GetScale;
            return objDomainP;
        }

        float sdCustomObject(in Vector3 p)
        {
            float accumD = 100000000.0f;
            foreach (var row in CustomObjectRows)
            {
                Vector3 objDomainP = transformToObjDomain(p, row);
                Vector2 dimVec2 = new Vector2(row.GetDimensions.X, row.GetDimensions.Y);

                float objD = 100000000.0f;
                if (row.Shape == "invtorus")
                {
                    objD = sdInvTorus(objDomainP, dimVec2);
                }
                else if (row.Shape == "torus")
                {
                    objD = sdTorus(objDomainP, dimVec2);
                }
                else if (row.Shape == "sphere")
                {
                    objD = sdSphere(objDomainP, row.GetDimensions.X);
                }
                else if (row.Shape == "cylinder")
                {
                    // todo implement cylinder
                    objD = sdSphere(objDomainP, row.GetDimensions.X);
                }

                if (row.Boolean == "add")
                {
                    accumD = sdOpUnion(accumD, objD);
                }
                else if (row.Boolean == "subtract")
                {
                    accumD = opSubtraction(accumD, objD);
                }
                else if (row.Boolean == "intersect")
                {
                    accumD = opIntersection(accumD, objD);
                }
                else if (row.Boolean == "xor")
                {
                    accumD = opXor(accumD, objD);
                }
            }
            return accumD;
        }

        Vector2 sdCustomObjectTex(in Vector3 p)
        {
            float thresholdMapthis = 0.0001f;
            foreach (var row in CustomObjectRows)
            {
                Vector3 objDomainP = transformToObjDomain(p, row);
                Vector2 dimVec2 = new Vector2(row.GetDimensions.X, row.GetDimensions.Y);

                if (row.Shape == "invtorus")
                {
                    if (sdInvTorus(objDomainP, dimVec2) < thresholdMapthis)
                        return sdTorusMapped(objDomainP, dimVec2);
                }
                else if (row.Shape == "torus")
                {
                    if (sdTorus(objDomainP, dimVec2) < thresholdMapthis)
                        return sdTorusMapped(objDomainP, dimVec2);
                }
                else if (row.Shape == "sphere")
                {
                    if (sdSphere(objDomainP, row.GetDimensions.X) < thresholdMapthis)
                        return sdSphereMapped(objDomainP, row.GetDimensions.X);
                }
            }
            return new Vector2(0, 0);
        }

        float map(in Vector3 pos)
        {
            if (ObjectType == "invtorus")
            {
                if (Repeat.X > 0)
                    return sdInvTorus(repeat(pos, Repeat), ObjMod);
                else
                    return sdInvTorus(pos, ObjMod);
            }
            else if (ObjectType == "torus")
            {
                if (Repeat.X > 0)
                    return sdTorus(repeat(pos, Repeat), ObjMod);
                else
                    return sdTorus(pos, ObjMod);
            }
            else if (ObjectType == "sphere")
            {
                if (Repeat.X > 0)
                    return sdSphere(repeat(pos, Repeat), ObjMod.X);
                else
                    return sdSphere(pos, ObjMod.X);
            }
            else if (ObjectType == "custom")
            {
                if (Repeat.X > 0)
                    return sdCustomObject(repeat(pos, Repeat));
                else
                    return sdCustomObject(pos);
            }
            return 10000000000;
        }

        Vector2 texMap(in Vector3 pos)
        {
            if (ObjectType == "invtorus")
            {
                if (Repeat.X > 0)
                    return sdTorusMapped(repeat(pos, Repeat), ObjMod);
                else
                    return sdTorusMapped(pos, ObjMod);
            }
            else if (ObjectType == "torus")
            {
                if (Repeat.X > 0)
                    return sdTorusMapped(repeat(pos, Repeat), ObjMod);
                else
                    return sdTorusMapped(pos, ObjMod);
            }
            else if (ObjectType == "sphere")
            {
                if (Repeat.X > 0)
                    return sdSphereMapped(repeat(pos, Repeat), ObjMod.X);
                else
                    return sdSphereMapped(pos, ObjMod.X);
            }
            else if (ObjectType == "custom")
            {
                if (Repeat.X > 0)
                    return sdCustomObjectTex(repeat(pos, Repeat));
                else
                    return sdCustomObjectTex(pos);
            }
            return new Vector2(0,0);
        }

        // https://iquilezles.org/articles/normalsSDF
        Vector3 calcNormal(in Vector3 pos)
        {
            Vector2 e = new Vector2(1.0f, -1.0f) * 0.5773f;
            Vector3 eXYY = new Vector3(e.X, e.Y, e.Y);
            Vector3 eYYX = new Vector3(e.Y, e.Y, e.X);
            Vector3 eYXY = new Vector3(e.Y, e.X, e.Y);
            Vector3 eXXX = new Vector3(e.X, e.X, e.X);
            const float eps = 0.0005f;
            return Vector3.Normalize(eXYY * map(pos + eXYY * eps) +
                              eYYX * map(pos + eYYX * eps) +
                              eYXY * map(pos + eYXY * eps) +
                              eXXX * map(pos + eXXX * eps));
        }

        public SdfFragment RenderMapping(Vector2 resolution, Vector2 fragCoord)
		{
			SdfFragment result = new SdfFragment();

            // Camera matrix
            Vector3 ww = Vector3.Normalize(LookAt - ViewPoint);
            Vector3 uu = Vector3.Normalize(Vector3.Cross(ww, ViewUp));
            Vector3 vv = Vector3.Normalize(Vector3.Cross(uu, ww));

            // Fragment position
            Vector2 p = (-resolution + 2.0f * fragCoord) / resolution.Y;

            // View ray
            Vector3 rd = Vector3.Normalize(p.X * uu + p.Y * vv + 1.5f * ww);

            // Raymarch
            float t = 0.0f;
            for (int i = 0; i < 256; i++)
            {
                Vector3 pos = ViewPoint + t * rd;
                float h = map(pos);
                if (h < 0.0001f || t > RenderDistance)
                {
                    // texture lookup on hit
                    result.UV = texMap(pos);

                    // apply tiling
                    result.UV *= Tiling;
                    result.UV.X %= 1.0f;
                    result.UV.Y %= 1.0f;

                    // apply repeat
                    if (result.UV.X < 0)
                        result.UV.X += 1.0f;
                    if (result.UV.Y < 0)
                        result.UV.Y += 1.0f;

                    break;
                }
                t += h;
            }

            // Shading
            result.Gradient = 0;
            if (t < RenderDistance)
            {
                Vector3 pos = ViewPoint + t * rd;
                Vector3 nor = calcNormal(pos);
                float dif = 0.4f * float.Clamp(Vector3.Dot(nor, new Vector3(0.7f, 1.6f, 0.4f)), 0.0f, 1.0f);
                float amb = 0.4f * Vector3.Dot(nor, new Vector3(0.0f, 0.8f, 0.6f));
                float col = amb + dif;

                //col *= MathF.Sqrt(col); // Gamma
                col = float.Clamp(col, 0.0f, 1); // Clamp to positive range
                //col *= 1.0f - (t / RenderDistance); // Distance fade

                result.Gradient = (int)Math.Floor(col * 7.9999f); // 0 to 7 gradient levels

                if (FlipGradient)
                    result.Gradient = 7 - result.Gradient;
            }

            return result;
		}
    }
}

