using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace LogFileCleaner
{
    internal class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {

            //Declare Variables and Set Values
            string FileDirectory;
            String ArchiveDirectory;
            string DeleteLookBack, ArchiveLookBack;
            FileDirectory = ConfigurationManager.AppSettings["FileDirectory"].ToString();
            ArchiveDirectory = ConfigurationManager.AppSettings["ArchiveDirectory"].ToString();
            DeleteLookBack = ConfigurationManager.AppSettings["DeleteLookBack"].ToString();
            ArchiveLookBack = ConfigurationManager.AppSettings["ArchiveLookBack"].ToString();
            try
            {
                //Log File Path
                if (args.Length>0 && !args[0].Equals("null")) 
                    FileDirectory = args[0];
                //Archive folder path
                if (args.Length > 1 && !args[1].Equals("null"))
                    ArchiveDirectory = args[1];
                // deletelookback time
                if (args.Length>2 && !args[2].Equals("null"))
                    DeleteLookBack = args[2];  
                //archivelookback time
                if (args.Length>3 && !args[3].Equals("null"))
                    ArchiveLookBack = args[3];

                var DeleteCutOffTime = GetCutOffTime(DeleteLookBack);
                var ArchiveCutOffTime = GetCutOffTime(ArchiveLookBack);

                //function call for archiving log file
                AcrhiveLogFile(ArchiveCutOffTime, FileDirectory, ArchiveDirectory);

                //function call for deleting file from archive
                DeleteFileFromArchive(DeleteCutOffTime, ArchiveDirectory);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return;
            }

        }

        //function for finding cutoff time
        static public DateTime? GetCutOffTime(string deleteLookBack)
        {
            DateTime? result = null;

            Regex pattern = new Regex(@"(?<duration>\d+)(?<timeFrame>[DMWY])");
            Match match = pattern.Match(deleteLookBack);
            if (match.Groups.Count == 3)
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
            else
                log.Warn($"match groups time is not 3");
            log.Info(result);
            return result;
        }

        //function for archiving log file
        public static void AcrhiveLogFile(DateTime? ArchiveCutoffTime, String FileDirectory, String ArchiveDirectory)
        {
            var files = new DirectoryInfo(FileDirectory).GetFiles("*.*");
            foreach (var file in files)
            {
                try
                {
                    // checking for the file last write time is lesser then archiveCutOffTime or not.
                    if (ArchiveCutoffTime != null && file.LastWriteTime < ArchiveCutoffTime)
                    {
                        //Creating Folders in Archive with year and month
                        string folder_YearMonth = $"{file.LastWriteTime.Year}{file.LastWriteTime.Month:D2}";
                        var destdirectory = Path.Combine(ArchiveDirectory, folder_YearMonth);
                        var desfilename = Path.Combine(destdirectory, file.Name);
                        if (!File.Exists(destdirectory))
                            Directory.CreateDirectory(destdirectory);
                        File.Move(file.FullName, desfilename);
                        log.Info($"{file.FullName} moved to archive successfully");
                    }
                    else
                        log.Warn($"{file.FullName} can't be move to archive");
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    return;
                }

            }

        }

        //function for deleting file from archive
        public static void DeleteFileFromArchive(DateTime? DeleteCutoffTime, String ArchiveDirectory)
        {
            try
            {
                String[] dirs = Directory.GetDirectories(ArchiveDirectory, "*");
                foreach(string dir in dirs)
                {
                    var files = new DirectoryInfo(dir).GetFiles("*.*");
                    foreach(var file in files)
                    {
                        // Delete
                        if (DeleteCutoffTime != null && file.LastWriteTime < DeleteCutoffTime)
                        {
                            file.Delete();
                            log.Info($"{file.FullName} deleted successfully");
                        }
                    }
                    if(Directory.GetFiles(dir).Length == 0)
                    {
                        Directory.Delete(dir, false);
                        log.Info($"{dir} deleted successfully");
                    }
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return;
            }
        }
    }
}