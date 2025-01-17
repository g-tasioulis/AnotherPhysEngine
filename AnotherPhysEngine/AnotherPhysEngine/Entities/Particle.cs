using System.Numerics;

namespace AnotherPhysEngine.Entities;

public class Particle(Vector2 position, Vector2 velocity, float mass)
{
    public Vector2 Position { get; set; } = position;
    public Vector2 Velocity { get; set; } = velocity;
    public float Mass { get; set; } = mass;
}