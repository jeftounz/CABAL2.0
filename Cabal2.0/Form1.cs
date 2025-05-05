using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace Cabal2._0
{
    public partial class Form1 : Form
    {
        // Lista de paneles para navegación
        private readonly List<Panel> _paneles = new List<Panel>();
        private int _indicePanelActual;

        // Dependencia inyectada para procesar archivos
        private readonly IProcesador _procesador;

        // Constructor con inyección de dependencias
        public Form1(IProcesador procesador)
        {
            InitializeComponent();
            _procesador = procesador;
        }

        // Evento Load del formulario
        private void Form1_Load(object sender, EventArgs e)
        {
            // Configuración inicial de paneles
            _paneles.Add(panel1);  // Panel de selección de idioma
            _paneles.Add(panel2);  // Panel de visualización de datos
            _paneles[_indicePanelActual].BringToFront();

            // Configura visibilidad de botones
            ActualizarVisibilidadBotones();
        }

        // Botón para buscar archivos
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Seleccionar archivo";
                openFileDialog.Filter = "Archivos compatibles (*.csv, *.txt)|*.csv;*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ProcesarArchivo(openFileDialog.FileName);
                }
            }
        }

        // Procesa el archivo seleccionado
        private void ProcesarArchivo(string rutaArchivo)
        {
            try
            {
                if (!IdiomaSeleccionado())
                {
                    MessageBox.Show("Por favor seleccione un idioma primero.", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                textBox1.Text = rutaArchivo;
                string idioma = RbEspanol.Checked ? "Español" : "Inglés";

                // Usa el procesador para obtener los datos
                dataGridView1.DataSource = _procesador.DataTableFromFile(rutaArchivo, idioma);

                // Ajusta automáticamente el ancho de las columnas
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el archivo:\n{ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón Continuar 
        private void btnContinuar_Click(object sender, EventArgs e)
        {
            if (_indicePanelActual < _paneles.Count - 1) 
            {
                if (!IdiomaSeleccionado())
                {
                    MessageBox.Show("Por favor seleccione un idioma.", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Por favor seleccione un archivo primero.", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _paneles[++_indicePanelActual].BringToFront();
                ActualizarVisibilidadBotones();
            }
        }

        // Botón Volver
        private void BtnVolver_Click(object sender, EventArgs e)
        {
            if (_indicePanelActual > 0)
            {
                LimpiarDataGridView();
                textBox1.Text = string.Empty;
                _paneles[--_indicePanelActual].BringToFront();
                ActualizarVisibilidadBotones();
            }
        }

        // --- Métodos auxiliares ---

        // Verifica si se seleccionó un idioma
        private bool IdiomaSeleccionado() => RbEspanol.Checked || RbIngles.Checked;

        // Actualiza la visibilidad de los botones de navegación
        private void ActualizarVisibilidadBotones()
        {
            BtnVolver.Visible = (_indicePanelActual > 0);
            btnContinuar.Visible = (_indicePanelActual < _paneles.Count - 1);

            // Cambiar texto del botón Continuar si es el último panel
            if (!btnContinuar.Visible)
            {
                btnContinuar.Text = "Finalizar";
            }
            else
            {
                btnContinuar.Text = "Continuar";
            }
        }

        // Limpia el DataGridView
        private void LimpiarDataGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        // Evento para manejar el cambio de selección de idioma
        private void RbIdioma_CheckedChanged(object sender, EventArgs e)
        {
            if (RbEspanol.Checked || RbIngles.Checked)
            {
                //Habilitar botones cuando se selecciona un idioma
                btnContinuar.Enabled = true;
            }
        }
    }
}