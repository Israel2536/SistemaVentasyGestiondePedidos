using System.Drawing;
using System.Windows.Forms;

namespace SistemaVentas
{
    partial class formularioRegistroCliente
    {
        private TextBox txtCedula;
        private TextBox txtNombre;
        private TextBox txtDireccion;
        private TextBox txtCorreo;
        private TextBox txtTelefono;
        private Button btnGuardar;
        private Button btnCancelar;

        private void InitializeComponent()
        {
            this.Text = "Registrar Cliente";
            this.ClientSize = new Size(400, 300);

            Label lblCedula = new Label { Text = "Cédula:", Location = new Point(20, 20), AutoSize = true };
            txtCedula = new TextBox { Location = new Point(120, 20), Width = 200, ReadOnly = true };

            Label lblNombre = new Label { Text = "Nombre:", Location = new Point(20, 60), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(120, 60), Width = 200 };

            Label lblDireccion = new Label { Text = "Dirección:", Location = new Point(20, 100), AutoSize = true };
            txtDireccion = new TextBox { Location = new Point(120, 100), Width = 200 };

            Label lblCorreo = new Label { Text = "Correo:", Location = new Point(20, 140), AutoSize = true };
            txtCorreo = new TextBox { Location = new Point(120, 140), Width = 200 };

            Label lblTelefono = new Label { Text = "Teléfono:", Location = new Point(20, 180), AutoSize = true };
            txtTelefono = new TextBox { Location = new Point(120, 180), Width = 200 };

            btnGuardar = new Button { Text = "Guardar", Location = new Point(120, 220), Width = 80 };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button { Text = "Cancelar", Location = new Point(220, 220), Width = 80 };
            btnCancelar.Click += BtnCancelar_Click;

            this.Controls.Add(lblCedula);
            this.Controls.Add(txtCedula);
            this.Controls.Add(lblNombre);
            this.Controls.Add(txtNombre);
            this.Controls.Add(lblDireccion);
            this.Controls.Add(txtDireccion);
            this.Controls.Add(lblCorreo);
            this.Controls.Add(txtCorreo);
            this.Controls.Add(lblTelefono);
            this.Controls.Add(txtTelefono);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }
    }
}
