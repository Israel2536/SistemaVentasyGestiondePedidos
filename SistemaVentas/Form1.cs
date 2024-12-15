using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SistemaVentas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            CrearBaseDeDatos();
            ConfigurarTablas();
            InsertarUsuarioAdmin();
        }

        private void CrearBaseDeDatos()
        {
            string dbPath = "sistema.db";

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                MessageBox.Show("Base de datos creada correctamente.", "SQLite");
            }
        }

        private void ConfigurarTablas()
        {
            string connectionString = "Data Source=sistema.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTablesQuery = @"
    CREATE TABLE IF NOT EXISTS Usuarios (
        ID INTEGER PRIMARY KEY AUTOINCREMENT,
        Usuario TEXT NOT NULL UNIQUE,
        Contrasena TEXT NOT NULL
    );

    CREATE TABLE IF NOT EXISTS Categorias (
        ID INTEGER PRIMARY KEY AUTOINCREMENT,
        Nombre TEXT NOT NULL UNIQUE
    );

    CREATE TABLE IF NOT EXISTS Productos (
        ID INTEGER PRIMARY KEY AUTOINCREMENT,
        Nombre TEXT NOT NULL,
        Precio REAL NOT NULL,
        ID_Categoria INTEGER NOT NULL,
        FOREIGN KEY (ID_Categoria) REFERENCES Categorias(ID)
    );

    CREATE TABLE IF NOT EXISTS Clientes (
        ID INTEGER PRIMARY KEY AUTOINCREMENT,
        Cedula TEXT UNIQUE NOT NULL,
        Nombre TEXT NOT NULL,
        Direccion TEXT NOT NULL,
        Correo TEXT,
        Telefono TEXT
    );

    CREATE TABLE IF NOT EXISTS MetodosPago (
        ID INTEGER PRIMARY KEY AUTOINCREMENT,
        Metodo TEXT NOT NULL
    );

    CREATE TABLE IF NOT EXISTS Facturas (
        NumeroFactura TEXT PRIMARY KEY,
        Fecha DATE NOT NULL,
        ClienteID INTEGER NOT NULL,
        Total REAL NOT NULL,
        MetodoPago TEXT NOT NULL,
        FOREIGN KEY (ClienteID) REFERENCES Clientes(ID)
    );

    CREATE TABLE IF NOT EXISTS DetalleFacturas (
        ID INTEGER PRIMARY KEY AUTOINCREMENT,
        NumeroFactura TEXT NOT NULL,
        ID_Producto INTEGER NOT NULL,
        Cantidad INTEGER NOT NULL,
        PrecioUnitario REAL NOT NULL,
        Subtotal REAL NOT NULL,
        FOREIGN KEY (NumeroFactura) REFERENCES Facturas(NumeroFactura),
        FOREIGN KEY (ID_Producto) REFERENCES Productos(ID)
    );

    -- Nueva tabla Configuracion
    CREATE TABLE IF NOT EXISTS Configuracion (
        Establecimiento TEXT NOT NULL,
        PuntoEmision TEXT NOT NULL
    );";

                SQLiteCommand command = new SQLiteCommand(createTablesQuery, connection);
                command.ExecuteNonQuery();

                // Insertar métodos de pago por defecto si no existen
                string insertMetodosPagoQuery = @"
    INSERT OR IGNORE INTO MetodosPago (Metodo)
    VALUES 
        ('Efectivo'),
        ('Transferencia');";
                SQLiteCommand insertMetodosPagoCommand = new SQLiteCommand(insertMetodosPagoQuery, connection);
                insertMetodosPagoCommand.ExecuteNonQuery();

                // Insertar configuración inicial si no existe
                string insertDefaultConfigQuery = @"
    INSERT OR IGNORE INTO Configuracion (Establecimiento, PuntoEmision) 
    VALUES ('001', '001');";
                SQLiteCommand insertConfigCommand = new SQLiteCommand(insertDefaultConfigQuery, connection);
                insertConfigCommand.ExecuteNonQuery();
            }
        }

        private string GenerarNumeroFactura()
        {
            string connectionString = "Data Source=sistema.db;Version=3;";
            string establecimiento = "001";
            string puntoEmision = "001";
            int siguienteNumero = 1;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT MAX(NumeroFactura)
            FROM Facturas
            WHERE NumeroFactura LIKE @serie || '-%'";

                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@serie", $"{establecimiento}-{puntoEmision}");
                object result = command.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    string ultimoNumero = result.ToString().Split('-')[2];
                    siguienteNumero = int.Parse(ultimoNumero) + 1;
                }
            }

            return $"{establecimiento}-{puntoEmision}-{siguienteNumero:D6}";
        }




        private void InsertarUsuarioAdmin()
        {
            string connectionString = "Data Source=sistema.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                INSERT OR IGNORE INTO Usuarios (Usuario, Contrasena) 
                VALUES ('admin', '1234')";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        private void BtnIniciarSesion_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            if (ValidarUsuario(usuario, contrasena))
            {

                // Abrir la pantalla principal
                this.Hide();
                PantallaPrincipal pantallaPrincipal = new PantallaPrincipal();
                pantallaPrincipal.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarUsuario(string usuario, string contrasena)
        {
            string connectionString = "Data Source=sistema.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM Usuarios WHERE Usuario = @usuario AND Contrasena = @contrasena";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@usuario", usuario);
                command.Parameters.AddWithValue("@contrasena", contrasena);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Cierra la aplicación
        }
    }
}
