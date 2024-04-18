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

        Perceptron perceptron = new Perceptron(trainSet);
        perceptron.Learn(trainSet, a);
        
        foreach (var testData in testSet)
        {
            perceptron.TestRecord(testData);
        }

        Console.WriteLine("Vectors;CorrectType;PredictedType");
        PrintList(testSet);
        
        Console.WriteLine("Accuracy: {0:P2}.", CalculateAccuracy(testSet));

        foreach (var entry in perceptron.TypesDict)
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
            Data testData = new Data(GetVectorsFromLineWithoutType(record));
            perceptron.TestRecord(testData);
            PrintData(testData);
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