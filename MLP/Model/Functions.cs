namespace MLP.Model;

public delegate double ActivationFunction(double value, bool deriv = false);

public static class Functions
{
    private const double Beta = 1.0;

    public static double SigmoidUnipolar(double x, bool deriv = false)
    {
        return deriv ? x * (1 - x) : 1.0 / (1 + Math.Exp(-x * Beta));
    }
    public static double SigmoidBipolar(double x, bool deriv = false)
    {
        return deriv ? 1 - Math.Tanh(x) * Math.Tanh(x) : Math.Tanh(x * Beta);
    }

    public static double Identity(double x, bool deriv = false)
    {
        return deriv ? 1 : x;
    }
}