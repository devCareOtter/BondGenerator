using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BondConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string assemblyPath = "";
            string outputPath = "";
            IEnumerable<string> types = Enumerable.Empty<string>();

            var argsQueue = new Queue<string>(args);
            while (argsQueue.Any())
            {
                var cur = argsQueue.Dequeue();
                switch (cur)
                {
                    case "-AssemblyPath":
                        assemblyPath = argsQueue.Dequeue();
                        break;
                    case "-OutputPath":
                        outputPath = argsQueue.Dequeue();
                        break;
                    case "-Types":
                        types = argsQueue.Dequeue().Split(',');
                        break;
                    default:
                        Console.WriteLine($"Unknown arg: {cur}");
                        break;
                }
            }

            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            var converter = new BondConverter(types.Select(x => assembly.GetType(x)));

            var str = converter.GenerateBondFile("test");

            File.WriteAllText(outputPath, str);
        }
    }
}
