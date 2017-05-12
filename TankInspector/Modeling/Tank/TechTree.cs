using System;
using System.Linq;
using Smellyriver.Utilities;
using HierachyInfoCollection = System.Collections.Generic.List<Smellyriver.TankInspector.Modeling.TechTree.HierachyInfoKey>;
using HierachyDictionary = System.Collections.Generic.Dictionary<Smellyriver.TankInspector.Modeling.TankObjectKey, System.Collections.Generic.List<Smellyriver.TankInspector.Modeling.TechTree.HierachyInfoKey>>;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.IO.XmlDecoding;
using Smellyriver.TankInspector.IO;
using System.IO;

namespace Smellyriver.TankInspector.Modeling
{
    

    [Serializable]
    internal class TechTree
    {

        private const string LayoutRelativePath = "gui/flash/techtree";
        private const string LayoutFilePostfix = "-tree.xml";

        [Serializable]
        internal class HierachyInfoKey
        {
            public TankObjectKey Target { get; set; }
            public int ExperiencePrice { get; set; }

            public HierachyInfoKey(TankObjectKey target, int experiencePrice)
            {
                this.Target = target;
                this.ExperiencePrice = experiencePrice;
            }

            public HierachyInfo<TTankObject> ToHierachyInfo<TTankObject>(NationalDatabase nation)
                where TTankObject : TankObject
            {
                return new HierachyInfo<TTankObject>((TTankObject)nation.GetTankObject(this.Target), this.ExperiencePrice);
            }
        }

        [NonSerialized]
        private HierachyDictionary _predecessorKeys;

        private TankObjectKey[] _predecessorKeysKeys;
        private HierachyInfoKey[][] _predecessorKeysValues;

        [NonSerialized]
        private HierachyDictionary _successorKeys;

        private TankObjectKey[] _successorKeysKeys;
        private HierachyInfoKey[][] _successorKeysValues;

        [NonSerialized]
        private NationalDatabase _nation;
        internal NationalDatabase Nation
        {
            get => _nation;
	        set => _nation = value;
        }

	    public TechTreeLayout Layout { get; private set; }

	    public TechTree(NationalDatabase nation)
        {
            _predecessorKeys = new HierachyDictionary();
            _successorKeys = new HierachyDictionary();

            this.Nation = nation;

            this.BuildTechTree();
            this.LoadLayout();
        }

        private void LoadLayout()
        {
            var packageFile = Path.Combine(this.Nation.Database.RootPath, PackageImage.GuiPackage);
            var path = Path.Combine(LayoutRelativePath, this.Nation.Key + LayoutFilePostfix).Replace('\\', '/');

            if (PackageStream.IsFileExisted(packageFile, path))
            {
                using (var stream = new PackageStream(packageFile, path))
                {
                    using (var reader = new BigworldXmlReader(stream))
                    {
                        reader.ReadStartElement();
                        this.Layout = new TechTreeLayout(this);
                        this.Layout.Deserialize(reader);
                    }
                }
            }
        }

        public HierachyInfo<TTankObject>[] GetPredecessors<TTankObject>(TankObject target)
            where TTankObject : TankObject
        {
	        if (_predecessorKeys.TryGetValue(target.ObjectKey, out HierachyInfoCollection predecessorKeys))
				return predecessorKeys.Select(key => key.ToHierachyInfo<TTankObject>(this.Nation)).ToArray();
	        return new HierachyInfo<TTankObject>[0];
        }

        public HierachyInfo<TTankObject>[] GetSuccessors<TTankObject>(TankObject target)
            where TTankObject : TankObject
        {
	        if (_successorKeys.TryGetValue(target.ObjectKey, out HierachyInfoCollection successorKeys))
				return successorKeys.Select(key => key.ToHierachyInfo<TTankObject>(this.Nation)).ToArray();
	        return new HierachyInfo<TTankObject>[0];
        }

        private void AddPredecessor(TankObject succssor, TankObject predecessor, int experiencePrice)
        {
            var predecessors = _predecessorKeys.GetOrCreate(succssor.ObjectKey, () => new HierachyInfoCollection());
            predecessors.Add(new HierachyInfoKey(predecessor.ObjectKey, experiencePrice));
        }

        private void AddSuccessor(TankObject predecessor, TankObject successor, int experiencePrice)
        {
            var successors = _successorKeys.GetOrCreate(predecessor.ObjectKey, () => new HierachyInfoCollection());
            successors.Add(new HierachyInfoKey(successor.ObjectKey, experiencePrice));
        }

        private void BuildTechTree()
        {
            foreach (var tank in this.Nation.Tanks.Values)
            {
                foreach (var module in tank.AllModules)
                {
                    foreach (var unlockInfo in module.Unlocks)
                    {
                        if (!unlockInfo.IsResolved)
                            unlockInfo.Resolve(tank);

                        if (unlockInfo.TankObject is Tank)
                        {
                            var unlockedTank = (Tank)unlockInfo.TankObject;
                            this.AddPredecessor(unlockedTank, tank, unlockInfo.ExperiencePrice);
                            this.AddSuccessor(tank, unlockedTank, unlockInfo.ExperiencePrice);
                            this.AddSuccessor(module, unlockedTank, unlockInfo.ExperiencePrice);
                        }
                        else
                        {
                            this.AddPredecessor(unlockInfo.TankObject, module, unlockInfo.ExperiencePrice);
                            this.AddSuccessor(module, unlockInfo.TankObject, unlockInfo.ExperiencePrice);
                        }

                    }
                }
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _predecessorKeysKeys = _predecessorKeys.Keys.ToArray();
            _predecessorKeysValues = _predecessorKeys.Values.Select(keys => keys.ToArray()).ToArray();

            _successorKeysKeys = _successorKeys.Keys.ToArray();
            _successorKeysValues = _successorKeys.Values.Select(keys => keys.ToArray()).ToArray();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _predecessorKeys = new HierachyDictionary();
            _successorKeys = new HierachyDictionary();

            for (int i = 0; i < _predecessorKeysKeys.Length; ++i)
                _predecessorKeys.Add(_predecessorKeysKeys[i], _predecessorKeysValues[i].ToList());

            for (int i = 0; i < _successorKeysKeys.Length; ++i)
                _successorKeys.Add(_successorKeysKeys[i], _successorKeysValues[i].ToList());

            if (this.Layout != null)
                this.Layout.TechTree = this;
        }
    }
}
