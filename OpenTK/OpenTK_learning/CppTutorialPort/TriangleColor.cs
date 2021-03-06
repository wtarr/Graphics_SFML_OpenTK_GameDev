﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CppTutorialPort
{
    /// <summary>
    /// Cpp to C# port
    /// Based on Open.gl tutorial
    /// http://open.gl/content/code/c2_color_triangle.txt
    /// </summary>
    class TriangleColor : GameWindow, IExample
    {
       private string vertexSource = @"
#version 150 core

in vec2 position;
in vec3 color;

out vec3 Color;

void main() {
Color = color;
gl_Position = vec4(position, 0.0, 1.0);
}";

        private string fragmentSource = @"
#version 150 core
in vec3 Color;
out vec4 outColor;

void main() {
outColor = vec4(Color, 1.0);
}";

        private int vertexShader,
            fragmentShader,
            shaderProgram,
            vao,
            vbo,
            posAttrib,
            colAttrib;

        private float[] vertices =
        {
         0.0f,  0.5f, 1.0f, 0.0f, 0.0f,
         0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.0f, 0.0f, 1.0f
        };
        
        public TriangleColor()
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
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(vertices.Length * sizeof(float)),
                vertices, BufferUsageHint.StaticDraw);

            // create and compile the vertex shader
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
            GL.VertexAttribPointer(posAttrib, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            


            colAttrib = GL.GetAttribLocation(shaderProgram, "color");
            GL.EnableVertexAttribArray(colAttrib);
            GL.VertexAttribPointer(colAttrib, 3, VertexAttribPointerType.Float, false,
                5 * sizeof(float), new IntPtr(2 * sizeof(float)));


        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                Exit();
                //CleanUp();

                Dispose();
                
            }

        }



        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(System.Drawing.Color.Green);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw a triangle from the 3 vertices
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.Flush();
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

        public void Execute()
        {
            this.Run(30);
        }
    }
}
