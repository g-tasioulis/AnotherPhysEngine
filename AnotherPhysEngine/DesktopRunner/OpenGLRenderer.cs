using AnotherPhysEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Diagnostics;
using System.Text;

namespace DesktopRunner
{
    public class OpenGLRenderer : GameWindow
    {
        private Simulation _simulation;
        private SimulationConfig _simulationConfig;

        private int _shaderProgram;
        private int _vertexBuffer;
        private int _vertexArray;
        
        private const int TargetFps = 60; // Desired FPS
        private const float TargetFrameTime = 1.0f / TargetFps; // Time per frame in seconds
        // FPS variables
        private int _frameCount = 0;
        private float _timeElapsed = 0f;
        private Stopwatch _stopwatch;
        private string _fpsText = "FPS: 0";

        public OpenGLRenderer(Simulation simulation, SimulationConfig simulationConfig)
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            _simulation = simulation;
            _simulationConfig = simulationConfig;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set the clear color
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            // Initialize shaders
            _shaderProgram = CreateShaderProgram();

            // Initialize buffers
            _vertexBuffer = GL.GenBuffer();
            _vertexArray = GL.GenVertexArray();

            GL.BindVertexArray(_vertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Update the simulation
            _simulation.Update(TargetFrameTime);

            // Get particle positions
            var particles = _simulation.GetState().Particles;
            var positions = new float[particles.Count * 2];

            for (int i = 0; i < particles.Count; i++)
            {
                positions[i * 2] = (particles[i].Position.X / _simulationConfig.GridWidth) * 2 - 1;
                positions[i * 2 + 1] = (particles[i].Position.Y / _simulationConfig.GridHeight) * 2 - 1;
            }

            // Update the vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions,
                BufferUsageHint.DynamicDraw);

            // Render particles
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vertexArray);

            GL.PointSize(10.0f); // Size of the dots
            GL.DrawArrays(PrimitiveType.Points, 0, particles.Count);

            GL.BindVertexArray(0);
            GL.UseProgram(0);
            
            _frameCount++;
            _timeElapsed += (float)args.Time;

            if (_timeElapsed >= 1.0f)
            {
                float fps = _frameCount / _timeElapsed;
                Console.WriteLine($"FPS: {fps:F2}");

                // Reset counters
                _frameCount = 0;
                _timeElapsed = 0.0f;
            }
            
            // Swap buffers
                SwapBuffers();
            
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Cleanup
            GL.DeleteBuffer(_vertexBuffer);
            GL.DeleteVertexArray(_vertexArray);
            GL.DeleteProgram(_shaderProgram);
        }

        private int CreateShaderProgram()
        {
            const string vertexShaderSource = @"
#version 330 core
layout (location = 0) in vec2 aPosition;
void main()
{
    gl_Position = vec4(aPosition, 0.0, 1.0);
}";

            const string fragmentShaderSource = @"
#version 330 core
out vec4 FragColor;
void main()
{
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}";

            int vertexShader = CompileShader(vertexShaderSource, ShaderType.VertexShader);
            int fragmentShader = CompileShader(fragmentShaderSource, ShaderType.FragmentShader);

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            // Check for linking errors
            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(shaderProgram);
                throw new Exception($"Error linking shader program: {infoLog}");
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private int CompileShader(string source, ShaderType type)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            // Check for compile errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling {type}: {infoLog}");
            }

            return shader;
        }
    }
}
