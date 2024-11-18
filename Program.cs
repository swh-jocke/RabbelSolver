
using Solver;
using System.Diagnostics;
using System.Text.Json;

namespace RabbelSolver
{
    internal class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();            
            var settingsJson = File.ReadAllText("settings.json");
            var settings = JsonSerializer.Deserialize<Settings>(settingsJson);
            var game = new Rabbel(settings);
            File.WriteAllLines("results.txt", game.Solve().OrderBy(x => x.Length));
            sw.Stop();
            Console.WriteLine($"Elapsed: {sw.Elapsed}");
        }
    }
}
