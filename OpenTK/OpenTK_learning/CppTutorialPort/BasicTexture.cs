using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CppTutorialPort
{
    class BasicTexture : GameWindow, IExample
    {
        private string vertexSource = @"
#version 150 core
in vec2 position;
in vec3 color;
in vec2 texcoord;
out vec3 Color;
out vec2 Texcoord;
void main() {
Color = color;
Texcoord = texcoord;
gl_Position = vec4(position, 0.0, 1.0);
}";

        private string fragmentSource = @"
#version 150 core
in vec3 Color;
in vec2 Texcoord;
out vec4 outColor;
uniform sampler2D tex;
void main() {
outColor = texture(tex, Texcoord) * vec4(Color, 1.0);
}";

        private int vertexShader,
            fragmentShader,
            shaderProgram,
            vao,
            vbo,
            ebo,
            posAttrib,
            colAttrib,
            texAttrib,
            tex,
            width,
            height;

        private float[] vertices =
        {
            // position   //Color           //Texcoords
            -0.5f,  0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, // Top-left
             0.5f,  0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // Top-right
             0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, // Bottom-right
            -0.5f, -0.5f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f  // Bottom-left
        };

        private int[] elements =
        {
            0, 1, 2,
            2, 3, 0
        };
        
        public BasicTexture()
            : base(800, 600, new GraphicsMode(), "Basic Texture Port",
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

            // Create an element array
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                new IntPtr(elements.Length * sizeof(int)),
                elements, BufferUsageHint.StaticDraw);

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
            GL.VertexAttribPointer(posAttrib, 2, VertexAttribPointerType.Float, false, 
                7 * sizeof(float), 0);

            colAttrib = GL.GetAttribLocation(shaderProgram, "color");
            GL.EnableVertexAttribArray(colAttrib);
            GL.VertexAttribPointer(colAttrib, 3, VertexAttribPointerType.Float, 
                false, 7 * sizeof(float),
                new IntPtr(2 * sizeof(float)));

            texAttrib = GL.GetAttribLocation(shaderProgram, "texcoord");
            GL.EnableVertexAttribArray(texAttrib);
            GL.VertexAttribPointer(texAttrib, 2, VertexAttribPointerType.Float,
                false, 7 * sizeof(float), new IntPtr(5 * sizeof(float)));

            // Load texture
            tex = GL.GenTexture();

            Bitmap bmp = new Bitmap("Resources//cat.jpg");
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            


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
            GL.ClearColor(System.Drawing.Color.Beige);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            // Draw a triangle from the 3 vertices
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.DrawElements(PrimitiveType.Triangles, elements.Length,
                DrawElementsType.UnsignedInt, IntPtr.Zero);

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
