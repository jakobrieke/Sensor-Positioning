namespace Optimization
{
  public class StandardPso2006 : ParticleSwarm
  {
    public StandardPso2006(SearchSpace searchSpace, 
      Objective fitness) : base(searchSpace, fitness)
    {
    }

    public override void Update(Particle particle)
    {
      global::Optimization.Update.UpdateSpso2006(particle, W, C1, C2, Random);
    }

    public override void Confinement(Particle particle)
    {
      global::Optimization.Confinement.Standard(particle, SearchSpace);
    }

    public override void Topology()
    {
      global::Optimization.Topology.RingTopology(Particles);
    }

    public override bool ShouldTopoUpdate()
    {
      return false;
    }
  }
}