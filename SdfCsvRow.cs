using System.Globalization;
using System.Numerics;
using CsvHelper.Configuration.Attributes;

namespace calc
{
	public class SdfCsvRow
	{
		[Name("Shape")]
        public string Shape { get; set; }

        [Name("Dimensions")]
        public string Dimensions { get; set; }
        public Vector3 GetDimensions { get { return parseVec3(Dimensions); } }

        [Name("Translate")]
        public string Translate { get; set; }
        public Vector3 GetTranslate { get { return parseVec3(Translate); } }

        [Name("Rotate")]
        public string Rotate { get; set; }
        public Vector3 GetRotate { get { return parseVec3(Rotate); } }

        [Name("Scale")]
        public string Scale { get; set; }
        public Vector3 GetScale { get { return parseVec3(Scale); } }

		[Name("Boolean")]
        public string Boolean { get; set; }

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