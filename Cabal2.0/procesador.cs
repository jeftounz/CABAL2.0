using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Cabal2._0
{
    // Clase que implementa la lógica para procesar archivos de texto
    public class Procesador : IProcesador, IDisposable
    {
        // Convierte un archivo en DataTable aplicando filtros
        public DataTable DataTableFromTextFile(string location, char delimeter = ',', string idiomaFiltro = null)
        {
            // 1. Lee y filtra las líneas del archivo
            var lineas = LeerLineasFiltradas(location, delimeter, idiomaFiltro).ToArray();

            // 2. Convierte las líneas filtradas en DataTable
            return ConvertirLineasADataTable(lineas, delimeter);
        }

        // Lee un archivo línea por línea y filtra por idioma
        private IEnumerable<string> LeerLineasFiltradas(string ruta, char delimitador, string idiomaFiltro)
        {
            using (var reader = new StreamReader(ruta)) // Libera recursos al finalizar
            {
                // Lee la cabecera (primera línea)
                string cabecera = reader.ReadLine();
                yield return cabecera;

                // Busca la posición de la columna "Idioma"
                int indiceIdioma = Array.IndexOf(cabecera.Split(delimitador), "Idioma");
                if (indiceIdioma == -1) yield break; // Si no existe, termina

                // Lee el resto de líneas y filtra
                string linea;
                while ((linea = reader.ReadLine()) != null)
                {
                    if (linea.Split(delimitador)[indiceIdioma].Trim()
                        .Equals(idiomaFiltro, StringComparison.OrdinalIgnoreCase))
                        yield return linea; // Devuelve solo líneas que coincidan con el idioma
                }
            }
        }

        // Convierte un array de líneas en DataTable
        private DataTable ConvertirLineasADataTable(string[] lineas, char delimeter)
        {
            DataTable dt = new DataTable();
            if (lineas.Length == 0) return dt; // Si no hay datos, retorna tabla vacía

            // Agrega las columnas desde la cabecera
            foreach (string columna in lineas[0].Split(delimeter))
                dt.Columns.Add(columna.Trim(), typeof(string));

            // Agrega las filas (omitiendo la cabecera)
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] valores = lineas[i].Split(delimeter);
                DataRow dr = dt.NewRow();
                for (int j = 0; j < valores.Length && j < dt.Columns.Count; j++)
                    dr[j] = valores[j].Trim();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        // Para liberar recursos no administrados (si los hubiera)
        public void Dispose()
        {
            // Ejemplo: Cerrar conexiones a BD o archivos abiertos .
        }
    }
}