namespace Optimization
{
  public class StandardPso2006 : ParticleSwarm
  {
    public StandardPso2006(SearchSpace searchSpace, 
      ObjectiveFunction fitness) : base(searchSpace, fitness)
    {
    }

    public override void Update(Particle particle)
    {
      Optimization.Update.UpdateSpso2006(particle, W, C1, C2);
    }

    public override void Confinement(Particle particle)
    {
      Optimization.Confinement.Standard(particle, SearchSpace);
    }

    public override void Topology()
    {
      Optimization.Topology.RingTopology(Particles);
    }

    public override bool ShouldTopoUpdate()
    {
      return false;
    }
  }
}