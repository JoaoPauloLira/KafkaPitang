namespace Api.Web.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> ToByteArrayAsync(this Stream? stream)
    {
        if (stream == null) return Array.Empty<byte>();
        await using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}