using System;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.Utilities;
using System.ComponentModel;

namespace Smellyriver.TankInspector.Modeling
{
	internal class ModificationContext : NotificationObject, ISupportInitialize, ICloneable
    {
        private bool _isInitializing;
        private HashSet<string> _changedDomainsDuringInitialization;

        private Dictionary<string, Dictionary<string, double>> _storage;

        public ModificationContext()
        {
            _storage = new Dictionary<string, Dictionary<string, double>>();
        }

        public void SetValue(string domain, string key, double value)
        {
            var domainStorage = _storage.GetOrCreate(domain, () => new Dictionary<string, double>());
            domainStorage[key] = value;

            this.RaiseDomainChanged(domain);
        }

        private void RaiseDomainChanged(string domain)
        {
            if (_isInitializing)
                _changedDomainsDuringInitialization.Add(domain);
            else
                this.RaisePropertyChanged(domain);
        }

        public bool HasDomain(string domain)
        {
            return _storage.ContainsKey(domain);
        }

        public double GetValue(string domain, string key, double defaultValue)
        {
            var value = this.GetValue(domain, key);
            if (value == null)
                return defaultValue;
	        return value.Value;
        }

        public double? GetValue(string domain, string key)
        {
			if (_storage.TryGetValue(domain, out Dictionary<string, double> domainStorage))
			{
				if (domainStorage.TryGetValue(key, out double value))
					return value;
			}

			return null;
        }

        public void Clear()
        {
            var domains = _storage.Keys.ToArray();
            _storage.Clear();
            foreach (var domain in domains)
                this.RaiseDomainChanged(domain);
        }



        public void BeginInit()
        {
            _isInitializing = true;
            _changedDomainsDuringInitialization = new HashSet<string>();
        }

        public void EndInit()
        {
            _isInitializing = false;

            foreach (var domain in _changedDomainsDuringInitialization)
                this.RaiseDomainChanged(domain);

            _changedDomainsDuringInitialization = null;
        }

        public ModificationContext Clone()
        {
            var clone = (ModificationContext)this.MemberwiseClone();
            clone._storage = new Dictionary<string, Dictionary<string, double>>();
            
            foreach(var pair in this._storage)
            {
                clone._storage.Add(pair.Key, new Dictionary<string, double>(pair.Value));
            }

            clone.ClearEventHandlers();

            return clone;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
