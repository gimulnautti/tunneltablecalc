using System;
using System.Numerics;
namespace calc
{
	public class TunnelRenderer
	{
        public static Vector2 mapTunnel(Vector2 uv, float aspect, Vector2 tiling, Vector3 mod, bool stretch)
        {
            uv.X *= aspect;

            Vector2 up = new Vector2(0, 1);
            Vector2 right = new Vector2(1, 0);
            Vector2 middle = new Vector2(0.5f * aspect, 0.5f);
            Vector2 toFrag = uv - middle;

            // angle + log distance from middle
            float angle = MathF.Acos(Vector2.Dot(toFrag, up) / toFrag.Length());
            float dist = MathF.Log(toFrag.Length());

            // have two sides to it
            if (Vector2.Dot(toFrag, right) > 0.0f)
            {
                angle = -angle;
            }

            // stretch
            if (stretch)
            {
                dist += MathF.Cos(dist * mod.Y + angle * mod.X) * mod.Z;
            }

            // resolve texture UV
            Vector2 texUv = new Vector2(angle / 3.141519f, 1.0f - dist);

            // tiling
            texUv.X *= tiling.X;
            texUv.Y *= tiling.Y;

            // strech pickup
            //texUv.Y += 1.5f * MathF.Cos(dist);

            // repeat
            texUv.X %= 1.0f;
            texUv.Y %= 1.0f;

            return texUv;
        }

        public static int mapColor(Vector2 uv, float aspect, Vector3 mod)
        {
            uv.X *= aspect;

            Vector2 up = new Vector2(0, 1);
            Vector2 right = new Vector2(1, 0);
            Vector2 middle = new Vector2(0.5f * aspect, 0.5f);
            Vector2 toFrag = uv - middle;

            // angle + log distance from middle
            float angle = MathF.Acos(Vector2.Dot(toFrag, up) / toFrag.Length());
            float dist = MathF.Log(toFrag.Length());

            // have two sides to it
            if (Vector2.Dot(toFrag, right) > 0.0)
            {
                angle = -angle;
            }

            // resolve color 
            int gradIdx = 7 - (int)MathF.Max(0, MathF.Min(7, mod.Z * ((1.0f / (toFrag.Length())) + mod.Y * (MathF.Cos(angle * mod.X + 1.57f) + 1.0f))));

            return gradIdx;
        }
    }
}

