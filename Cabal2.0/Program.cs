using System;
using System.Windows.Forms;

namespace Cabal2._0
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configuración de dependencias
            IProcesador procesador = new Procesador();
            Application.Run(new Form1(procesador));
        }
    }
}