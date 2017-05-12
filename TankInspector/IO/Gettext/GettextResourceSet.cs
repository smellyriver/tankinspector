using System;
using System.Collections;
using System.IO;
using System.Resources;

namespace Smellyriver.TankInspector.IO.Gettext
{
	/// <summary>
    /// <para>
    /// Each instance of this class encapsulates a single PO file.
    /// </para>
    /// <para>
    /// This API of this class is not meant to be used directly; use
    /// <c>GettextResourceManager</c> instead.
    /// </para>
    /// </summary>
    // We need this subclass of ResourceSet, because the plural formula must come
    // from the same ResourceSet as the object containing the plural forms.
    public class GettextResourceSet : ResourceSet
    {

        /// <summary>
        /// Creates a new message catalog. When using this constructor, you
        /// must override the <c>ReadResources</c> method, in order to initialize
        /// the <c>Table</c> property. The message catalog will support plural
        /// forms only if the <c>ReadResources</c> method installs values of type
        /// <c>String[]</c> and if the <c>PluralEval</c> method is overridden.
        /// </summary>
        protected GettextResourceSet()
            : base(DummyResourceReader)
        {
        }

        /// <summary>
        /// Creates a new message catalog, by reading the string/value pairs from
        /// the given <paramref name="reader"/>. The message catalog will support
        /// plural forms only if the reader can produce values of type
        /// <c>String[]</c> and if the <c>PluralEval</c> method is overridden.
        /// </summary>
        public GettextResourceSet(IResourceReader reader)
            : base(reader)
        {
        }

        /// <summary>
        /// Creates a new message catalog, by reading the string/value pairs from
        /// the given <paramref name="stream"/>, which should have the format of
        /// a <c>.resources</c> file. The message catalog will not support plural
        /// forms.
        /// </summary>
        public GettextResourceSet(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Creates a new message catalog, by reading the string/value pairs from
        /// the file with the given <paramref name="fileName"/>. The file should
        /// be in the format of a <c>.resources</c> file. The message catalog will
        /// not support plural forms.
        /// </summary>
        public GettextResourceSet(String fileName)
            : base(fileName)
        {
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/>.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or <c>null</c> if
        ///          none is found</returns>
        // The default implementation essentially does (String)Table[msgid].
        // Here we also catch the plural form case.
        public override String GetString(String msgid)
        {
            Object value = this.GetObject(msgid);
            if (value == null || value is String)
                return (String)value;
	        if (value is String[])
		        // A plural form, but no number is given.
		        // Like the C implementation, return the first plural form.
		        return ((String[])value)[0];
	        throw new InvalidOperationException("resource for \"" + msgid + "\" in " + this.GetType().FullName + " is not a string");
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/>, with possibly
        /// case-insensitive lookup.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or <c>null</c> if
        ///          none is found</returns>
        // The default implementation essentially does (String)Table[msgid].
        // Here we also catch the plural form case.
        public override String GetString(String msgid, bool ignoreCase)
        {
            Object value = this.GetObject(msgid, ignoreCase);
            if (value == null || value is String)
                return (String)value;
	        if (value is String[])
		        // A plural form, but no number is given.
		        // Like the C implementation, return the first plural form.
		        return ((String[])value)[0];
	        throw new InvalidOperationException("resource for \"" + msgid + "\" in " + this.GetType().FullName + " is not a string");
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> and
        /// <paramref name="msgidPlural"/>, choosing the right plural form
        /// depending on the number <paramref name="n"/>.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <param name="msgidPlural">the English plural of <paramref name="msgid"/>,
        ///                           an ASCII string</param>
        /// <param name="n">the number, should be &gt;= 0</param>
        /// <returns>the translation, or <c>null</c> if none is found</returns>
        public virtual String GetPluralString(String msgid, String msgidPlural, long n)
        {
            Object value = this.GetObject(msgid);
            if (value == null || value is String)
                return (String)value;
	        if (value is String[])
	        {
		        String[] choices = (String[])value;
		        long index = this.PluralEval(n);
		        return choices[index >= 0 && index < choices.Length ? index : 0];
	        }
	        throw new InvalidOperationException("resource for \"" + msgid + "\" in " + this.GetType().FullName + " is not a string");
        }

        /// <summary>
        /// Returns the index of the plural form to be chosen for a given number.
        /// The default implementation is the Germanic plural formula:
        /// zero for <paramref name="n"/> == 1, one for <paramref name="n"/> != 1.
        /// </summary>
        protected virtual long PluralEval(long n)
        {
            return (n == 1 ? 0 : 1);
        }

        /// <summary>
        /// Returns the keys of this resource set, i.e. the strings for which
        /// <c>GetObject()</c> can return a non-null value.
        /// </summary>
        public virtual ICollection Keys => Table.Keys;

	    /// <summary>
        /// A trivial instance of <c>IResourceReader</c> that does nothing.
        /// </summary>
        // Needed by the no-arguments constructor.
        private static readonly IResourceReader DummyResourceReader = new DummyIResourceReader();

    }
}
