using System;
using System.Numerics;
using CsvHelper;
using calc;

class Program
{
  static void Main()
  {
    Vector2[] tunnelMap = new Vector2[80*50];
    Vector2 tiling = new Vector2(3.0f, 1.75f);
    float aspect = 80.0f / 50.0f;

    for (int i=0; i<50; ++i)
    {
      for (int j=0; j<80; ++j)
      {
        Vector2 uv = new Vector2((float)j / 80.0f, (float)i / 50.0f);
        int index = i * 80 + j;
        tunnelMap[index] = TunnelRenderer.mapTunnel(uv, aspect, new Vector2(3.0f, 1.75f), new Vector3(0,0,0), false);
      }
    }

    C64Writer.WriteTunnel("tunnel.bin", tunnelMap);

    tiling = new Vector2(3.0f, 1.5f);
    for (int i=0; i<50; ++i)
    {
      for (int j=0; j<80; ++j)
      {
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
        Vector2 uv = new Vector2((float)j / 40.0f, (float)i / 25.0f);
        int index = i * 40 + j;
        colorMap[index] = TunnelRenderer.mapColor(uv, aspect, new Vector3(3.0f,3.5f,0.6f));
      }
    }

    C64Writer.WriteColor("color3.bin", colorMap);

    Console.WriteLine("Finished");
  }
}