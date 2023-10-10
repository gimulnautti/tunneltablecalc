using System;
using System.IO;
using System.Numerics;

namespace calc
{
	public class C64Writer
	{
        public static void WriteMapping(String fileName, Vector2[] tunnelMap, Vector2 texSize)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                for (int i = 0; i < 25; ++i)
                {
                    for (int j = 0; j < 40; ++j)
                    {
                        // single character 2x2 
                        for (int k = 0; k < 2; ++k)
                        {
                            for (int l = 0; l < 2; ++l)
                            {
                                int index = (i * 2 + k) * 80 + (j * 2 + l);
                                Vector2 value = tunnelMap[index];

                                int x = (int)MathF.Floor(value.X * texSize.X);
                                int y = (int)MathF.Floor(value.Y * texSize.Y);

                                int writeValue = y * (int)Math.Floor(texSize.X) + x;
                                byte lowestByte = (byte)(writeValue & 0xFF); // Extract the lowest byte
                                writer.Write(lowestByte); // Write the lowest byte to the binary file
                            }
                        }
                    }
                }
            }
        }

        public static void WriteColor(String fileName, int[] colorMap)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                for (int i = 0; i < 25; ++i)
                {
                    for (int j = 0; j < 40; ++j)
                    {
                        int writeValue = colorMap[i * 40 + j];
                        byte lowestByte = (byte)(writeValue & 0xFF); // Extract the lowest byte
                        writer.Write(lowestByte); // Write the lowest byte to the binary file
                    }
                }
            }
        }

        public static void WriteArray(String fileName, int[] colorMap)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                for (int j = 0; j < colorMap.Length; ++j)
                {
                    int writeValue = colorMap[j];
                    byte lowestByte = (byte)(writeValue & 0xFF); // Extract the lowest byte
                    writer.Write(lowestByte); // Write the lowest byte to the binary file
                }
                
            }
        }
    }
}

