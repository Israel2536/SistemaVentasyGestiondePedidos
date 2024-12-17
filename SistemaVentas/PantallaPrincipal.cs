using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
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
            this.WindowState = FormWindowState.Maximized;
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
            dgvCarrito.Rows.Add(producto, "", precio, 1, precio);
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

        private void TxtDineroDado_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (decimal.TryParse(txtDineroDado.Text, out decimal dineroRecibido) &&
                  decimal.TryParse(txtTotal.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal totalCompra))

                {
                    decimal vuelto = dineroRecibido - totalCompra;
                    txtVuelto.Text = vuelto >= 0 ? vuelto.ToString("C2") : "Faltante";
                }
                else
                {
                    txtVuelto.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el cálculo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                printDocument.PrintPage += (s, ev) => PrintDocument_PrintPageFactura(ev); // Llama al método exclusivo para la factura
                PrintDialog printDialog = new PrintDialog { Document = printDocument };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();

                    // Segunda impresión: Resumen para la cocina
                    printDocument = new PrintDocument();
                    printDocument.PrintPage += (s, ev) => PrintDocument_PrintPageCocina(ev); // Llama al método exclusivo para el papel de cocina
                    printDocument.Print();

                    // Guardar la factura después de imprimir ambas copias
                    GuardarFactura();

                    // Actualizar número de factura y orden
                    string nuevoNumero = GenerarNumeroOrden();
                    txtNumeroFactura.Text = nuevoNumero;
                    txtNumeroOrden.Text = nuevoNumero;
                }
                BtnLimpiar_Click(null, null);

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
        private void PrintDocument_PrintPageFactura(PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // Configuración de márgenes y dimensiones
            int startY = 10;
            int offset = 15; // Reducimos la altura entre líneas
            int paperWidth = 270;
            int contentWidth = paperWidth - 20;
            int colProductoWidth = 100; // Ancho máximo para Producto

            // Fuentes
            Font headerFont = new Font("Arial", 8, FontStyle.Bold);
            Font regularFont = new Font("Courier New", 7);
            Font boldFont = new Font("Courier New", 7, FontStyle.Bold);

            // LOGO
            if (logotipo != null)
            {
                g.DrawImage(logotipo, 60, startY, 150, 60);
                startY += 70;
            }

            // ENCABEZADO
            g.DrawString("RUC: 1750071472001", regularFont, Brushes.Black, 10, startY);
            startY += offset;
            g.DrawString("Dirección: Juan Montalvo e Imbabura", regularFont, Brushes.Black, 10, startY);
            startY += offset;
            g.DrawString("N6-59, Cayambe", regularFont, Brushes.Black, 10, startY);
            startY += offset;
            g.DrawString("Teléfono: 0987933932", regularFont, Brushes.Black, 10, startY);
            startY += offset;

            g.DrawString($"Fecha: {txtFecha.Text}", regularFont, Brushes.Black, 10, startY);
            startY += offset;
            g.DrawString($"Factura: {txtNumeroFactura.Text}  Orden: {txtNumeroOrden.Text}", boldFont, Brushes.Black, 10, startY);
            startY += offset;
            g.DrawString($"Cédula: {txtCedulaCliente.Text}", regularFont, Brushes.Black, 10, startY);
            startY += offset;
            g.DrawString($"Cliente: {txtNombreCliente.Text}", boldFont, Brushes.Black, 10, startY);
            startY += offset;

            // TABLA ENCABEZADOS
            g.DrawLine(Pens.Black, 10, startY, contentWidth, startY);
            startY += offset;
            g.DrawString("Producto", boldFont, Brushes.Black, 10, startY);
            g.DrawString("Cant.", boldFont, Brushes.Black, 120, startY);
            g.DrawString("P.Uni", boldFont, Brushes.Black, 170, startY);
            g.DrawString("Total", boldFont, Brushes.Black, 220, startY);
            startY += offset;

            // TABLA DETALLE
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (row.Cells["Producto"].Value != null)
                {
                    string producto = row.Cells["Producto"].Value.ToString();
                    string cantidad = row.Cells["Cantidad"].Value.ToString();
                    string precioUnitario = row.Cells["PrecioUnitario"].Value.ToString();
                    string total = row.Cells["ValorTotal"].Value.ToString();

                    // Aplicar WrapText al producto
                    string[] productoLineas = WrapText(producto, colProductoWidth, g, regularFont);
                    int lineHeight = offset;
                    int tempY = startY;

                    // Imprimir Producto (con salto de línea)
                    foreach (string linea in productoLineas)
                    {
                        g.DrawString(linea, regularFont, Brushes.Black, 10, tempY);
                        tempY += lineHeight;
                    }

                    // Imprimir Cantidad, P.Uni y Total alineados
                    g.DrawString(cantidad, regularFont, Brushes.Black, 120, startY);
                    g.DrawString(precioUnitario, regularFont, Brushes.Black, 170, startY);
                    g.DrawString(total, regularFont, Brushes.Black, 220, startY);

                    // Ajustar posición vertical según la altura máxima de las líneas
                    startY += productoLineas.Length * lineHeight;
                }
            }

            // LÍNEA FINAL Y TOTAL
            startY += offset;
            g.DrawLine(Pens.Black, 10, startY, contentWidth, startY);
            startY += offset;
            g.DrawString($"TOTAL: {txtTotal.Text}", boldFont, Brushes.Black, 10, startY);
        }


        private void PrintDocument_PrintPageCocina(PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // Configuración de márgenes y dimensiones
            int startY = 50; // Margen superior
            int offset = 15; // Altura entre líneas
            int paperWidth = 270;
            int contentWidth = paperWidth - 20;
            int colProductoWidth = 100;
            int colDescripcionWidth = 120;
            //int colCantidadWidth = 50;

            // Fuentes
            Font boldFont = new Font("Courier New", 7, FontStyle.Bold);
            Font regularFont = new Font("Courier New", 7);

            // Encabezados de la tabla
            g.DrawLine(Pens.Black, 10, startY, contentWidth, startY);
            startY += offset;
            g.DrawString("Producto", boldFont, Brushes.Black, 10, startY);
            g.DrawString("Descripción", boldFont, Brushes.Black, 10 + colProductoWidth, startY);
            g.DrawString("Cant.", boldFont, Brushes.Black, 10 + colProductoWidth + colDescripcionWidth, startY);
            startY += offset;

            // Detalle de productos
            foreach (DataGridViewRow row in dgvCarrito.Rows)
            {
                if (row.Cells["Producto"].Value != null)
                {
                    string producto = row.Cells["Producto"].Value.ToString();
                    string descripcion = row.Cells["Descripcion"].Value?.ToString() ?? "";
                    string cantidad = row.Cells["Cantidad"].Value.ToString();

                    int lineHeight = offset;

                    // Wrap text para Producto y Descripción
                    string[] productoLineas = WrapText(producto, colProductoWidth, g, regularFont);
                    string[] descripcionLineas = WrapText(descripcion, colDescripcionWidth, g, regularFont);

                    int maxLines = Math.Max(productoLineas.Length, descripcionLineas.Length);
                    int tempY = startY;

                    // Imprimir Producto
                    foreach (string linea in productoLineas)
                    {
                        g.DrawString(linea, regularFont, Brushes.Black, 10, tempY);
                        tempY += lineHeight;
                    }

                    // Imprimir Descripción
                    tempY = startY;
                    foreach (string linea in descripcionLineas)
                    {
                        g.DrawString(linea, regularFont, Brushes.Black, 10 + colProductoWidth, tempY);
                        tempY += lineHeight;
                    }

                    // Imprimir Cantidad (alineado a la derecha)
                    g.DrawString(cantidad, regularFont, Brushes.Black, 10 + colProductoWidth + colDescripcionWidth, startY);

                    // Ajustar la posición vertical para la siguiente fila
                    startY += maxLines * lineHeight;

                    // Dibujar una línea delgada después de cada producto
                    g.DrawLine(Pens.Gray, 10, startY - 3, contentWidth, startY - 3);
                }
            }

            // Línea final
            g.DrawLine(Pens.Black, 10, startY, contentWidth, startY);
        }



        private string[] WrapText(string text, int maxWidth, Graphics graphics, Font font)
        {
            var words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = "";

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                var testWidth = graphics.MeasureString(testLine, font).Width;

                if (testWidth > maxWidth)
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            return lines.ToArray();
        }

    }
}