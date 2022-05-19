namespace MLP.Model;

public class Neuron
{
    public double[] InputWeights { get; init; }
    public double Bias { get; set; }
    public double Value { get; set; }
    
    public Neuron(double[] inputWeights)
    {
        InputWeights = inputWeights;
    }
}