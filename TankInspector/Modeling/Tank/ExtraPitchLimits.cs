using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class ExtraPitchLimits: ISectionDeserializable
    {
        private ExtraPitchLimit _front;
        [ExpandableStat("FrontExtraPitchLimit")]
        public ExtraPitchLimit Front => _front;

	    private ExtraPitchLimit _back;
        [ExpandableStat("BackExtraPitchLimit")]
        public ExtraPitchLimit Back => _back;

	    private double _transition;

        [Stat("ExtraPitchLimitTransition", DataAnalysis.ComparisonMode.Plain)]
        public double Transition => _transition;


	    public double FrontLimitsLeftBound => this.Front == null ? 0 : 360 - this.Front.HalfRange;

	    public double FrontLimitsRightBound => this.Front == null ? 0 : this.Front.HalfRange;

	    public double FrontTransitionLeftBound => this.Front == null ? 0 : 360 - this.Front.HalfRange - this.Transition;

	    public double FrontTransitionRightBound => this.Front == null ? 0 : this.Front.HalfRange + this.Transition;


	    public double BackLimitsLeftBound => this.Back == null ? 180 : 180 + this.Back.HalfRange;

	    public double BackLimitsRightBound => this.Back == null ? 180 : 180 - this.Back.HalfRange;

	    public double BackTransitionLeftBound => this.Back == null ? 180 : 180 + this.Back.HalfRange + this.Transition;

	    public double BackTransitionRightBound => this.Back == null ? 180 : 180 - this.Back.HalfRange - this.Transition;

	    public ExtraPitchLimits(ExtraPitchLimit front, ExtraPitchLimit back, double transition)
        {
            _front = front;
            _back = back;
            _transition = transition;
        }

        public ExtraPitchLimits()
        {

        }

        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case SectionDeserializableImpl.TitleToken:
                    return false;
                case "front":                   
                    reader.Read(out _front);
                    return true;
                case "back":
                    reader.Read(out _back);
                    return true;
                case "transition":
                    reader.Read(out _transition);
                    return true;
                default:
                    return false;
            }
        }


        bool ISectionDeserializable.IsWrapped => false;
    }
}
