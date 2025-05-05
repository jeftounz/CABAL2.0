using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Cabal2._0
{
    public partial class Form1 : Form
    {
        // Lista de paneles para navegación
        private readonly List<Panel> _paneles = new List<Panel>();
        private int _indicePanelActual; // Panel actual visible

        // Dependencia inyectada para procesar archivos
        private readonly IProcesador _procesador;

        // Constructor que recibe el procesador (inyección de dependencias)
        public Form1(IProcesador procesador)
        {
            _procesador = procesador;
            InitializeComponent();
        }

        // Al cargar el formulario
        private void Form1_Load(object sender, EventArgs e)
        {
            _paneles.Add(panel1); // Panel 1: Selección de idioma
            _paneles.Add(panel2); // Panel 2: Visualización de datos
            _paneles[_indicePanelActual].BringToFront(); // Muestra el panel inicial
            ActualizarVisibilidadBotones(); // Ajusta botones
        }

        // Botón "Continuar"
        private void btnContinuar_Click(object sender, EventArgs e)
        {
            if (!IdiomaSeleccionado())
            {
                MostrarError("Por favor seleccione un idioma!");
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MostrarError("Por favor suba un archivo primero!");
                return;
            }

            if (_indicePanelActual < _paneles.Count - 1)
            {
                _paneles[++_indicePanelActual].BringToFront(); // Avanza al siguiente panel
                ActualizarVisibilidadBotones();
            }
        }

        // Botón "Volver"
        private void BtnVolver_Click(object sender, EventArgs e)
        {
            if (_indicePanelActual > 0)
            {
                LimpiarDataGridView(); // Limpia la tabla
                textBox1.Text = string.Empty; // Limpia la ruta del archivo
                _paneles[--_indicePanelActual].BringToFront(); // Retrocede al panel anterior
                ActualizarVisibilidadBotones();
            }
        }

        // Botón "Buscar Archivo"
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog()) // Diálogo para seleccionar archivo
            {
                openFileDialog.Title = "Buscar archivo de texto";
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ProcesarArchivo(openFileDialog.FileName); // Procesa el archivo seleccionado
                }
            }
        }

        // === Métodos auxiliares ===

        // Verifica si se seleccionó un idioma
        private bool IdiomaSeleccionado() => RbEspanol.Checked || RbIngles.Checked;

        // Actualiza la visibilidad de los botones "Volver" y "Continuar"
        private void ActualizarVisibilidadBotones()
        {
            BtnVolver.Visible = (_indicePanelActual > 0);
            btnContinuar.Visible = (_indicePanelActual < _paneles.Count - 1);
        }

        // Limpia el DataGridView
        private void LimpiarDataGridView()
        {
            dataGridView1.DataSource = null;
        }

        // Muestra un mensaje de error
        private void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Procesa el archivo y carga los datos en el DataGridView
        private void ProcesarArchivo(string rutaArchivo)
        {
            try
            {
                textBox1.Text = rutaArchivo;
                string idioma = RbEspanol.Checked ? "Español" : "Inglés";
                dataGridView1.DataSource = _procesador.DataTableFromTextFile(rutaArchivo, ',', idioma);
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar el archivo: {ex.Message}");
            }
        }
    }
}