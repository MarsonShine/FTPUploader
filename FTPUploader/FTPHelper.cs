using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FTPUploader
{
    public class FTPHelper
    {
        private readonly string _serverNameOrIPAddress;
        private readonly string _ftpRemotePath;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _ftpUrl;
        /// <summary>
        /// 初始化FTP
        /// </summary>
        /// <param name="serverNameOrIPAddress">FTP服务器名称或IP地址</param>
        /// <param name="ftpRemotePath">FTP连接当前目录</param>
        /// <param name="userName">FTP用户名</param>
        /// <param name="password">FTP密码</param>
        public FTPHelper(string serverNameOrIPAddress,string ftpRemotePath, string userName,string password)
        {
            _serverNameOrIPAddress = serverNameOrIPAddress;
            _ftpRemotePath = ftpRemotePath;
            _userName = userName;
            _password = password;
            _ftpUrl = $"ftp://{_serverNameOrIPAddress}/{ftpRemotePath}/";
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        public async Task UploadFile(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            string url = _ftpUrl + fileInfo.Name;
            var ftp = GetRequest(url);
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            ftp.ContentLength = fileInfo.Length;
            const int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLength;
            using (var fileStream = fileInfo.OpenRead())
            using (var stream = await ftp.GetRequestStreamAsync())
            {
                contentLength = await fileStream.ReadAsync(buff, 0, buffLength);
                while (contentLength!=0)
                {
                    await stream.WriteAsync(buff, 0, contentLength);
                    contentLength = await fileStream.ReadAsync(buff, 0, buffLength);
                }
            }

        }

        public async Task DeleteFile(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            var url = _ftpUrl + fileInfo.Name;
            var ftp = GetRequest(url);
            ftp.Method = WebRequestMethods.Ftp.DeleteFile;
            string result = "";
            FtpWebResponse ftpWebResponse = (FtpWebResponse)(await ftp.GetResponseAsync());
            long size = ftpWebResponse.ContentLength;
            using (var stream = ftpWebResponse.GetResponseStream())
            using (var streamReader = new StreamReader(stream))
            {
                result = await streamReader.ReadToEndAsync();
            }
            ftpWebResponse.Close();
            ftpWebResponse.Dispose();
        }
        private FtpWebRequest GetRequest(string uRI)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(uRI);
            request.Credentials = new NetworkCredential(_userName, _password);
            request.KeepAlive = true;
            request.UseBinary = true;
            request.UsePassive = true;
            return request;
        }
    }
}
