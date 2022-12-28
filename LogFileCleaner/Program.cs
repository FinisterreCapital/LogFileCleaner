using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            string DeleteLookBack, ArchiveLookBack;
            //Log File Path
            FileDirectory = ConfigurationManager.AppSettings["FileDirectory"].ToString();
            //Archive folder path
            ArchiveDirectory = ConfigurationManager.AppSettings["ArchiveDirectory"].ToString();

            DeleteLookBack = ConfigurationManager.AppSettings["DeleteLookBack"].ToString();
            ArchiveLookBack = ConfigurationManager.AppSettings["ArchiveLookBack"].ToString();

            var DeleteCutOffTime = GetCutOffTime(DeleteLookBack);
            var ArchiveCutOffTime = GetCutOffTime(ArchiveLookBack);
            
            var files = new DirectoryInfo(FileDirectory).GetFiles("*.*");
            foreach (var file in files)
            {
                try
                {
                    // Delete
                    if (DeleteCutOffTime != null && file.CreationTime < DeleteCutOffTime)
                    {
                        file.Delete();
                    }
                    else if (ArchiveCutOffTime != null && file.CreationTime < ArchiveCutOffTime)
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
                catch (Exception)
                {

                    throw;
                }

            }

        }
        
        static public DateTime? GetCutOffTime(string deleteLookBack)
        {
            DateTime? result = null;

            Regex pattern = new Regex(@"(?<duration>\d+)(?<timeFrame>[DMWY])");
            Match match = pattern.Match(deleteLookBack);
            if (match.Length == 2)
            {
                var duration = int.Parse(match.Groups["duration"].Value);
                var timeFrame = match.Groups["timeFrame"].Value;
                switch (timeFrame)
                {
                    case "D":
                        result = DateTime.Now.AddDays(-duration);
                        break;
                    case "W":
                        result = DateTime.Now.AddDays(-duration * 7);
                        break;
                    case "M":
                        result = DateTime.Now.AddMonths(-duration);
                        break;
                    case "Y":
                        result = DateTime.Now.AddYears(-duration);
                        break;
                }
            }

            return result;
        }
    }
}