using System;

class ECCDiffieHellman
{
    static int a = 1;
    static int b = 1;
    static int p = 23;

    struct Point
    {
        public int x;
        public int y;
        public bool IsInfinity;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
            IsInfinity = false;
        }

        public static Point Infinity => new Point() { IsInfinity = true };
    }

    static int Mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    static bool IsOnCurve(Point P)
    {
        if (P.IsInfinity) return true;
        int left = Mod(P.y * P.y, p);
        int right = Mod((P.x * P.x * P.x) + a * P.x + b, p);
        return left == right;
    }

    static int InverseMod(int k, int m)
    {
        if (k == 0) throw new ArgumentException("Нет обратного для 0");
        int s = 0, old_s = 1;
        int t = 1, old_t = 0;
        int r = m, old_r = k;
        while (r != 0)
        {
            int q = old_r / r;
            (old_r, r) = (r, old_r - q * r);
            (old_s, s) = (s, old_s - q * s);
            (old_t, t) = (t, old_t - q * t);
        }
        if (old_r > 1) throw new ArgumentException("k и m не взаимно просты");
        return Mod(old_s, m);
    }

    static Point AddPoints(Point P, Point Q)
    {
        if (P.IsInfinity) return Q;
        if (Q.IsInfinity) return P;

        if (P.x == Q.x && P.y != Q.y) return Point.Infinity;

        int m;

        if (P.x == Q.x && P.y == Q.y)
        {
            int numerator = Mod(3 * P.x * P.x + a, p);
            int denominator = InverseMod(2 * P.y, p);
            m = Mod(numerator * denominator, p);
        }
        else
        {
            int numerator = Mod(Q.y - P.y, p);
            int denominator = InverseMod(Q.x - P.x, p);
            m = Mod(numerator * denominator, p);
        }

        int rx = Mod(m * m - P.x - Q.x, p);
        int ry = Mod(m * (P.x - rx) - P.y, p);

        return new Point(rx, ry);
    }

    static Point MultiplyPoint(Point P, int k)
    {
        Point result = Point.Infinity;
        Point addend = P;

        while (k > 0)
        {
            if ((k & 1) == 1)
                result = AddPoints(result, addend);
            addend = AddPoints(addend, addend);
            k >>= 1;
        }
        return result;
    }

    static void Main()
    {
        Point G = new Point(6, 4);

        if (!IsOnCurve(G))
        {
            Console.WriteLine("Точка G не лежит на кривой.");
            return;
        }

        int k1 = 8;
        int k2 = 9;

        Point P1 = MultiplyPoint(G, k1);
        Point P2 = MultiplyPoint(G, k2);

        Console.WriteLine($"P1 = k1*G = ({P1.x}, {P1.y})");
        Console.WriteLine($"P2 = k2*G = ({P2.x}, {P2.y})");

        Point S1 = MultiplyPoint(P2, k1);
        Point S2 = MultiplyPoint(P1, k2);

        Console.WriteLine($"S1 = k1*P2 = ({S1.x}, {S1.y})");
        Console.WriteLine($"S2 = k2*P1 = ({S2.x}, {S2.y})");

        Console.WriteLine(S1.x == S2.x && S1.y == S2.y
            ? "Общий секрет совпадает."
            : "Ошибка: секреты не совпадают.");
    }
}