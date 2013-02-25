namespace Kvam.TravellingSalesman.Core.TspSpecific
{
  public abstract class SalesmanGenotype : Genotype
  {
    private int[] _cities;

    public double Distance { get; set; }

    public int[] Cities
    {
      get { return _cities; }
      set
      {
        _cities = value;
        double distance = 0;
        int last = value.Length - 1;
        for (int i = 0; i < value.Length; ++i)
        {
          distance += Runner.Distances[_cities[i], _cities[last]];
          last = i;
        }
        Distance = distance;
      }
    }

    public abstract SalesmanGenotype Mutate();

    public abstract SalesmanGenotype[] Recombination(SalesmanGenotype firstparent);
  }
}
