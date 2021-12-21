using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace XMUer.Utility
{
    public class FileUploadHelper
    {
        public static void FileUploadToLinux(string url, FileUpload File)
        {
            WebClient wc = new WebClient();
            Stream fs = File.files.OpenReadStream();
            byte[] bt = new byte[fs.Length];
            fs.Read(bt, 0, bt.Length);
            wc.UploadData(url, "PUT", bt);
        }
    }
}
