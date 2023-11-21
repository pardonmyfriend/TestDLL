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

            // Zapisanie DLL z funkcją testową
            string funDLL = @"C:\Users\evake\Desktop\RastriginFunctionLibrary.dll";
            string destinationPathFun = Path.Combine(testFunctionsFolder, Path.GetFileName(funDLL));

            if (File.Exists(destinationPathFun))
            {
                File.Copy(funDLL, destinationPathFun, true);
                Console.WriteLine($"Plik {Path.GetFileName(destinationPathFun)} został nadpisany.");
            }
            else
            {
                File.Copy(funDLL, destinationPathFun);
                Console.WriteLine($"Plik {Path.GetFileName(destinationPathFun)} został skopiowany.");
            }

            // Wczytanie DLL z funkcjami testowymi z katalogu TestFunctions
            List<ITestFunction> testFunctions = LoadTestFunctions(testFunctionsFolder, dim);

            if (testFunctions.Any())
            {
                Console.WriteLine("Załadowane funkcje testowe:");

                foreach (var testFunction in testFunctions)
                {
                    Console.WriteLine($"Nazwa: {testFunction.Name}, Dim: {testFunction.Dim}");
                }
            }
            else
            {
                Console.WriteLine("Brak funkcji testowych do załadowania.");
            }

            string algDLL = @"C:\Users\evake\Desktop\SnakeOptimizationLibrary.dll";
            string algFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OptimizationAlgorithms"); // Katalog "Algorithms" w katalogu, w którym uruchomiona jest aplikacja
            Directory.CreateDirectory(algFolder);

            string algPath = Path.Combine(algFolder, Path.GetFileName(algDLL));

            if (File.Exists(algPath))
            {
                File.Copy(algDLL, algPath, true);
                Console.WriteLine($"Plik {Path.GetFileName(algPath)} został nadpisany.");
            }
            else
            {
                File.Copy(algDLL, algPath);
                Console.WriteLine($"Plik {Path.GetFileName(algPath)} został skopiowany.");
            }

            var algAssembly = Assembly.LoadFile(algPath);

            Type[] algTypesInNamespace = algAssembly.GetTypes()
            .Where(t => String.Equals(t.Namespace, "SnakeOptimizationLibrary", StringComparison.Ordinal))
            .ToArray();

            Type algType = algTypesInNamespace
                .FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IOptimizationAlgorithm)));

            if (algType != null)
            {
                IOptimizationAlgorithm objAlgorithm = (IOptimizationAlgorithm)Activator.CreateInstance(algType);
            }
            else
            {
                Console.WriteLine("Nie znaleziono klasy implementującej interfejs IOptimizationAlgorithm w danej przestrzeni nazw.");
            }

            var algorithm = algAssembly.GetType("SnakeOptimizationLibrary.SnakeOptimization");

            TestOptimizationAlgorithm.RunTests(T, N, algorithm, testFunctions);

            Console.Read();
        }

        static List<ITestFunction> LoadTestFunctions(string folderPath, int dim)
        {
            List<ITestFunction> testFunctions = new List<ITestFunction>();

            string[] dllFiles = Directory.GetFiles(folderPath, "*.dll");

            foreach (var dllFile in dllFiles)
            {
                var asm = Assembly.LoadFile(dllFile);

                Type[] typesInNamespace = asm.GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(ITestFunction)))
                    .ToArray();

                foreach (var functionType in typesInNamespace)
                {
                    ITestFunction testFunction = (ITestFunction)Activator.CreateInstance(functionType, dim);
                    testFunctions.Add(testFunction);
                }
            }

            return testFunctions;
        }
    }
}
