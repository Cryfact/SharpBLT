namespace SharpBLT;

using HttpClientFactory.Impl;

public sealed class Http
{
    const int HTTP_BUFFER_SIZE = 81920; // 80kB

    private static readonly PerHostHttpClientFactory _httpClientFactory = new();

    public async static Task DoHttpReqAsync(string url, HttpEventData data, Action<HttpEventData, byte[]> onDone, Action<HttpEventData, long, long>? onProgress = null, CancellationToken cancellationToken = default)
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
                    HttpEventQueue.Instance().QueueProgressEvent(onProgress, data, totalBytesRead, totalBytes);
                }
                HttpEventQueue.Instance().QueueProgressEvent(onProgress, data, totalBytes, totalBytes);
            }

            // Reset pos for read
            target.Position = 0;

            byte[] result = target.ToArray();

            HttpEventQueue.Instance().QueueDoneEvent(onDone, data, result);
        }
        catch (Exception e)
        {
            Logger.Instance().Log(LogType.Warn, e.Message + Environment.NewLine + e.StackTrace);
        }
    }

}
