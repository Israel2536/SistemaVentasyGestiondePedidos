using System;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace SistemaVentas
{
    public partial class PantallaPrincipal : Form
    {
        private PrintDocument printDocument;
        private Image logotipo;
        public PantallaPrincipal()
        {
            InitializeComponent();
            CargarCategorias();
            ConfigurarDatosIniciales();
            CargarLogotipo();
            btnBuscarCliente.Click += BtnBuscarCliente_Click;
            btnLimpiar.Click += BtnLimpiar_Click;
            btnImprimir.Click += BtnImprimir_Click;

            // Suscripción al evento para actualizar cuando cambia la cantidad
            dgvCarrito.CellEndEdit += DgvCarrito_CellEndEdit;
            dgvCarrito.CellContentClick += DgvCarrito_CellContentClick;
        }

        private void ConfigurarDatosIniciales()
        {
            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string numeroConsecutivo = GenerarNumeroOrden(); // Genera el número para ambos
            txtNumeroFactura.Text = numeroConsecutivo;
            txtNumeroOrden.Text = numeroConsecutivo;
            ConfigurarConsumidorFinal();
        }



        private void CargarLogotipo()
        {
            try
            {
                // Usa la ruta relativa, asumiendo que el archivo "logo.png" está junto al ejecutable
                string rutaLogotipo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                if (File.Exists(rutaLogotipo))
                {
                    logotipo = Image.FromFile(rutaLogotipo);
                }
                else
                {
                    MessageBox.Show("El logotipo no fue encontrado en la ruta especificada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el logotipo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                string cedula = txtCedulaCliente.Text.Trim();

                if (string.IsNullOrWhiteSpace(cedula))
                {
                    MessageBox.Show("Por favor, ingrese una cédula válida.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string connectionString = "Data Source=sistema.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Nombre FROM Clientes WHERE Cedula = @cedula";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@cedula", cedula);

                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        txtNombreCliente.Text = reader["Nombre"].ToString();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(
                            "No se encontró ningún cliente con esta cédula. ¿Desea registrar un nuevo cliente?",
                            "Cliente no encontrado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Yes)
                        {
                            AbrirModalRegistroCliente(cedula);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirModalRegistroCliente(string cedulaInicial)
        {
            using (var formRegistro = new formularioRegistroCliente(cedulaInicial))
            {
                if (formRegistro.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Cliente registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtCedulaCliente.Text = formRegistro.CedulaCliente;
                    txtNombreCliente.Text = formRegistro.NombreCliente;
                }
            }
        }

        private void DgvCarrito_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica si se hizo clic en la columna de "Eliminar"
            if (e.ColumnIndex == dgvCarrito.Columns["Eliminar"].Index && e.RowIndex >= 0)
            {
                try
                {
                    dgvCarrito.Rows.RemoveAt(e.RowIndex); // Elimina la fila seleccionada
                    ActualizarTotal(); // Recalcula el total después de la eliminación
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ConfigurarConsumidorFinal()
        {
            txtCedulaCliente.Text = "9999999999";
            txtNombreCliente.Text = "CONSUMIDOR FINAL";
        }

        private string GenerarNumeroOrden()
        {
            string connectionString = "Data Source=sistema.db;Version=3;";
            int siguienteNumero = 1;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Contar las facturas generadas en el día actual
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Facturas 
                        WHERE DATE(Fecha) = DATE('now')";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    siguienteNumero += Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el número de orden: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return siguienteNumero.ToString("D4"); // Número con 4 dígitos
        }

        private void CargarCategorias()
        {
            try
            {
                string connectionString = "Data Source=sistema.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Nombre FROM Categorias";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    panelCategorias.Controls.Clear();

                    while (reader.Read())
                    {
                        string categoria = reader["Nombre"].ToString();
                        Button btnCategoria = new Button
                        {
                            Text = categoria,
                            Size = new Size(150, 40),
                            BackColor = Color.LightBlue,
                            FlatStyle = FlatStyle.Flat
                        };
                        btnCategoria.Click += (s, e) => MostrarProductosPorSeccion(categoria);
                        panelCategorias.Controls.Add(btnCategoria);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarProductosPorSeccion(string categoria)
        {
            try
            {
                string connectionString = "Data Source=sistema.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT p.Nombre, p.Precio 
                        FROM Productos p
                        INNER JOIN Categorias c ON p.ID_Categoria = c.ID
                        WHERE c.Nombre = @categoria";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@categoria", categoria);

                    SQLiteDataReader reader = command.ExecuteReader();
                    panelProductos.Controls.Clear();

                    while (reader.Read())
                    {
                        string producto = reader["Nombre"].ToString();
                        decimal precio = Convert.ToDecimal(reader["Precio"]);

                        Button btnProducto = new Button
                        {
                            Text = $"{producto}\n${precio:F2}",
                            Size = new Size(150, 60),
                            BackColor = Color.LightGreen,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Tag = precio
                        };
                        btnProducto.Click += (s, e) => AgregarProducto(producto, precio);
                        panelProductos.Controls.Add(btnProducto);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar productos de la categoría: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarProducto(string producto, decimal precio)
        {
            dgvCarrito.Rows.Add(producto, precio, 1, precio);
            ActualizarTotal();
        }

        private void DgvCarrito_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCarrito.Columns["Cantidad"].Index && e.RowIndex >= 0)
            {
                try
                {
                    var cantidadCell = dgvCarrito.Rows[e.RowIndex].Cells["Cantidad"].Value;
                    var precioUnitarioCell = dgvCarrito.Rows[e.RowIndex].Cells["PrecioUnitario"].Value;

                    if (cantidadCell != null && precioUnitarioCell != null)
                    {
                        int cantidad = Convert.ToInt32(cantidadCell);
                        decimal precioUnitario = Convert.ToDecimal(precioUnitarioCell);

                        dgvCarrito.Rows[e.RowIndex].Cells["ValorTotal"].Value = cantidad * precioUnitario;
                        ActualizarTotal();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar la cantidad: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ActualizarTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (row.Cells["ValorTotal"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["ValorTotal"].Value);
                }
            }
            txtTotal.Text = total.ToString("C2");
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            ConfigurarConsumidorFinal();
            dgvCarrito.Rows.Clear();
            txtTotal.Text = "0.00";
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                // Primera impresión: Factura completa para el cliente
                printDocument = new PrintDocument();
                printDocument.PrintPage += (s, ev) => PrintDocument_PrintPage(ev, true); // true indica que es para el cliente
                PrintDialog printDialog = new PrintDialog { Document = printDocument };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();

                    // Segunda impresión: Resumen para la cocina
                    printDocument = new PrintDocument();
                    printDocument.PrintPage += (s, ev) => PrintDocument_PrintPage(ev, false); // false indica que es para la cocina
                    printDocument.Print();

                    // Guardar la factura después de imprimir ambas copias
                    GuardarFactura();

                    // Actualizar número de factura y orden
                    string nuevoNumero = GenerarNumeroOrden();
                    txtNumeroFactura.Text = nuevoNumero;
                    txtNumeroOrden.Text = nuevoNumero;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void GuardarFactura()
        {
            string connectionString = "Data Source=sistema.db;Version=3;";
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO Facturas (NumeroFactura, Fecha, ClienteID, Total, MetodoPago)
                        VALUES (@numeroFactura, @fecha, @clienteID, @total, @metodoPago)";

                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@numeroFactura", txtNumeroFactura.Text);
                    command.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@clienteID", txtCedulaCliente.Text);
                    command.Parameters.AddWithValue("@total", txtTotal.Text.Replace("$", "").Trim());
                    command.Parameters.AddWithValue("@metodoPago", cmbMetodoPago.SelectedItem?.ToString() ?? "CONTADO");

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la factura en la base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void PrintDocument_PrintPage(PrintPageEventArgs e, bool esParaCliente)
        {
            Graphics g = e.Graphics;

            // Configuración de márgenes y dimensiones
            int startY = 10;
            int offset = 20;
            int paperWidth = 270;
            int contentWidth = paperWidth - 20;
            int xCenter = paperWidth / 2;

            // Fuentes
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font regularFont = new Font("Courier New", 8);
            Font boldFont = new Font("Courier New", 8, FontStyle.Bold);

            // Si es para el cliente, imprime el encabezado
            if (esParaCliente)
            {
                // Dibujar el logo centrado
                if (logotipo != null)
                {
                    int logoWidth = 150;
                    int logoHeight = 60;
                    g.DrawImage(logotipo, xCenter - (logoWidth / 2), startY, logoWidth, logoHeight);
                    startY += logoHeight + 10;
                }

                // Encabezado
                g.DrawString("RUC: 1750071472001", regularFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;
                g.DrawString("Dirección: Juan Montalvo e Imbabura", regularFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;
                g.DrawString("N6-59, Cayambe", regularFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;
                g.DrawString("Teléfono: 0987933932", regularFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;

                // Información de la factura
                g.DrawString($"Fecha: {txtFecha.Text}", regularFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;
                g.DrawString($"Factura: {txtNumeroFactura.Text}  Orden: {txtNumeroOrden.Text}", boldFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;

                // Información del cliente
                g.DrawString("Cliente:", boldFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;
                g.DrawString($"Cédula: {txtCedulaCliente.Text}", regularFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;
                g.DrawString($"Nombre: {txtNombreCliente.Text}", regularFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
                startY += offset;
            }

            // Detalle de productos
            g.DrawLine(Pens.Black, new Point(10, startY), new Point(contentWidth, startY));
            startY += offset;
            g.DrawString("Producto      Cant.  P.Uni  Total", boldFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
            startY += offset;

            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (row.Cells["Producto"].Value != null)
                {
                    string producto = row.Cells["Producto"].Value.ToString();
                    string cantidad = row.Cells["Cantidad"].Value.ToString();
                    string precioUnitario = row.Cells["PrecioUnitario"].Value.ToString();
                    string total = row.Cells["ValorTotal"].Value.ToString();

                    // Imprimir detalle del producto
                    g.DrawString(producto, regularFont, Brushes.Black, new RectangleF(10, startY, 140, offset));
                    g.DrawString(cantidad, regularFont, Brushes.Black, new RectangleF(150, startY, 30, offset));
                    if (esParaCliente)
                    {
                        g.DrawString(precioUnitario, regularFont, Brushes.Black, new RectangleF(180, startY, 40, offset));
                        g.DrawString(total, regularFont, Brushes.Black, new RectangleF(220, startY, 50, offset));
                    }
                    startY += offset;
                }
            }

            // Total (solo para cliente)
            if (esParaCliente)
            {
                g.DrawLine(Pens.Black, new Point(10, startY), new Point(contentWidth, startY));
                startY += offset;
                g.DrawString($"TOTAL: {txtTotal.Text}", boldFont, Brushes.Black, new RectangleF(10, startY, contentWidth, offset));
            }
        }


    }
}