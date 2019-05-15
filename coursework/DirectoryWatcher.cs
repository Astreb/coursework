using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coursework
{
    class DirectoryWatcher
    {
        string directoryPath;
        MainWindow window;

        public DirectoryWatcher(MainWindow window, string directoryPath)
        {
            this.directoryPath = directoryPath;
            this.window = window;
        }

        public void PrintChangesLog()
        {

        }
    }
}
