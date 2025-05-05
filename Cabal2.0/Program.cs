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

            // Crea una instancia de Procesador (implementación de IProcesador)
            IProcesador procesador = new Procesador(); //  Aquí instanciamos la dependencia

            // Pasa el procesador al constructor de Form1
            Application.Run(new Form1(procesador)); // Ahora sí cumple con el parámetro requerido
        }
    }
}