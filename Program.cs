
using LearnOpenTK.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;



namespace UAS_Digimon
{
    class Program
    {
        static void Main(string[] args)
        {
            var nativeWindowSetting = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 800),
                Title = "Digimon"
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSetting))
            {
                window.Run();
            }
        }
    }
}
