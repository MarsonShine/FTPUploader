using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPUploader
{
    public class FileUploader
    {
        public static string HttpUploadFile(FileForm fileForm)
        {
            var success = WriteFile(Path.Combine(Directory.GetCurrentDirectory(), "upload","images"), fileForm.Avator, fileForm.Name,fileForm.TypeName);
            return success.ToString();
        }

        private static bool WriteFile(string path, byte[] avator, string name,string fileType)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                di.Create();
            }
            FileStream fstream = null;
            try
            {
                fstream = File.Create(path + "\\" + name+ "."+ fileType, avator.Length);
                fstream.Write(avator, 0, avator.Length);   //二进制转换成文件  
            }
            catch (Exception ex)
            {
                //抛出异常信息  
                return false;
            }
            finally
            {
                if (fstream != null)
                    fstream.Close();
            }
            return true;
        }
    }

    public class FileForm
    {
        public FileForm(byte[] avator,string name,string typeName,string location)
        {
            Avator = avator;
            Name = name;
            TypeName = typeName;
            Location = location;
        }
        public byte[] Avator { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Location { get; set; }
    }
}
