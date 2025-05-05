using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cabal2._0
{
    public partial class Form1 : Form
    {
        List<Panel> listPanel = new List<Panel>();
        int index;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listPanel.Add(panel1);
            listPanel.Add(panel2);
            listPanel[index].BringToFront();

            // Configurar visibilidad inicial de los botones
            EstadoBoton();
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {

            if (index < listPanel.Count - 1)
            {
                if (!RbEspanol.Checked && !RbIngles.Checked)
                {
                    MessageBox.Show("Porfavor Seleccione un idioma!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    if (string.IsNullOrEmpty(textBox1.Text))
                    {
                        MessageBox.Show("Por favor suba un archivo primero!", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    { 
                        listPanel[++index].BringToFront();
                        EstadoBoton(); // Actualizar visibilidad de botones
                    }
                    
                }
                
            }
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            if (index > 0)
            {
                // Limpiar el DataGridView
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                // Vaciar la ruta del archivo
                textBox1.Text = string.Empty;

                // Volver al panel anterior
                listPanel[--index].BringToFront();
                EstadoBoton();
            }
        }


        // Método para actualizar la visibilidad de los botones
        private void EstadoBoton()
        {
            BtnVolver.Visible = (index > 0);
            btnContinuar.Visible = (index < listPanel.Count - 1);
        }


        //Método para buscar el archivo y procesarlo con la clase procesador
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Buscar archivo de texto",
                Filter = "txt files (*.txt)|*.txt",
                RestoreDirectory = true
            };

            string idiomaFiltro = null;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                

                if (RbEspanol.Checked)
                {
                    idiomaFiltro = "Español";
                    MessageBox.Show("Filtrando libros en Español", "Información",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (RbIngles.Checked)
                {
                    idiomaFiltro = "Inglés";
                    MessageBox.Show("Filtrando libros en Inglés", "Información",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                try
                {
                    textBox1.Text = openFileDialog1.FileName;
                    dataGridView1.DataSource = procesador.DataTableFromTextFile(
                        location: openFileDialog1.FileName,
                        idiomaFiltro: idiomaFiltro
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar el archivo: {ex.Message}", "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}