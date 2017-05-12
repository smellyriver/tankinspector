namespace Smellyriver.TankInspector.Modeling
{
	internal interface IShell : ICommodity
    {
        string Name { get; }
        double Caliber { get; }
        ShellDamage Damage { get; }
        double ExplosionRadius { get; }
        double Gravity { get; }
        double MaxDistance { get; }
        PiercingPower PiercingPower { get; }
        double PiercingPowerLossFactorByDistance { get; }
        double Speed { get; }
        ShellType Type { get; }
    }
}
