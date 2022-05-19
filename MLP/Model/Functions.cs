using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP.Model;

public delegate double ActivationFunction(double value);

public static class Functions
{
    private const double Beta = 1.0;

    public static double SigmoidUnipolar(double x)
    {
        return 1.0 / (1 + Math.Exp(-x * Beta));
    }
    public static double SigmoidBipolar(double x)
    {
        return Math.Tanh(x * Beta);
    }

    public static double Indentity(double x)
    {
        return x;
    }
}