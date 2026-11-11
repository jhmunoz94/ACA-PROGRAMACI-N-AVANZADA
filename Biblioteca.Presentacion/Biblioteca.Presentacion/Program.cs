using System;
using System.Windows.Forms;

namespace Biblioteca.Presentacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMenu()); // El punto de arranque principal
        }
    }
}
