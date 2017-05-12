namespace Smellyriver.TankInspector.DataAnalysis
{
	internal interface ICustomData
    {
        object Subtract(object other);

        double GetDeltaRatio(object delta);

        bool CanCompare { get; }

        string ToString(bool explicitSign);
    }
}
