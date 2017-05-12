using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Smellyriver.TankInspector.IO.Gettext
{
	/// <summary>
    /// Each instance of this class can be used to lookup translations for a
    /// given resource name. For each <c>CultureInfo</c>, it performs the lookup
    /// in several assemblies, from most specific over territory-neutral to
    /// language-neutral.
    /// </summary>
    public class GettextResourceManager : ResourceManager
    {

        // ======================== Public Constructors ========================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseName">the resource name, also the assembly base
        ///                        name</param>
        public GettextResourceManager(String baseName)
            : base(baseName, Assembly.GetCallingAssembly(), typeof(GettextResourceSet))
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseName">the resource name, also the assembly base
        ///                        name</param>
        public GettextResourceManager(String baseName, Assembly assembly)
            : base(baseName, assembly, typeof(GettextResourceSet))
        {
        }

        // ======================== Implementation ========================

        /// <summary>
        /// Loads and returns a satellite assembly.
        /// </summary>
        // This is like Assembly.GetSatelliteAssembly, but uses resourceName
        // instead of assembly.GetName().Name, and works around a bug in
        // mono-0.28.
        private static Assembly GetSatelliteAssembly(Assembly assembly, String resourceName, CultureInfo culture)
        {
            String satelliteExpectedLocation =
              Path.GetDirectoryName(assembly.Location)
              + Path.DirectorySeparatorChar + culture.Name
              + Path.DirectorySeparatorChar + resourceName + ".resources.dll";
            return Assembly.LoadFrom(satelliteExpectedLocation);
        }

        /// <summary>
        /// Loads and returns the satellite assembly for a given culture.
        /// </summary>
        private Assembly MySatelliteAssembly(CultureInfo culture)
        {
            return GettextResourceManager.GetSatelliteAssembly(MainAssembly, this.BaseName, culture);
        }

        /// <summary>
        /// Converts a resource name to a class name.
        /// </summary>
        /// <returns>a nonempty string consisting of alphanumerics and underscores
        ///          and starting with a letter or underscore</returns>
        private static String ConstructClassName(String resourceName)
        {
            // We could just return an arbitrary fixed class name, like "Messages",
            // assuming that every assembly will only ever contain one
            // GettextResourceSet subclass, but this assumption would break the day
            // we want to support multi-domain PO files in the same format...
            bool valid = (resourceName.Length > 0);
            for (int i = 0; valid && i < resourceName.Length; i++)
            {
                char c = resourceName[i];
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == '_')
                      || (i > 0 && c >= '0' && c <= '9')))
                    valid = false;
            }
            if (valid)
                return resourceName;
	        // Use hexadecimal escapes, using the underscore as escape character.
	        String hexdigit = "0123456789abcdef";
	        StringBuilder b = new StringBuilder();
	        b.Append("__UESCAPED__");
	        for (int i = 0; i < resourceName.Length; i++)
	        {
		        char c = resourceName[i];
		        if (c >= 0xd800 && c < 0xdc00
		            && i + 1 < resourceName.Length
		            && resourceName[i + 1] >= 0xdc00 && resourceName[i + 1] < 0xe000)
		        {
			        // Combine two UTF-16 words to a character.
			        char c2 = resourceName[i + 1];
			        int uc = 0x10000 + ((c - 0xd800) << 10) + (c2 - 0xdc00);
			        b.Append('_');
			        b.Append('U');
			        b.Append(hexdigit[(uc >> 28) & 0x0f]);
			        b.Append(hexdigit[(uc >> 24) & 0x0f]);
			        b.Append(hexdigit[(uc >> 20) & 0x0f]);
			        b.Append(hexdigit[(uc >> 16) & 0x0f]);
			        b.Append(hexdigit[(uc >> 12) & 0x0f]);
			        b.Append(hexdigit[(uc >> 8) & 0x0f]);
			        b.Append(hexdigit[(uc >> 4) & 0x0f]);
			        b.Append(hexdigit[uc & 0x0f]);
			        i++;
		        }
		        else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')
		                   || (c >= '0' && c <= '9')))
		        {
			        int uc = c;
			        b.Append('_');
			        b.Append('u');
			        b.Append(hexdigit[(uc >> 12) & 0x0f]);
			        b.Append(hexdigit[(uc >> 8) & 0x0f]);
			        b.Append(hexdigit[(uc >> 4) & 0x0f]);
			        b.Append(hexdigit[uc & 0x0f]);
		        }
		        else
			        b.Append(c);
	        }
	        return b.ToString();
        }

        /// <summary>
        /// Instantiates a resource set for a given culture.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///   The expected type name is not valid.
        /// </exception>
        /// <exception cref="ReflectionTypeLoadException">
        ///   satelliteAssembly does not contain the expected type.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///   The type has no no-arguments constructor.
        /// </exception>
        private static GettextResourceSet InstantiateResourceSet(Assembly satelliteAssembly, String resourceName, CultureInfo culture)
        {
            // We expect a class with a culture dependent class name.
            Type clazz = satelliteAssembly.GetType(GettextResourceManager.ConstructClassName(resourceName) + "_" + culture.Name.Replace('-', '_'));
            // We expect it has a no-argument constructor, and invoke it.
            ConstructorInfo constructor = clazz.GetConstructor(Type.EmptyTypes);
            return (GettextResourceSet)constructor.Invoke(null);
        }

        private static readonly GettextResourceSet[] EmptyResourceSetArray = new GettextResourceSet[0];

        // Cache for already loaded GettextResourceSet cascades.
        private readonly Hashtable /* CultureInfo -> GettextResourceSet[] */ _loaded = new Hashtable();

        /// <summary>
        /// Returns the array of <c>GettextResourceSet</c>s for a given culture,
        /// loading them if necessary, and maintaining the cache.
        /// </summary>
        private GettextResourceSet[] GetResourceSetsFor(CultureInfo culture)
        {
            //Console.WriteLine(">> GetResourceSetsFor "+culture);
            // Look up in the cache.
            GettextResourceSet[] result = (GettextResourceSet[])_loaded[culture];
            if (result == null)
            {
                lock (this)
                {
                    // Look up again - maybe another thread has filled in the entry
                    // while we slept waiting for the lock.
                    result = (GettextResourceSet[])_loaded[culture];
                    if (result == null)
                    {
                        // Determine the GettextResourceSets for the given culture.
                        if (culture.Parent == null || culture.Equals(CultureInfo.InvariantCulture))
                            // Invariant culture.
                            result = EmptyResourceSetArray;
                        else
                        {
                            // Use a satellite assembly as primary GettextResourceSet, and
                            // the result for the parent culture as fallback.
                            GettextResourceSet[] parentResult = this.GetResourceSetsFor(culture.Parent);
                            Assembly satelliteAssembly;
                            try
                            {
                                satelliteAssembly = this.MySatelliteAssembly(culture);
                            }
                            catch (FileNotFoundException)
                            {
                                satelliteAssembly = null;
                            }
                            if (satelliteAssembly != null)
                            {
                                GettextResourceSet satelliteResourceSet;
                                try
                                {
                                    satelliteResourceSet = GettextResourceManager.InstantiateResourceSet(satelliteAssembly, this.BaseName, culture);
                                }
                                catch (Exception e)
                                {
                                    Console.Error.WriteLine(e);
                                    Console.Error.WriteLine(e.StackTrace);
                                    satelliteResourceSet = null;
                                }
                                if (satelliteResourceSet != null)
                                {
                                    result = new GettextResourceSet[1 + parentResult.Length];
                                    result[0] = satelliteResourceSet;
                                    Array.Copy(parentResult, 0, result, 1, parentResult.Length);
                                }
                                else
                                    result = parentResult;
                            }
                            else
                                result = parentResult;
                        }
                        // Put the result into the cache.
                        _loaded.Add(culture, result);
                    }
                }
            }
            //Console.WriteLine("<< GetResourceSetsFor "+culture);
            return result;
        }

        /*
        /// <summary>
        /// Releases all loaded <c>GettextResourceSet</c>s and their assemblies.
        /// </summary>
        // TODO: No way to release an Assembly?
        public override void ReleaseAllResources () {
          ...
        }
        */

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> in a given culture.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or
        ///          <paramref name="msgid"/> if none is found</returns>
        public override String GetString(String msgid, CultureInfo culture)
        {
            foreach (GettextResourceSet rs in this.GetResourceSetsFor(culture))
            {
                String translation = rs.GetString(msgid);
                if (translation != null)
                    return translation;
            }
            // Fallback.
            return msgid;
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> and
        /// <paramref name="msgidPlural"/> in a given culture, choosing the right
        /// plural form depending on the number <paramref name="n"/>.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <param name="msgidPlural">the English plural of <paramref name="msgid"/>,
        ///                           an ASCII string</param>
        /// <param name="n">the number, should be &gt;= 0</param>
        /// <returns>the translation, or <paramref name="msgid"/> or
        ///          <paramref name="msgidPlural"/> if none is found</returns>
        public virtual String GetPluralString(String msgid, String msgidPlural, long n, CultureInfo culture)
        {
            foreach (GettextResourceSet rs in this.GetResourceSetsFor(culture))
            {
                String translation = rs.GetPluralString(msgid, msgidPlural, n);
                if (translation != null)
                    return translation;
            }
            // Fallback: Germanic plural form.
            return (n == 1 ? msgid : msgidPlural);
        }

        // ======================== Public Methods ========================

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> in the context
        /// of <paramref name="msgctxt"/> a given culture.
        /// </summary>
        /// <param name="msgctxt">the context for the key string, an ASCII
        ///                       string</param>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or
        ///          <paramref name="msgid"/> if none is found</returns>
        public String GetParticularString(String msgctxt, String msgid, CultureInfo culture)
        {
            String combined = msgctxt + "\u0004" + msgid;
            foreach (GettextResourceSet rs in this.GetResourceSetsFor(culture))
            {
                String translation = rs.GetString(combined);
                if (translation != null)
                    return translation;
            }
            // Fallback.
            return msgid;
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> and
        /// <paramref name="msgidPlural"/> in the context of
        /// <paramref name="msgctxt"/> in a given culture, choosing the right
        /// plural form depending on the number <paramref name="n"/>.
        /// </summary>
        /// <param name="msgctxt">the context for the key string, an ASCII
        ///                       string</param>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <param name="msgidPlural">the English plural of <paramref name="msgid"/>,
        ///                           an ASCII string</param>
        /// <param name="n">the number, should be &gt;= 0</param>
        /// <returns>the translation, or <paramref name="msgid"/> or
        ///          <paramref name="msgidPlural"/> if none is found</returns>
        public virtual String GetParticularPluralString(String msgctxt, String msgid, String msgidPlural, long n, CultureInfo culture)
        {
            String combined = msgctxt + "\u0004" + msgid;
            foreach (GettextResourceSet rs in this.GetResourceSetsFor(culture))
            {
                String translation = rs.GetPluralString(combined, msgidPlural, n);
                if (translation != null)
                    return translation;
            }
            // Fallback: Germanic plural form.
            return (n == 1 ? msgid : msgidPlural);
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> in the current
        /// culture.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or
        ///          <paramref name="msgid"/> if none is found</returns>
        public override String GetString(String msgid)
        {
            return this.GetString(msgid, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> and
        /// <paramref name="msgidPlural"/> in the current culture, choosing the
        /// right plural form depending on the number <paramref name="n"/>.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <param name="msgidPlural">the English plural of <paramref name="msgid"/>,
        ///                           an ASCII string</param>
        /// <param name="n">the number, should be &gt;= 0</param>
        /// <returns>the translation, or <paramref name="msgid"/> or
        ///          <paramref name="msgidPlural"/> if none is found</returns>
        public virtual String GetPluralString(String msgid, String msgidPlural, long n)
        {
            return this.GetPluralString(msgid, msgidPlural, n, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> in the context
        /// of <paramref name="msgctxt"/> in the current culture.
        /// </summary>
        /// <param name="msgctxt">the context for the key string, an ASCII
        ///                       string</param>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or
        ///          <paramref name="msgid"/> if none is found</returns>
        public String GetParticularString(String msgctxt, String msgid)
        {
            return this.GetParticularString(msgctxt, msgid, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> and
        /// <paramref name="msgidPlural"/> in the context of
        /// <paramref name="msgctxt"/> in the current culture, choosing the
        /// right plural form depending on the number <paramref name="n"/>.
        /// </summary>
        /// <param name="msgctxt">the context for the key string, an ASCII
        ///                       string</param>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <param name="msgidPlural">the English plural of <paramref name="msgid"/>,
        ///                           an ASCII string</param>
        /// <param name="n">the number, should be &gt;= 0</param>
        /// <returns>the translation, or <paramref name="msgid"/> or
        ///          <paramref name="msgidPlural"/> if none is found</returns>
        public virtual String GetParticularPluralString(String msgctxt, String msgid, String msgidPlural, long n)
        {
            return this.GetParticularPluralString(msgctxt, msgid, msgidPlural, n, CultureInfo.CurrentUICulture);
        }

    }
}
