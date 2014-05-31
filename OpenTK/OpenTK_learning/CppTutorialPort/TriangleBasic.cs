using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;


namespace CppTutorialPort
{
    /// <summary>
    /// Port of CPP OpenGL tutorial to OpenTK
    /// </summary>
    class TriangleBasic : GameWindow
    {
        private string vertexSource = @"
#version 150 core

in vec2 position;

void main() {
gl_Position = vec4(position, 0.0, 1.0);
}";

        private string fragmentSource = @"
#version 150 core

out vec4 outColor;

void main() {
outColor = vec4(1.0, 1.0, 1.0, 1.0);
}";

        private int vertexShader,
            fragmentShader,
            shaderProgram,
            vao,
            vbo,
            posAttrib;

        private Vector2[] vertices =
        {
            new Vector2(0.0f, 0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(-0.5f, -0.5f),
        };
        
        public TriangleBasic()
            : base(640, 480, new GraphicsMode(), "Basic Hello World Triangle Port",
                0, DisplayDevice.Default, 3, 2, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            //create vertex array object
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // crate vertex buffer object
            vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                new IntPtr(vertices.Length * Vector2.SizeInBytes),
                vertices, BufferUsageHint.StaticDraw);


            // create shader
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);

            Debug.WriteLine(GL.GetShaderInfoLog(vertexShader));
            Debug.WriteLine(GL.GetShaderInfoLog(fragmentShader));

            // Link the vertex and fragment shader into a shader program
            shaderProgram = GL.CreateProgram();

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.BindFragDataLocation(shaderProgram, 0, "outColor");
            GL.LinkProgram(shaderProgram);
            GL.UseProgram(shaderProgram);

            GL.LinkProgram(shaderProgram);
            Debug.WriteLine(GL.GetProgramInfoLog(shaderProgram));
            GL.UseProgram(shaderProgram);

            // specify the layout of the vertex data
            posAttrib = GL.GetAttribLocation(shaderProgram, "position");
            GL.EnableVertexAttribArray(posAttrib);
            GL.VertexAttribPointer(posAttrib, 2, VertexAttribPointerType.Float, false, 0, 0);

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                Exit();
                CleanUp();
            }

        }



        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(System.Drawing.Color.Green);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw a triangle from the 3 vertices
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }
        private void CleanUp()
        {
            GL.DeleteProgram(shaderProgram);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteBuffers(1, ref vbo);
            GL.DeleteVertexArrays(1, ref vao);
        }

        static void Main(string[] args)
        {
            using (var example = new TriangleBasic())
            {
                example.Run(30);
            }
        }
    }
}
