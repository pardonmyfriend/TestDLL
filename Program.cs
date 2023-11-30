using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] T = { 5, 10, 20, 40, 60, 80 };
            int[] N = { 10, 20, 40, 80 };
            int dim = 2;

            string testFunctionsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFunctions");
            Directory.CreateDirectory(testFunctionsFolder);

            // Zapisanie DLL z funkcją testową do katalogu TestFunctions
            string testFunctionDLL = @"C:\Users\evake\Desktop\RastriginFunctionLibrary.dll";
            string testFunctionPath = Path.Combine(testFunctionsFolder, Path.GetFileName(testFunctionDLL));

            if (File.Exists(testFunctionPath))
            {
                File.Copy(testFunctionDLL, testFunctionPath, true);
                Console.WriteLine($"Plik {Path.GetFileName(testFunctionPath)} został nadpisany.");
            }
            else
            {
                File.Copy(testFunctionDLL, testFunctionPath);
                Console.WriteLine($"Plik {Path.GetFileName(testFunctionPath)} został skopiowany.");
            }

            // Wczytanie funkcji testowych z plików DLL z katalogu TestFunctions
            //TODO: wczytanie tylko podanych funkcji testowych
            List<object> testFunctions = LoadTestFunctions(testFunctionsFolder, dim);

            if (testFunctions.Any())
            {
                Console.WriteLine("Załadowane funkcje testowe:");

                foreach (var testFunction in testFunctions)
                {
                    var name = testFunction.GetType().GetProperty("Name").GetValue(testFunction);
                    //var dimension = testFunction.GetType().GetProperty("Dim").GetValue(testFunction);
                    //var xmin = testFunction.GetType().GetProperty("Xmin").GetValue(testFunction);
                    //var xmax = testFunction.GetType().GetProperty("Xmax").GetValue(testFunction);

                    Console.WriteLine($"Nazwa: {name}");
                }
            }
            else
            {
                Console.WriteLine("Brak funkcji testowych do załadowania.");
            }

            string optimizationAlgorithmsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OptimizationAlgorithms");
            Directory.CreateDirectory(optimizationAlgorithmsFolder);

            string optimizationAlgorithmDLL = @"C:\Users\evake\Desktop\SnakeOptimizationLibrary.dll";

            string optimizationAlgorithmPath = Path.Combine(optimizationAlgorithmsFolder, Path.GetFileName(optimizationAlgorithmDLL));

            if (File.Exists(optimizationAlgorithmPath))
            {
                File.Copy(optimizationAlgorithmDLL, optimizationAlgorithmPath, true);
                Console.WriteLine($"Plik {Path.GetFileName(optimizationAlgorithmPath)} został nadpisany.");
            }
            else
            {
                File.Copy(optimizationAlgorithmDLL, optimizationAlgorithmPath);
                Console.WriteLine($"Plik {Path.GetFileName(optimizationAlgorithmPath)} został skopiowany.");
            }

            // Wczytanie agorytmu optymizacyjnego z pliku SnakeOptimizationLibrary.dll z katalogu OptimizationAlgorithms
            //TODO: wczytanie wybranego algorytmu

            var assembly = Assembly.LoadFile(optimizationAlgorithmPath);

            var types = assembly.GetTypes();

            var delegateFunction = assembly.GetType("Function");

            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract && !typeof(Delegate).IsAssignableFrom(type))
                {
                    Console.WriteLine(type.FullName);
                    TestOptimizationAlgorithm.RunTests(T, N, type, testFunctions, delegateFunction);
                }
            }

            Console.Read();
        }

        static List<object> LoadTestFunctions(string folderPath, int dim)
        {
            List<object> testFunctions = new List<object>();

            string[] dllFiles = Directory.GetFiles(folderPath, "*.dll");

            foreach (var dllFile in dllFiles)
            {
                var assembly = Assembly.LoadFile(dllFile);

                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass && !type.IsAbstract)
                    {
                        var instance = Activator.CreateInstance(type, dim);

                        testFunctions.Add(instance);
                    }
                }
            }

            return testFunctions;
        }
    }
}
