using System.IO;
using System.Text;

namespace FTPUploader
{
    public static class StreamHelper
    {
        public static string StreamToStr(this Stream stream)
        {
            if (stream == null || !stream.CanRead) return "";
            var bytes = GetbytesByStream(stream);
            return GetStringByBytes(bytes);
        }

        private static byte[] GetbytesByStream(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        private static string GetStringByBytes(byte[] bytes)
        {
            if (null == bytes || bytes.Length == 0) return "";
            var str = Encoding.UTF8.GetString(bytes);
            return str;
        }
    }
}
