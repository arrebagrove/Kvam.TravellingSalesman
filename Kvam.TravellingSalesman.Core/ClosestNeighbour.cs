using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvam.TravellingSalesman.Core
{
  public class ClosestNeighbour
  {
    private readonly List<Point> _cities;
    private readonly double[,] _distances;
    public ClosestNeighbour(List<Point> points, double[,] distances)
    {
      _cities = points;
      _distances = distances;
    }

    public int[] FindBest()
    {
      var remaining = new HashSet<Point>(_cities);
      var first = remaining.First();
      var route = new List<Point> { first };
      remaining.Remove(first);

      var numericRoute = new List<int>{_cities.IndexOf(first)};
      var distance = 0.0d;
      while (remaining.Any())
      {
        var shortest = double.MaxValue;
        Point next = null;
        foreach (var p in remaining)
        {
          var d = Distance(route.Last(), p);
          if (d < shortest)
          {
            shortest = d;
            next = p;
          }
        }
        route.Add(next);
        numericRoute.Add(_cities.IndexOf(next));
        remaining.Remove(next);
        distance += shortest;
    
      }


      distance += Distance(route.First(), route.Last());
      Console.WriteLine("Distance calculated in closestneighbour: " + distance);
      return numericRoute.ToArray();
    }

    private double Distance(Point p1, Point p2)
    {
      return _distances[_cities.IndexOf(p1), _cities.IndexOf(p2)];
    }
  }
}
