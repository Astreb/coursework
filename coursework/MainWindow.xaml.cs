using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace coursework
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DirectoryWatcher watcher;
        FileSystemWatcher fs;
        DateTime fsLastRaised;

        public MainWindow()
        {
            InitializeComponent();
            this.watcher = null;
        }


        private void RunWatcher()
        {
            changesLog.Items.Clear();
            this.changesLog.Items.Add("Начало работы.");
            fs.EnableRaisingEvents = true;
        }


        private void StopWatcher()
        {
            this.changesLog.Items.Add("Работа завершена.");
            fs.EnableRaisingEvents = false;
        }


        private void DirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.directoryPath.Text = fbd.SelectedPath;
            }
        }

        protected void OnRenamed(object fsrenamed, RenamedEventArgs changeEvent)
        {

            if (DateTime.Now.Subtract(fsLastRaised).TotalMilliseconds > 1000)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    string log = watcher.get_log_line(changeEvent.FullPath, changeEvent, "Renamed");
                    changesLog.Items.Add(log);
                }));

            }
        }

        protected void OnDeleted(object fsdeleted, FileSystemEventArgs changeEvent)
        {
                if (DateTime.Now.Subtract(fsLastRaised).TotalMilliseconds > 1000)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                        {
                            string log = watcher.get_log_line(changeEvent.FullPath, changeEvent, "Deleted");
                            changesLog.Items.Add(log);
                        }));
                }
        }

        protected void OnChanged(object fschanged, FileSystemEventArgs changeEvent)
        {
            if (DateTime.Now.Subtract(fsLastRaised).TotalMilliseconds > 1000)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    string log = watcher.get_log_line(changeEvent.FullPath, changeEvent, "Changed");
                    changesLog.Items.Add(log);
                }));
            }
        }


        protected void OnCreated(object fscreated, FileSystemEventArgs changeEvent)
        {
            if (DateTime.Now.Subtract(fsLastRaised).TotalMilliseconds > 1000)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    string log = watcher.get_log_line(changeEvent.FullPath, changeEvent, "Created");
                    changesLog.Items.Add(log);
                }));
            }
        }


        void LogBufferError(object sender, ErrorEventArgs event_)
        {
            string log = string.Format("{0} | Переполнен внутренний буфер", DateTime.Now);
            this.Dispatcher.Invoke((Action)(() =>
            {
                changesLog.Items.Add(log);
                StopWatcher();
            }));
        }


        private void RunButtonClick(object sender, RoutedEventArgs e)
        {
            bool status = true;
            if (this.runButton.Content.ToString() == "Включить мониторинг")
                status = true;
            else
                status = false;

            if (!status)
            {
                this.runButton.Content = "Включить мониторинг";
                StopWatcher();
            }
            else
            {
                string directoryPath = this.directoryPath.Text;
                bool folderExists = Directory.Exists(directoryPath);

                if (!folderExists)
                {
                    System.Windows.Forms.MessageBox.Show("Выбранная директория не существует!",
                                                "Мониторинг", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                watcher = new DirectoryWatcher(this, directoryPath);
                string filter = filterSelector.Text;
                if (filter == "")
                    filter = "*.*";
                fs = new FileSystemWatcher(directoryPath, filter);

                //fs.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                //                  NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;

                RunWatcher();
                fs.IncludeSubdirectories = true;
                fs.Created += new FileSystemEventHandler(OnCreated);
                fs.Changed += new FileSystemEventHandler(OnChanged);
                fs.Renamed += new RenamedEventHandler(OnRenamed);
                fs.Deleted += new FileSystemEventHandler(OnDeleted);
                fs.Error += new ErrorEventHandler(LogBufferError);
                this.runButton.Content = "Завершить мониторинг";
            }
        }
    }
}
