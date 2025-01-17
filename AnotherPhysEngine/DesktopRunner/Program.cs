using AnotherPhysEngine;
using DesktopRunner;

class Program
{
    static void Main(string[] args)
    {
        var config = new SimulationConfig(800, 600, 20, 5000.0f);
        var simulation = new Simulation(config);

        simulation.Initialize(config);

        using var renderer = new OpenGLRenderer(simulation, config);
        renderer.Run();
    }
}