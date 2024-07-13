using System.Windows.Forms;

namespace LookInMe
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnDirectory = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grpTable = new System.Windows.Forms.GroupBox();
            this.dgvtablo = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.chbCache = new System.Windows.Forms.CheckBox();
            this.btnCachErase = new System.Windows.Forms.Button();
            this.lblCache = new System.Windows.Forms.Label();
            this.lblChbox = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pgBar = new CustomProgressBar();
            this.btnStopSearch = new System.Windows.Forms.Button();
            this.panelForCustomGrid = new System.Windows.Forms.GroupBox();
            this.cbColor = new System.Windows.Forms.ComboBox();
            this.btnReadInCache = new System.Windows.Forms.Button();
            this.grpCache = new System.Windows.Forms.GroupBox();
            this.lblReadInCache = new System.Windows.Forms.Label();
            this.grpTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvtablo)).BeginInit();
            this.grpCache.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.LightSteelBlue;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.txtSearch.Location = new System.Drawing.Point(685, 37);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(439, 24);
            this.txtSearch.TabIndex = 0;
            // 
            // btnDirectory
            // 
            this.btnDirectory.BackColor = System.Drawing.Color.SteelBlue;
            this.btnDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.btnDirectory.ForeColor = System.Drawing.Color.White;
            this.btnDirectory.Location = new System.Drawing.Point(217, 71);
            this.btnDirectory.Name = "btnDirectory";
            this.btnDirectory.Size = new System.Drawing.Size(100, 28);
            this.btnDirectory.TabIndex = 1;
            this.btnDirectory.Text = "Auswahl";
            this.btnDirectory.UseVisualStyleBackColor = false;
            this.btnDirectory.Click += new System.EventHandler(this.btnDirectory_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.SteelBlue;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(1024, 71);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 28);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Suchen";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click_1);
            // 
            // grpTable
            // 
            this.grpTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTable.Controls.Add(this.dgvtablo);
            this.grpTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.grpTable.Location = new System.Drawing.Point(23, 156);
            this.grpTable.Name = "grpTable";
            this.grpTable.Size = new System.Drawing.Size(1115, 374);
            this.grpTable.TabIndex = 3;
            this.grpTable.TabStop = false;
            this.grpTable.Text = "Such Ergebnis";
            // 
            // dgvtablo
            // 
            this.dgvtablo.AllowUserToAddRows = false;
            this.dgvtablo.AllowUserToDeleteRows = false;
            this.dgvtablo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvtablo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvtablo.Location = new System.Drawing.Point(14, 34);
            this.dgvtablo.Name = "dgvtablo";
            this.dgvtablo.ReadOnly = true;
            this.dgvtablo.Size = new System.Drawing.Size(1087, 334);
            this.dgvtablo.TabIndex = 0;
            this.dgvtablo.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvtablo_CellContentClick);
            this.dgvtablo.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvtablo_CellMouseEnter);
            this.dgvtablo.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvtablo_CellMouseLeave);
            this.dgvtablo.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvtablo_CellPainting);
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 964);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1689, 22);
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // txtDirectory
            // 
            this.txtDirectory.BackColor = System.Drawing.Color.LightSteelBlue;
            this.txtDirectory.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.txtDirectory.Location = new System.Drawing.Point(23, 37);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(294, 24);
            this.txtDirectory.TabIndex = 13;
            // 
            // chbCache
            // 
            this.chbCache.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.chbCache.Location = new System.Drawing.Point(85, 44);
            this.chbCache.Name = "chbCache";
            this.chbCache.Size = new System.Drawing.Size(192, 22);
            this.chbCache.TabIndex = 15;
            this.chbCache.Text = "Nur Cache aus lesen";
            this.chbCache.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chbCache.UseVisualStyleBackColor = true;
            this.chbCache.CheckedChanged += new System.EventHandler(this.chbCache_CheckedChanged);
            // 
            // btnCachErase
            // 
            this.btnCachErase.BackColor = System.Drawing.Color.SteelBlue;
            this.btnCachErase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCachErase.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCachErase.ForeColor = System.Drawing.Color.White;
            this.btnCachErase.Location = new System.Drawing.Point(214, 101);
            this.btnCachErase.Name = "btnCachErase";
            this.btnCachErase.Size = new System.Drawing.Size(100, 28);
            this.btnCachErase.TabIndex = 16;
            this.btnCachErase.Text = "Cache";
            this.btnCachErase.UseVisualStyleBackColor = false;
            this.btnCachErase.Click += new System.EventHandler(this.btnCachErase_Click);
            // 
            // lblCache
            // 
            this.lblCache.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.lblCache.Location = new System.Drawing.Point(28, 107);
            this.lblCache.Name = "lblCache";
            this.lblCache.Size = new System.Drawing.Size(162, 28);
            this.lblCache.TabIndex = 17;
            this.lblCache.Text = "Cache komplett Leeren";
            this.lblCache.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblChbox
            // 
            this.lblChbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChbox.Location = new System.Drawing.Point(1286, 43);
            this.lblChbox.Name = "lblChbox";
            this.lblChbox.Size = new System.Drawing.Size(322, 18);
            this.lblChbox.TabIndex = 18;
            this.lblChbox.Text = "Während der Suche ist die Checkbox deaktiviert";
            this.lblChbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(35, 545);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(650, 28);
            this.lblStatus.TabIndex = 20;
            this.lblStatus.Text = "Such Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // pgBar
            // 
            this.pgBar.Location = new System.Drawing.Point(0, 367);
            this.pgBar.Name = "pgBar";
            this.pgBar.Size = new System.Drawing.Size(17, 20);
            this.pgBar.TabIndex = 0;
            // 
            // btnStopSearch
            // 
            this.btnStopSearch.BackColor = System.Drawing.Color.SteelBlue;
            this.btnStopSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopSearch.ForeColor = System.Drawing.Color.White;
            this.btnStopSearch.Location = new System.Drawing.Point(870, 71);
            this.btnStopSearch.Name = "btnStopSearch";
            this.btnStopSearch.Size = new System.Drawing.Size(100, 28);
            this.btnStopSearch.TabIndex = 21;
            this.btnStopSearch.Text = "Stop";
            this.btnStopSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnStopSearch.UseVisualStyleBackColor = false;
            // 
            // panelForCustomGrid
            // 
            this.panelForCustomGrid.Location = new System.Drawing.Point(23, 613);
            this.panelForCustomGrid.Name = "panelForCustomGrid";
            this.panelForCustomGrid.Size = new System.Drawing.Size(1115, 292);
            this.panelForCustomGrid.TabIndex = 22;
            this.panelForCustomGrid.TabStop = false;
            this.panelForCustomGrid.Text = "groupBox1";
            this.panelForCustomGrid.Enter += new System.EventHandler(this.panelForCustomGrid_Enter);
            // 
            // cbColor
            // 
            this.cbColor.BackColor = System.Drawing.Color.SteelBlue;
            this.cbColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbColor.ForeColor = System.Drawing.SystemColors.Window;
            this.cbColor.FormattingEnabled = true;
            this.cbColor.Items.AddRange(new object[] {
            "Light",
            "Dark",
            "Blue",
            "Custom"});
            this.cbColor.Location = new System.Drawing.Point(1183, 613);
            this.cbColor.Name = "cbColor";
            this.cbColor.Size = new System.Drawing.Size(121, 26);
            this.cbColor.TabIndex = 23;
            this.cbColor.Text = "Theme";
            this.cbColor.SelectedIndexChanged += new System.EventHandler(this.cbColor_SelectedIndexChanged);
            // 
            // btnReadInCache
            // 
            this.btnReadInCache.BackColor = System.Drawing.Color.SteelBlue;
            this.btnReadInCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReadInCache.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReadInCache.ForeColor = System.Drawing.Color.White;
            this.btnReadInCache.Location = new System.Drawing.Point(28, 145);
            this.btnReadInCache.Margin = new System.Windows.Forms.Padding(0);
            this.btnReadInCache.Name = "btnReadInCache";
            this.btnReadInCache.Size = new System.Drawing.Size(100, 28);
            this.btnReadInCache.TabIndex = 24;
            this.btnReadInCache.Text = "Einlesen";
            this.btnReadInCache.UseVisualStyleBackColor = false;
            // 
            // grpCache
            // 
            this.grpCache.Controls.Add(this.lblReadInCache);
            this.grpCache.Controls.Add(this.btnCachErase);
            this.grpCache.Controls.Add(this.chbCache);
            this.grpCache.Controls.Add(this.lblCache);
            this.grpCache.Controls.Add(this.btnReadInCache);
            this.grpCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpCache.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCache.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.grpCache.Location = new System.Drawing.Point(1277, 83);
            this.grpCache.Name = "grpCache";
            this.grpCache.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.grpCache.Size = new System.Drawing.Size(345, 197);
            this.grpCache.TabIndex = 25;
            this.grpCache.TabStop = false;
            this.grpCache.Text = "Cache Box";
            // 
            // lblReadInCache
            // 
            this.lblReadInCache.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReadInCache.Location = new System.Drawing.Point(142, 152);
            this.lblReadInCache.Name = "lblReadInCache";
            this.lblReadInCache.Size = new System.Drawing.Size(177, 23);
            this.lblReadInCache.TabIndex = 26;
            this.lblReadInCache.Text = "Cache einlesen komplett";
            this.lblReadInCache.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1689, 986);
            this.Controls.Add(this.grpCache);
            this.Controls.Add(this.cbColor);
            this.Controls.Add(this.panelForCustomGrid);
            this.Controls.Add(this.btnStopSearch);
            this.Controls.Add(this.pgBar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblChbox);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnDirectory);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.grpTable);
            this.Name = "MainWindow";
            this.Text = "Look in Me!";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.grpTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvtablo)).EndInit();
            this.grpCache.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnDirectory;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grpTable;
        private System.Windows.Forms.DataGridView dgvtablo;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TextBox txtDirectory;
        private CheckBox chbCache;
        private Button btnCachErase;
        private Label lblCache;
        private Label lblChbox;
        private Label lblStatus;
        private CustomProgressBar pgBar;
        private Button btnStopSearch;
        private GroupBox panelForCustomGrid;
        private ComboBox cbColor;
        private Button btnReadInCache;
        private GroupBox grpCache;
        private Label lblReadInCache;
    }
}

/*
 * 
*/