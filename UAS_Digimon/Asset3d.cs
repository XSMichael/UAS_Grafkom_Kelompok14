using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace UAS_Digimon
{
    class Asset3d
    {
        private readonly string path = "../../../";

        private List<Vector3> vertices = new List<Vector3>();
        private List<uint> indices = new List<uint>();
        private List<uint> indices_n = new List<uint>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector3> tempNormals = new List<Vector3>();
        private List<Vector3> texCoords = new List<Vector3>();

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        private Shader _shader;
        private Texture _texture;

        private Matrix4 model = Matrix4.Identity;
        private Matrix4 normalMat = Matrix4.Identity;

        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        public Vector3 objColor;
        public uint type;//render type

        private string vertName;
        private string fragName;

        public List<Vector3> _euler = new List<Vector3>();
        public Vector3 objectCenter = Vector3.Zero;

        public List<Asset3d> child = new List<Asset3d>();

        public float ambientStrength;
        public float specStrength;
        public float spotAngle;
        public Vector3 lightDirection;

        public Asset3d(string vertName, string fragName, Vector3 ambient, Vector3 diffuse, Vector3 specular, float alpha = 1)
        {
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.vertName = vertName;
            this.fragName = fragName;
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }
        public Asset3d(string file_path, string vertName, string fragName, Vector3 ambient, Vector3 diffuse, Vector3 specular, float alpha = 1)
        {
            LoadObjectFile(file_path);
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.vertName = vertName;
            this.fragName = fragName;
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }
        public Asset3d(string vertName, string fragName, Vector3 objColor)
        {
            this.objColor = objColor;
            this.vertName = vertName;
            this.fragName = fragName;
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }
        public void LoadObjectFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Unable to open \"" + path + "\", does not exist.");
            }

            using (StreamReader streamReader = new StreamReader(path))
            {
                while (!streamReader.EndOfStream)
                {
                    List<string> words = new List<string>(streamReader.ReadLine().ToLower().Split(' '));
                    words.RemoveAll(s => s == string.Empty);

                    if (words.Count == 0)
                        continue;

                    string type = words[0];
                    words.RemoveAt(0);


                    switch (type)
                    {
                        case "v":
                            vertices.Add(new Vector3((float.Parse(words[0], CultureInfo.InvariantCulture.NumberFormat) / 5),
                                (float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat) / 5),
                                (float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat) / 5)));
                            break;

                        case "vt":
                            /*texCoords.Add(new Vector3(float.Parse(words[0]), float.Parse(words[1]),
                                                            words.Count < 3 ? 0 : float.Parse(words[2])));*/
                            break;

                        case "vn":
                            tempNormals.Add(new Vector3(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]))*-1);
                            break;

                        case "f":
                            foreach (string w in words)
                            {
                                if (w.Length == 0)
                                    continue;

                                string[] comps = w.Split('/');

                                indices.Add(uint.Parse(comps[0]) - 1);
                                indices_n.Add(uint.Parse(comps[2]) - 1);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        public void load()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            if (indices_n.Count > 0)
            {
                for (int i = 0; i < indices_n.Count; i++)
                {
                   /* Console.WriteLine(i);
                    Console.WriteLine(indices_n.Count);*/
                    normals.Add(tempNormals[(int)indices_n[i]]);
                }
            }

            if (texCoords.Count == 0 && normals.Count == 0)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
            }
            else if (texCoords.Count > 0)
            {
                var combinedData = new List<Vector3>();
                for (int i = 0; i < vertices.Count; i++)
                {
                    combinedData.Add(vertices[i]);
                    combinedData.Add(texCoords[i]);
                }

                GL.BufferData(BufferTarget.ArrayBuffer, combinedData.Count * Vector3.SizeInBytes, combinedData.ToArray(), BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }
            else if (normals.Count > 0)
            {
                var combinedData = new List<Vector3>(); 

                for (int i = 0; i < vertices.Count; i++)
                {
                    combinedData.Add(vertices[i]);
                    if (i > normals.Count - 1)
                    {
                        combinedData.Add(new Vector3(0, 0, 0));
                    }
                    else
                    {
                        combinedData.Add(normals[i]);
                    }
                }


                GL.BufferData(BufferTarget.ArrayBuffer, combinedData.Count * Vector3.SizeInBytes, combinedData.ToArray(), BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }

            if (indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            _shader = new Shader(path + "Shaders/" + vertName, path + "Shaders/" + fragName);
            _shader.Use();

            if (texCoords.Count > 0)
            {
                _texture = Texture.LoadFromFile(path + "Textures/Cubemap/top.png");
                _texture.Use(TextureUnit.Texture0);
            }

            foreach (var i in child)
            {
                i.load();
            }


        }

        public void render(int line, Matrix4 camera_view, Matrix4 camera_projection, Vector3 cameraPos, List<Asset3d> dirlightList, List<Asset3d> pointlightList, List<Asset3d> spotlightList)
        {
            _shader.Use();

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);

            GL.BindVertexArray(_vertexArrayObject);
            if (texCoords.Count > 0)
            {
                _texture.Use(TextureUnit.Texture0);
            }
            else
            {
                if (objColor != Vector3.Zero)
                {
                    _shader.SetVector3("objColor", objColor);
                }
                else
                {
                    _shader.SetVector3("material.ambient", ambient);
                    _shader.SetVector3("material.specular", specular);
                    _shader.SetVector3("material.diffuse", diffuse);
                }
            }


            if (normals.Count > 0)
            {
                _shader.SetMatrix4("normalMat", normalMat);
                _shader.SetVector3("viewPos", cameraPos);

                for (int i = 0; i < dirlightList.Count; i++)
                {
                    //_shader.SetVector3($"directList[{i}].lightPos", dirlightList[i].objectCenter);
                    _shader.SetVector3($"directList[{i}].lightColor", dirlightList[i].objColor);
                    _shader.SetFloat($"directList[{i}].ambientStre", dirlightList[i].ambientStrength);
                    _shader.SetFloat($"directList[{i}].specStre", dirlightList[i].specStrength);

                    _shader.SetVector3($"directList[{i}].lightDir", -dirlightList[i].lightDirection);
                }
                for (int i = 0; i < pointlightList.Count; i++)
                {
                    _shader.SetVector3($"pointList[{i}].lightPos", pointlightList[i].objectCenter);
                    _shader.SetVector3($"pointList[{i}].lightColor", pointlightList[i].objColor);
                    _shader.SetFloat($"pointList[{i}].ambientStre", pointlightList[i].ambientStrength);
                    _shader.SetFloat($"pointList[{i}].specStre", pointlightList[i].specStrength);
                }
                for (int i = 0; i < spotlightList.Count; i++)
                {
                    //_shader.SetVector3($"spotList[{i}].lightPos", spotlightList[i].objectCenter);
                    _shader.SetVector3($"spotList[{i}].lightPos", cameraPos);
                    _shader.SetVector3($"spotList[{i}].lightColor", spotlightList[i].objColor);
                    _shader.SetFloat($"spotList[{i}].ambientStre", spotlightList[i].ambientStrength);
                    _shader.SetFloat($"spotList[{i}].specStre", spotlightList[i].specStrength);

                    _shader.SetVector3($"spotList[{i}].spotDir", -spotlightList[i].lightDirection);

                    _shader.SetFloat($"spotList[{i}].spotAngleCos", (float)MathHelper.Cos(MathHelper.DegreesToRadians(spotlightList[i].spotAngle)));
                }



            }
            if (indices.Count != 0)
            {
                switch (line)
                {
                    case 1:
                        GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
                        break;
                    case -1:
                        GL.DrawElements(PrimitiveType.LineStrip, indices.Count, DrawElementsType.UnsignedInt, 0);
                        break;
                }
            }
            else
            {
                switch (line)
                {
                    case 1:
                        GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count);
                        break;
                    case -1:
                        GL.DrawArrays(PrimitiveType.LineStrip, 0, vertices.Count);
                        break;
                }

            }

            foreach (var i in child)
            {
                i.render(line, camera_view, camera_projection,cameraPos,dirlightList,pointlightList,spotlightList);
            }

        }


        #region setLights

        public void setPointLight(float ambientStrength, float specStrength)
        {
            this.ambientStrength = ambientStrength;
            this.specStrength = specStrength;
        }

        public void setDirectLight(float ambientStrength, float specStrength, Vector3 lightDirection)
        {
            setPointLight(ambientStrength, specStrength);
            this.lightDirection = lightDirection;
        }

        public void setSpotLight(float ambientStrength, float specStrength, Vector3 spotDirection, float spotAngle)
        {
            setDirectLight(ambientStrength, specStrength, spotDirection);
            this.spotAngle = spotAngle;
        }
        #endregion

        #region solidObjects

        public void createCuboid(float x_, float y_, float z_, float length, bool useNormals, bool useTextures)
        {
            objectCenter = new Vector3(x_, y_, z_);

            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //Titik 1
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 2
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 3
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 4
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 5
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 6
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 7
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 8
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
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

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }



            if (useTextures)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                texCoords = new List<Vector3>()
                {
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0)
                };
            }

            if (!useNormals && !useTextures)
            {
                vertices = tempVertices;
                indices = tempIndices;
            }
        }

        public void createCuboid(float x_, float y_, float z_, float lengthX, float lengthY, float lengthZ, bool useNormals, bool useTextures)
        {
            objectCenter = new Vector3(x_, y_, z_);

            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //Titik 1
            temp_vector.X = x_ - lengthX / 2.0f;
            temp_vector.Y = y_ + lengthY / 2.0f;
            temp_vector.Z = z_ - lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 2
            temp_vector.X = x_ + lengthX / 2.0f;
            temp_vector.Y = y_ + lengthY / 2.0f;
            temp_vector.Z = z_ - lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 3
            temp_vector.X = x_ - lengthX / 2.0f;
            temp_vector.Y = y_ - lengthY / 2.0f;
            temp_vector.Z = z_ - lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 4
            temp_vector.X = x_ + lengthX / 2.0f;
            temp_vector.Y = y_ - lengthY / 2.0f;
            temp_vector.Z = z_ - lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 5
            temp_vector.X = x_ - lengthX / 2.0f;
            temp_vector.Y = y_ + lengthY / 2.0f;
            temp_vector.Z = z_ + lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 6
            temp_vector.X = x_ + lengthX / 2.0f;
            temp_vector.Y = y_ + lengthY / 2.0f;
            temp_vector.Z = z_ + lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 7
            temp_vector.X = x_ - lengthX / 2.0f;
            temp_vector.Y = y_ - lengthY / 2.0f;
            temp_vector.Z = z_ + lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 8
            temp_vector.X = x_ + lengthX / 2.0f;
            temp_vector.Y = y_ - lengthY / 2.0f;
            temp_vector.Z = z_ + lengthZ / 2.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
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

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }



            if (useTextures)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                texCoords = new List<Vector3>()
                {
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0)
                };
            }

            if (!useNormals && !useTextures)
            {
                vertices = tempVertices;
                indices = tempIndices;
            }
        }

        public void createEllipsoid(float x, float y, float z, float radX, float radY, float radZ, float sectorCount, float stackCount)
        {
            objectCenter = new Vector3(x, y, z);

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * pi / sectorCount;
            float stackStep = pi / stackCount;
            float sectorAngle, stackAngle, tempX, tempY, tempZ;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = pi / 2 - i * stackStep;
                tempX = radX * (float)Math.Cos(stackAngle);
                tempY = radY * (float)Math.Sin(stackAngle);
                tempZ = radZ * (float)Math.Cos(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x + tempX * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y + tempY;
                    temp_vector.Z = z + tempZ * (float)Math.Sin(sectorAngle);

                    vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);

                    }

                    if (i != stackCount - 1)
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }
                }
            }
        }

        #endregion

        #region transforms
        public void rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            var radAngle = MathHelper.DegreesToRadians(angle);

            var arbRotationMatrix = new Matrix4
                (
                new Vector4((float)(Math.Cos(radAngle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) + vector.Z * Math.Sin(radAngle)), (float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.Y * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) - vector.Z * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.X * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.Y * Math.Sin(radAngle)), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.X * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(radAngle))), 0),
                Vector4.UnitW
                );

            model *= Matrix4.CreateTranslation(-pivot);
            model *= arbRotationMatrix;
            model *= Matrix4.CreateTranslation(pivot);

            normalMat = Matrix4.Transpose(Matrix4.Invert(model));

            for (int i = 0; i < 3; i++)
            {
                _euler[i] = Vector3.Normalize(getRotationResult(pivot, vector, radAngle, _euler[i], true));
            }

            objectCenter = getRotationResult(pivot, vector, radAngle, objectCenter);

            foreach (var i in child)
            {
                i.rotate(pivot, vector, angle);
            }
        }

        public Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;

            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));

            newPosition.Y =
                temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));

            newPosition.Z =
                temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void translate(float x, float y, float z)
        {
            model *= Matrix4.CreateTranslation(x, y, z);

            normalMat = Matrix4.Transpose(Matrix4.Invert(model));

            objectCenter.X += x;
            objectCenter.Y += y;
            objectCenter.Z += z;

            foreach (var i in child)
            {
                i.translate(x, y, z);
            }
        }

        public void scale(float scaleX, float scaleY, float scaleZ, Vector3 scaleCenter)
        {
            model *= Matrix4.CreateTranslation(-scaleCenter);
            model *= Matrix4.CreateScale(scaleX, scaleY, scaleZ);
            model *= Matrix4.CreateTranslation(scaleCenter);

            normalMat = Matrix4.Transpose(Matrix4.Invert(model));

            foreach (var i in child)
            {
                i.scale(scaleX, scaleY, scaleZ, scaleCenter);
            }
        }

        public void resetEuler()
        {
            _euler.Clear();
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }
        #endregion
    }
}
