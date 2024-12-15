 using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SistemaVentas
    {
        public partial class formularioRegistroCliente : Form
        {
            public string CedulaCliente { get; private set; }
            public string NombreCliente { get; private set; }

            public formularioRegistroCliente(string cedulaInicial)
            {
                InitializeComponent();
                txtCedula.Text = cedulaInicial; // Mostrar la cédula escrita previamente
            }

            private void BtnGuardar_Click(object sender, EventArgs e)
            {
                try
                {
                    string cedula = txtCedula.Text.Trim();
                    string nombre = txtNombre.Text.Trim();
                    string direccion = txtDireccion.Text.Trim();
                    string correo = txtCorreo.Text.Trim();
                    string telefono = txtTelefono.Text.Trim();

                    if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(direccion))
                    {
                        MessageBox.Show("Los campos Cédula, Nombre y Dirección son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string connectionString = "Data Source=sistema.db;Version=3;";
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                        INSERT INTO Clientes (Cedula, Nombre, Direccion, Correo, Telefono)
                        VALUES (@cedula, @nombre, @direccion, @correo, @telefono)";
                        SQLiteCommand command = new SQLiteCommand(query, connection);
                        command.Parameters.AddWithValue("@cedula", cedula);
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@direccion", direccion);
                        command.Parameters.AddWithValue("@correo", correo);
                        command.Parameters.AddWithValue("@telefono", telefono);

                        command.ExecuteNonQuery();
                    }

                    // Propagar los datos registrados
                    CedulaCliente = cedula;
                    NombreCliente = nombre;

                    this.DialogResult = DialogResult.OK; // Indicar que todo fue exitoso
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al registrar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnCancelar_Click(object sender, EventArgs e)
            {
                this.DialogResult = DialogResult.Cancel; // Cerrar sin registrar
                this.Close();
            }
        }
    }
