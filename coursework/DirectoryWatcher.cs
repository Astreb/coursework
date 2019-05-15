using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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


        public string get_md5(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return result;
            }
        }


        public long get_size(string path)
        {
            return new FileInfo(path).Length;
        }


        public string get_log_line(string path, FileSystemEventArgs changeEvent, string event_type)
        {
            string log = string.Format("{0} | {1} | {2}", DateTime.Now, changeEvent.FullPath, changeEvent.ChangeType);
            if (event_type == "Deleted")
                return log;

            FileAttributes attr = File.GetAttributes(path);

            if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                log += string.Format(" | size={0} | md5={1}", get_size(path), get_md5(path));

            return log;
        }


        public string get_log_line(string path, RenamedEventArgs changeEvent, string event_type)
        {
            string log = string.Format("{0} | {1} | {2}", DateTime.Now, changeEvent.FullPath, changeEvent.ChangeType);
            if (event_type == "Deleted")
                return log;

            FileAttributes attr = File.GetAttributes(path);

            if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                log += string.Format(" | size={0} | md5={1}", get_size(path), get_md5(path));

            log += string.Format(" | last_name={0}", changeEvent.OldName);

            return log;
        }


        public DirectoryWatcher(MainWindow window, string directoryPath)
        {
            this.directoryPath = directoryPath;
            this.window = window;
        }
    }
}
