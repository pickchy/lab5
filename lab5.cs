using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        const double I = Math.Exp(1) * Math.Log(Math.Exp(1) / 2) + Math.Log(2) + 1 - Math.Exp(1);

        try
        {
            List<double> weight = new List<double> { 1.0, 1.0 };
            List<double> node = new List<double> { -1.0, 1.0 };

            Quadrature lab = new Quadrature(1, Math.Exp(1), weight, node);

            
            Console.WriteLine("{0,12:E6} {1,12:E6} {2,12:E6} {3,12:E6} {4,12:E6} {5,12:E6} {6,12:E6}",
                              "1/h", "I - h1", "(I - h1)/(I - h2)", "Temp", "h2 + Temp", "I - h2 - Temp", "log2(K)");

            for (int i = 1; i < 64; i *= 2)
            {
                double h1 = lab.Calculate(i);
                double h2 = lab.Calculate(2 * i);

                double step = 1.0 / i;
                double diff1 = I - h1;
                double diff2 = I - h2;
                double ratio = diff1 / diff2;

                double K = Math.Abs(1.0 + (h2 - h1) / diff2);
                double temp = (h2 - h1) / (K - 1);

                Console.WriteLine("{0,12:E6} {1,12:E6} {2,12:E6} {3,12:E6} {4,12:E6} {5,12:E6} {6,12:E6}",
                                  step, diff1, ratio, temp, h2 + temp, diff2 - temp, Math.Log2(K));
            }

            weight = new List<double> { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
            node = new List<double> { -Math.Sqrt(3.0 / 5.0), 0.0, Math.Sqrt(3.0 / 5.0) };

            lab.SetMethod(weight, node);

            for (int i = 1; i < 64; i *= 2)
            {
                double h1 = lab.Calculate(i);
                double h2 = lab.Calculate(2 * i);

                double step = 1.0 / i;
                double diff1 = I - h1;
                double diff2 = I - h2;
                double ratio = diff1 / diff2;

                double K = Math.Abs(1.0 + (h2 - h1) / diff2);
                double temp = (h2 - h1) / (K - 1);

                Console.WriteLine("{0,12:E6} {1,12:E6} {2,12:E6} {3,12:E6} {4,12:E6} {5,12:E6} {6,12:E6}",
                                  step, diff1, ratio, temp, h2 + temp, diff2 - temp, Math.Log2(K));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}

class Quadrature
{
    private double left;
    private double right;
    private double h;
    private List<double> weight;
    private List<double> node;

    public Quadrature(double L, double R, List<double> W, List<double> N)
    {
        if ((R - L) < double.Epsilon)
            throw new Exception("Right value < left value");

        if (W.Count != N.Count)
            throw new Exception("Number of nodes is not equal to number of weights");

        left = L;
        right = R;
        weight = W.ToList();
        node = N.ToList();
        h = 1.0;
    }

    public void SetMethod(List<double> W, List<double> N)
    {
        if (W.Count != N.Count)
            throw new Exception("Number of nodes is not equal to number of weights");

        weight = W.ToList();
        node = N.ToList();
    }

    private double X(int i, int k)
    {
        return (h * (node[i] + 1.0) / 2.0) + left + h * k;
    }

    private double F(double x)
    {
        return Math.Log(0.5 * x);
    }

    public double Calculate(int n)
    {
        if (n <= 0)
            throw new Exception("Bad N (<= 0)");

        h = (right - left) / n;

        double result = 0.0;
        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < weight.Count; i++)
            {
                result += h * weight[i] * F(X(i, k));
            }
        }

        result /= 2.0;
        return result;
    }
}
