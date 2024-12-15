using System.Drawing;
using System.Windows.Forms;

namespace SistemaVentas
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Controles del formulario
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Button btnIniciarSesion;
        private Button btnSalir;

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
            this.Text = "Inicio de Sesión";
            this.ClientSize = new System.Drawing.Size(400, 200);

            // Etiqueta Usuario
            Label lblUsuario = new Label();
            lblUsuario.Text = "Usuario:";
            lblUsuario.Location = new Point(50, 50);
            lblUsuario.AutoSize = true;

            // Caja de texto para Usuario
            this.txtUsuario = new TextBox();
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Location = new Point(150, 50);
            this.txtUsuario.Width = 200;

            // Etiqueta Contraseña
            Label lblContrasena = new Label();
            lblContrasena.Text = "Contraseña:";
            lblContrasena.Location = new Point(50, 100);
            lblContrasena.AutoSize = true;

            // Caja de texto para Contraseña
            this.txtContrasena = new TextBox();
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.Location = new Point(150, 100);
            this.txtContrasena.Width = 200;
            this.txtContrasena.PasswordChar = '*';

            // Botón para iniciar sesión
            this.btnIniciarSesion = new Button();
            this.btnIniciarSesion.Text = "Iniciar Sesión";
            this.btnIniciarSesion.Location = new Point(150, 150);
            this.btnIniciarSesion.Click += new System.EventHandler(this.BtnIniciarSesion_Click);

            // Botón para salir
            this.btnSalir = new Button();
            this.btnSalir.Text = "Salir";
            this.btnSalir.Location = new Point(250, 150);
            this.btnSalir.Click += new System.EventHandler(this.BtnSalir_Click);

            // Agregar controles al formulario
            this.Controls.Add(lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(lblContrasena);
            this.Controls.Add(this.txtContrasena);
            this.Controls.Add(this.btnIniciarSesion);
            this.Controls.Add(this.btnSalir);
        }
    }
}
