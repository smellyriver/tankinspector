using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class SurveyingDevice : DamageableComponent, ISurveyingDevice
    {
        public override string Name => App.GetLocalizedString("SurveyingDevice");

	    public SurveyingDevice(Database database)
            : base(database)
        {

        }
    }
}
