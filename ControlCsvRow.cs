using System.Globalization;
using System.Numerics;
using CsvHelper.Configuration.Attributes;

namespace calc
{
	public class ControlCsvRow
	{
        [Name("File")]
        public string File { get; set; }

        [Name("Type")]
        public string Type { get; set; }

        [Name("Type2")]
        public string Type2 { get; set; }

        [Name("Filename")]
        public string Filename { get; set; }

        [Name("TexSize")]
        public string TexSize { get; set; }
        public Vector2 GetTexSize { get { return parseVec2(TexSize); } }

        [Name("Tiling")]
        public string Tiling { get; set; }
        public Vector2 GetTiling { get { return parseVec2(Tiling); } }

        [Name("Modulation")]
        public string Modulation { get; set; }
        public Vector3 GetModulation { get { return parseVec3(Modulation); } }

        [Name("Stretch")]
        public bool Stretch { get; set; }

        [Name("Object")]
        public string Object { get; set; }

        [Name("ObjectMod")]
        public string ObjectMod { get; set; }
        public Vector2 GetObjectMod { get { return parseVec2(ObjectMod); } }

        [Name("ViewPoint")]
        public string ViewPoint { get; set; }
        public Vector3 GetViewPoint { get { return parseVec3(ViewPoint); } }

        [Name("LookAt")]
        public string LookAt { get; set; }
        public Vector3 GetLookAt { get { return parseVec3(LookAt); } }

        [Name("ViewUp")]
        public string ViewUp { get; set; }
        public Vector3 GetViewUp { get { return parseVec3(ViewUp); } }

        [Name("Repeat")]
        public string Repeat { get; set; }
        public Vector3 GetRepeat { get { return parseVec3(Repeat); } }

        [Name("FlipGradient")]
        public bool FlipGradient { get; set; }

        [Name("RenderDistance")]
        public string RenderDistance { get; set; }
        public float GetRenderDistance { get { return float.Parse(RenderDistance, CultureInfo.InvariantCulture); } }

        private Vector2 parseVec2(string src)
        {
            string[] sArray = src.Split(',');

            if (sArray.Length != 2)
                return new Vector2(0, 0);

            Vector2 result = new Vector2(
                float.Parse(sArray[0].Trim(), CultureInfo.InvariantCulture),
                float.Parse(sArray[1].Trim(), CultureInfo.InvariantCulture));

            return result;
        }

        private Vector3 parseVec3(string src)
        {
            string[] sArray = src.Split(',');

            if (sArray.Length != 3)
                return new Vector3(0, 0, 0);

            Vector3 result = new Vector3(
                float.Parse(sArray[0].Trim(), CultureInfo.InvariantCulture),
                float.Parse(sArray[1].Trim(), CultureInfo.InvariantCulture),
                float.Parse(sArray[2].Trim(), CultureInfo.InvariantCulture));

            return result;
        }
    }
}

