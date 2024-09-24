using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpBLT
{
    internal class DieselDB : Singleton<DieselDB>
    {
        private readonly DslFile[] m_filesList;
        private readonly Dictionary<KeyValuePair<IdString, IdString>, DslFile> m_files;

        // "Future" proofing.
        private static readonly List<string> ms_blb_names = ["all", "bundle_db"];
#pragma warning disable CS0649
        struct LanguageData
		{
			public IdString name;
			public uint id;
			public uint padding;
		}

#pragma warning disable IDE1006 // Benennungsstile
        struct dsl_vector
#pragma warning restore IDE1006 // Benennungsstile
        {
			public uint size;
			public uint capacity;
			public IntPtr contents_ptr;
            public IntPtr alocator;
		}

        struct MiniFile
        {
            public IdString type;
            public IdString name;
            public uint langId;
            public uint zero_1;
            public uint fileId;
            public uint zero_2;
        }

        struct FilePos
        {
            public uint fileId;
            public uint offset;
        }

        struct BundleInfo
        {
            public IntPtr id;
            public dsl_vector vec;
            public IntPtr zero;
            public IntPtr one;
        };

        struct ItemInfo
        {
            public uint fileId;
            public uint offset;
            public uint length;
        };
#pragma warning restore CS0649
        public DieselDB()
        {
            m_files = [];
            m_filesList = [];

            ulong start_time = MonotonicTimeMicros();
			Logger.Instance().Log(LogType.Log, "Start loading DB info");

            string blb_suffix = ".blb";
            string? blb_path = null;

			foreach (string name in Utils.GetDirectoryContents("assets"))
			{
                if (name.Length <= blb_suffix.Length)
                    continue;

				var fileInfo = new FileInfo(name);

                if (fileInfo.Extension != blb_suffix)
                    continue;

                bool valid_name = false;

                foreach (string blb_name in ms_blb_names) 
				{
                    if (name[..blb_name.Length] == blb_name)
                    {
                        valid_name = true;
                        break;
                    }
                }

                if (!valid_name)
                    continue;

                blb_path = name;
            }

            var fileName = "assets/" + blb_path;

            if (string.IsNullOrEmpty(blb_path) || !File.Exists(fileName))
			{
				Logger.Instance().Log(LogType.Error, "No 'all.blb' or 'bundle_db.blb' found in 'assets' folder, not loading asset database!");
				return;
			}

			var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

			fileStream.Seek(Unsafe.SizeOf<IntPtr>(), SeekOrigin.Current);

			var languages = new Dictionary<uint, IdString>();

			foreach (var lang in LoadVector<LanguageData>(fileStream, 0))
                languages[lang.id] = lang.name;


            // Sortmap

            fileStream.Seek(Unsafe.SizeOf<IntPtr>() * 3, SeekOrigin.Current); // * 2 for pay day, * 3 for raid

			var miniFiles = LoadVector<MiniFile>(fileStream, 0);
            m_filesList = new DslFile[miniFiles.Length];

            foreach (var mini in miniFiles)
			{
                // Since the file IDs form a sequence of 1 upto the file count (though not in
                // order), we can use those as indexes into our file list.

                if (mini.fileId > m_filesList.Length)
                    Array.Resize(ref m_filesList, (int)mini.fileId);

                var fi = m_filesList[mini.fileId - 1];

                fi.name = mini.name;
                fi.type = mini.type;
                fi.fileId = mini.fileId;

                // Look up the language idstring, if applicable
                fi.rawLangId = mini.langId;
                if (mini.langId == 0)
                    fi.langId = IdString.Empty;
                else if (languages.Any((x) => x.Key == mini.langId))
                    fi.langId = languages[mini.langId];
                else
                    fi.langId = new IdString(0x11df684c9591b7e0); // 'unknown' - is in the hashlist, so you'll be able to find it

                // If it's a repeated file, the language must be different
                if (m_files.TryGetValue(fi.Key(), out var prev))
                {
                    fi.next = prev;
                }

                m_files[fi.Key()] = fi;
            }


            Logger.Instance().Log(LogType.Log, $"File count: {m_files.Count}\n");

            // Load each of the bundle headers
            string suffix = "_h.bundle";
            string prefix = "all_";

            foreach (var name in Utils.GetDirectoryContents("assets"))
	        {
                if (name.Length <= suffix.Length)
                    continue;
                if (name.Substring(name.Length - suffix.Length, suffix.Length).CompareTo(suffix) != 0)
                    continue;
                if (name == "all_h.bundle")
                    continue; // all_h handling later

                bool package = true;
                if (name.Substring(name.Length - suffix.Length, suffix.Length).CompareTo(prefix) == 0)
                    package = false;

                string headerPath = "assets/" + name;

                // Find the headerPath to the data file - chop out the '_h' bit
                string dataPath = headerPath;
                dataPath = dataPath.Remove(dataPath.Length - 9, dataPath.Length - 7);

                if (package)
                    LoadPackageHeader(headerPath, dataPath, m_filesList);
		        else
                    LoadBundleHeader(headerPath, dataPath, m_filesList);

                var end_time = MonotonicTimeMicros();

                Logger.Instance().Log(LogType.Log, $"Finished loading DB info: {m_filesList.Length} files in {(int)(end_time - start_time) / 1000} ms");
            }
        }

        public DslFile? Find(IdString name, IdString ext)
        {
            if (!m_files.TryGetValue(new KeyValuePair<IdString, IdString>(name, ext), out var res))
                return null;

            return res;
        }

        public static BLTAbstractDataStore Open(DieselBundle bundle)
        {
            var fds = BLTFileDataStore.Open(bundle.path);
            return fds;
        }

        private static void LoadBundleHeader(string headerPath, string dataPath, DslFile[] files)
        {
            var fileStream = new FileStream(headerPath, FileMode.Open, FileAccess.Read);

            fileStream.Seek(4, SeekOrigin.Current);

            var data = new byte[Unsafe.SizeOf<BundleInfo>()];
            var bundle = Marshal.PtrToStructure<BundleInfo>(Marshal.UnsafeAddrOfPinnedArrayElement(data, 0));

            var dieselBundle = new DieselBundle
            {
                headerPath = headerPath,
                path = dataPath
            };

            foreach (var item in LoadVector<ItemInfo>(fileStream, 4, bundle.vec))
            {
                var fi = files[item.fileId - 1];

                fi.bundle = dieselBundle;
                fi.offset = item.offset;
                fi.length = item.length;
            }
        }

        private static void LoadPackageHeader(string headerPath, string dataPath, DslFile[] files)
        {
            var bundle = new DieselBundle
            {
                headerPath = headerPath,
                path = dataPath
            };

            var fileStream = new FileStream(bundle.headerPath, FileMode.Open, FileAccess.Read);

            fileStream.Seek(4, SeekOrigin.Current);

            var positions = LoadVector<FilePos>(fileStream, 4);

            DslFile? prev = null;

            foreach (var pos in positions)
            {
                var fi = files[pos.fileId - 1];

                fi.bundle = bundle;
                fi.offset = pos.offset;

                if (prev != null)
                    prev.length = fi.offset - prev.offset;

                prev = fi;
            }

            // TODO set a length for the last file
            if (prev != null)
                prev.length = (uint)(fileStream.Length - prev.offset); // maybe correct length of last file
        }

        private unsafe static T[] LoadVector<[DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>
            (Stream stream, int offset, dsl_vector vec)
        {
            int elementSize = Unsafe.SizeOf<T>();
            var data = new byte[vec.size * elementSize];
            var result = new T[elementSize];

            var pos = stream.Position;

            stream.Seek(vec.contents_ptr + offset, SeekOrigin.Begin);
            stream.Read(data);
            stream.Seek(pos, SeekOrigin.Begin);

            fixed (byte* p = data)
            {
                for (int i = 0; i < data.Length; i += elementSize)
                    result[i] = Marshal.PtrToStructure<T>(new IntPtr(p + i)) ?? throw new NotSupportedException();
            }

            return result;
        }

        private static T[] LoadVector<[DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>
            (Stream stream, int offset)
        {
            int size = Unsafe.SizeOf<dsl_vector>();
            byte[] arr = new byte[size];

            stream.Read(arr);
            var vec = Marshal.PtrToStructure<dsl_vector>(Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0));

            return LoadVector<T>(stream, offset, vec);
        }

        private static ulong MonotonicTimeMicros()
        {
            Kernel32.QueryPerformanceFrequency(out var Frequency);
            Kernel32.QueryPerformanceCounter(out var StartingTime);

            StartingTime *= 1000000;
            StartingTime /= Frequency;
            return (ulong)StartingTime;
        }
    }
}
