namespace Optimization
{
  public class StandardPso2011 : ParticleSwarm
  {
    public StandardPso2011(SearchSpace searchSpace, 
      ObjectiveFunction fitness) : base(searchSpace, fitness)
    {
    }

    public override void Update(Particle particle)
    {
      Optimization.Update.UpdateSpso2011(particle, W, C1, C2);
    }

    public override void Confinement(Particle particle)
    {
      Optimization.Confinement.DeterministicBack(particle, SearchSpace);
    }

    public override void Topology()
    {
      Optimization.Topology.AdaptiveRandomTopology(Particles, 3);
    }

    public override bool ShouldTopoUpdate()
    {
      return GlobalBestChanged;
    }
  }
}