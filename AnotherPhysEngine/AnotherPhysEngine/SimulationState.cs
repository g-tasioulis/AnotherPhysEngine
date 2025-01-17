using AnotherPhysEngine.Entities;

namespace AnotherPhysEngine;

public class SimulationState(List<Particle> particles)
{
    public List<Particle> Particles { get; set; } = particles;
}