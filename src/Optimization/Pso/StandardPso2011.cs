namespace Optimization
{
  public class StandardPso2011 : ParticleSwarm
  {
    /// <summary>
    /// The factor K is used by Adaptive Random Topology and defines the maximum
    /// number of neighbours a particle might have.
    /// </summary>
    /// <remarks>Needs to be smaller or equal the number of particles.</remarks>
    public uint K = 3;
    
    public StandardPso2011(SearchSpace searchSpace, 
      Objective fitness) : base(searchSpace, fitness)
    {
    }

    public override void Update(Particle particle)
    {
      global::Optimization.Update.UpdateSpso2011(particle, W, C1, C2, Random);
    }

    public override void Confinement(Particle particle)
    {
      global::Optimization.Confinement.DeterministicBack(particle, SearchSpace);
    }

    public override void Topology()
    {
      global::Optimization.Topology.AdaptiveRandomTopology(Particles, K);
    }

    public override bool ShouldTopoUpdate()
    {
      return GlobalBestChanged;
    }
  }
}