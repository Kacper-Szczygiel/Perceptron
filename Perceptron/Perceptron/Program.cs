using System.Globalization;

namespace Perceptron;

public class Program
{
    public static void Main(string[] args)
    {
        var trainSetName = ReadTrainingFileFromUser();
        var testSetName = ReadTestFileFromUser();
        var a = ReadKFromUser();

        List<Data> trainSet = FileToList(trainSetName);
        List<Data> testSet = FileToList(testSetName);
        
        List<double> weightVector = AssignRandomValuesToWeightVector(trainSet.ElementAt(0).Vectors.Count);
        double t = GetPseudoDoubleWithinRange(-5, 5);
        Dictionary<string, int> typeDict = CreateTypesDict(trainSet);

        Learn(trainSet, a, weightVector, t, typeDict);
        
        foreach (var testData in testSet)
        {
            TestRecord(testData, weightVector, t, typeDict);
        }

        Console.WriteLine("Vectors;CorrectType;PredictedType");
        PrintList(testSet);
        
        Console.WriteLine("Accuracy: {0:P2}.", CalculateAccuracy(testSet));

        foreach (var entry in typeDict)
        {
            Console.WriteLine(entry.Key + " accuracy : {0:P2}.", CalculateAccuracy(testSet, entry.Key));
        }
        
        string answer;
        do
        {
            Console.WriteLine("Would you like to add a new record? (yes/no)");
            answer = Console.ReadLine();
            if (answer == "no") { Environment.Exit(0); }
            Console.WriteLine("Enter record");
            String record = Console.ReadLine();
            Data dataRecord = new Data(GetVectorsFromLineWithoutType(record));
            TestRecord(dataRecord, weightVector, t, typeDict);
            PrintData(dataRecord);
        } while (answer == "yes");
        
    }

    private static void PrintList(List<Data> list)
    {
        foreach (var data in list)
        {
            PrintData(data);
        }
    }
    
    private static void PrintData(Data data)
    {
        foreach (var vector in data.Vectors)
        {
            Console.Write(vector + ";");
        }

        if (data.CorrectType is null)
        {
            Console.WriteLine(data.PredictedType);
        }
        else
        {
            Console.WriteLine(data.CorrectType + ";" + data.PredictedType);
        }
    }
    
    private static string ReadTrainingFileFromUser()
    {
        Console.WriteLine("Enter file name for training set");
        var trainingFile = Console.ReadLine();
        try
        {
            if (!File.Exists(trainingFile))
            {
                throw new FileNotFoundException("File doesn't exist");
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return trainingFile;
    }

    private static string ReadTestFileFromUser()
    {
        Console.WriteLine("Enter file name for test set");
        var testFile = Console.ReadLine();
        try
        {
            if (!File.Exists(testFile))
            {
                throw new FileNotFoundException("File doesn't exist");
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return testFile;
    }
    
    private static double ReadKFromUser()
    {
        Console.WriteLine("Enter a");
        string k = Console.ReadLine() ?? throw new InvalidOperationException();
        return double.Parse(k, CultureInfo.InvariantCulture);
    }
    
    private static List<Data> FileToList(string fileName)
    {
        List<Data> returnList = new List<Data>();
        var lines = File.ReadLines(fileName);
        foreach (var line in lines)
        {
            string? type = GetTypeFromLine(line);
            List<string> vectors = GetVectorsFromLine(line);
            Data data = new Data(vectors, type);
            returnList.Add(data);
        }

        return returnList;
    }
    
    private static string GetTypeFromLine(string line)
    {
        string[] lineSplit = line.Split(";");
        return lineSplit[^1];
    }
    
    private static List<string> GetVectorsFromLineWithoutType(string line)
    {
        List<string> returnList = new List<string>(line.Split(";"));
        return returnList;
    }

    private static List<string> GetVectorsFromLine(string line)
    {
        List<string> returnList = new List<string>(line.Split(";"));
        returnList.RemoveAt(returnList.Count - 1);
        return returnList;
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

    private static double GetPseudoDoubleWithinRange(double lowerBound, double upperBound)
    {
        var random = new Random();
        var rDouble = random.NextDouble();
        var rRangeDouble = rDouble * (upperBound - lowerBound) + lowerBound;
        return rRangeDouble;
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

    private static void Learn(List<Data> trainSet, double a, List<double> weightVector, double t, Dictionary<string, int> typeDict)
    {
        foreach (var trainData in trainSet)
        {
            int y = Delta(trainData, weightVector, t, typeDict);
            int d = 0;
            
            foreach (var entry in typeDict)
            {
                if (entry.Key == trainData.CorrectType)
                {
                    d = entry.Value;
                }
            }

            for (int i = 0; i < weightVector.Count; i++)
            {
                weightVector[i] +=
                    (d - y) * a * double.Parse(trainData.Vectors.ElementAt(i), CultureInfo.InvariantCulture);
            }

            t -= (d - y) * a;
        }
    }

    private static void TestList(List<Data> testSet, List<double> weightVector, double t, Dictionary<string, int> typeDict)
    {
        foreach (var testData in testSet)
        {
            int y = Delta(testData, weightVector, t, typeDict);
            
            foreach (var entry in typeDict)
            {
                if (entry.Value == y)
                {
                    testData.PredictedType = entry.Key;
                }
            }
        }
    }

    private static void TestRecord(Data data, List<double> weightVector, double t, Dictionary<string, int> typeDict)
    {
        int y = Delta(data, weightVector, t, typeDict);
            
        foreach (var entry in typeDict)
        {
            if (entry.Value == y)
            {
                data.PredictedType = entry.Key;
            }
        }
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
    
    private static double CalculateAccuracy(List<Data> testSet)
    {
        double allRecords = testSet.Count;
        double correctRecord = 0;
        
        foreach (var testData in testSet)
        {
            if (testData.PredictedType == testData.CorrectType)
            {
                correctRecord++;
            }
        }

        return correctRecord / allRecords;
    }
    private static double CalculateAccuracy(List<Data> testSet, string type)
    {
        double allRecords = 0;
        double correctRecord = 0;
        
        foreach (var testData in testSet)
        {
            if (testData.CorrectType == type)
            {
                allRecords++;
                
                if (testData.PredictedType == testData.CorrectType)
                {
                    correctRecord++;
                }
            }
        }

        return correctRecord / allRecords;
    }
}