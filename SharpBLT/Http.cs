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
            HttpClient httpClient = _httpClientFactory.GetHttpClient(url);
            using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            long? len = response.Content.Headers.ContentLength;

            using MemoryStream target = new();

            if (onProgress == null || !len.HasValue)
            {
                // Ignore progress reporting when there is no handler
                await response.Content.CopyToAsync(target, cancellationToken);
            }
            else
            {
                using Stream source = await response.Content.ReadAsStreamAsync(cancellationToken);
                byte[] buffer = new byte[HTTP_BUFFER_SIZE];
                long totalBytes = len.Value;
                long totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) != 0)
                {
                    await target.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
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
        catch (Exception ex)
        {
            Logger.Instance().Log(LogType.Warn, ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

}
