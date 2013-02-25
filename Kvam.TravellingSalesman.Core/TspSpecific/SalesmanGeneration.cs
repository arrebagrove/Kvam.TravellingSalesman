using System;
using Kvam.TravellingSalesman.Models;

namespace Kvam.TravellingSalesman.Core.TspSpecific
{
  public class SalesmanGeneration : Generation<SalesmanPhenotype, SalesmanGenotype>
  {
    public int PopulationInGeneration;

    public override double CalculateFitness(Phenotype<SalesmanGenotype> item, params int[] parameters)
    {
      return 1.0d / Math.Pow(item.GenotypeCollection[0].Distance, 1.0d / 3.0d);
    }

    public override void Add(Phenotype<SalesmanGenotype> item)
    {
      item.Fitness = CalculateFitness(item);
      generation.Add(item);
    }

    /*public int Compare(SalesmanPhenotype first, SalesmanPhenotype second)
    {
        if (first.Fitness > second.Fitness)
            return 1;
        else if (first.Fitness < second.Fitness)
            return -1;
        else
            return 0;
    }*/
  }
}
