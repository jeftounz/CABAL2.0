using System;
using System.Data;
using System.IO;
using System.Linq;

namespace Cabal2._0
{
    public static class procesador
    {
        public static string file;

        public static DataTable DataTableFromTextFile(string location, char delimeter = ',', string idiomaFiltro = null)
        {
            // Leer todas las líneas del archivo
            string[] todasLasLineas = File.ReadAllLines(location);

            // Obtener las líneas que coinciden con el filtro de idioma
            var lineasFiltradas = FiltrarLineasPorIdioma(todasLasLineas, delimeter, idiomaFiltro);

            // Convertir a DataTable
            return ConvertirLineasADataTable(lineasFiltradas, delimeter);
        }

        private static string[] FiltrarLineasPorIdioma(string[] lineas, char delimeter, string idiomaFiltro)
        {
            if (string.IsNullOrEmpty(idiomaFiltro))
                return lineas;

            // Obtener el índice de la columna "Idioma"
            string[] cabeceras = lineas[0].Split(delimeter);
            int indiceIdioma = Array.IndexOf(cabeceras, "Idioma");

            if (indiceIdioma == -1)
                throw new Exception("El archivo no contiene la columna 'Idioma'");

            // Filtrar líneas (incluyendo siempre la cabecera)
            return lineas.Where((linea, index) =>
                index == 0 || // Incluir siempre la cabecera
                linea.Split(delimeter)[indiceIdioma].Trim().Equals(idiomaFiltro, StringComparison.OrdinalIgnoreCase)
            ).ToArray();
        }

        private static DataTable ConvertirLineasADataTable(string[] lineas, char delimeter)
        {
            DataTable dt = new DataTable();

            if (lineas.Length == 0)
                return dt;

            // Agregar columnas
            string[] columnas = lineas[0].Split(delimeter);
            foreach (string columna in columnas)
                dt.Columns.Add(columna.Trim(), typeof(string));

            // Agregar filas
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
    }
}