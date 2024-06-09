using System;
using System.Numerics;
using CsvHelper;
using calc;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

class Program
{
    static void Main()
    {
        List<ControlCsvRow> records;

        using (var reader = new StreamReader("control.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = new List<ControlCsvRow>(csv.GetRecords<ControlCsvRow>());
        }

        foreach (var row in records)
        {
            if (row.Type == "mapping" && row.Type2 == "tunnel")
            {
                Vector2[] mapping = new Vector2[80 * 50];
                float aspect = 80.0f / 50.0f;

                for (int i = 0; i < 50; ++i)
                {
                    for (int j = 0; j < 80; ++j)
                    {
                        Vector2 uv = new Vector2((float)j / 80.0f, (float)i / 50.0f);
                        int index = i * 80 + j;
                        mapping[index] = TunnelRenderer.mapTunnel(uv, aspect, row.GetTiling, row.GetModulation, row.Stretch);
                    }
                }

                C64Writer.WriteMapping(row.File, mapping, row.GetTexSize);
            }
            else if (row.Type == "color" && row.Type2 == "tunnel")
            {
                int[] colorMap = new int[40 * 25];

                for (int i = 0; i < 25; ++i)
                {
                    for (int j = 0; j < 40; ++j)
                    {
                        float aspect = 80.0f / 50.0f;

                        Vector2 uv = new Vector2((float)j / 40.0f, (float)i / 25.0f);
                        int index = i * 40 + j;
                        colorMap[index] = TunnelRenderer.mapColor(uv, aspect, row.GetModulation, row.FlipGradient);
                    }
                }

                C64Writer.WriteColor(row.File, colorMap);
            }
            else if (row.Type2 == "sdf")
            {
                SdfRenderer r = new SdfRenderer(row.Object, row.Filename, row.GetTiling, row.GetObjectMod, row.GetViewPoint, row.GetLookAt, row.GetViewUp, row.GetRepeat, row.GetRenderDistance, row.FlipGradient, row.GetDiffuseDir, row.GetDiffuseAmt, row.GetAmbientDir, row.GetAmbientAmt);

                if (row.Type == "mapping")
                {
                    Vector2[] mapping = new Vector2[80 * 50];

                    for (int i = 0; i < 50; ++i)
                    {
                        for (int j = 0; j < 80; ++j)
                        {
                            Vector2 fragCoord = new Vector2(j, i);
                            int index = i * 80 + j;

                            SdfFragment f = r.RenderMapping(new Vector2(80, 50), fragCoord);
                            mapping[index] = f.UV;
                        }
                    }

                    C64Writer.WriteMapping(row.File, mapping, row.GetTexSize);
                }
                else if (row.Type == "color")
                {
                    int[] colorMap = new int[40 * 25];

                    for (int i = 0; i < 25; ++i)
                    {
                        for (int j = 0; j < 40; ++j)
                        {
                            Vector2 fragCoord = new Vector2(j, i);
                            int index = i * 40 + j;

                            SdfFragment f = r.RenderMapping(new Vector2(40, 25), fragCoord);
                            colorMap[index] = f.Gradient;
                        }
                    }

                    C64Writer.WriteColor(row.File, colorMap);
                }
            }
            else if (row.Type2 == "readpng" && row.Type == "texture")
            {
                using (Image<Rgba32> image = Image.Load<Rgba32>(row.Filename))
                {
                    int[] texture = new int[(int)row.GetTexSize.X * (int)row.GetTexSize.Y];

                    image.ProcessPixelRows(accessor =>
                    {
                        for (int i = 0; i < row.GetTexSize.Y; ++i)
                        {
                            Span<Rgba32> pixelRow = accessor.GetRowSpan(i);
                            for (int j = 0; j < row.GetTexSize.X; ++j)
                            {
                                ref Rgba32 pixel = ref pixelRow[j];
                                int index = i * (int)row.GetTexSize.X + j;
                                texture[index] = (pixel.A > 0 && pixel.R > 0 && pixel.G > 0 && pixel.B > 0) ? 1 : 0;
                            }
                        }
                    });

                    C64Writer.WriteArray(row.File, texture);
                }
            }
        }
        
        Console.WriteLine("Finished");
    }
}