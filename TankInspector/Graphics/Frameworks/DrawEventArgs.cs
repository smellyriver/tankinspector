using System;
using System.Windows;

namespace Smellyriver.TankInspector.Graphics.Frameworks
{
    public class DrawEventArgs : EventArgs
    {
        public DrawEventArgs()
        {
            TotalTime = TimeSpan.Zero;
        }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan DeltaTime { get; set; }
        public Size RenderSize { get; set; }
        public D3D9ImageSource Target { get; set; }
    }
}
