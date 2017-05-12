namespace Smellyriver.TankInspector.Modeling
{
	internal interface IModifier
    {
        string[] EffectiveDomains { get; }
        void Execute(ModificationContext context, object args);

    }
}
