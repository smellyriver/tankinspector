using System;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class ExtraPitchLimit : PitchLimits
    {
        public double Range { get; }

        public double HalfRange => this.Range / 2;

	    public ExtraPitchLimit(double elevation, double depression, double range)
            : base(elevation, depression)
        {
            this.Range = range;
        }

        public ExtraPitchLimit()
        {

        }

        //public override void Deserialize(XmlReader reader)
        //{
        //    var values = reader.ReadString();
        //    var valuesArray = values.Split(' ');
        //    base.Elevation = -double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
        //    base.Depression = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
        //    this.Range = double.Parse(valuesArray[2], CultureInfo.InvariantCulture) * 2;
        //}
    }
}
