namespace Security
{
    public static class Program
    {
        private static readonly int[] Permutation = { 2, 6, 3, 5, 1, 4 };
        private static readonly int BlockSize = Permutation.Length;

        static void Main()
        {
            Console.WriteLine("Шифр перестановки (русский алфавит)");
            Console.Write("Введите текст: ");
            var text = Console.ReadLine()!.ToUpper();

            var encrypted = Encrypt(text);
            Console.WriteLine($"Зашифрованный текст: {encrypted}");

            var decrypted = Decrypt(encrypted);
            Console.WriteLine($"Расшифрованный текст: {decrypted}");
        }

        static string Encrypt(string text)
        {
            while (text.Length % BlockSize != 0)
                text += " ";

            char[] result = new char[text.Length];

            for (int i = 0; i < text.Length; i += BlockSize)
                for (int j = 0; j < BlockSize; j++)
                    result[i + j] = text[i + Permutation[j] - 1];

            return new string(result);
        }

        static string Decrypt(string text)
        {
            char[] result = new char[text.Length];

            for (int i = 0; i < text.Length; i += BlockSize)
                for (int j = 0; j < BlockSize; j++)
                    result[i + Permutation[j] - 1] = text[i + j];

            return new string(result).TrimEnd();
        }
    }
}