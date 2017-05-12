namespace Smellyriver.TankInspector.UIComponents
{
	internal interface IPriorityItem
    {
        int Priority { get; }
        bool IsPrioritySufficient { get; set; }
        double DesiredHeight { get; }
    }
}
