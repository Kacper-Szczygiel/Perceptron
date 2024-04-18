using System.Globalization;

namespace Perceptron;

public class Perceptron
{
    public double T;
    public List<double> WeightVector;
    public Dictionary<string, int> TypesDict;

    public Perceptron(List<Data> trainSet)
    {
        T = GetPseudoDoubleWithinRange(-5, 5);
        WeightVector = AssignRandomValuesToWeightVector(trainSet.ElementAt(0).Vectors.Count);
        TypesDict = CreateTypesDict(trainSet);
    }
    
    private static double GetPseudoDoubleWithinRange(double lowerBound, double upperBound)
    {
        var random = new Random();
        var rDouble = random.NextDouble();
        var rRangeDouble = rDouble * (upperBound - lowerBound) + lowerBound;
        return rRangeDouble;
    }
    
    private static List<double> AssignRandomValuesToWeightVector(int length)
    {
        List<double> weightVector = new List<double>();
        for (int i = 0; i < length; i++)
        {
            weightVector.Add(GetPseudoDoubleWithinRange(-5, 5));
        }

        return weightVector;
    }
    
    private static Dictionary<string, int> CreateTypesDict(List<Data> trainSet)
    {
        Dictionary<string, int> returnDict = new Dictionary<string, int>();
        int value = 0;
        foreach (var testData in trainSet)
        {
            if (testData.CorrectType != null && !returnDict.ContainsKey(testData.CorrectType))
            {
                returnDict.Add(testData.CorrectType, value++);
            }
        }

        return returnDict;
    }
    
    private static int Delta(Data data, List<double> weightVector, double t, Dictionary<string, int> typeDict)
    {
        double net = 0;
        int y;
        for (int i = 0; i < data.Vectors.Count; i++)
        {
            net += double.Parse(data.Vectors.ElementAt(i), CultureInfo.InvariantCulture) *
                   weightVector.ElementAt(i);
        }
        net -= t;

        if (net < 0)
        {
            y = 0;
        }
        else
        {
            y = 1;
        }

        return y;
    }
    
    public void Learn(List<Data> trainSet, double a)
    {
        foreach (var trainData in trainSet)
        {
            int y = Delta(trainData, WeightVector, T, TypesDict);
            int d = 0;
            
            foreach (var entry in TypesDict)
            {
                if (entry.Key == trainData.CorrectType)
                {
                    d = entry.Value;
                }
            }

            for (int i = 0; i < WeightVector.Count; i++)
            {
                WeightVector[i] +=
                    (d - y) * a * double.Parse(trainData.Vectors.ElementAt(i), CultureInfo.InvariantCulture);
            }

            T -= (d - y) * a;
        }
    }
    
    public void Test(List<Data> testSet)
    {
        foreach (var testData in testSet)
        {
            int y = Delta(testData, WeightVector, T, TypesDict);
            
            foreach (var entry in TypesDict)
            {
                if (entry.Value == y)
                {
                    testData.PredictedType = entry.Key;
                }
            }
        }
    }
}