using System;
using System.Numerics;
using System.IO;

class Program
{
  static Vector2 mapTunnel(Vector2 uv, float aspect, Vector2 tiling)
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

    // resolve texture UV
    Vector2 texUv = new Vector2(angle / 3.141519f, 1.0f-dist);
    
    // tiling
    texUv.X *= tiling.X;
    texUv.Y *= tiling.Y;

    // repeat
    texUv.X %= 1.0f;
    texUv.Y %= 1.0f;

    return texUv;
  }

  static void Main()
  {
    Vector2[] tunnelMap = new Vector2[80*50];
    Vector2 tiling = new Vector2(4.0f, 2.0f);
    float aspect = 80.0f / 50.0f;

    for (int i=0; i<50; ++i)
    {
      for (int j=0; j<80; ++j)
      {
        Vector2 uv = new Vector2((float)j / 80.0f, (float)i / 50.0f);
        int index = i * 80 + j;
        tunnelMap[index] = mapTunnel(uv, aspect, tiling);
      }
    }

    using (BinaryWriter writer = new BinaryWriter(File.Open("tunnel.bin", FileMode.Create)))
    {
        for (int i=0; i<25; ++i)
        {
          for (int j=0; j<40; ++j)
          {
            // single character 2x2 
            for (int k=0; k<2; ++k)
            {
              for (int l=0; l<2; ++l)
              {
                int index = (i * 2 + k) * 80 + (j * 2 + l);
                Vector2 value = tunnelMap[index];

                int x = (int)MathF.Floor(value.X * 8.0f);
                int y = (int)MathF.Floor(value.Y * 8.0f);

                int writeValue = y * 8 + x;
                byte lowestByte = (byte)(writeValue & 0xFF); // Extract the lowest byte
                writer.Write(lowestByte); // Write the lowest byte to the binary file
              }
            }
          }
        }
    }

    Console.WriteLine("Finished");
  }
}