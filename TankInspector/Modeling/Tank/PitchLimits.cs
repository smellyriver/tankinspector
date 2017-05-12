using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class PitchLimits : ISectionDeserializable
    {
        [Serializable]
        public struct Component
        {
            public double Angle { get; set; }
            public double Limit { get; set; }

            public Component(double angle, double limit)
                : this()
            {
                this.Angle = angle;
                this.Limit = limit;
            }
        }

        [Stat("Elevation", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Elevation { get; protected set; }

        [Stat("Depression", DataAnalysis.ComparisonMode.HigherBetter)]
        public double Depression { get; protected set; }

        public Component[] ElevationData { get; private set; }
        public Component[] DepressionData { get; private set; }
        public bool IsPost909Format { get; private set; }

        public PitchLimits(double elevation, double depression)
        {
            this.Elevation = elevation;
            this.Depression = depression;
        }

        public PitchLimits()
        {

        }

        //public virtual void Deserialize(XmlReader reader)
        //{



        //    // pitchlimits structure is changed in 0.9.10

        //    if (reader.NodeType == XmlNodeType.Text)
        //    {
        //        var values = reader.ReadString();
        //        var valuesArray = values.Split(' ');
        //        this.Elevation = -double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
        //        this.Depression = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
        //    }

        //    if (reader.IsStartElement("minPitch"))
        //    {
        //        reader.ReadStartElement("minPitch");
        //        var values = reader.ReadString();
        //        var valuesArray = values.Split(' ');
        //        this.Elevation = -double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
        //        reader.ReadEndElement();

        //        reader.ReadStartElement("maxPitch");
        //        values = reader.ReadString();
        //        valuesArray = values.Split(' ');
        //        this.Depression = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);
        //        reader.ReadEndElement();
        //    }
        //}


        public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }


        public bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case null:
                    var values = reader.ReadString();
                    var valuesArray = values.Split(' ');
                    this.Elevation = -double.Parse(valuesArray[0], CultureInfo.InvariantCulture);
                    this.Depression = double.Parse(valuesArray[1], CultureInfo.InvariantCulture);

                    this.IsPost909Format = false;

                    this.ElevationData = new[] { new Component(0, this.Elevation), new Component(1, this.Elevation) };
                    this.DepressionData = new[] { new Component(0, this.Depression), new Component(1, this.Depression) };
                    return true;
                case "minPitch":
                    var elevationData = this.ParseComponents(reader.ReadString(), -1);
                    if (this.ElevationData != null && elevationData == null)
                        return true;
                    this.ElevationData = elevationData;
                    this.IsPost909Format = true;
                    if (this.ElevationData != null)
                        this.Elevation = this.ElevationData.Max(e => e.Limit);
                    return true;
                case "maxPitch":
                    var depressionData = this.ParseComponents(reader.ReadString(), 1);
                    if (this.DepressionData != null && depressionData == null)
                        return true;
                    this.DepressionData = depressionData;
                    this.IsPost909Format = true;
                    if (this.DepressionData != null)
                        this.Depression = this.DepressionData.Max(e => e.Limit);
                    return true;
                default:
                    return false;
            }
        }

        private Component[] ParseComponents(string values, double factor)
        {
            if (string.IsNullOrWhiteSpace(values))
                return null;

            var valuesArray = values.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var components = new List<Component>();

            for (var i = 0; i < valuesArray.Length; i += 2)
            {
                var angle = double.Parse(valuesArray[i], CultureInfo.InvariantCulture);
                var limit = double.Parse(valuesArray[i + 1], CultureInfo.InvariantCulture) * factor;
                components.Add(new Component(angle, limit));
            }

            return components.ToArray();

        }

        public bool IsWrapped => false;
    }
}
