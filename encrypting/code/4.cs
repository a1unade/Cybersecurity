using System;
using System.Numerics;
using System.Text;

namespace RSA_Lab
{
    class Program
    {
        // Заданные по варианту простые числа
        private const int P = 199;
        private const int Q = 337;

        static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // 1. Генерация ключей
            BigInteger n = P * Q;                    // модуль
            BigInteger phi = (P - 1) * (Q - 1);      // функция Эйлера

            BigInteger e = FindMinE(phi);            // минимально возможное e
            BigInteger d = ModInverse(e, phi);       // приватная экспонента

            Console.WriteLine("--- RSA: ключевые параметры ---\n" +
                              $"p  = {P}\nq  = {Q}\nn  = {n}\nphi= {phi}\ne  = {e}\nd  = {d}\n");

            // 2. Ввод сообщения
            Console.Write("Введите сообщение: ");
            string message = Console.ReadLine() ?? string.Empty;
            byte[] msgBytes = Encoding.UTF8.GetBytes(message);

            // 3. Шифрование байт‑за‑байтом (каждый m < n, так как n=67063 > 255)
            byte[] cipherBytes = new byte[msgBytes.Length * sizeof(int)]; // 4 байта на каждый символ
            for (int i = 0; i < msgBytes.Length; i++)
            {
                BigInteger m = msgBytes[i];
                BigInteger c = BigInteger.ModPow(m, e, n);
                int cVal = (int)c;
                Buffer.BlockCopy(BitConverter.GetBytes(cVal), 0, cipherBytes, i * 4, 4);
            }

            string cipherHex = BitConverter.ToString(cipherBytes).Replace("-", "");
            Console.WriteLine($"\nШифрограмма (hex): {cipherHex}\n");

            // 4. Дешифрование
            byte[] decrypted = new byte[msgBytes.Length];
            for (int i = 0; i < decrypted.Length; i++)
            {
                int cVal = BitConverter.ToInt32(cipherBytes, i * 4);
                BigInteger c = cVal;
                BigInteger m = BigInteger.ModPow(c, d, n);
                decrypted[i] = (byte)m;
            }

            string plainBack = Encoding.UTF8.GetString(decrypted);
            Console.WriteLine($"Расшифровка: {plainBack}\n");
        }

        private static BigInteger FindMinE(BigInteger phi)
        {
            for (BigInteger cand = 3; cand < phi; cand += 2) // 1 и 2 отбрасываем, начинаем с 3
            {
                if (BigInteger.GreatestCommonDivisor(cand, phi) == 1)
                    return cand;
            }
            throw new Exception("Не удалось найти подходящее e");
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger mod)
        {
            BigInteger t = 0, newT = 1;
            BigInteger r = mod, newR = a;

            while (newR != 0)
            {
                BigInteger q = r / newR;
                (t, newT) = (newT, t - q * newT);
                (r, newR) = (newR, r - q * newR);
            }
            if (r > 1) throw new Exception("a не взаимно просто с mod");
            if (t < 0) t += mod;
            return t;
        }
    }
}
