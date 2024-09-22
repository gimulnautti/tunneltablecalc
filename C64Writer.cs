using System;
using System.IO;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace calc
{
	public static class C64Writer
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

        private static Image ToImageSharp(this byte[] byteArrayIn, int width, int height)
        {
            Image<Rgb24> image = new Image<Rgb24>(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int baseIdx = (y * width + x) * 3;
                    image[x, y] = new Rgb24(byteArrayIn[baseIdx], byteArrayIn[baseIdx + 1], byteArrayIn[baseIdx + 2]);
                }
            }
            return image;
        }

        private static byte[] tunnelMapToImageBytes(Vector2[] tunnelMap, int count)
        {
            byte[] bytes = new byte[count * 3];
            for (int i = 0; i < count; ++i)
            {
                byte valueX = (byte)MathF.Round(tunnelMap[i].X * 255.0f);
                byte valueY = (byte)MathF.Round(tunnelMap[i].Y * 255.0f);
                bytes[i*3] = valueX;
                bytes[i*3+1] = 0;
                bytes[i*3+2] = valueY;
            }
            return bytes;
        }

        private static byte[] colorMapToImageBytes(int[] colorMap, int count)
        {
            byte[] bytes = new byte[count * 3];
            for (int i = 0; i < count; ++i)
            {
                byte value = (byte)(colorMap[i] * 255 / 7);
                bytes[i*3] = value;
                bytes[i*3+1] = value;
                bytes[i*3+2] = value;
            }
            return bytes;
        }

        public static void WriteDebugMappingImage(String fileName, Vector2[] tunnelMap)
        {
            byte[] bytes = tunnelMapToImageBytes(tunnelMap, 80 * 50);
            Image image = ToImageSharp(bytes, 80, 50);
            image.Save(fileName);
        }

        public static void WriteDebugColorImage(String fileName, int[] colorMap)
        {
            byte[] bytes = colorMapToImageBytes(colorMap, 40 * 25);
            Image image = ToImageSharp(bytes, 40, 25);
            image.Save(fileName);        
        }
    }
}

