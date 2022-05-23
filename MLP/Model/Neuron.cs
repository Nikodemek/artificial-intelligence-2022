﻿namespace MLP.Model;

public class Neuron
{
    public double[] InputWeights { get; init; }
    public double[] PreviousInputWeights { get; init; }
    public double Bias { get; set; }
    public double Value { get; set; }
    public double PrevValue { get; set; }
    public double Delta { get; set; }
    public double PrevDelta { get; set; }

    public Neuron(double[] inputWeights, double[]? previousInputWeights = default)
    {
        InputWeights = inputWeights;
        PreviousInputWeights = previousInputWeights ?? new double[inputWeights.Length];
    }
}