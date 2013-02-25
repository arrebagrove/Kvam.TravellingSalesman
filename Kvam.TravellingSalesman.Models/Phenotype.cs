using System.Collections.Generic;
using Kvam.TravellingSalesman.Core;

namespace Kvam.TravellingSalesman.Models
{
  public abstract class Phenotype<T> where T : Genotype
  {
    // Any environment has a genotypeCollection. Other non-generic
    // characteristics may be added by inheritance.
    protected double fitness;

    public double Fitness
    {
      get { return fitness; }
      set { fitness = value; }
    }

    public List<T> GenotypeCollection { get; set; }

    protected Phenotype(List<T> population)
    {
      GenotypeCollection = population;
    }

    protected Phenotype(T item)
    {
      GenotypeCollection = new List<T> { item };
    }

    protected Phenotype()
    {
    }
  }
}
