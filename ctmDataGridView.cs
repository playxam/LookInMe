using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public enum ColorTheme
{
    Light,
    Dark,
    Blue,
    Green
}

public class ctmDataGridView : DataGridView
{
    private Color originalColor;
    private ToolTip toolTip;
    // Liste der Debug-Nachrichten
    private List<string> debugMessages = new List<string>();
    public ctmDataGridView()
    {
        this.DoubleBuffered = true; // Verbessert die Performance und reduziert Flackern
        this.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.originalColor = this.BackgroundColor;
        this.toolTip = new ToolTip();
        this.toolTip.SetToolTip(this, "Ziehe die Tabelle, um sie zu verschieben");
        this.AllowUserToResizeColumns = true; // Erlaubt das Anpassen der Spaltenbreiten
        this.AllowUserToResizeRows = true; // Erlaubt das Anpassen der Zeilenhöhen

        // Richtiges Hinzufügen der Event-Handler
        this.MouseEnter += new EventHandler(OnMouseEnter);
        this.MouseLeave += new EventHandler(OnMouseLeave);
    }

    // Methode zum Setzen von Farben für die Tabelle
    public void SetColors(Color headerColor, Color rowColor)
    {
        this.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
        this.RowsDefaultCellStyle.BackColor = rowColor;
    }

    // Methode zur automatischen Anpassung der Spaltenbreiten
    public void AdjustColumnWidth()
    {
        var previousMode = this.AutoSizeColumnsMode;
        this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Temporär deaktivieren
        foreach (DataGridViewColumn col in this.Columns)
        {
            col.Width = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
        }
        this.AutoSizeColumnsMode = previousMode; // Ursprünglichen Modus wiederherstellen
    }

    public void AdjustColumnWidths()
    {
        foreach (DataGridViewColumn column in this.Columns)
        {
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
        foreach (DataGridViewColumn column in this.Columns)
        {
            int colWidth = column.Width;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = colWidth;
        }
    }

    public void AdjustRowHeights()
    {
        foreach (DataGridViewRow row in this.Rows)
        {
            row.Height = row.GetPreferredHeight(row.Index, DataGridViewAutoSizeRowMode.AllCells, true);
        }
    }

    // Methode zur Anwendung eines Themes (Farben und Spaltenbreiten)
    public void ApplyTheme(ColorTheme theme)
    {
        switch (theme)
        {
            case ColorTheme.Light:
                SetColors(Color.PaleTurquoise, Color.Aquamarine);
                break;
            case ColorTheme.Dark:
                SetColors(Color.DarkGray, Color.Goldenrod);
                break;
            case ColorTheme.Blue:
                SetColors(Color.Indigo, Color.AliceBlue);
                break;
            case ColorTheme.Green:
                SetColors(Color.LightGreen, Color.Honeydew);
                break;
        }
        AdjustColumnWidth();
    }

    // Methode zur Initialisierung der Spalten mit Namen
    public void InitializeColumns()
    {
        this.Columns.Clear();
        this.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Auflistung", Name = "Auflistung" });
        this.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Dokumenten Ort", Name = "Dokumenten Ort" });
        var iconColumn = new DataGridViewImageColumn
        {
            HeaderText = "Dokumenten Typ",
            Name = "Dokumenten Typ",
            Width = 50,
            ImageLayout = DataGridViewImageCellLayout.Zoom
        };
        this.Columns.Add(iconColumn);
        this.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Dokumenten Name", Name = "Dokumenten Name" });
        this.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ergebnisse", Name = "Ergebnisse" });
    }

    // Methode zur dynamischen Anpassung der Zeilenhöhe
    protected override void OnRowHeightChanged(DataGridViewRowEventArgs e)
    {
        base.OnRowHeightChanged(e);
        foreach (DataGridViewRow row in this.Rows)
        {
            row.Height = row.GetPreferredHeight(row.Index, DataGridViewAutoSizeRowMode.AllCells, true);
        }
    }

    // Methode zur dynamischen Anpassung der Spaltenbreiten
    protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
    {
        base.OnColumnWidthChanged(e);
        foreach (DataGridViewColumn col in this.Columns)
        {
            col.Width = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
        }
    }



    // Methode zum Loggen von Debug-Nachrichten
    private void LogDebugMessage(string message, string logFileName = "ctmdgvTablos.log")
    {
        string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LookInMe", "Logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        string logFilePath = Path.Combine(logDirectory, logFileName);
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }

    // Methode zum Speichern der Debug-Nachrichten in eine Log-Datei
    private void SaveDebugMessagesToFile(string logFileName = "ctmdgvTablos.log")
    {
        string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LookInMe", "Logs");
        LogDebugMessage($"Logs Directory: {logDirectory}", logFileName);

        if (!Directory.Exists(logDirectory))
        {
            LogDebugMessage("Logs directory does not exist, creating directory.", logFileName);
            Directory.CreateDirectory(logDirectory);
        }

        string logFilePath = Path.Combine(logDirectory, logFileName);
        LogDebugMessage($"Log file path: {logFilePath}", logFileName);

        File.WriteAllLines(logFilePath, debugMessages);

        debugMessages.Clear();
        LogDebugMessage($"Debug-Nachrichten wurden in die Datei geschrieben: {logFilePath}", logFileName);
    }

    // Methode zum Handhaben des MouseEnter-Events
    private void OnMouseEnter(object sender, EventArgs e)
    {
        this.originalColor = this.BackgroundColor;
        this.BackgroundColor = Color.LightYellow;
    }

    // Methode zum Handhaben des MouseLeave-Events
    private void OnMouseLeave(object sender, EventArgs e)
    {
        this.BackgroundColor = this.originalColor;
    }

    // Methode zur Anzeige eines Tooltips beim Überfahren des Rahmens
    protected override void OnMouseHover(EventArgs e)
    {
        base.OnMouseHover(e);
        var hitTest = this.HitTest(PointToClient(Cursor.Position).X, PointToClient(Cursor.Position).Y);
        if (hitTest.Type == DataGridViewHitTestType.Cell)
        {
            var cell = this[hitTest.ColumnIndex, hitTest.RowIndex];
            if (!string.IsNullOrEmpty(cell.ToolTipText))
            {
                toolTip.SetToolTip(this, "Ziehe die Tabelle, um sie zu verschieben");
            }
        }
    }
}
