using System.Collections.Generic;
using System.IO;
using System.Web;
using TFI.Utilities;

namespace OCM.TFI.OCM.Utilities
{
    public static class FileHelper
    {
        public static List<string> ReadFileUploaded(this HttpPostedFileBase file)
        {
            var fileContent = new List<string>();

            using var sr = new StreamReader(file.InputStream);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;
                fileContent.Add(line.Trim());
            }

            return fileContent;
        }
        
        public static List<FileLine> ReadFileRowsUploaded(this HttpPostedFileBase file)
        {
            var fileContent = new List<FileLine>();
            var index = 1;
            using var sr = new StreamReader(file.InputStream);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;
                fileContent.Add(new FileLine(){Content = line.Trim(), Index = index});
                index++;
            }

            return fileContent;
        }
    }
}