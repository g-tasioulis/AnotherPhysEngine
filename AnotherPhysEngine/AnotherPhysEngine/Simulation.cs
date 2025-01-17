using System.Numerics;
using AnotherPhysEngine.Entities;

namespace AnotherPhysEngine;

public class Simulation(SimulationConfig simulationConfig): ISimulation
{
    private SimulationState? _simulationState;
    
    public void Initialize(SimulationConfig config)
    {
        var particles = new List<Particle>();
        var rand = new Random();
        for (int i = 0; i < simulationConfig.ParticleCount; i++)
        {
            var position = new Vector2(
                (float)rand.NextDouble() * simulationConfig.GridWidth,
                (float)rand.NextDouble() * simulationConfig.GridHeight
            );
            var velocity = new Vector2(
                (float)(rand.NextDouble()*2-1),
                (float)rand.NextDouble()*2-1);
            particles.Add(new Particle(position, velocity, 1.0f));
        }

        _simulationState = new SimulationState(particles);
    }

    public void Update(float deltaTime)
    {
        foreach (var particle in _simulationState.Particles)
        {
            particle.Position += particle.Velocity * deltaTime;
            
            if(particle.Position.X < 0 || particle.Position.X > simulationConfig.GridWidth)
            {
                particle.Velocity= particle.Velocity with { X = -particle.Velocity.X, Y = particle.Velocity.Y };
            }
            if(particle.Position.Y < 0 || particle.Position.Y > simulationConfig.GridHeight)
            {
                particle.Velocity = particle.Velocity with { X = particle.Velocity.X, Y = -particle.Velocity.Y };
            }
        }
    }

    public SimulationState GetState()
    {
        return _simulationState;
    }

    public void ApplyInput(SimulationInput input)
    {
        
    }
}