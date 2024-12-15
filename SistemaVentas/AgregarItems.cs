using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SistemaVentas
{
    public partial class AgregarItems : Form
    {
        public AgregarItems()
        {
            InitializeComponent();
            CargarCategoriasEnComboBox(cbxCategoriaProducto);
            CargarCategoriasEnComboBox(cbxCategoriaEliminarProducto);
            CargarCategoriasEnComboBox(cbxCategoriaEliminar);
        }

        private void BtnGuardarProducto_Click(object sender, EventArgs e)
        {
            string nombreProducto = txtNombreProducto.Text.Trim();
            string categoriaSeleccionada = cbxCategoriaProducto.SelectedItem?.ToString();
            decimal precioProducto;

            // Validar entrada de datos
            if (string.IsNullOrWhiteSpace(nombreProducto) || string.IsNullOrWhiteSpace(categoriaSeleccionada) || !decimal.TryParse(txtPrecioProducto.Text, out precioProducto))
            {
                MessageBox.Show("Completa todos los campos correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=sistema.db;Version=3;"))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Productos (Nombre, Precio, ID_Categoria)
                        VALUES (@nombre, @precio, (SELECT ID FROM Categorias WHERE Nombre = @categoria))";
                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("@nombre", nombreProducto);
                    command.Parameters.AddWithValue("@precio", precioProducto.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("@categoria", categoriaSeleccionada);

                    Console.WriteLine($"Precio enviado a SQLite: {precioProducto}"); // Debug para validar el valor enviado
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Producto guardado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar los campos después de guardar
                txtNombreProducto.Clear();
                txtPrecioProducto.Clear();
                cbxCategoriaProducto.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardarCategoria_Click(object sender, EventArgs e)
        {
            string nombreCategoria = txtNombreCategoria.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreCategoria))
            {
                MessageBox.Show("El nombre de la categoría no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=sistema.db;Version=3;"))
                {
                    connection.Open();
                    string query = "INSERT INTO Categorias (Nombre) VALUES (@nombre)";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@nombre", nombreCategoria);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Categoría guardada correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNombreCategoria.Clear();

                // Actualizar los ComboBoxes
                CargarCategoriasEnComboBox(cbxCategoriaProducto);
                CargarCategoriasEnComboBox(cbxCategoriaEliminarProducto);
                CargarCategoriasEnComboBox(cbxCategoriaEliminar);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la categoría: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarProducto_Click(object sender, EventArgs e)
        {
            string productoSeleccionado = cbxProductoEliminar.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(productoSeleccionado))
            {
                MessageBox.Show("Selecciona un producto para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=sistema.db;Version=3;"))
                {
                    connection.Open();
                    string query = "DELETE FROM Productos WHERE Nombre = @nombre";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@nombre", productoSeleccionado);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Producto eliminado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizar los productos de la categoría seleccionada
                CargarProductosDeCategoria(cbxCategoriaEliminarProducto.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarCategoria_Click(object sender, EventArgs e)
        {
            string categoriaSeleccionada = cbxCategoriaEliminar.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(categoriaSeleccionada))
            {
                MessageBox.Show("Selecciona una categoría para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=sistema.db;Version=3;"))
                {
                    connection.Open();
                    string query = "DELETE FROM Categorias WHERE Nombre = @nombre";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@nombre", categoriaSeleccionada);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Categoría eliminada correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizar los ComboBoxes
                CargarCategoriasEnComboBox(cbxCategoriaProducto);
                CargarCategoriasEnComboBox(cbxCategoriaEliminarProducto);
                CargarCategoriasEnComboBox(cbxCategoriaEliminar);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la categoría: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CbxCategoriaEliminarProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            string categoriaSeleccionada = cbxCategoriaEliminarProducto.SelectedItem?.ToString();

            if (!string.IsNullOrWhiteSpace(categoriaSeleccionada))
            {
                CargarProductosDeCategoria(categoriaSeleccionada);
            }
        }

        private void CargarCategoriasEnComboBox(ComboBox comboBox)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=sistema.db;Version=3;"))
                {
                    connection.Open();
                    string query = "SELECT Nombre FROM Categorias";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    comboBox.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox.Items.Add(reader["Nombre"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductosDeCategoria(string categoria)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=sistema.db;Version=3;"))
                {
                    connection.Open();
                    string query = @"
                        SELECT p.Nombre 
                        FROM Productos p
                        INNER JOIN Categorias c ON p.ID_Categoria = c.ID
                        WHERE c.Nombre = @categoria";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@categoria", categoria);
                    SQLiteDataReader reader = command.ExecuteReader();

                    cbxProductoEliminar.Items.Clear();

                    while (reader.Read())
                    {
                        cbxProductoEliminar.Items.Add(reader["Nombre"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
