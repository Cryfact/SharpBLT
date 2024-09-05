namespace SharpBLT;

using System.Security.Cryptography;
using System.Text;

public static class Hasher
{
    private static void GetDirectories(HashAlgorithm sha, string directory, Dictionary<string, string> dict)
    {
        foreach (string path in Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
        {
            using FileStream stream = File.OpenRead(path);
            byte[] hash = sha.ComputeHash(stream);
            dict.Add(path, BitConverter.ToString(hash).Replace("-", string.Empty));
        }

        foreach (string file in Directory.GetDirectories(directory))
        {
            DirectoryInfo directoryInfo = new(file);

            if (directoryInfo.Name == ".hg" || directoryInfo.Name == ".git")
                continue;

            GetDirectories(sha, file, dict);
        }
    }

    public static string GetDirectoryHash(string directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        using SHA256 sha256 = SHA256.Create();
        Dictionary<string, string> dict = [];

        GetDirectories(sha256, directory, dict);

        IOrderedEnumerable<KeyValuePair<string, string>> hashes = dict.OrderBy(x => x.Key.ToLower(), StringComparer.Ordinal);

        StringBuilder result = new();

        foreach (KeyValuePair<string, string> keyValuePair in hashes)
            result.Append(keyValuePair.Value);

        byte[] hash = sha256.ComputeHash(Encoding.Default.GetBytes(result.ToString().ToLower()));
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
    }

    public static string GetFileHash(string file)
    {
        ArgumentNullException.ThrowIfNull(file);

        using SHA256 sha256 = SHA256.Create();
        using FileStream stream = File.OpenRead(file);

        byte[] hash = sha256.ComputeHash(stream);
        string strHash = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

        hash = sha256.ComputeHash(Encoding.Default.GetBytes(strHash));

        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
    }
}