using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
	internal abstract class CrewSkillBase : IModifier
    {
        public abstract string Name { get; }
        public abstract string ShortDescription { get; }
        public abstract string Description { get; }
        public abstract string Icon { get; }
        public virtual int Priority => 0;

	    public virtual bool IsEnabled => true;
	    public abstract DuplicatedCrewSkillPolicy DuplicatedCrewSkillPolicy { get; }

        protected abstract CrewSkillType Type { get; }

        public abstract CrewRoleType CrewRole { get; }

        public Dictionary<string, double> Parameters { get; }

        public CrewSkillBase()
        {
            this.Parameters = new Dictionary<string, double>();
        }

        public abstract string[] EffectiveDomains { get; }

        public string EffectiveDomain => this.EffectiveDomains[0];

	    public void Execute(ModificationContext context, object args)
        {
            var currentLevel = (double)args;

            var levels = new List<double>();
            levels.Add(currentLevel);

            int i = 0;

            for (; ; ++i)
            {
                var level = context.GetValue(this.EffectiveDomains[0], $"__level{i}");
                if (level == null)
                    break;
	            levels.Add(level.Value);
            }

            context.SetValue(this.EffectiveDomains[0], $"__level{i}", currentLevel);

            double actualValue;

            switch (this.DuplicatedCrewSkillPolicy)
            {
                case DuplicatedCrewSkillPolicy.Average:
                    actualValue = levels.Average();
                    break;
                case DuplicatedCrewSkillPolicy.Highest:
                    actualValue = levels.Max();
                    break;
                case DuplicatedCrewSkillPolicy.Lowest:
                    actualValue = levels.Min();
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (this.Type == CrewSkillType.Skill || (this.Type == CrewSkillType.Perk && actualValue >= 100))
                this.Execute(context, actualValue);
            else
                this.Clear(context);
        }

        protected abstract void Execute(ModificationContext context, double level);

        protected abstract void Clear(ModificationContext context);
    }
}
