namespace AnotherPhysEngine;

public class SimulationConfig(int gridWidth, int gridHeight, int particleCount, float deltaTime)
{
    public int GridWidth { get; set; } = gridWidth;
    public int GridHeight { get; set; } = gridHeight;
    public int ParticleCount { get; set; } = particleCount;
    public float DeltaTime { get; set; } = deltaTime;
}