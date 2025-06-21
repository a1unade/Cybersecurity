using System;

class PasswordSampling
{
    static string correctPassword = "SecurePass123";

    static int[] samplePositions = { 1, 3, 6 };

    public static bool CheckPasswordSample(string inputSample)
    {
        if (inputSample.Length != samplePositions.Length)
            return false;

        for (int i = 0; i < samplePositions.Length; i++)
        {
            if (correctPassword[samplePositions[i]] != inputSample[i])
                return false;
        }
        return true;
    }

    static void Main()
    {
        Console.WriteLine("Введите символы пароля на позициях:");
        foreach (var pos in samplePositions)
            Console.Write($"{pos + 1} ");

        Console.WriteLine("\nВведите символы подряд без пробелов:");

        string userInput = Console.ReadLine();

        if (CheckPasswordSample(userInput))
            Console.WriteLine("Доступ разрешён.");
        else
            Console.WriteLine("Неверные символы. Доступ запрещён.");
    }
}