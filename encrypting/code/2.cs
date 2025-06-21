using System.Text;

namespace Security
{
    public static class Program
    {
        private const int B = 8;                
        private const int M = 1 << B;          

        public static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.Write("Введите текст для шифрования: ");
            string inputText = Console.ReadLine() ?? string.Empty;

            const int A = 17;
            const int C = 43;
            const byte T0 = 123;

            byte[] encrypted = Encrypt(Encoding.GetEncoding(1251).GetBytes(inputText), A, C, T0);
            string hexCipher = BitConverter.ToString(encrypted).Replace("-", "");
            Console.WriteLine($"\nШифрограмма (hex): {hexCipher}\n");

            string decrypted = Encoding.GetEncoding(1251).GetString(Decrypt(encrypted, A, C, T0));
            Console.WriteLine($"Дешифровка: {decrypted}\n");

            const int A2 = 19;
            const int C2 = 101;
            byte[] encrypted2 = Encrypt(Encoding.GetEncoding(1251).GetBytes(inputText), A2, C2, T0);
            string hexCipher2 = BitConverter.ToString(encrypted2).Replace("-", "");
            Console.WriteLine($"Шифрограмма c A={A2}, C={C2} (hex): {hexCipher2}\n");
        }

        public static byte[] Encrypt(byte[] data, int A, int C, byte T0)
        {
            int state = T0;
            byte[] result = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                byte gamma = (byte)state;
                byte cipher = (byte)(data[i] ^ gamma);
                result[i] = cipher;
                
                state = (A * cipher + C) % M;
            }
            return result;
        }

        public static byte[] Decrypt(byte[] cipher, int A, int C, byte T0)
        {
            int state = T0;
            byte[] result = new byte[cipher.Length];

            for (int i = 0; i < cipher.Length; i++)
            {
                byte gamma = (byte)state;
                byte plain = (byte)(cipher[i] ^ gamma);
                result[i] = plain;

                state = (A * cipher[i] + C) % M;
            }
            return result;
        }
    }
}
