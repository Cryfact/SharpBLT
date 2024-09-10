using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpBLT
{
    internal class DieselDB : Singleton<DieselDB>
    {
		private static ulong monotonicTimeMicros()
		{
            // https://docs.microsoft.com/en-us/windows/win32/sysinfo/acquiring-high-resolution-time-stamps
            long StartingTime;
            long Frequency;

            Kernel32.QueryPerformanceFrequency(out Frequency);
            Kernel32.QueryPerformanceCounter(out StartingTime);

            StartingTime *= 1000000;
            StartingTime /= Frequency;
            return (ulong)StartingTime;
        }

        private List<DslFile> filesList;
        private Dictionary<KeyValuePair<IdString, IdString>, DslFile> files;

        // "Future" proofing.
        private List<string> blb_names = ["all", "bundle_db"];

        public DieselDB()
        {
            ulong start_time = monotonicTimeMicros();
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

                foreach (string blb_name in blb_names) 
				{
                    if (name.Substring(0, blb_name.Length) == blb_name)
                    {
                        valid_name = true;
                        break;
                    }
                }

                if (!valid_name)
                    continue;

                blb_path = name;
            }

			if (string.IsNullOrEmpty(blb_path))
			{
				Logger.Instance().Log(LogType.Error, "");
			}

            /*

	if (blb_path.empty()) {
		PD2HOOK_LOG_ERROR("No 'all.blb' or 'bundle_db.blb' found in 'assets' folder, not loading asset database!");
		return;
	}

	in.open("assets/" + blb_path, std::ios::binary);

	// Skip a pointer - vtable or allocator probably?
	in.seekg(sizeof(void*), std::ios::cur);

	// Build out the LanguageID-to-idstring mappings
	struct LanguageData
	{
		idstring name;
		uint32_t id;
		uint32_t padding; // Probably padding, at least - always zero
	};
	static_assert(sizeof(LanguageData) == 16);
	std::map<int, idstring> languages;
	for (const LanguageData& lang : loadVector<LanguageData>(in, 0))
	{
		languages[lang.id] = lang.name;
	}

	// Sortmap
	in.seekg(sizeof(void*) * 2, std::ios::cur);
#if defined(GAME_RAID)
	in.seekg(sizeof(void*), std::ios::cur);
#endif

	// Files
	struct MiniFile
	{
		idstring type;
		idstring name;
		uint32_t langId;
		uint32_t zero_1;
		uint32_t fileId;
		uint32_t zero_2;
	};
	static_assert(sizeof(MiniFile) == 32); // Same on 32 and 64 bit
	std::vector<MiniFile> miniFiles = loadVector<MiniFile>(in, 0);
	filesList.resize(miniFiles.size());

	for (size_t i = 0; i < miniFiles.size(); i++)
	{
		MiniFile& mini = miniFiles[i];
		//printf("File: %016llx.%016llx\n", mini.name, mini.type);

#if defined(GAME_PD2) || defined(GAME_RAID) //PDTH seemingly stores something here.
		assert(mini.zero_1 == 0);
		assert(mini.zero_2 == 0);
#endif

		// Since the file IDs form a sequence of 1 upto the file count (though not in
		// order), we can use those as indexes into our file list.

		if (mini.fileId > filesList.size()) {
			filesList.resize(mini.fileId);
		}

		DslFile& fi = filesList.at(mini.fileId - 1);

		fi.name = mini.name;
		fi.type = mini.type;
		fi.fileId = mini.fileId;

		// Look up the language idstring, if applicable
		fi.rawLangId = mini.langId;
		if (mini.langId == 0)
			fi.langId = 0;
		else if (languages.count(mini.langId))
			fi.langId = languages[mini.langId];
		else
			fi.langId = 0x11df684c9591b7e0; // 'unknown' - is in the hashlist, so you'll be able to find it

		// If it's a repeated file, the language must be different
		const auto& prev = files.find(fi.Key());
		if (prev != files.end())
		{
			assert(prev->second->langId != fi.langId);
			fi.next = prev->second;
		}

		files[fi.Key()] = &fi;
	}

	//printf("File count: %ld\n", files.size());

	// Load each of the bundle headers
	std::string suffix = "_h.bundle";
	std::string prefix = "all_";
	for (const std::string& name : pd2hook::Util::GetDirectoryContents("assets"))
	{
		if (name.length() <= suffix.size())
			continue;
		if (name.compare(name.size() - suffix.size(), suffix.size(), suffix) != 0)
			continue;
		if (name == "all_h.bundle")
			continue; // all_h handling later
#if defined(GAME_PD2) // Some people might have leftover bundle modder fix related headers in PD2.
		if (name.size() != 25)
		{
			PD2HOOK_LOG_WARN("Invalid bundle name '" + name + "' - ignoring");
			continue;
		}
#endif

		bool package = true;
		if (name.compare(0, prefix.size(), prefix) == 0)
			package = false;

		std::string headerPath = "assets/" + name;

		// Find the headerPath to the data file - chop out the '_h' bit
		std::string dataPath = headerPath;
		dataPath.erase(dataPath.end() - 9, dataPath.end() - 7);

		if (package)
			loadPackageHeader(headerPath, dataPath, filesList);
#if defined(GAME_RAID) || defined(GAME_PDTH)
		else
			loadBundleHeader(headerPath, dataPath, filesList);
#endif
	}

#if defined(GAME_PD2)
	loadMultiBundleHeader("assets/all_h.bundle", filesList);
#endif

	// We're done loading, print out how long it took and how many files it's tracking (to estimate memory usage)
	uint64_t end_time = monotonicTimeMicros();

	char buff[1024];
	memset(buff, 0, sizeof(buff));
	snprintf(buff, sizeof(buff) - 1, "Finished loading DB info: %zd files in %d ms", filesList.size(),
	         (int)(end_time - start_time) / 1000);
	PD2HOOK_LOG_LOG(buff);       
             */
        }

        public DslFile? Find(IdString name, IdString ext)
        {
            if (!files.TryGetValue(new KeyValuePair<IdString, IdString>(name, ext), out var res))
                return null;

            return res;
        }

        public BLTAbstractDataStore Open(DieselBundle bundle)
        {
            var fds = BLTFileDataStore.Open(bundle.path);
            return fds;
        }
    }
}
