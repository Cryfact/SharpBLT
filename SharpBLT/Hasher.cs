namespace SharpBLT;

using System.Security.Cryptography;
using System.Text;

public static class Hasher
{
    private static void GetDirectories(HashAlgorithm sha, string directory, Dictionary<string, string> dict)
    {
        foreach (var path in Directory.GetFiles(directory, "*.*"))
        {
            using var stream = File.OpenRead(path);
            var hash = sha.ComputeHash(stream);
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

        using var sha256 = SHA256.Create();
        var dict = new Dictionary<string, string>();

        GetDirectories(sha256, directory, dict);

        var hashes = dict.OrderBy(x => x.Key.ToLower(), StringComparer.Ordinal);

        StringBuilder result = new();

        foreach (var keyValuePair in hashes)
            result.Append(keyValuePair.Value);

        var hash = sha256.ComputeHash(Encoding.Default.GetBytes(result.ToString().ToLower()));
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
    }

    public static string GetFileHash(string file)
    {
        ArgumentNullException.ThrowIfNull(file);

        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(file);

        var hash = sha256.ComputeHash(stream);
        var strHash = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

        hash = sha256.ComputeHash(Encoding.Default.GetBytes(strHash));

        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
    }
}