using System.Collections.Generic;
using Kvam.TravellingSalesman.Core;

namespace Kvam.TravellingSalesman.Models
{
  public abstract class Generation<T, TU>
    where T : Phenotype<TU>
    where TU : Genotype
  {
    protected List<Phenotype<TU>> generation;

    protected Generation(List<Phenotype<TU>> generation)
    {
      Items = generation;
    }

    protected Generation()
    {
      Items = new List<Phenotype<TU>>();
    }

    public List<Phenotype<TU>> Items
    {
      set { generation = value; }
      get { return generation; }
    }

    public abstract void Add(Phenotype<TU> item);

    public abstract double CalculateFitness(Phenotype<TU> item, params int[] parameters);

  }
}
