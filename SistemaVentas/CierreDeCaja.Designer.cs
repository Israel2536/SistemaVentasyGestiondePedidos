namespace SistemaVentas
{
    partial class CierreDeCaja
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvFacturasContado;
        private System.Windows.Forms.DataGridView dgvFacturasTransferencia;
        private System.Windows.Forms.Button btnGenerarCierre;
        private System.Windows.Forms.Label lblTotalContado;
        private System.Windows.Forms.Label lblTotalTransferencia;

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
            this.dgvFacturasContado = new System.Windows.Forms.DataGridView();
            this.dgvFacturasTransferencia = new System.Windows.Forms.DataGridView();
            this.btnGenerarCierre = new System.Windows.Forms.Button();
            this.lblTotalContado = new System.Windows.Forms.Label();
            this.lblTotalTransferencia = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturasContado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturasTransferencia)).BeginInit();
            this.SuspendLayout();

            // DataGridView Contado
            this.dgvFacturasContado.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFacturasContado.Location = new System.Drawing.Point(12, 40);
            this.dgvFacturasContado.Name = "dgvFacturasContado";
            this.dgvFacturasContado.Size = new System.Drawing.Size(400, 300);
            this.dgvFacturasContado.TabIndex = 0;

            // DataGridView Transferencia
            this.dgvFacturasTransferencia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFacturasTransferencia.Location = new System.Drawing.Point(420, 40);
            this.dgvFacturasTransferencia.Name = "dgvFacturasTransferencia";
            this.dgvFacturasTransferencia.Size = new System.Drawing.Size(400, 300);
            this.dgvFacturasTransferencia.TabIndex = 1;

            // Botón Generar Cierre
            this.btnGenerarCierre.Location = new System.Drawing.Point(360, 360);
            this.btnGenerarCierre.Name = "btnGenerarCierre";
            this.btnGenerarCierre.Size = new System.Drawing.Size(120, 30);
            this.btnGenerarCierre.TabIndex = 2;
            this.btnGenerarCierre.Text = "Generar Cierre de Caja";
            this.btnGenerarCierre.UseVisualStyleBackColor = true;

            // Label Total Contado
            this.lblTotalContado.AutoSize = true;
            this.lblTotalContado.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalContado.Location = new System.Drawing.Point(12, 360);
            this.lblTotalContado.Name = "lblTotalContado";
            this.lblTotalContado.Size = new System.Drawing.Size(114, 17);
            this.lblTotalContado.TabIndex = 3;
            this.lblTotalContado.Text = "Total Contado:";

            // Label Total Transferencia
            this.lblTotalTransferencia.AutoSize = true;
            this.lblTotalTransferencia.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalTransferencia.Location = new System.Drawing.Point(520, 360);
            this.lblTotalTransferencia.Name = "lblTotalTransferencia";
            this.lblTotalTransferencia.Size = new System.Drawing.Size(150, 17);
            this.lblTotalTransferencia.TabIndex = 4;
            this.lblTotalTransferencia.Text = "Total Transferencia:";

            // Form CierreDeCaja
            this.ClientSize = new System.Drawing.Size(850, 400);
            this.Controls.Add(this.lblTotalContado);
            this.Controls.Add(this.lblTotalTransferencia);
            this.Controls.Add(this.btnGenerarCierre);
            this.Controls.Add(this.dgvFacturasTransferencia);
            this.Controls.Add(this.dgvFacturasContado);
            this.Name = "CierreDeCaja";
            this.Text = "Cierre de Caja";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturasContado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturasTransferencia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
