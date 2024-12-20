using System;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Forms;

namespace SistemaVentas
{
    public partial class CierreDeCaja : Form
    {
        public CierreDeCaja()
        {
            InitializeComponent();
            btnGenerarCierre.Click += BtnGenerarCierre_Click;
        }

        private void BtnGenerarCierre_Click(object sender, EventArgs e)
        {
            GenerarDetallesCierreCaja();
        }

        private void GenerarDetallesCierreCaja()
        {
            string connectionString = "Data Source=sistema.db;Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Consulta para facturas pagadas en efectivo
                    string queryContado = @"
                        SELECT 
                            F.NumeroFactura AS 'Número de Factura',
                            F.Fecha AS 'Fecha',
                            C.Nombre AS 'Cliente',
                            P.Nombre AS 'Producto',
                            D.Cantidad AS 'Cantidad',
                            D.PrecioUnitario AS 'Precio Unitario',
                            D.Subtotal AS 'Subtotal'
                        FROM Facturas F
                        JOIN Clientes C ON F.ClienteID = C.ID
                        JOIN DetalleFacturas D ON F.NumeroFactura = D.NumeroFactura
                        JOIN Productos P ON D.ID_Producto = P.ID
                        WHERE DATE(F.Fecha) = DATE('now') AND F.MetodoPago = 'CONTADO'
                        ORDER BY F.NumeroFactura";

                    // Consulta para facturas pagadas con transferencia
                    string queryTransferencia = @"
                        SELECT 
                            F.NumeroFactura AS 'Número de Factura',
                            F.Fecha AS 'Fecha',
                            C.Nombre AS 'Cliente',
                            P.Nombre AS 'Producto',
                            D.Cantidad AS 'Cantidad',
                            D.PrecioUnitario AS 'Precio Unitario',
                            D.Subtotal AS 'Subtotal'
                        FROM Facturas F
                        JOIN Clientes C ON F.ClienteID = C.ID
                        JOIN DetalleFacturas D ON F.NumeroFactura = D.NumeroFactura
                        JOIN Productos P ON D.ID_Producto = P.ID
                        WHERE DATE(F.Fecha) = DATE('now') AND F.MetodoPago = 'TRANSFERENCIA'
                        ORDER BY F.NumeroFactura";

                    // Calcular totales
                    string queryTotalContado = @"
                        SELECT SUM(F.Total) AS TotalContado
                        FROM Facturas F
                        WHERE DATE(F.Fecha) = DATE('now') AND F.MetodoPago = 'CONTADO'";

                    string queryTotalTransferencia = @"
                        SELECT SUM(F.Total) AS TotalTransferencia
                        FROM Facturas F
                        WHERE DATE(F.Fecha) = DATE('now') AND F.MetodoPago = 'TRANSFERENCIA'";

                    // Llenar los DataGridViews
                    dgvFacturasContado.DataSource = ObtenerDatos(connection, queryContado);
                    dgvFacturasTransferencia.DataSource = ObtenerDatos(connection, queryTransferencia);

                    // Obtener totales y mostrarlos en los labels
                    lblTotalContado.Text = $"Total Contado: ${ObtenerTotal(connection, queryTotalContado):F2}";
                    lblTotalTransferencia.Text = $"Total Transferencia: ${ObtenerTotal(connection, queryTotalTransferencia):F2}";

                    MessageBox.Show("Cierre de caja generado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el cierre de caja: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ObtenerDatos(SQLiteConnection connection, string query)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        private decimal ObtenerTotal(SQLiteConnection connection, string query)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                object result = command.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }
    }
}
