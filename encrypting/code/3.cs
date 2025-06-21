using System;
using System.Text;

namespace FeistelCipher
{
    class Program
    {
        private const int ROUNDS = 18;              // количество раундов по условию варианта
        private const int BLOCK_SIZE = 8;           // 64‑битный блок = 2 * 32‑битных половины

        static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.Write("Введите текст для шифрования: ");
            string plain = Console.ReadLine() ?? string.Empty;

            Console.Write("Введите ключ (произвольная строка): ");
            string keyStr = Console.ReadLine() ?? string.Empty;
            byte[] keyBytes = Encoding.UTF8.GetBytes(keyStr);

            uint[] roundKeys = GenerateRoundKeys(keyBytes);

            // --- Шифрование ---
            byte[] data = Pad(Encoding.UTF8.GetBytes(plain));
            for (int i = 0; i < data.Length; i += BLOCK_SIZE)
                EncryptBlock(data, i, roundKeys);

            string hexCipher = BitConverter.ToString(data).Replace("-", "");
            Console.WriteLine($"\nШифрограмма (hex): {hexCipher}\n");

            for (int i = 0; i < data.Length; i += BLOCK_SIZE)
                DecryptBlock(data, i, roundKeys);

            string decrypted = Encoding.UTF8.GetString(data).TrimEnd('\0');
            Console.WriteLine($"Расшифровка: {decrypted}\n");
        }

        private static void EncryptBlock(byte[] buffer, int offset, uint[] keys)
        {
            uint L = BitConverter.ToUInt32(buffer, offset);
            uint R = BitConverter.ToUInt32(buffer, offset + 4);
            for (int i = 0; i < ROUNDS; i++)
            {
                uint temp = L ^ F(R, keys[i]);
                L = R;
                R = temp;
            }
            Array.Copy(BitConverter.GetBytes(L), 0, buffer, offset, 4);
            Array.Copy(BitConverter.GetBytes(R), 0, buffer, offset + 4, 4);
        }

        private static void DecryptBlock(byte[] buffer, int offset, uint[] keys)
        {
            uint L = BitConverter.ToUInt32(buffer, offset);
            uint R = BitConverter.ToUInt32(buffer, offset + 4);
            for (int i = ROUNDS - 1; i >= 0; i--)
            {
                uint temp = R ^ F(L, keys[i]);
                R = L;
                L = temp;
            }
            Array.Copy(BitConverter.GetBytes(L), 0, buffer, offset, 4);
            Array.Copy(BitConverter.GetBytes(R), 0, buffer, offset + 4, 4);
        }

        private static uint F(uint halfBlock, uint roundKey)
        {
            uint mixed = halfBlock ^ roundKey;
            return (mixed << 1) | (mixed >> 31); 
        }

        private static uint[] GenerateRoundKeys(byte[] keyBytes)
        {
            if (keyBytes.Length < 16) Array.Resize(ref keyBytes, 16); 

            uint[] keys = new uint[ROUNDS];
            for (int i = 0; i < ROUNDS; i++)
            {
                int idx = (i * 4) % keyBytes.Length;
                uint k = BitConverter.ToUInt32(keyBytes, idx);
                keys[i] = RotateLeft(k, i + 1); 
            }
            return keys;
        }

        private static uint RotateLeft(uint value, int count)
            => (value << count) | (value >> (32 - count));

        private static byte[] Pad(byte[] data)
        {
            int pad = BLOCK_SIZE - (data.Length % BLOCK_SIZE);
            Array.Resize(ref data, data.Length + pad);
            return data;
        }
    }
}
