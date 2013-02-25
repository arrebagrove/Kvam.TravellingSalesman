using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvam.TravellingSalesman.Core.TspSpecific.GenotypeImplementations
{
  /*
   * Another direct encoding as integers, but in this case, useWhitley et. al’s Edge Recombination Operator
   * (ERO) during crossover.
   */
  public class GenotypeEro : SalesmanGenotype
  {
    public GenotypeEro(int[] permutation = null)
    {
      if (permutation != null)
        Cities = permutation;
    }

    public override SalesmanGenotype Mutate()
    {
      var next = Randomizer.Random.Next();
      if (next % 3 == 0)
      {
        return MutateByIntegerSwap();
      }
      if (next % 3 == 1)
      {
        return MutateByShift();
      }
      return MutateByIntegerSwap(swapNeighbours: true);
    }

    private SalesmanGenotype MutateByShift()
    {
      int[] cities = Cities;
      int firstcity = Randomizer.Random.Next(0, cities.Length - 1);
      int secondcity = Randomizer.Random.Next(0, cities.Length - 1);

      int tmp = cities[firstcity];
      if (secondcity < firstcity)
      {
        Array.Copy(cities, secondcity, cities, secondcity + 1, firstcity - secondcity);
      }
      else
      {
        Array.Copy(cities, firstcity + 1, cities, firstcity, secondcity - firstcity);
      }
      cities[secondcity] = tmp;

      return new GenotypeEro { Cities = cities };
    }


    private GenotypeEro MutateByIntegerSwap(bool swapNeighbours = false)
    {
      // We mutate by swapping two randomly selected cities.
      int[] cities = Cities;
      int firstcity = Randomizer.Random.Next(0, cities.Length - 1);
      // We don't want to swap a city with itself, and we avoid that by picking second cities
      // until we have different cities.

      int secondcity = firstcity;

      if (swapNeighbours)
      {
        secondcity = (secondcity + 1) % cities.Length;
      }
      else
      {
        while (firstcity == secondcity)
        {
          secondcity = Randomizer.Random.Next(0, cities.Length - 1);
        }
      }

      int temp = cities[firstcity];
      cities[firstcity] = cities[secondcity];
      cities[secondcity] = temp;
      return new GenotypeEro(cities);
    }

    public override SalesmanGenotype[] Recombination(SalesmanGenotype secondparent)
    {
      return EdgeRecombination(this, (GenotypeEro)secondparent);
    }

    private SalesmanGenotype[] EdgeRecombination(GenotypeEro parent1, GenotypeEro parent2)
    {
      var neighbours = new List<NeighbourList>(parent1.Cities.Length);

      for (int i = 0; i < parent1.Cities.Length; ++i)
        neighbours.Add(new NeighbourList(i));

      var p1Cities = new List<int>(parent1.Cities);
      var p2Cities = new List<int>(parent2.Cities);
      for (int i = 0; i < p1Cities.Count; ++i)
      {
        neighbours[p1Cities[i]].Neighbour.Add(p1Cities[(i + 1 + p1Cities.Count) % p1Cities.Count]);
        neighbours[p1Cities[i]].Neighbour.Add(p1Cities[(i - 1 + p1Cities.Count) % p1Cities.Count]);
        neighbours[p2Cities[i]].Neighbour.Add(p2Cities[(i + 1 + p2Cities.Count) % p2Cities.Count]);
        neighbours[p2Cities[i]].Neighbour.Add(p2Cities[(i - 1 + p2Cities.Count) % p2Cities.Count]);
      }
      for (int i = 0; i < p1Cities.Count; ++i)
      {
        neighbours[i].Neighbour = neighbours[i].Neighbour.Distinct().ToList();
      }

      var mutation = new List<int>(parent1.Cities.Length);

      int N = 0, x = 0;
      while (mutation.Count < p1Cities.Count)
      {
        mutation.Add(N);

        foreach (int i in neighbours[N].Neighbour)
          neighbours[i].Neighbour.Remove(N);

        if (neighbours[N].Neighbour.Count > 0)
        {
          int shortest = neighbours[N].Neighbour[0];
          foreach (int n in neighbours[N].Neighbour)
            if (neighbours[n].Neighbour.Count < neighbours[shortest].Neighbour.Count)
              shortest = n;
          N = shortest;
        }
        else if (mutation.Count < p1Cities.Count)
        {
          int shortest = int.MaxValue;
          for (; x < p1Cities.Count; ++x)
          {
            if (neighbours[x].Neighbour.Count > 0 && !mutation.Contains(x))
            {
              shortest = x;
              break;
            }
          }
          if (shortest == int.MaxValue)
          {
            for (int i = neighbours.Count - 1; i >= 0; --i)
            {
              if (!mutation.Contains(i))
              {
                shortest = i;
                break;
              }
            }
          }
          N = shortest;
        }
      }
      return new SalesmanGenotype[] { new GenotypeEro(mutation.ToArray()) };
    }

    public static GenotypeEro RandomGenotype(int dimention)
    {
      var notChosen = new List<int>(dimention);

      for (int i = 0; i < dimention; ++i)
        notChosen.Add(i);

      var permutation = new int[dimention];

      for (int i = 0; i < dimention; ++i)
      {
        int current = Randomizer.Random.Next(notChosen.Count);
        permutation[i] = notChosen[current];
        notChosen.Remove(notChosen[current]);
      }

      return new GenotypeEro(permutation);
    }
  }

  public class NeighbourList
  {
    public List<int> Neighbour { get; set; }
    public int City { get; set; }

    public NeighbourList(int city)
    {
      Neighbour = new List<int>();
      City = city;
    }
  }
}
