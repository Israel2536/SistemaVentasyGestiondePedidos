using System.Drawing;
using System.Windows.Forms;

namespace SistemaVentas
{
    partial class PantallaPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        // Controles principales
        private FlowLayoutPanel panelCategorias;
        private FlowLayoutPanel panelProductos;
        private GroupBox gbDatosCliente;
        private DataGridView dgvCarrito;
        private Label lblTotal;
        private TextBox txtTotal;
        private TextBox txtCedulaCliente;
        private TextBox txtNombreCliente;
        private ComboBox cmbMetodoPago;
        private TextBox txtFecha;
        private TextBox txtNumeroFactura;
        private TextBox txtNumeroOrden;
        private Button btnBuscarCliente;
        private Button btnLimpiar;
        private Button btnImprimir;
        private TabControl tabControlPantallaPrincipal; // TabControl principal
        private TabPage tabPantallaPrincipal; // Pestaña Pantalla Principal
        private TabPage tabAgregarItems; // Pestaña Agregar Ítems
        private TextBox txtDineroDado;
        private TextBox txtVuelto;

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
            this.Text = "Sistema de Ventas";
            this.ClientSize = new Size(1200, 800);

            // Inicializa el TabControl
            tabControlPantallaPrincipal = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Pestaña Pantalla Principal
            tabPantallaPrincipal = new TabPage("Pantalla Principal");

            // Inicializar controles existentes y moverlos a la pestaña principal
            InicializarControlesPantallaPrincipal();
            tabPantallaPrincipal.Controls.Add(panelCategorias);
            tabPantallaPrincipal.Controls.Add(panelProductos);
            tabPantallaPrincipal.Controls.Add(gbDatosCliente);
            tabPantallaPrincipal.Controls.Add(dgvCarrito);
            tabPantallaPrincipal.Controls.Add(lblTotal);
            tabPantallaPrincipal.Controls.Add(txtTotal);
            tabPantallaPrincipal.Controls.Add(cmbMetodoPago);
            tabPantallaPrincipal.Controls.Add(btnImprimir);

            // Pestaña Agregar Ítems
            tabAgregarItems = new TabPage("Agregar Ítems");

            // Crear una instancia del formulario AgregarItems
            var agregarItemsForm = new AgregarItems
            {
                TopLevel = false, // Necesario para usarlo como control
                Dock = DockStyle.Fill
            };
            agregarItemsForm.Show();
            tabAgregarItems.Controls.Add(agregarItemsForm);

            // Agregar pestañas al TabControl
            tabControlPantallaPrincipal.TabPages.Add(tabPantallaPrincipal);
            tabControlPantallaPrincipal.TabPages.Add(tabAgregarItems);

            // Agregar el TabControl al formulario principal
            this.Controls.Add(tabControlPantallaPrincipal);
            // Label para "Dinero Recibido"
            Label lblDineroDado = new Label
            {
                Text = "Dinero Recibido:",
                Location = new Point(1040, 560), // Debajo del Total
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            tabPantallaPrincipal.Controls.Add(lblDineroDado);

            // TextBox para "Dinero Recibido"
            txtDineroDado = new TextBox
            {
                Location = new Point(1150, 555), // Alineado con el Total
                Width = 100,
                Font = new Font("Arial", 10)
            };
            tabPantallaPrincipal.Controls.Add(txtDineroDado);

            // Label para "Vuelto"
            Label lblVuelto = new Label
            {
                Text = "Vuelto:",
                Location = new Point(1040, 590), // Debajo del Dinero Recibido
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            tabPantallaPrincipal.Controls.Add(lblVuelto);

            // TextBox para "Vuelto"
            txtVuelto = new TextBox
            {
                Location = new Point(1150, 585),
                Width = 100,
                ReadOnly = true,
                Font = new Font("Arial", 10)
            };
            tabPantallaPrincipal.Controls.Add(txtVuelto);
            txtDineroDado.TextChanged += TxtDineroDado_TextChanged;
            // Botón Atrás
            Button btnFacturaAtras = new Button
            {
                Text = "Atrás",
                Location = new Point(50, 700), // Ajusta la posición (X, Y)
                Size = new Size(80, 30)
            };
            btnFacturaAtras.Click += BtnFacturaAtras_Click;
            tabPantallaPrincipal.Controls.Add(btnFacturaAtras); // Agregado al TabPage

            // Botón Adelante
            Button btnFacturaAdelante = new Button
            {
                Text = "Adelante",
                Location = new Point(150, 700), // Ajusta la posición (X, Y)
                Size = new Size(80, 30)
            };
            btnFacturaAdelante.Click += BtnFacturaAdelante_Click;
            tabPantallaPrincipal.Controls.Add(btnFacturaAdelante); // Agregado al TabPage

            // Botón Actual
            Button btnFacturaActual = new Button
            {
                Text = "Actual",
                Location = new Point(250, 700), // Ajusta la posición (X, Y)
                Size = new Size(80, 30)
            };
            btnFacturaActual.Click += BtnFacturaActual_Click;
            tabPantallaPrincipal.Controls.Add(btnFacturaActual); // Agregado al TabPage


        }

        private void InicializarControlesPantallaPrincipal()
        {
            // Panel Categorías
            panelCategorias = new FlowLayoutPanel
            {
                //Dock = DockStyle.Left,
                Location = new Point(0, 0),   // Mantener la posición inicial
                Size = new Size(200, 400),
                BackColor = Color.White,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true
            };

            // Panel Productos
            panelProductos = new FlowLayoutPanel
            {
                Location = new Point(210, 10),
                Size = new Size(400, 500),
                BackColor = Color.LightYellow,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true
            };

            // Grupo de Datos del Cliente
            gbDatosCliente = new GroupBox
            {
                Text = "Datos del Cliente",
                Location = new Point(620, 10),
                Size = new Size(560, 180)
            };

            Label lblCedula = new Label { Text = "Cédula:", Location = new Point(10, 30), AutoSize = true };
            txtCedulaCliente = new TextBox { Location = new Point(70, 25), Size = new Size(200, 25) };
            btnBuscarCliente = new Button { Text = "Buscar", Location = new Point(280, 25), Size = new Size(80, 25) };
            btnLimpiar = new Button { Text = "Limpiar", Location = new Point(370, 25), Size = new Size(80, 25) };
            Label lblNombre = new Label { Text = "Nombre:", Location = new Point(10, 70), AutoSize = true };
            txtNombreCliente = new TextBox { Location = new Point(70, 65), Size = new Size(300, 25), ReadOnly = true };
            Label lblFecha = new Label { Text = "Fecha:", Location = new Point(10, 110), AutoSize = true };
            txtFecha = new TextBox { Location = new Point(70, 105), Size = new Size(200, 25), ReadOnly = true };
            Label lblNumeroFactura = new Label { Text = "Número de Factura:", Location = new Point(280, 110), AutoSize = true };
            txtNumeroFactura = new TextBox { Location = new Point(400, 105), Size = new Size(100, 25), ReadOnly = true };
            Label lblNumeroOrden = new Label { Text = "Número Orden:", Location = new Point(10, 150), AutoSize = true };
            txtNumeroOrden = new TextBox { Location = new Point(120, 145), Size = new Size(150, 25), ReadOnly = true };

            gbDatosCliente.Controls.Add(lblCedula);
            gbDatosCliente.Controls.Add(txtCedulaCliente);
            gbDatosCliente.Controls.Add(btnBuscarCliente);
            gbDatosCliente.Controls.Add(btnLimpiar);
            gbDatosCliente.Controls.Add(lblNombre);
            gbDatosCliente.Controls.Add(txtNombreCliente);
            gbDatosCliente.Controls.Add(lblFecha);
            gbDatosCliente.Controls.Add(txtFecha);
            gbDatosCliente.Controls.Add(lblNumeroFactura);
            gbDatosCliente.Controls.Add(txtNumeroFactura);
            gbDatosCliente.Controls.Add(lblNumeroOrden);
            gbDatosCliente.Controls.Add(txtNumeroOrden);

            // DataGridView
            dgvCarrito = new DataGridView
            {
                Location = new Point(620, 200),
                Size = new Size(560, 300),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvCarrito.Columns.Add("Producto", "Producto");
            DataGridViewTextBoxColumn colDescripcion = new DataGridViewTextBoxColumn
            {
                Name = "Descripcion",
                HeaderText = "Descripción",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dgvCarrito.Columns.Add(colDescripcion);
            dgvCarrito.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvCarrito.Columns.Add("Cantidad", "Cantidad");
            dgvCarrito.Columns.Add("ValorTotal", "Valor Total");

            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                HeaderText = "Eliminar",
                Text = "X",
                UseColumnTextForButtonValue = true
            };
            dgvCarrito.Columns.Add(colEliminar);

            // Botón Imprimir
            btnImprimir = new Button
            {
                Text = "Imprimir",
                Location = new Point(820, 560),
                Size = new Size(100, 30)
            };

            // Total y Métodos de Pago
            lblTotal = new Label
            {
                Text = "Total:",
                Location = new Point(1040, 520),
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold)
            };
            txtTotal = new TextBox
            {
                Location = new Point(1100, 515),
                Width = 150,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ReadOnly = true
            };

            Label lblMetodoPago = new Label { Text = "Pago:", Location = new Point(620, 520), AutoSize = true };
            cmbMetodoPago = new ComboBox { Location = new Point(670, 515), Size = new Size(150, 25) };
            cmbMetodoPago.Items.AddRange(new[] { "CONTADO", "TRANSFERENCIA" });
        }
    }
}
