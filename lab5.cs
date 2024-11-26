using System;
using System.Collections.Generic;

public class Quadrature
{
    private double left;
    private double right;
    private double h;
    private List<double> weight;
    private List<double> node;

    // Конструктор по умолчанию (квадратура Симсона)
    public Quadrature()
    {
        left = 0.0;
        right = 1.0;
        h = 1.0;
        weight = new List<double> { 1.0 / 3.0, 4.0 / 3.0, 1.0 / 3.0 };
        node = new List<double> { -1.0, 0.0, 1.0 };
    }

    // Конструктор с параметрами
    public Quadrature(double L, double R, IEnumerable<double> W, IEnumerable<double> N)
    {
        if ((R - L) < double.Epsilon)
            throw new InvalidOperationException("right value < left value");

        var weights = new List<double>(W);
        var nodes = new List<double>(N);

        if (weights.Count != nodes.Count)
            throw new InvalidOperationException("Number of nodes is not equal to number of weights");

        left = L;
        right = R;
        weight = weights;
        node = nodes;
        h = 1.0;
    }

    // Конструктор копирования
    public Quadrature(Quadrature other)
    {
        left = other.left;
        right = other.right;
        h = other.h;
        weight = new List<double>(other.weight);
        node = new List<double>(other.node);
    }

    // Метод для вычисления текущего узла
    private double X(int i, int k)
    {
        return ((node[i] + 1.0) * h / 2.0) + left + k * h;
    }

    // Метод для вычисления функции
    private double F(double x)
    {
        if (x <= 0)
            throw new InvalidOperationException("Logarithm argument <= 0");
        return Math.Log(0.5 * x);
    }

    // Установить новый интервал
    public void SetSegment(double L, double R)
    {
        if ((R - L) < double.Epsilon)
            throw new InvalidOperationException("right value < left value");

        left = L;
        right = R;
    }

    // Установить метод
    public void SetMethod(IEnumerable<double> W, IEnumerable<double> N)
    {
        var weights = new List<double>(W);
        var nodes = new List<double>(N);

        if (weights.Count != nodes.Count)
            throw new InvalidOperationException("Number of nodes is not equal to number of weights");

        weight = weights;
        node = nodes;
    }

    // Расчет интеграла
    public double Calculate(int n)
    {
        if (n <= 0)
            throw new InvalidOperationException("Bad N (<= 0)");

        h = (right - left) / n;
        double result = 0.0;

        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < weight.Count; i++)
            {
                result += h * weight[i] * F(X(i, k));
            }
        }

        return result;
    }
}
