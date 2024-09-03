namespace SharpBLT;

using HttpClientFactory.Impl;

public sealed class Http
{
    const int HTTP_BUFFER_SIZE = 81920; // 80kB

    private static readonly object _httpClientLock = new();
    private static readonly PerHostHttpClientFactory _httpClientFactory = new();

    public async static Task DoHttpReqAsync<T>(string url, T data, Action<T, string> onDone, Action<T, long, long>? onProgress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpClient = _httpClientFactory.GetHttpClient(url);
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var len = response.Content.Headers.ContentLength;

            using var target = new MemoryStream();

            if (onProgress == null || !len.HasValue)
            {
                // Ignore progress reporting when there is no handler
                await response.Content.CopyToAsync(target, cancellationToken);
            }
            else
            {
                using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
                var buffer = new byte[HTTP_BUFFER_SIZE];
                long totalBytes = len.Value;
                long totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) != 0)
                {
                    await target.WriteAsync(buffer, cancellationToken);
                    totalBytesRead += bytesRead;
                    lock (_httpClientLock)
                        onProgress.Invoke(data, totalBytesRead, totalBytes);
                }
                lock (_httpClientLock)
                    onProgress.Invoke(data, totalBytes, totalBytes);
            }

            // Reset pos for read
            target.Position = 0;

            // FIXME: binary (zip) breaks this!!
            var result = System.Text.Encoding.Default.GetString(target.ToArray(), 0, (int)target.Length);

            lock (_httpClientLock)
                onDone.Invoke(data, result);
        }
        catch (Exception e)
        {
            Logger.Instance().Log(LogType.Warn, e.Message + Environment.NewLine + e.StackTrace);
        }
    }

}
