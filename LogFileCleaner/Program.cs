using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileCleaner
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //Declare Variables and Set Values
            string FileDirectory;
            String ArchiveDirectory;
            string DeleteLookBack;
            //Log File Path
            FileDirectory = ConfigurationManager.AppSettings["FileDirectory"].ToString();
            //Archive folder path
            ArchiveDirectory = ConfigurationManager.AppSettings["ArchiveDirectory"].ToString();

            DeleteLookBack = ConfigurationManager.AppSettings["DeleteLookBack"].ToString();

            var files = new DirectoryInfo(FileDirectory).GetFiles("*.*");
            foreach (var file in files)
            {
                try
                {
                    var Time = Math.Floor((DateTime.Now - file.CreationTime).TotalDays);
                    int check = DeleteLookBack == "1M" ? 30 : 60;
                    if (Time >= check)
                    {
                        file.Delete();
                    }
                    else
                    {
                        var ArchiveTime = Math.Floor((DateTime.Now - file.CreationTime).TotalDays);
                        if (ArchiveTime > 3)
                        {
                            //Creating Folders in Archive with year and month
                            string folder_YearMonth = $"{file.LastWriteTime.Year}{file.LastWriteTime.Month:D2}";
                            var destdirectory = Path.Combine(ArchiveDirectory, folder_YearMonth);
                            var desfilename = Path.Combine(destdirectory, file.Name);
                            if (!File.Exists(destdirectory))
                                Directory.CreateDirectory(destdirectory);
                            File.Move(file.FullName, desfilename);
                        }

                    }

                }
                catch (Exception)
                {

                    throw;
                }

            }

        }
    }
}