namespace Smellyriver.TankInspector.Modeling
{
	internal interface IDamageable
    {
        double MaxHealth { get; }
        double MaxRegenHealth { get; }
        double RepairCostFactor { get; }
        double BulkHealthFactor { get; }
    }
}
