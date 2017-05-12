using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class VirtualSurveyingDevice : VirtualDamageableComponent, ISurveyingDevice
    {


        public override string Name => App.GetLocalizedString("SurveyingDevice");
    }
}
