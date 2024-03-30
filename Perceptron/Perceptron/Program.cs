namespace Perceptron;

public class Program
{
    public static void Main(string[] args)
    {
        var trainSetName = ReadTrainingFileFromUser();
        var testSetName = ReadTestFileFromUser();
        var a = ReadKFromUser();
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
        Console.WriteLine("Enter file name for training set");
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
        return double.Parse(k);
    }
}