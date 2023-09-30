using System;
using System.Numerics;
using CsvHelper;
using calc;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

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

                C64Writer.WriteTunnel(row.File, mapping);
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
                        colorMap[index] = TunnelRenderer.mapColor(uv, aspect, row.GetModulation);
                    }
                }

                C64Writer.WriteColor(row.File, colorMap);
            }
        }
        /*
        Vector2[] tunnelMap = new Vector2[80*50];
        
        for (int i=0; i<50; ++i)
        {
            for (int j=0; j<80; ++j)
            {
                float aspect = 80.0f / 50.0f;

                Vector2 uv = new Vector2((float)j / 80.0f, (float)i / 50.0f);
                int index = i * 80 + j;
                tunnelMap[index] = TunnelRenderer.mapTunnel(uv, aspect, new Vector2(3.0f, 1.75f), new Vector3(0,0,0), false);
            }
        }

        C64Writer.WriteTunnel("tunnel.bin", tunnelMap);

        for (int i=0; i<50; ++i)
        {
            for (int j=0; j<80; ++j)
            {
                float aspect = 80.0f / 50.0f;

                Vector2 uv = new Vector2((float)j / 80.0f, (float)i / 50.0f);
            int index = i * 80 + j;
            tunnelMap[index] = TunnelRenderer.mapTunnel(uv, aspect, new Vector2(3.0f, 1.75f), new Vector3(3, 3, 4.5f), true);
            }
        }

        C64Writer.WriteTunnel("tunnel2.bin", tunnelMap);

        int[] colorMap = new int[40*25];

        for (int i=0; i<25; ++i)
        {
            for (int j=0; j<40; ++j)
            {
                float aspect = 80.0f / 50.0f;

                Vector2 uv = new Vector2((float)j / 40.0f, (float)i / 25.0f);
            int index = i * 40 + j;
            colorMap[index] = TunnelRenderer.mapColor(uv, aspect, new Vector3(2.0f,0.75f,0.4f));
            }
        }

        C64Writer.WriteColor("color.bin", colorMap);

        for (int i=0; i<25; ++i)
        {
            for (int j=0; j<40; ++j)
            {
                float aspect = 80.0f / 50.0f;

                Vector2 uv = new Vector2((float)j / 40.0f, (float)i / 25.0f);
            int index = i * 40 + j;
            colorMap[index] = TunnelRenderer.mapColor(uv, aspect, new Vector3(5.0f,1.75f,0.6f));
            }
        }

        C64Writer.WriteColor("color2.bin", colorMap);

        for (int i=0; i<25; ++i)
        {
            for (int j=0; j<40; ++j)
            {
                float aspect = 80.0f / 50.0f;

                Vector2 uv = new Vector2((float)j / 40.0f, (float)i / 25.0f);
            int index = i * 40 + j;
            colorMap[index] = TunnelRenderer.mapColor(uv, aspect, new Vector3(3.0f,3.5f,0.6f));
            }
        }

        C64Writer.WriteColor("color3.bin", colorMap);
        */
        Console.WriteLine("Finished");
    }
}