using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            FTPHelper ftp = new FTPHelper("syw3185190001.my3w.com", "", "syw3185190001", "klwly888");
            var filePhysicalPath = Path.Combine("C:\\Users\\Administrator\\Desktop", "EDI20170920.txt");
            ftp.DeleteFile(filePhysicalPath).GetAwaiter().GetResult();
        }
    }
}
