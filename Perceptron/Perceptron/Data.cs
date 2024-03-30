namespace Perceptron;

public class Data
{
    public List<string> Vectors { get; set; }
    public string? CorrectType { get; set; }
    public string? PredictedType { get; set; }

    public Data(List<string> vectors, string correctType)
    {
        Vectors = vectors;
        CorrectType = correctType;
    }

    public Data(List<string> vectors)
    {
        Vectors = vectors;
    }
}