using System.Drawing;
using System.Windows.Forms;

namespace SistemaVentas
{
    partial class AgregarItems
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlCrearProducto;
        private Panel pnlCrearCategoria;
        private Panel pnlEliminarProducto;
        private Panel pnlEliminarCategoria;
        private TextBox txtNombreProducto;
        private TextBox txtPrecioProducto;
        private ComboBox cbxCategoriaProducto;
        private Button btnGuardarProducto;
        private TextBox txtNombreCategoria;
        private Button btnGuardarCategoria;
        private ComboBox cbxCategoriaEliminarProducto;
        private ComboBox cbxProductoEliminar;
        private Button btnEliminarProducto;
        private ComboBox cbxCategoriaEliminar;
        private Button btnEliminarCategoria;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text = "Gestión de Items";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Panel Crear Producto
            pnlCrearProducto = new Panel
            {
                Location = new Point(50, 50),
                Size = new Size(700, 100),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlCrearProducto);

            Label lblCrearProducto = new Label
            {
                Text = "Crear Producto:",
                Location = new Point(10, 10),
                AutoSize = true
            };
            pnlCrearProducto.Controls.Add(lblCrearProducto);

            Label lblNombreProducto = new Label
            {
                Text = "Nombre:",
                Location = new Point(10, 40),
                AutoSize = true
            };
            pnlCrearProducto.Controls.Add(lblNombreProducto);

            txtNombreProducto = new TextBox
            {
                Location = new Point(80, 40),
                Size = new Size(150, 30)
            };
            pnlCrearProducto.Controls.Add(txtNombreProducto);

            Label lblPrecioProducto = new Label
            {
                Text = "Precio:",
                Location = new Point(240, 40),
                AutoSize = true
            };
            pnlCrearProducto.Controls.Add(lblPrecioProducto);

            txtPrecioProducto = new TextBox
            {
                Location = new Point(300, 40),
                Size = new Size(100, 30)
            };
            pnlCrearProducto.Controls.Add(txtPrecioProducto);

            cbxCategoriaProducto = new ComboBox
            {
                Location = new Point(410, 40),
                Size = new Size(150, 30)
            };
            pnlCrearProducto.Controls.Add(cbxCategoriaProducto);

            btnGuardarProducto = new Button
            {
                Text = "Guardar Producto",
                Location = new Point(580, 40),
                Size = new Size(100, 30)
            };
            btnGuardarProducto.Click += BtnGuardarProducto_Click;
            pnlCrearProducto.Controls.Add(btnGuardarProducto);

            // Panel Crear Categoría
            pnlCrearCategoria = new Panel
            {
                Location = new Point(50, 170),
                Size = new Size(700, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlCrearCategoria);

            Label lblCrearCategoria = new Label
            {
                Text = "Crear Categoría:",
                Location = new Point(10, 10),
                AutoSize = true
            };
            pnlCrearCategoria.Controls.Add(lblCrearCategoria);

            txtNombreCategoria = new TextBox
            {
                Location = new Point(150, 10),
                Size = new Size(200, 30)
            };
            pnlCrearCategoria.Controls.Add(txtNombreCategoria);

            btnGuardarCategoria = new Button
            {
                Text = "Guardar Categoría",
                Location = new Point(370, 10),
                Size = new Size(150, 30)
            };
            btnGuardarCategoria.Click += BtnGuardarCategoria_Click;
            pnlCrearCategoria.Controls.Add(btnGuardarCategoria);

            // Panel Eliminar Producto
            pnlEliminarProducto = new Panel
            {
                Location = new Point(50, 270),
                Size = new Size(700, 100),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlEliminarProducto);

            Label lblEliminarProducto = new Label
            {
                Text = "Eliminar Producto:",
                Location = new Point(10, 10),
                AutoSize = true
            };
            pnlEliminarProducto.Controls.Add(lblEliminarProducto);

            cbxCategoriaEliminarProducto = new ComboBox
            {
                Location = new Point(150, 10),
                Size = new Size(200, 30)
            };
            cbxCategoriaEliminarProducto.SelectedIndexChanged += CbxCategoriaEliminarProducto_SelectedIndexChanged;
            pnlEliminarProducto.Controls.Add(cbxCategoriaEliminarProducto);

            cbxProductoEliminar = new ComboBox
            {
                Location = new Point(370, 10),
                Size = new Size(150, 30)
            };
            pnlEliminarProducto.Controls.Add(cbxProductoEliminar);

            btnEliminarProducto = new Button
            {
                Text = "Eliminar Producto",
                Location = new Point(550, 10),
                Size = new Size(120, 30)
            };
            btnEliminarProducto.Click += BtnEliminarProducto_Click;
            pnlEliminarProducto.Controls.Add(btnEliminarProducto);

            // Panel Eliminar Categoría
            pnlEliminarCategoria = new Panel
            {
                Location = new Point(50, 400),
                Size = new Size(700, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlEliminarCategoria);

            Label lblEliminarCategoria = new Label
            {
                Text = "Eliminar Categoría:",
                Location = new Point(10, 10),
                AutoSize = true
            };
            pnlEliminarCategoria.Controls.Add(lblEliminarCategoria);

            cbxCategoriaEliminar = new ComboBox
            {
                Location = new Point(150, 10),
                Size = new Size(200, 30)
            };
            pnlEliminarCategoria.Controls.Add(cbxCategoriaEliminar);

            btnEliminarCategoria = new Button
            {
                Text = "Eliminar Categoría",
                Location = new Point(370, 10),
                Size = new Size(150, 30)
            };
            btnEliminarCategoria.Click += BtnEliminarCategoria_Click;
            pnlEliminarCategoria.Controls.Add(btnEliminarCategoria);
        }
    }
}
