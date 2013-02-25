using System;
using System.Collections.Generic;
using Kvam.TravellingSalesman.Models;

namespace Kvam.TravellingSalesman.Core.TspSpecific
{
  public class SalesmanPhenotype : Phenotype<SalesmanGenotype>, IComparable
  {
    public SalesmanPhenotype(SalesmanGenotype geno)
    {
      GenotypeCollection = new List<SalesmanGenotype> { geno };
    }

    public int CompareTo(object other)
    {
      var sp = (SalesmanPhenotype)other;
      if (Fitness > sp.Fitness)
        return 1;
      if (Fitness < sp.Fitness)
        return -1;
      return 0;
    }
  }
}
