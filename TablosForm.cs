using System;
using System.Windows.Forms;

namespace LookInMe
{
    public partial class TablosForm : Form
    {
        public DataGridView ctmdgvTablos;

        public TablosForm(DataGridView dgv)
        {
            // Initialisieren der DataGridView und Hinzufügen zur TablosForm
            this.ctmdgvTablos = dgv;
            this.Controls.Add(ctmdgvTablos);
            this.ctmdgvTablos.Dock = DockStyle.Fill;

            // Ereignisbehandlung für das Schließen der TablosForm
            this.FormClosing += new FormClosingEventHandler(this.TablosForm_FormClosing);
            this.Load += new EventHandler(this.TablosForm_Load);
        }

        private void TablosForm_Load(object sender, EventArgs e)
        {
            // Hier können Sie zusätzlichen Code ausführen, wenn die Form geladen wird
        }

        private void TablosForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Verschieben Sie die DataGridView zurück ins Hauptformular
            MainWindow mainForm = Application.OpenForms["Form1"] as MainWindow;
            if (mainForm != null)
            {
             //   mainForm.EmbedCtmdgvTablos();
            }
        }
    }
}
