using System;
using System.Text;

public class DigitalSignature
{
    private static readonly byte key = 0x5A; // простой ключ для XOR подписи

    private static int CountBits(byte b)
    {
        int count = 0;
        while (b != 0)
        {
            count += b & 1;
            b >>= 1;
        }
        return count;
    }

    public static int ComputeHash(string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        int hash = 0;
        foreach (var b in bytes)
            hash += CountBits(b);
        return hash;
    }

    public static byte CreateSignature(int hash)
    {
        return (byte)(hash ^ key);
    }

    public static bool VerifySignature(string message, byte signature)
    {
        int hash = ComputeHash(message);
        byte expectedSignature = (byte)(hash ^ key);
        return expectedSignature == signature;
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.Write("Введите сообщение для подписания: ");
        string message = Console.ReadLine();

        int hash = DigitalSignature.ComputeHash(message);
        byte signature = DigitalSignature.CreateSignature(hash);

        Console.WriteLine($"\nСообщение: {message}");
        Console.WriteLine($"Хэш (кол-во единиц): {hash}");
        Console.WriteLine($"Подпись: 0x{signature:X2}");

        bool valid = DigitalSignature.VerifySignature(message, signature);
        Console.WriteLine($"Проверка подписи: {(valid ? "Успешна" : "Неудачна")}");

        byte tamperedSignature = (byte)(signature ^ 0xFF);
        bool validTampered = DigitalSignature.VerifySignature(message, tamperedSignature);
        Console.WriteLine($"Проверка изменённой подписи: {(validTampered ? "Успешна" : "Неудачна")}");
    }
}