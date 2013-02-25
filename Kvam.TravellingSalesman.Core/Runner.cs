using Kvam.TravellingSalesman.Core.TspSpecific;
using Kvam.TravellingSalesman.Core.TspSpecific.GenotypeImplementations;
using Kvam.TravellingSalesman.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kvam.TravellingSalesman.Core
{
  public class Runner
  {
    private readonly int _populationInGeneration;
    private readonly int _generationsToRun;
    private readonly int _minutesToRun;
    private readonly bool _startWithRandomRoute;

    private readonly Location _location;
    private static double _numberOfRecombinations;

    public static double[,] Distances { get; set; }

    public static double ShortestPath = double.MaxValue;

    private List<Point> _cities;
    private readonly int _numberOfMutations;

    private readonly StreamWriter _logger;

    private readonly string _logDirectory;
    private readonly Stopwatch _stopwatch;

    public Runner(Location location, int seed, string outputPath)
    {
      _stopwatch = Stopwatch.StartNew();

      _location = location;
      _logDirectory = string.Format(outputPath + "{0} - run {1}\\", _location, DateTime.Now.TimeOfDay.ToString().Replace(':', '.'));
      Randomizer.Random = new Random(seed);

      _minutesToRun = 360;
      _generationsToRun = 10000000;
      _populationInGeneration = 500;
      _numberOfRecombinations = 0.72;
      _numberOfMutations = 5;
      _startWithRandomRoute = false;

      if (!Directory.Exists(_logDirectory))
      {
        Directory.CreateDirectory(_logDirectory);
      }

      _logger = new StreamWriter(_logDirectory + "_log.txt");

      _logger.WriteLine("_generationsToRun: " + _generationsToRun);
      _logger.WriteLine("_populationInGeneration: " + _populationInGeneration);
      _logger.WriteLine("_numberOfRecombinations: " + _numberOfRecombinations);
      _logger.WriteLine("_numberOfMutations: " + _numberOfMutations);
      _logger.WriteLine("seed: " + seed);
    }

    public void Run()
    {
      LoadCities();

      SalesmanGeneration generation;
      if (_startWithRandomRoute)
      {
        generation = CreateRandomGeneration();
      }
      else
      {
        generation = CreateGenerationFromClostestNeighbour();
      }
      BestSolutionGenotype3(generation);
    }

    private SalesmanGeneration CreateRandomGeneration()
    {
      var generation = new SalesmanGeneration { PopulationInGeneration = _populationInGeneration };

      for (int i = 0; i < generation.PopulationInGeneration; ++i)
      {
        generation.Add(new SalesmanPhenotype(GenotypeEro.RandomGenotype(_cities.Count)));
      }

      generation.Items.Sort();

      return generation;
    }

    private SalesmanGeneration CreateGenerationFromClostestNeighbour()
    {
      var route = new ClosestNeighbour(_cities, Distances).FindBest();


      var pointList = route.Select(x => _cities[x]).ToList();

      new Task(() => new Graphics.MapDrawer(_logDirectory + "Initial state - closest neighbour.png",
                                            pointList.Select(x => new System.Drawing.Point((int)x.X, (int)x.Y)),
                                            4000)).Start();

      var generation = new SalesmanGeneration { PopulationInGeneration = _populationInGeneration };
      for (int i = 0; i < generation.PopulationInGeneration; ++i)
      {
        generation.Add(new SalesmanPhenotype(new GenotypeEro((int[])route.Clone())));
      }
      return generation;
    }

    private void BestSolutionGenotype3(SalesmanGeneration generation)
    {
      for (int generationCount = 0; _stopwatch.Elapsed < TimeSpan.FromMinutes(_minutesToRun); ++generationCount)
      {
        if (ShortestPath > generation.Items.Last().GenotypeCollection.First().Distance)
        {
          UpdateShortestPath(generation, generationCount);
        }
        else if (generationCount % 10 == 0)
        {
          Console.WriteLine(generationCount);
        }

        var newGeneration = new SalesmanGeneration();
        for (int j = 0; newGeneration.Items.Count < _numberOfRecombinations * generation.Items.Count; ++j)
        {
          GenotypeEro genotype1 = (GenotypeEro)generation.Items[generation.Items.Count - j - 1].GenotypeCollection[0],
                      genotype2 = (GenotypeEro)generation.Items[generation.Items.Count - j - 2].GenotypeCollection[0];

          var randomGenotypes = genotype1.Recombination(genotype2);
          foreach (SalesmanGenotype genotype in randomGenotypes)
          {
            newGeneration.Add(new SalesmanPhenotype(genotype));
          }
        }
        for (int j = 0; newGeneration.Items.Count < generation.Items.Count; ++j)
        {
          for (int mutation = 0; mutation < _numberOfMutations; ++mutation)
          {
            var genotype = (GenotypeEro)generation.Items[generation.Items.Count - j - 1 - mutation].GenotypeCollection[0];
            newGeneration.Add(new SalesmanPhenotype(genotype.Mutate()));
          }
        }

        generation = newGeneration;
        generation.Items.Sort();

      }
    }

    private void UpdateShortestPath(SalesmanGeneration generation, int generationCount)
    {
      var bestPath = generation.Items.Last().GenotypeCollection.First();
      var previousShortestPath = ShortestPath;
      ShortestPath = bestPath.Distance;

      var message = previousShortestPath == double.MaxValue
                      ? string.Format("{0}\tNew best: {1:F0}", generationCount, ShortestPath)
                      : string.Format("{0}\tNew best: {1:F0}\tOld best: {2:F0}\tDown {3:F3}%", generationCount, ShortestPath, previousShortestPath, (100.0) * (1 - ShortestPath / previousShortestPath));

      Console.WriteLine(message);

      _logger.WriteLine(string.Join(" ", bestPath.Cities));
      _logger.WriteLine(message);
      _logger.WriteLine();

      var pointList = bestPath.Cities.Select(x => _cities[x]).ToList();

      new Task(() => new Graphics.MapDrawer(string.Format(@"{0}Generation {1} - Improvement {2} - {3}.png", _logDirectory, generationCount, "{0}", bestPath.Distance),
                                            pointList.Select(x => new System.Drawing.Point((int)x.X, (int)x.Y)),
                                            4000)).Start();
    }

    private void LoadCities()
    {
      Func<string, List<Point>> cityParser = input => input.Split('\n').Select(x => x.Split(' ')).Select(x => new Point { X = (int)double.Parse(x[1]), Y = (int)double.Parse(x[2]) }).ToList();
      switch (_location)
      {
        case Location.WesternSahara:
          LoadCities(CityRepository.WesternSahara);
          _cities = cityParser(CityRepository.WesternSahara);
          break;

        case Location.Djibouti:
          LoadCities(CityRepository.Djibouti);
          _cities = cityParser(CityRepository.Djibouti);
          break;

        case Location.Zimbabwe:
          LoadCities(CityRepository.Zimbabwe);
          _cities = cityParser(CityRepository.Zimbabwe);
          break;

        case Location.Qatar:
          LoadCities(CityRepository.Qatar);
          _cities = cityParser(CityRepository.Qatar);
          break;
      }
    }

    private void LoadCities(string cities)
    {
      var citylocations =
          cities
            .Split('\n')
            .Select(x => x.Substring(x.IndexOf(' ') + 1))
            .Distinct()
            .OrderBy(x => x)
            .Select(x => x.Replace('.', ',').Trim().Split(' ').Select(double.Parse))
            .Select(y => new Point { X = y.First(), Y = y.Last() })
            .ToList();

      Distances = new double[citylocations.Count, citylocations.Count];

      for (int i = 0; i < citylocations.Count; ++i)
      {
        for (int j = 0; j < citylocations.Count; ++j)
        {
          double x = citylocations[i].X - citylocations[j].X;
          double y = citylocations[i].Y - citylocations[j].Y;

          Distances[i, j] = Math.Sqrt(x * x + y * y);
        }
      }
    }

    public enum Location
    {
      WesternSahara,
      Djibouti,
      Zimbabwe,
      Qatar
    }
  }

  public class Point
  {
    public double X { get; set; }
    public double Y { get; set; }
  }
}
