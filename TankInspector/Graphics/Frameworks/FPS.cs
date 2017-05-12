using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Smellyriver.TankInspector.Graphics.Frameworks
{
    public class Fps : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan AveragingInterval
        {
            get => _mAveragingInterval;
	        set
            {
                if (value == _mAveragingInterval)
                    return;
                if (value < TimeSpan.FromSeconds(0.1))
                    throw new ArgumentOutOfRangeException();
                
                _mAveragingInterval = value;
                OnPropertyChanged("AveragingInterval");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        public void AddFrame(TimeSpan ts)
        {
            var sec = AveragingInterval;
            var index = _mFrames.FindLastIndex(aTs => ts - aTs > sec);
            if (index > -1)
                _mFrames.RemoveRange(0, index);
            _mFrames.Add(ts);

            UpdateValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _mFrames.Clear();
            UpdateValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public double Value
        {
            get => _mValue;
	        private set
            {
                if (value == _mValue)
                    return;
                _mValue = value;
                OnPropertyChanged("Value");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void UpdateValue()
        {
            if (_mFrames.Count < 2)
            {
                Value = -1;
            }
            else
            {
                var dt = _mFrames[_mFrames.Count - 1] - _mFrames[0];
                Value = dt.Ticks > 100 ? _mFrames.Count / dt.TotalSeconds : -1;
            }
        }

        #region INotifyPropertyChanged Members

	    private void OnPropertyChanged(string name)
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private double _mValue;
        private TimeSpan _mAveragingInterval = TimeSpan.FromSeconds(1);
        private readonly List<TimeSpan> _mFrames = new List<TimeSpan>();

    }
}
