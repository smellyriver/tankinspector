using System;
using System.Linq;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Graphics.Frameworks
{
	/// <summary>
	/// SharpDX 1.3 requires explicit dispose of all its ComObject.
	/// This method makes it easier.
	/// (Remark: I attempted to hack a correct Dispose implementation but it crashed the app on first GC!)
	/// </summary>
	public class DisposeGroup : IDisposable
	{
        /// <summary>
        /// 
        /// </summary>        
		public void Add(params IDisposable[] objects)
		{
			_mList.AddRange(from o in objects where o != null select o);
		}

        /// <summary>
        /// 
        /// </summary>
		public T Add<T>(T ob)
			where T : IDisposable
		{
			if (ob != null)
				_mList.Add(ob);
			return ob;
		}

        /// <summary>
        /// 
        /// </summary>
		public void Dispose()
		{
            //for (int i = list.Count - 1; i >= 0; i--)
            //{
            //    var d = list[i];
            //    list.RemoveAt(i);
            //    d.Dispose();
            //}
            for (int i = 0; i < _mList.Count; i++)
            {
                _mList[i].Dispose();
            }
            _mList = null;
		}

        /// <summary>
        /// 
        /// </summary>
		private List<IDisposable> _mList = new List<IDisposable>();

	}
}
