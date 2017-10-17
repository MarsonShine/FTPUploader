using FTPUploader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace UploadImage.Test
{
    /// <summary>
    /// upload 的摘要说明
    /// </summary>
    public class upload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "multipart/form-data";
            var str = "";
            ParseFiles(
                context.Request.InputStream,
                context.Request.ContentType,
                (a, s) =>
            {
                if (a.Contains("."))
                {
                    //文件
                    StreamToFile(s, a);
                }
                else
                {
                    //一般文本
                    str = s.StreamToStr();
                }
            });
            //context.Response.Write(str);
            context.Response.End();
        }

        public static async Task ParseFiles(Stream data, string contentType, Action<string, Stream> fileProcessor)
        {
            var streamCotent = new StreamContent(data);
            streamCotent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            if (streamCotent.IsMimeMultipartContent())
            {
                var provider = await streamCotent.ReadAsMultipartAsync();
                foreach (var httpContent in provider.Contents)
                {
                    var filename = httpContent.Headers.ContentDisposition.FileName;
                    using (var fileContents = await httpContent.ReadAsStreamAsync())
                    {
                        if (string.IsNullOrWhiteSpace(filename))
                        {
                            fileProcessor(httpContent.Headers.ContentDisposition.Name.Trim('\"'), fileContents);
                            continue;
                        }
                        fileProcessor(filename.Trim('\"'), fileContents);
                    }
                }
            }
            else
            {
                var formdata = await streamCotent.ReadAsFormDataAsync();
                var str = await streamCotent.ReadAsStringAsync();
                var stream = await streamCotent.ReadAsStreamAsync();
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void StreamToFile(Stream stream, string fileName)
        {
            var url = Path.Combine(HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath), "upload", "images");
            DirectoryInfo di = new DirectoryInfo(url);
            if (!di.Exists)
            {
                di.Create();
            }
            // 把 Stream 转换成 byte[]   
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始   
            stream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件   
            FileStream fs = new FileStream(Path.Combine(url, fileName), FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }
    }
}