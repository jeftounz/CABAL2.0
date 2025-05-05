using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Cabal2._0
{
    // Implementa IProcesador (contrato para procesamiento) y IDisposable (para liberar recursos)
    public class Procesador : IProcesador, IDisposable
    {
        // Método principal que convierte un archivo en DataTable filtrando por idioma
        public DataTable DataTableFromFile(string location, string idiomaFiltro = null)
        {
            // 1. Detecta automáticamente el delimitador (coma, punto y coma o tabulación)
            char delimitador = DeterminarDelimitador(location);

            // 2. Lee y filtra las líneas del archivo
            var lineas = LeerLineasFiltradas(location, delimitador, idiomaFiltro).ToArray();

            // 3. Convierte las líneas filtradas en un DataTable
            return ConvertirLineasADataTable(lineas, delimitador);
        }

        // Determina el carácter delimitador analizando la primera línea del archivo
        private char DeterminarDelimitador(string filePath)
        {
            string primeraLinea = File.ReadLines(filePath).First(); // Lee solo la primera línea
            if (primeraLinea.Contains(';')) return ';';  // CSV con punto y coma
            if (primeraLinea.Contains('\t')) return '\t'; // TSV (tabulación)
            return ','; // CSV estándar o TXT con comas (valor por defecto)
        }

        // Lee el archivo línea por línea y filtra por idioma usando yield return (eficiente en memoria)
        private IEnumerable<string> LeerLineasFiltradas(string ruta, char delimitador, string idiomaFiltro)
        {
            using (var reader = new StreamReader(ruta)) // Libera recursos automáticamente
            {
                // 1. Lee la cabecera (primera línea)
                string cabecera = reader.ReadLine();

                // Validaciones iniciales
                if (string.IsNullOrEmpty(cabecera))
                    throw new Exception("El archivo está vacío o no tiene cabecera.");

                // 2. Busca la posición de la columna "Idioma"
                int indiceIdioma = Array.IndexOf(cabecera.Split(delimitador), "Idioma");
                if (indiceIdioma == -1)
                    throw new Exception("El archivo no contiene la columna 'Idioma'.");

                yield return cabecera; // Devuelve la cabecera primero

                // 3. Procesa el resto de líneas
                string linea;
                while ((linea = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(linea)) continue; // Ignora líneas vacías

                    string[] valores = linea.Split(delimitador);

                    // Filtra por idioma (comparación insensible a mayúsculas/minúsculas)
                    if (valores.Length > indiceIdioma &&
                        valores[indiceIdioma].Trim().Equals(idiomaFiltro, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return linea; // Devuelve solo líneas que coincidan con el filtro
                    }
                }
            }
        }

        // Convierte un array de líneas en un DataTable estructurado
        private DataTable ConvertirLineasADataTable(string[] lineas, char delimeter)
        {
            DataTable dt = new DataTable();
            if (lineas.Length == 0) return dt; // Retorna tabla vacía si no hay datos

            // 1. Configura las columnas usando la primera línea (cabecera)
            string[] columnas = lineas[0].Split(delimeter);
            foreach (string columna in columnas)
                dt.Columns.Add(columna.Trim(), typeof(string)); // Todas como cadenas

            // 2. Llena las filas con los datos
            for (int i = 1; i < lineas.Length; i++) // Empieza desde 1 (omitir cabecera)
            {
                string[] valores = lineas[i].Split(delimeter);
                DataRow dr = dt.NewRow();

                // Asigna valores a cada columna (protegiendo contra índices fuera de rango)
                for (int j = 0; j < valores.Length && j < dt.Columns.Count; j++)
                    dr[j] = valores[j].Trim();

                dt.Rows.Add(dr);
            }

            return dt;
        }

        // Implementación de IDisposable para liberar recursos no administrados
        public void Dispose()
        {
            // Ejemplo: Cerrar conexiones a BD o archivos abiertos
        }
    }
}