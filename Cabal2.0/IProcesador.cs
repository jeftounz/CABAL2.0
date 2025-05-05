using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Cabal2._0
{
    // Define el contrato que debe cumplir cualquier clase que procese archivos
    public interface IProcesador
    {
        // Método para convertir un archivo de texto en un DataTable filtrado por idioma
        // - location: Ruta del archivo
        // - idiomaFiltro: Idioma para filtrar (ej. "Español")
        DataTable DataTableFromFile(string location, string idiomaFiltro = null);
    }
}
