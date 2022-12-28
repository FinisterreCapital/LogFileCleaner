//// See https://aka.ms/new-console-template for more information
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;



////Declare Variables and Set Values
//string FileDirectory;
//String CopytoDirectory;
//int RetentionPeriodinDays;


////Log File Path
//FileDirectory = @\\internal.finisterre.com\FCAP\Home$\Mohammed.Samad\Documents\TestLogs;
////Archive folder path
//CopytoDirectory = \\\\internal.finisterre.com\\FCAP\\Home$\\Mohammed.Samad\\Documents\\TestLogs\\Archive;
//RetentionPeriodinDays = 7;


//var files = new DirectoryInfo(FileDirectory).GetFiles("*.*");
//foreach (var file in files)
//{
//    try
//    {


//        if (file.CreationTime < DateTime.Now.AddDays(-RetentionPeriodinDays))
//        {
//            file.Delete();
//        }
//        else
//        {
//            //Creating Folders in Archive with year and month
//            string folder_YearMonth = $"{file.LastWriteTime.Year}{file.LastWriteTime.Month:D2}";
//            var destDirectory = Path.Combine(CopytoDirectory, folder_YearMonth);
//            var destFileName = Path.Combine(destDirectory, file.Name);
//            if (!File.Exists(destDirectory))
//                Directory.CreateDirectory(destDirectory);

//            File.Move(file.FullName, destFileName);


//        }


//    }
//    catch (Exception)
//    {


//        throw;
//    }


//}

