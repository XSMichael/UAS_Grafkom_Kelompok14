using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace LearnOpenTK.Common
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture
    {
        private TextureTarget selectedMode;
        public readonly int Handle;

        // Load file (gambar) teksture. path = path untuk ke file tekstur
        public static Texture LoadFromFile(string path)
        {
            // Generate handle
            int handle = GL.GenTexture();

            // Bind the handle
            GL.ActiveTexture(TextureUnit.Texture0); //ada 30 slot tekstur. maka dalam 1 objek bisa pakai max 30 tekstur
            GL.BindTexture(TextureTarget.Texture2D, handle);

            // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

            // Load the image
            using (var image = new Bitmap(path))
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);


                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    );

                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }

            //Kalau gambar lebih besar daripada objek
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); //S -> untuk sumbu x
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); //T -> untuk sumbu y
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat); // untuk z
            //kalua main" dengan CUBEMAP, menggunakan U
            //repeat -> kalau tidak full, bagian kosong akan copas teksturnya
            //mirrored repeat -> seperti repeat tapi akan di-mirror setiap di-repeat
            //Clamp_to_edge -> untuk CubeMap
            //Clamp_to_border -> bagian kosong diisi dengan 1 warna


            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            //Mipmap -> peta tekstur untuk GL

            return new Texture(handle, TextureTarget.Texture2D);
        }

        public static Texture LoadFromFile(List<string> imagePaths)
        {
            // Generate handle
            int handle = GL.GenTexture();

            // Bind the handle
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, handle);

            // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

            // Load the image
            var i = 0;
            foreach (string path in imagePaths)
            {
                using (var image = new Bitmap(path))
                {
                    var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                        0,
                        PixelInternalFormat.Rgba,
                        image.Width,
                        image.Height,
                        0,
                        PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        data.Scan0);
                }
                i++;
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            return new Texture(handle, TextureTarget.TextureCubeMap);
        }


        public Texture(int glHandle, TextureTarget textureTarget)
        {
            Handle = glHandle;
            selectedMode = textureTarget;
        }

        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use(TextureUnit unit)
        {
            switch (selectedMode)
            {
                case TextureTarget.Texture2D:
                    GL.ActiveTexture(unit);
                    GL.BindTexture(TextureTarget.Texture2D, Handle);
                    break;
                case TextureTarget.TextureCubeMap:
                    GL.ActiveTexture(unit);
                    GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
                    break;
            }
        }
    }
}
