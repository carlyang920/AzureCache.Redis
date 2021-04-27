using System.IO;
using System.IO.Compression;
using System.Text;

namespace AzureCache.Redis.Lib.Helpers
{
    public static class GZipHelper
    {
        private static void CopyTo(
            Stream source, 
            Stream target
            )
        {
            var buffer = new byte[4096];
            int count;
            while (0 != (count = source.Read(buffer, 0, buffer.Length)))
            {
                target.Write(buffer, 0, count);
            }
        }

        public static byte[] Compress(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            using (var input = new MemoryStream(bytes))
            {
                using (var output = new MemoryStream())
                {
                    using (var zipped = new GZipStream(output, CompressionMode.Compress))
                    {
                        CopyTo(input, zipped);
                    }

                    return output.ToArray();
                }
            }
        }

        public static string Decompress(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            {
                using (var output = new MemoryStream())
                {
                    using (var unzipped = new GZipStream(input, CompressionMode.Decompress))
                    {
                        CopyTo(unzipped, output);
                    }

                    return Encoding.UTF8.GetString(output.ToArray());
                }
            }
        }
    }
}
