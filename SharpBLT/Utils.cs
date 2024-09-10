using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public static class Utils
    {
        public static string[] GetDirectoryContents(string path, bool files = false)
        {
            string[] directories;

            if (!files)
                directories = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            else
                directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            return directories.Select((x) => x.Replace(path, string.Empty)).ToArray();
        }

    }
}
