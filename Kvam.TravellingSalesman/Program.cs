using Kvam.TravellingSalesman.Core;
using System;
using System.Linq;

namespace Kvam.TravellingSalesman
{
  static class Program
  {
    static void Main(string[] args)
    {
      string outputPath;
      if (args != null && args.Any())
      {
        outputPath = args.First();
      }
      else
      {
        Console.WriteLine("Drop images to folder:");
        outputPath = Console.ReadLine();
      }
      var runner = new Runner(location: Runner.Location.WesternSahara,
                              seed: 121285,
                              outputPath: outputPath);
      runner.Run();
    }
  }
}
