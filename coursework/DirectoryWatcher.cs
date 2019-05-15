using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coursework
{
    class DirectoryWatcher
    {
        string directoryPath;
        string backupPath;
        MainWindow window;

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void CreateBackupFolder()
        {
            string curDir = Directory.GetCurrentDirectory();
            string backupPath = Path.Combine(curDir, "backup");
            Directory.CreateDirectory(backupPath);
            string[] dirs = Directory.GetDirectories(directoryPath);
            string[] files = Directory.GetFiles(directoryPath);

            for (int i = 0; i < dirs.Length; i++)
            {
                string dName = dirs[i].Substring(directoryPath.Length + 1);
                DirectoryInfo source = new DirectoryInfo(directoryPath);
                DirectoryInfo target = new DirectoryInfo(backupPath);
                CopyAll(source, target);
            }
            for (int i = 0; i < files.Length; i++)
            {
                string fName = files[i].Substring(directoryPath.Length + 1);
                File.Copy(Path.Combine(directoryPath, fName), Path.Combine(backupPath, fName), true);
            }

        }

        public DirectoryWatcher(MainWindow window, string directoryPath)
        {
            this.directoryPath = directoryPath;
            this.window = window;
        }
    }
}
