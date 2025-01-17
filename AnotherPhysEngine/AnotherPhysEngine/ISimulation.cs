using AnotherPhysEngine.Entities;

namespace AnotherPhysEngine;

public interface ISimulation
{
    void Initialize(SimulationConfig config);
    void Update(float deltaTime);
    public SimulationState GetState();
    public void ApplyInput(SimulationInput input);
}