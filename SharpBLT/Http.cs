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
            var contentLength = response.Content.Headers.ContentLength;
            using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var target = new MemoryStream();

            if (onProgress == null || !contentLength.HasValue)
            {
                // Ignore progress reporting when there is no handler or the content length is unknown
                await source.CopyToAsync(target, HTTP_BUFFER_SIZE, cancellationToken);
            }
            else
            {
                var buffer = new byte[HTTP_BUFFER_SIZE];
                long totalBytes = contentLength.Value;
                long totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = await source.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
                {
                    await target.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                    totalBytesRead += bytesRead;
                    lock (_httpClientLock)
                        onProgress.Invoke(data, totalBytesRead, totalBytes);
                }
                lock (_httpClientLock)
                    onProgress.Invoke(data, totalBytes, totalBytes);
            }

            using StreamReader reader = new(target);
            var result = reader.ReadToEnd();
            lock (_httpClientLock)
                onDone.Invoke(data, result);
        }
        catch (Exception e)
        {
            Logger.Instance().Log(LogType.Warn, e.Message + Environment.NewLine + e.StackTrace);
        }
    }

}
