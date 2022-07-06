using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAS_Digimon
{
    class Cubemap
    {
        private readonly string path = "../../../";

        private List<Vector3> vertices = new List<Vector3>();

        private int _vertexBufferObject;
        private int _vertexArrayObject;

        private Shader _shader;
        private Texture _texture;

        private string vertName;
        private string fragName;
        private List<string> cubemapPaths;

        public Cubemap(string vertName, string fragName, List<string> cubemapPaths)
        {
            this.vertName = vertName;
            this.fragName = fragName;
            this.cubemapPaths = cubemapPaths;

            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //Titik 1
            temp_vector.X = -0.5f;
            temp_vector.Y = 0.5f;
            temp_vector.Z = -0.5f;
            tempVertices.Add(temp_vector);

            //Titik 2
            temp_vector.X = 0.5f;
            temp_vector.Y = 0.5f;
            temp_vector.Z = -0.5f;
            tempVertices.Add(temp_vector);

            //Titik 3
            temp_vector.X = -0.5f;
            temp_vector.Y = -0.5f;
            temp_vector.Z = -0.5f;
            tempVertices.Add(temp_vector);

            //Titik 4
            temp_vector.X = 0.5f;
            temp_vector.Y = -0.5f;
            temp_vector.Z = -0.5f;
            tempVertices.Add(temp_vector);

            //Titik 5
            temp_vector.X = -0.5f;
            temp_vector.Y = 0.5f;
            temp_vector.Z = 0.5f;
            tempVertices.Add(temp_vector);

            //Titik 6
            temp_vector.X = 0.5f;
            temp_vector.Y = 0.5f;
            temp_vector.Z = 0.5f;
            tempVertices.Add(temp_vector);

            //Titik 7
            temp_vector.X = -0.5f;
            temp_vector.Y = -0.5f;
            temp_vector.Z = 0.5f;
            tempVertices.Add(temp_vector);

            //Titik 8
            temp_vector.X = 0.5f;
            temp_vector.Y = -0.5f;
            temp_vector.Z = 0.5f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<int>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            for (int i = 0; i < tempIndices.Count; i++)
            {
                vertices.Add(tempVertices[tempIndices[i]]);
            }
        }

        public void load()
        {
            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader = new Shader(path + "Shaders/" + vertName, path + "Shaders/" + fragName);
            _shader.Use();

            _texture = Texture.LoadFromFile(cubemapPaths);
            _texture.Use(TextureUnit.Texture0);
        }

        public void render(Matrix4 camera_view, Matrix4 camera_projection)
        {
            GL.BindVertexArray(_vertexArrayObject);

            _shader.Use();
            _texture.Use(TextureUnit.Texture0);

            _shader.SetMatrix4("view", new Matrix4(new Matrix3(camera_view)));
            _shader.SetMatrix4("projection", camera_projection);

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count);
        }
    }
}
