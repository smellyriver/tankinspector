using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Smellyriver.TankInspector.IO.MoDecoding
{
    /*
        The format of the generated MO files is best described by a picture, which appears below.

    The first two words serve the identification of the file. The magic number will always signal GNU MO files. The number is stored in the byte order of the generating machine, so the magic number really is two numbers: 0x950412de and 0xde120495. The second word describes the current revision of the file format. For now the revision is 0. This might change in future versions, and ensures that the readers of MO files can distinguish new formats from old ones, so that both can be handled correctly. The version is kept separate from the magic number, instead of using different magic numbers for different formats, mainly because `/etc/magic´ is not updated often. It might be better to have magic separated from internal format version identification.

    Follow a number of pointers to later tables in the file, allowing for the extension of the prefix part of MO files without having to recompile programs reading them. This might become useful for later inserting a few flag bits, indication about the charset used, new tables, or other things.

    Then, at offset O and offset T in the picture, two tables of string descriptors can be found. In both tables, each string descriptor uses two 32 bits integers, one for the string length, another for the offset of the string in the MO file, counting in bytes from the start of the file. The first table contains descriptors for the original strings, and is sorted so the original strings are in increasing lexicographical order. The second table contains descriptors for the translated strings, and is parallel to the first table: to find the corresponding translation one has to access the array slot in the second array with the same index.

    Having the original strings sorted enables the use of simple binary search, for when the MO file does not contain an hashing table, or for when it is not practical to use the hashing table provided in the MO file. This also has another advantage, as the empty string in a PO file GNU gettext is usually translated into some system information attached to that particular MO file, and the empty string necessarily becomes the first in both the original and translated tables, making the system information very easy to find.

    The size S of the hash table can be zero. In this case, the hash table itself is not contained in the MO file. Some people might prefer this because a precomputed hashing table takes disk space, and does not win that much speed. The hash table contains indices to the sorted array of strings in the MO file. Conflict resolution is done by double hashing. The precise hashing algorithm used is fairly dependent on GNU gettext code, and is not documented here.

    As for the strings themselves, they follow the hash file, and each is terminated with a NUL, and this NUL is not counted in the length which appears in the string descriptor. The msgfmt program has an option selecting the alignment for MO file strings. With this option, each string is separately aligned so it starts at an offset which is a multiple of the alignment value. On some RISC machines, a correct alignment will speed things up.

    Plural forms are stored by letting the plural of the original string follow the singular of the original string, separated through a NUL byte. The length which appears in the string descriptor includes both. However, only the singular of the original string takes part in the hash table lookup. The plural variants of the translation are all stored consecutively, separated through a NUL byte. Here also, the length in the string descriptor includes all of them.

    Nothing prevents a MO file from having embedded NULs in strings. However, the program interface currently used already presumes that strings are NUL terminated, so embedded NULs are somewhat useless. But the MO file format is general enough so other interfaces would be later possible, if for example, we ever want to implement wide characters right in MO files, where NUL bytes may accidently appear. (No, we don't want to have wide characters in MO files. They would make the file unnecessarily large, and the `wchar_t´ type being platform dependent, MO files would be platform dependent as well.)

    This particular issue has been strongly debated in the GNU gettext development forum, and it is expectable that MO file format will evolve or change over time. It is even possible that many formats may later be supported concurrently. But surely, we have to start somewhere, and the MO file format described here is a good start. Nothing is cast in concrete, and the format may later evolve fairly easily, so we should feel comfortable with the current approach.

            byte
                 +------------------------------------------+
              0  | magic number = 0x950412de                |
                 |                                          |
              4  | file format revision = 0                 |
                 |                                          |
              8  | number of strings                        |  == N
                 |                                          |
             12  | offset of table with original strings    |  == O
                 |                                          |
             16  | offset of table with translation strings |  == T
                 |                                          |
             20  | size of hashing table                    |  == S
                 |                                          |
             24  | offset of hashing table                  |  == H
                 |                                          |
                 .                                          .
                 .    (possibly more entries later)         .
                 .                                          .
                 |                                          |
              O  | length & offset 0th string  ----------------.
          O + 8  | length & offset 1st string  ------------------.
                  ...                                    ...   | |
    O + ((N-1)*8)| length & offset (N-1)th string           |  | |
                 |                                          |  | |
              T  | length & offset 0th translation  ---------------.
          T + 8  | length & offset 1st translation  -----------------.
                  ...                                    ...   | | | |
    T + ((N-1)*8)| length & offset (N-1)th translation      |  | | | |
                 |                                          |  | | | |
              H  | start hash table                         |  | | | |
                  ...                                    ...   | | | |
      H + S * 4  | end hash table                           |  | | | |
                 |                                          |  | | | |
                 | NUL terminated 0th string  <----------------' | | |
                 |                                          |    | | |
                 | NUL terminated 1st string  <------------------' | |
                 |                                          |      | |
                  ...                                    ...       | |
                 |                                          |      | |
                 | NUL terminated 0th translation  <---------------' |
                 |                                          |        |
                 | NUL terminated 1st translation  <-----------------'
                 |                                          |
                  ...                                    ...
                 |                                          |
                 +------------------------------------------+
    */


	internal class MoData
    {
        public Dictionary<string, string> StringsTable { get; }

        protected MoData(List<String> originalStrings, List<String> translationStrings)
        {
            StringsTable = new Dictionary<string, string>();

            for (int i = 0; i != originalStrings.Count; ++i)
            {
                StringsTable[originalStrings[i]] = translationStrings[i];
            }
        }

        public String Gettext( string messageId)
        {
	        if (StringsTable.TryGetValue(messageId, out string text))
			{
				return text;
			}
	        return $"$missing: {messageId}";
        }


        protected struct StringInfo
        {
            public int Length;
            public int Offset;
        };

        public static MoData ReadFrom(string path)
        {
            var stream = File.OpenRead(path);
            using (var br = new FastBinaryReader(stream))
            {
                uint magicNum = br.ReadUint32();
                uint version = br.ReadUint32();
                Contract.Assert(magicNum == 0x950412de);
                int numberOfString = br.ReadInt32();

                int offsetOriginalStrings = br.ReadInt32();
                int offsetTranslationStrings = br.ReadInt32();
                int hashTableSize = br.ReadInt32();
                int hashTableOffset = br.ReadInt32();

                

                List<StringInfo> originalStringsInfo = new List<StringInfo>();

                stream.Position = offsetOriginalStrings;
                for (int i = 0; i != numberOfString; i++)
                {
                    var stringInfo = new StringInfo { Length = br.ReadInt32(), Offset = br.ReadInt32() };
                    originalStringsInfo.Add(stringInfo);
                }

                List<StringInfo> translationStringsInfo = new List<StringInfo>();

                stream.Position = offsetTranslationStrings;
                for (int i = 0; i != numberOfString; i++)
                {
                    var stringInfo = new StringInfo { Length = br.ReadInt32(), Offset = br.ReadInt32() };
                    translationStringsInfo.Add(stringInfo);
                }

                List<string> originalStrings = new List<string>();

                foreach (var o in originalStringsInfo)
                {
                    stream.Position = o.Offset;
                    originalStrings.Add(br.ReadString(o.Length));
                }

                List<string> translationStrings = new List<string>();

                foreach (var o in translationStringsInfo)
                {
                    stream.Position = o.Offset;
                    translationStrings.Add(br.ReadString(o.Length));
                }
                return new MoData(originalStrings, translationStrings);
            }
        }
    }
}
