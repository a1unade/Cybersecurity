using System;
using System.IO;

class LSBSteganography
{
    public static void EmbedMessage(string bmpInPath, string bmpOutPath, string message, int bitsToUse)
    {
        byte[] bmpBytes = File.ReadAllBytes(bmpInPath);
        byte[] msgBytes = System.Text.Encoding.UTF8.GetBytes(message);
        int offset = BitConverter.ToInt32(bmpBytes, 10);
        int capacity = (bmpBytes.Length - offset) * bitsToUse / 8;

        if (msgBytes.Length > capacity)
            throw new Exception("Сообщение слишком большое для этого изображения.");
        
        byte[] data = new byte[msgBytes.Length + 1];
        Array.Copy(msgBytes, data, msgBytes.Length);
        data[data.Length - 1] = 0;

        int dataBitIndex = 0;
        for (int i = offset; i < bmpBytes.Length && dataBitIndex < data.Length * 8; i++)
        {
            for (int bit = 0; bit < bitsToUse; bit++)
            {
                int dataByteIndex = dataBitIndex / 8;
                int dataBitPos = 7 - (dataBitIndex % 8);
                int bitVal = (data[dataByteIndex] >> dataBitPos) & 1;

                // Очистить бит в bmpBytes[i] и записать bitVal
                bmpBytes[i] = (byte)((bmpBytes[i] & ~(1 << bit)) | (bitVal << bit));

                dataBitIndex++;
                if (dataBitIndex >= data.Length * 8) break;
            }
        }

        File.WriteAllBytes(bmpOutPath, bmpBytes);
    }

    public static string ExtractMessage(string bmpPath, int bitsToUse)
    {
        byte[] bmpBytes = File.ReadAllBytes(bmpPath);
        int offset = BitConverter.ToInt32(bmpBytes, 10);

        var messageBytes = new List<byte>();
        byte currentByte = 0;
        int bitCount = 0;

        for (int i = offset; i < bmpBytes.Length; i++)
        {
            for (int bit = 0; bit < bitsToUse; bit++)
            {
                int bitVal = (bmpBytes[i] >> bit) & 1;
                currentByte = (byte)((currentByte << 1) | bitVal);
                bitCount++;

                if (bitCount == 8)
                {
                    if (currentByte == 0)
                        return System.Text.Encoding.UTF8.GetString(messageBytes.ToArray());

                    messageBytes.Add(currentByte);
                    currentByte = 0;
                    bitCount = 0;
                }
            }
        }
        return System.Text.Encoding.UTF8.GetString(messageBytes.ToArray());
    }
}

class Program
{
    static void Main()
    {
        string inputImage = "image.bmp";
        string outputImage = "image_steg.bmp";

        Console.WriteLine("Введите сообщение для скрытия:");
        string message = Console.ReadLine();

        int bitsToUse = 2;

        try
        {
            // Внедряем сообщение
            LSBSteganography.EmbedMessage(inputImage, outputImage, message, bitsToUse);
            Console.WriteLine($"Сообщение успешно внедрено в файл {outputImage}");

            // Извлекаем сообщение из нового файла
            string extracted = LSBSteganography.ExtractMessage(outputImage, bitsToUse);
            Console.WriteLine("Извлечённое сообщение:");
            Console.WriteLine(extracted);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}
