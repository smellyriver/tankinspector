using System.Collections;
using System.Resources;

namespace Smellyriver.TankInspector.IO.Gettext
{
	/// <summary>
    /// A trivial <c>IResourceReader</c> implementation.
    /// </summary>
	internal class DummyIResourceReader : IResourceReader
    {

        // Implementation of IDisposable.
        void System.IDisposable.Dispose()
        {
        }

        // Implementation of IEnumerable.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        // Implementation of IResourceReader.
        void IResourceReader.Close()
        {
        }
        IDictionaryEnumerator IResourceReader.GetEnumerator()
        {
            return null;
        }

    }
}
