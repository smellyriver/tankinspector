using Smellyriver.TankInspector.Modeling;

namespace Smellyriver.TankInspector.Graphics.Scene
{
    public class TestShellInfo
    {
        public ShellType ShellType { get; set; }
        public double Caliber { get; set; }

        public TestShellInfo(ShellType shellType, double caliber)
            
        {
            this.ShellType = shellType;
            this.Caliber = caliber;
        }
    }
}
