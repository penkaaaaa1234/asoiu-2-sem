using System;
using System.Windows.Forms;
using Dz3.Data;
using Dz3.Forms;

namespace Dz3
{
    static class Program
    {
        /// <summary>
        /// Точка входа приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            DatabaseInitializer.Initialize();
            Application.Run(new MainForm());
        }
    }
}
