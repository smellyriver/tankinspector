using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class BenchmarkSurveyingDevice : VirtualSurveyingDevice
    {
        internal static BenchmarkSurveyingDevice Create(IEnumerable<ISurveyingDevice> enumerable)
        {
            var benchmark = new BenchmarkSurveyingDevice();
            BenchmarkDamageableComponent.Initialize(benchmark, enumerable);
            return benchmark;
        }
    }
}
