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
        private int indiceFacturaActual = 0; // Controla el índice de la factura
        private List<string> listaFacturas = new List<string>(); // Guarda las facturas disponibles

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

        private void BtnGenerarCierreCaja_Click(object sender, EventArgs e)
        {
            try
            {
                // Validación de fechas seleccionadas
                DateTime fechaInicio = dtpInicio.Value;
                DateTime fechaFin = dtpFin.Value;

                if (fechaInicio > fechaFin)
                {
                    MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha fin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Abrir la pantalla de cierre de caja
                using (var cierreDeCajaForm = new CierreDeCaja())
                {
                    cierreDeCajaForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el cierre de caja: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void PantallaPrincipal_Load(object sender, EventArgs e)
        {
            listaFacturas.Clear();
            string query = @"
    SELECT NumeroFactura 
    FROM Facturas 
    WHERE DATE(Fecha) = DATE('now') -- Restricción de fecha del día actual
    ORDER BY Fecha ASC";


            using (var conexion = new SQLiteConnection("Data Source=miBaseDeDatos.db;Version=3;"))
            {
                conexion.Open();
                using (var cmd = new SQLiteCommand(query, conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaFacturas.Add(reader["NumeroFactura"].ToString());
                        }
                    }
                }
            }

            if (listaFacturas.Count > 0)
            {
                indiceFacturaActual = listaFacturas.Count - 1; // Por defecto, carga la última factura
                CargarFactura(listaFacturas[indiceFacturaActual]);
            }
        }

        private void CargarFactura(string numeroFactura)
        {
            try
            {
                // Consulta SQL para cargar la factura y su detalle
                string query = "SELECT P.Nombre AS Producto, DF.Cantidad, DF.PrecioUnitario, DF.Subtotal " +
                               "FROM DetalleFacturas DF " +
                               "JOIN Productos P ON DF.ID_Producto = P.ID " +
                               "WHERE DF.NumeroFactura = @NumeroFactura";

                using (var conexion = new SQLiteConnection("Data Source=miBaseDeDatos.db;Version=3;"))
                {
                    conexion.Open();
                    using (var cmd = new SQLiteCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@NumeroFactura", numeroFactura);
                        using (var reader = cmd.ExecuteReader())
                        {
                            dgvCarrito.Rows.Clear(); // Limpia la tabla

                            while (reader.Read())
                            {
                                dgvCarrito.Rows.Add(
                                    reader["Producto"].ToString(),
                                    "", // Columna descripción vacía
                                    reader["PrecioUnitario"].ToString(),
                                    reader["Cantidad"].ToString(),
                                    reader["Subtotal"].ToString()
                                );
                            }
                        }
                    }
                }
                txtNumeroFactura.Text = numeroFactura; // Actualiza el número de factura en pantalla
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la factura: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    // Guardar en Facturas
                    string queryFactura = @"
                INSERT INTO Facturas (NumeroFactura, Fecha, ClienteID, Total, MetodoPago)
                VALUES (@numeroFactura, @fecha, @clienteID, @total, @metodoPago)";

                    using (SQLiteCommand commandFactura = new SQLiteCommand(queryFactura, connection))
                    {
                        commandFactura.Parameters.AddWithValue("@numeroFactura", txtNumeroFactura.Text);
                        commandFactura.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        commandFactura.Parameters.AddWithValue("@clienteID", ObtenerClienteID(txtCedulaCliente.Text));
                        commandFactura.Parameters.AddWithValue("@total", Convert.ToDecimal(txtTotal.Text.Replace("$", "").Trim()));
                        commandFactura.Parameters.AddWithValue("@metodoPago", cmbMetodoPago.SelectedItem?.ToString() ?? "CONTADO");

                        commandFactura.ExecuteNonQuery();
                    }

                    // Guardar detalles en DetalleFacturas
                    foreach (DataGridViewRow row in dgvCarrito.Rows)
                    {
                        if (row.Cells["Producto"].Value != null)
                        {
                            string queryDetalle = @"
                        INSERT INTO DetalleFacturas (NumeroFactura, ID_Producto, Cantidad, PrecioUnitario, Subtotal)
                        VALUES (@numeroFactura, @idProducto, @cantidad, @precioUnitario, @subtotal)";

                            using (SQLiteCommand commandDetalle = new SQLiteCommand(queryDetalle, connection))
                            {
                                int idProducto = ObtenerIDProducto(row.Cells["Producto"].Value.ToString());
                                commandDetalle.Parameters.AddWithValue("@numeroFactura", txtNumeroFactura.Text);
                                commandDetalle.Parameters.AddWithValue("@idProducto", idProducto);
                                commandDetalle.Parameters.AddWithValue("@cantidad", row.Cells["Cantidad"].Value);
                                commandDetalle.Parameters.AddWithValue("@precioUnitario", row.Cells["PrecioUnitario"].Value);
                                commandDetalle.Parameters.AddWithValue("@subtotal", row.Cells["ValorTotal"].Value);

                                commandDetalle.ExecuteNonQuery();
                            }
                        }
                    }
                }

                MessageBox.Show("Factura guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la factura: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObtenerIDProducto(string nombreProducto)
        {
            string connectionString = "Data Source=sistema.db;Version=3;";
            int idProducto = -1;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID FROM Productos WHERE Nombre = @nombreProducto";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombreProducto", nombreProducto);
                    object result = command.ExecuteScalar();

                    if (result != null)
                        idProducto = Convert.ToInt32(result);
                }
            }
            return idProducto;
        }


        private int ObtenerClienteID(string cedulaCliente)
        {
            string connectionString = "Data Source=sistema.db;Version=3;";
            int clienteID = -1;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID FROM Clientes WHERE Cedula = @cedula";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cedula", cedulaCliente);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        clienteID = Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("Cliente no encontrado. Verifique la cédula.");
                    }
                }
            }

            return clienteID;
        }




        // Evento para el botón "Atrás"
        private void BtnFacturaAtras_Click(object sender, EventArgs e)
        {
            try
            {
                // Número de factura actual
                string numeroFacturaActual = txtNumeroFactura.Text.Trim();

                string connectionString = "Data Source=sistema.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Consulta SQL para obtener la factura anterior del día actual
                    string query = @"
                SELECT NumeroFactura, Fecha, ClienteID, Total, MetodoPago 
                FROM Facturas 
                WHERE NumeroFactura < @numeroFacturaActual 
                AND DATE(Fecha) = DATE('now') -- Solo del día actual
                ORDER BY NumeroFactura DESC 
                LIMIT 1";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@numeroFacturaActual", numeroFacturaActual);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Actualiza los campos principales
                                txtNumeroFactura.Text = reader["NumeroFactura"].ToString();
                                txtNumeroOrden.Text = reader["NumeroFactura"].ToString(); // Sincroniza Número Orden
                                txtFecha.Text = reader["Fecha"].ToString();
                                txtCedulaCliente.Text = reader["ClienteID"].ToString();
                                txtTotal.Text = Convert.ToDecimal(reader["Total"]).ToString("C2");
                                cmbMetodoPago.Text = reader["MetodoPago"].ToString();

                                // Cargar el detalle de la factura
                                CargarDetalleFactura(txtNumeroFactura.Text);
                            }
                            else
                            {
                                MessageBox.Show("No hay facturas anteriores para el día actual.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la factura anterior: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void CargarDetalleFactura(string numeroFactura)
        {
            string connectionString = "Data Source=sistema.db;Version=3;";
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Limpia el DataGridView antes de cargar nuevos detalles
                    dgvCarrito.Rows.Clear();

                    // Consulta para obtener los detalles de la factura
                    string queryDetalle = @"
                SELECT p.Nombre AS Producto, d.Cantidad, d.PrecioUnitario, d.Subtotal
                FROM DetalleFacturas d
                JOIN Productos p ON d.ID_Producto = p.ID
                WHERE d.NumeroFactura = @numeroFactura";

                    using (SQLiteCommand command = new SQLiteCommand(queryDetalle, connection))
                    {
                        command.Parameters.AddWithValue("@numeroFactura", numeroFactura);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvCarrito.Rows.Add(
                                    reader["Producto"].ToString(),
                                    "", // Columna de descripción vacía
                                    Convert.ToDecimal(reader["PrecioUnitario"]).ToString("F2"),
                                    reader["Cantidad"].ToString(),
                                    Convert.ToDecimal(reader["Subtotal"]).ToString("F2")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el detalle de la factura: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void BtnFacturaAdelante_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener el número de factura actual
                string numeroFacturaActual = txtNumeroFactura.Text.Trim();

                string connectionString = "Data Source=sistema.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Consulta SQL para obtener la siguiente factura del día actual
                    string query = @"
                SELECT NumeroFactura, Fecha, ClienteID, Total, MetodoPago 
                FROM Facturas 
                WHERE NumeroFactura > @numeroFacturaActual 
                AND DATE(Fecha) = DATE('now') -- Solo del día actual
                ORDER BY NumeroFactura ASC 
                LIMIT 1";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@numeroFacturaActual", numeroFacturaActual);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Si hay una factura siguiente
                            {
                                // Actualiza los campos principales
                                txtNumeroFactura.Text = reader["NumeroFactura"].ToString();
                                txtNumeroOrden.Text = reader["NumeroFactura"].ToString(); // Sincroniza Número Orden
                                txtFecha.Text = reader["Fecha"].ToString();
                                txtCedulaCliente.Text = reader["ClienteID"].ToString();
                                txtTotal.Text = Convert.ToDecimal(reader["Total"]).ToString("C2");
                                cmbMetodoPago.Text = reader["MetodoPago"].ToString();

                                // Cargar el detalle de la factura
                                CargarDetalleFactura(txtNumeroFactura.Text);
                            }
                            else
                            {
                                MessageBox.Show("No hay facturas siguientes para el día actual.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la factura siguiente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFacturaActual_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpia los campos de la pantalla
                BtnLimpiar_Click(null, null);

                // Genera el siguiente número de factura disponible para el día actual
                string siguienteNumeroFactura = GenerarNumeroOrden();
                txtNumeroFactura.Text = siguienteNumeroFactura;
                txtNumeroOrden.Text = siguienteNumeroFactura;
                txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // Actualiza la fecha a la actual

                MessageBox.Show("Listo para registrar una nueva factura.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al preparar una nueva factura: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Fuentes
            Font boldFont = new Font("Courier New", 7, FontStyle.Bold);
            Font regularFont = new Font("Courier New", 7);

            // Encabezados de la tabla
            g.DrawLine(Pens.Black, 10, startY, contentWidth, startY);
            g.DrawString($"# Orden: {txtNumeroOrden.Text}", boldFont, Brushes.Black, contentWidth - 80, startY - 15); // Agregamos el número de orden
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

            // Dibujar borde inferior
            startY += offset;
            g.DrawLine(Pens.Black, 10, startY, contentWidth, startY);

            // Última línea con información adicional
            startY += offset;
            g.DrawString("Ánimo, nuevo pedido!!!!!!", boldFont, Brushes.Black, 10, startY);
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