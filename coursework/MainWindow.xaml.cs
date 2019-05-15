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

        public MainWindow()
        {
            InitializeComponent();
            this.watcher = null;
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
                this.changesLog.Items.Add("Работа завершена.");
                this.watcher = null;
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

                this.watcher = new DirectoryWatcher(this, directoryPath);
                this.runButton.Content = "Завершить мониторинг";
            }
        }
    }
}
