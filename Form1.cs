using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Content;
using iText;
//using PdfiumViewer;
using Microsoft.Extensions.Logging;
using System.Threading;
//using PdfSharp.Pdf.IO;
using System.Text;
using PdfSharp.Pdf.Content.Objects;
using System.Text.RegularExpressions;

namespace LookInMe
{
    public partial class MainWindow : Form
    {
        // Füge diese Zeile hinzu
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        //Deklaration Caching
        private SearchResultCache searchResultCache;
        private DiskCache diskCache;
        // Andere Felder
        private System.Windows.Forms.Timer blinkTimer;


        private ctmDataGridView ctmdgvTablos;

        // Verzeichnispfade definieren
        private string baseDirectory;
        private string logsDirectory;
        private string dataDirectory;
        private string iconsDirectory;
        private string cacheDirectory;

        // Liste zum Speichern der Debug-Nachrichten
        private List<string> debugMessages = new List<string>();

        // Diese Variablen speichern die Position der Maus über der DataGridView-Zelle
        private int hoverRowIndex = -1;
        private int hoverColumnIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            EnsureDataDirectoryExists(); // Überprüfe und erstelle das Data-Verzeichnis
            SetupAutoComplete(); // Richte die automatische Vervollständigung ein

            InitializectmdgvTablos();
           // InitializeDataGridView();


            // Erstellen und konfigurieren Sie die benutzerdefinierte ProgressBar
            //pgBar = new CustomProgressBar();
            pgBar.Location = new System.Drawing.Point(700, 545);
            pgBar.Size = new System.Drawing.Size(420, 24);
            this.Controls.Add(pgBar);

            // Initialisieren des RAM-Caches und des Festplatten-Caches
            searchResultCache = new SearchResultCache();
            baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            iconsDirectory = Path.Combine(baseDirectory, "icons");
            cacheDirectory = Path.Combine(baseDirectory, "Data", "cache");
            diskCache = new DiskCache(cacheDirectory);

            // Laden des Caches von der Festplatte beim Start
            var loadedCache = diskCache.LoadCache();
            foreach (var entry in loadedCache)
            {
                searchResultCache.Add(entry.Key, entry.Value);
            }

            LogDebugMessage("MainWindow initialized");

            // Laden des letzten Verzeichnisses
            txtDirectory.Text = Properties.Settings.Default.LastDirectory;

  /*          // Initialisieren der DataGridView nur einmal
            if (dgvtablo.Columns.Count == 0)
            {
                LogDebugMessage("Calling InitializeDataGridView from Constructor");
                InitializeDataGridView();
            }*/
   

            // Label initialisieren und verstecken
            lblChbox.Visible = false;
        }

        //ctmdgvTablos Tabelle Methode
        private void InitializectmdgvTablos()
        {

            LogDebugMessage("InitializectmdgvTablos started", "ctmdgvTablos.log");

            ctmdgvTablos = new ctmDataGridView();
            LogDebugMessage("ctmDataGridView instance created", "ctmdgvTablos.log");

            ctmdgvTablos.Dock = DockStyle.Fill;  // Wichtig für die korrekte Positionierung innerhalb des Containers
            LogDebugMessage("Dock style set to Fill", "ctmdgvTablos.log");

            // Initialisieren Sie die Spalten mit den gewünschten Namen
            ctmdgvTablos.InitializeColumns();
            LogDebugMessage("Columns initialized", "ctmdgvTablos.log");

            // Wenden Sie das gewünschte Theme an
            ctmdgvTablos.ApplyTheme(ColorTheme.Light);
            LogDebugMessage("Theme applied", "ctmdgvTablos.log");

            // Hier sollte der richtige Container verwendet werden
            var container = this.Controls.Find("panelForCustomGrid", true).FirstOrDefault();
            if (container != null)
            {
                container.Controls.Add(ctmdgvTablos);
            }
            else
            {
                this.Controls.Add(ctmdgvTablos);
                LogDebugMessage("ctmDataGridView added to Form controls", "ctmdgvTablos.log");

            }

            ctmdgvTablos.BringToFront(); // Bringen Sie die Tabelle in den Vordergrund
            LogDebugMessage("ctmDataGridView brought to front", "ctmdgvTablos.log");

            LogDebugMessage("InitializectmdgvTablos completed", "ctmdgvTablos.log");
        }






        protected override void OnFormClosing(FormClosingEventArgs closingEvent)
        {
            base.OnFormClosing(closingEvent);

            // Stoppe alle laufenden Aufgaben
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            // Speichern des RAM-Caches auf die Festplatte
            if (searchResultCache != null && diskCache != null)
            {
                var cacheToSave = searchResultCache.GetAll();
                diskCache.SaveCache(cacheToSave);
            }

            LogDebugMessage("Form is closing");
            SaveDebugMessagesToFile();
        }

        // Methode zum Überprüfen und Erstellen des Data-Verzeichnisses
        private void EnsureDataDirectoryExists()
        {
            string baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string dataDirectory = Path.Combine(baseDirectory, "Data");
            cacheDirectory = Path.Combine(dataDirectory, "cache");

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
                LogDebugMessage($"Data directory created: {dataDirectory}");
            }
            else
            {
                LogDebugMessage($"Data directory already exists: {dataDirectory}");
            }

            if (!Directory.Exists(cacheDirectory))
            {
                Directory.CreateDirectory(cacheDirectory);
                LogDebugMessage($"Cache directory created: {cacheDirectory}");
            }
            else
            {
                LogDebugMessage($"Cache directory already exists: {cacheDirectory}");
            }
        }

        // Methode zum Speichern eines Suchbegriffs in eine Datei
        private void SaveSearchTerm(string searchTerm)
        {
            string baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string dataDirectory = Path.Combine(baseDirectory, "Data");

            string logFileName = $"SearchTerms_{DateTime.Now:yyyyMMdd}.txt";
            string logFilePath = Path.Combine(dataDirectory, logFileName);

            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine($"{DateTime.Now:HH:mm:ss} - {searchTerm}");
                LogDebugMessage($"Saved search term: {searchTerm} to file: {logFilePath}");
            }
        }

        // Methode zum Laden von Suchbegriffen aus den Dateien im Data-Verzeichnis
        private List<string> LoadSearchTerms()
        {
            string baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string dataDirectory = Path.Combine(baseDirectory, "Data");

            var searchTerms = new List<string>();

            foreach (var file in Directory.GetFiles(dataDirectory, "SearchTerms_*.txt"))
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    var searchTerm = line.Split(new string[] { " - " }, StringSplitOptions.None)[1];
                    searchTerms.Add(searchTerm);
                }
                LogDebugMessage($"Loaded search terms from file: {file}");
            }

            return searchTerms;
        }

        // Methode zur Einrichtung der automatischen Vervollständigung im Suchfeld
        private void SetupAutoComplete()
        {
            var searchTerms = LoadSearchTerms();
            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(searchTerms.ToArray());

            txtSearch.AutoCompleteCustomSource = autoComplete;
            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;

            LogDebugMessage("AutoComplete setup complete with loaded search terms");
        }

        // Methode zur Anzeige und Ausblendung des Cache-Labels
        private void ToggleCacheLabel(bool isVisible, string text = "")
        {
            lblChbox.Visible = isVisible;
            lblChbox.Text = text;
        }




        // Methode zum Initialisieren der DataGridView
        private void InitializeDataGridView()
        {
            LogDebugMessage("InitializeDataGridView called");

            // Überprüfen, ob die Spalten bereits hinzugefügt wurden
            if (dgvtablo.Columns.Count == 0)
            {
                dgvtablo.Columns.Add(new DataGridViewTextBoxColumn { Name = "Index", HeaderText = "Index", Width = 50 });
                dgvtablo.Columns.Add(new DataGridViewTextBoxColumn { Name = "Path", HeaderText = "Path", Width = 150 });
                dgvtablo.Columns.Add(new DataGridViewImageColumn { Name = "Typ", HeaderText = "Typ", Width = 50, ImageLayout = DataGridViewImageCellLayout.Zoom });
                dgvtablo.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Name", Width = 100 });
                dgvtablo.Columns.Add(new DataGridViewTextBoxColumn { Name = "Findings", HeaderText = "Findings", Width = 500, DefaultCellStyle = { WrapMode = DataGridViewTriState.True } });
            }
            LogDebugMessage("Spalten hinzugefügt: Index, Path, Typ, Name, Findings");
        }
  
        // Methode zum Laden des Icons für die DataGridView
        private Bitmap LoadIcon(string iconName)
        {
            string iconPath = Path.Combine(iconsDirectory, iconName);
            LogDebugMessage($"Loading icon from path: {iconPath}");
            if (File.Exists(iconPath))
            {
                LogDebugMessage($"Icon found: {iconPath}");
                using (var originalImage = Image.FromFile(iconPath))
                {
                    return new Bitmap(originalImage, new Size(32, 32)); // Skalieren Sie das Icon auf 32x32 Pixel
                }
            }
            else
            {
                LogDebugMessage($"Icon not found: {iconPath}");
                return null; // oder ein Standard-Icon verwenden
            }
        }


        // Methode zum Aktualisieren der DataGridView-Zeilen
        private void UpdateDataGridView(System.Data.DataTable searchResults)
        {
            // Löschen Sie nur die Zeilen, nicht die Spalten
            dgvtablo.Rows.Clear();

            // Fügen Sie die neuen Zeilen hinzu
            foreach (DataRow dr in searchResults.Rows)
            {
                // Laden Sie das Icon basierend auf dem Typ
                var icon = LoadIcon(dr["Typ"].ToString());
                dgvtablo.Rows.Add(dr["Index"], dr["Path"], icon, dr["Name"], dr["Findings"]);
            }

            // Stellen Sie sicher, dass die Textumbruch- und Größenanpassungseinstellungen beibehalten werden
            dgvtablo.Columns["Findings"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvtablo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvtablo.Refresh();
        }


        // Event-Handler für das Klicken auf den btnDirectory Button
        private void btnDirectory_Click(object sender, EventArgs e)
        {
            LogDebugMessage("btnDirectory clicked");
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.ValidateNames = false;
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.FileName = "Folder Selection.";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = Path.GetDirectoryName(openFileDialog.FileName);
                    txtDirectory.Text = folderPath;
                    LogDebugMessage($"Directory selected: {folderPath}");

                    // Speichern des ausgewählten Verzeichnisses in den Anwendungseinstellungen
                    Properties.Settings.Default.LastDirectory = folderPath;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    LogDebugMessage("Directory selection canceled");
                }
            }
        }

        // Event-Handler für das Klicken auf den btnSearch Button
        private async void btnSearch_Click_1(object sender, EventArgs e)
        {
            LogDebugMessage("Search button clicked");

            // Verzeichnis und Suchbegriff aus den Textfeldern auslesen
            string searchDirectory = txtDirectory.Text;
            string searchTerm = txtSearch.Text.Trim();

            // Überprüfen, ob das Verzeichnis existiert und der Suchbegriff nicht leer ist
            if (Directory.Exists(searchDirectory) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                LogDebugMessage($"Starting search in directory: {searchDirectory} for term: {searchTerm}");

                // Ändere den Button-Text und Farbe, um anzuzeigen, dass die Suche läuft
                btnSearch.Text = "Searching...";
                btnSearch.BackColor = Color.Magenta;
                btnSearch.Enabled = false; // Deaktiviere den Button, um Mehrfachklicks zu verhindern

                // Zeige das Label und deaktiviere die Checkbox
                ToggleCacheLabel(true, "Deaktiviert während der Suche");
                chbCache.Enabled = false;

                // Speichere den Suchbegriff in der Datei
                SaveSearchTerm(searchTerm);

                var totalStopwatch = Stopwatch.StartNew(); // Starten der Stoppuhr für die Gesamtzeit

                var stopwatch = Stopwatch.StartNew();

                // Überprüfen, ob "Nur Cache lesen" aktiviert ist
                if (chbCache.Checked && searchResultCache.Contains(searchTerm))
                {
                    // Holen Sie die Ergebnisse aus dem Cache
                    var cachedResults = searchResultCache.Get(searchTerm);
                    LogDebugMessage($"Cache hit for search term: {searchTerm}");

                    // Aktualisieren Sie die DataGridView auf dem UI-Thread
                    dgvtablo.Invoke(new Action(() =>
                    {
                        dgvtablo.DataSource = cachedResults;
                        dgvtablo.Columns["Findings"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        dgvtablo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                        dgvtablo.DataBindingComplete += (s, klicker) =>
                        {
                            foreach (DataGridViewRow row in dgvtablo.Rows)
                            {
                                int maxLines = 3; // Begrenze die Anzahl der Zeilen
                                int charHeight = TextRenderer.MeasureText("A", dgvtablo.DefaultCellStyle.Font).Height;
                                int maxHeight = maxLines * charHeight;
                                row.Height = Math.Min(maxHeight, row.Height);
                            }
                        };

                        dgvtablo.Refresh();
                    }));
                }
                else
                {
                    // Führe die Suche aus, wenn "Nur Cache lesen" deaktiviert ist oder der Suchbegriff nicht im Cache vorhanden ist
                    await SearchFilesAsync(searchDirectory, searchTerm);
                }

                stopwatch.Stop();
                LogDebugMessage($"Search completed in {stopwatch.ElapsedMilliseconds} ms");

                totalStopwatch.Stop(); // Stoppen der Stoppuhr für die Gesamtzeit
                LogDebugMessage($"Total time from clicking search to filling table: {totalStopwatch.ElapsedMilliseconds} ms");

                // Setze den Button-Text und Farbe zurück, wenn die Suche abgeschlossen ist
                btnSearch.Text = "Search";
                btnSearch.BackColor = Color.SteelBlue;
                btnSearch.Enabled = true; // Reaktiviere den Button

                // Verstecke das Label und aktiviere die Checkbox
                ToggleCacheLabel(false);
                chbCache.Enabled = true;
            }
            else
            {
                // Fehlermeldung, wenn das Verzeichnis ungültig ist oder der Suchbegriff fehlt
                MessageBox.Show("Bitte geben Sie ein gültiges Verzeichnis und einen Suchbegriff ein.");
                LogDebugMessage("Invalid directory or search term");
            }
        }


 


        // Methode zur Analyse der Suchbegriffe und Identifikation der häufig verwendeten Begriffe
        private Dictionary<string, int> GetSearchTermStatistics()
        {
            // Lade die Suchbegriffe aus den Dateien
            var searchTerms = LoadSearchTerms();
            var statistics = new Dictionary<string, int>();

            // Durchlaufe alle Suchbegriffe und zähle deren Häufigkeit
            foreach (var term in searchTerms)
            {
                if (statistics.ContainsKey(term))
                {
                    statistics[term]++;
                }
                else
                {
                    statistics[term] = 1;
                }
            }

            // Logge die gefundenen Suchbegriff-Statistiken
            LogDebugMessage("Search term statistics calculated");
            foreach (var stat in statistics)
            {
                LogDebugMessage($"Term: {stat.Key}, Count: {stat.Value}");
            }

            return statistics;
        }

        // Methode zum Kürzen des Textsnippets auf die ersten 20 Wörter
        private string GetFirst20Words(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length <= 20)
                return text;

            return string.Join(" ", words.Take(20)) + "...";
        }

        // Methode zum Bestimmen des Icons für die Datei
        private string GetIconForFile(string filePath)
        {
            LogDebugMessage($"Determining icon for file: {filePath}");
            // Extrahiert die Dateierweiterung
            string extension = Path.GetExtension(filePath).ToLower();

            // Basierend auf der Dateierweiterung das entsprechende Icon auswählen
            switch (extension)
            {
                case ".docx":
                    return "word.png";
                case ".xlsx":
                    return "excel.png";
                case ".pptx":
                    return "powerpoint.png";
                case ".pdf":
                    return "pdf.png";
                case ".csv":
                case ".txt":
                    return "text.png";
                case ".xml":
                case ".html":
                case ".css":
                    return "code.png";
                default:
                    return "file.png";
            }
        }

        // Asynchrone Methode zur Durchsuchung von Dateien in einem Verzeichnis nach einem Suchbegriff																							  
        private async System.Threading.Tasks.Task SearchFilesAsync(string directory, string searchTerm)
        {
            var cancellationToken = cancellationTokenSource.Token;

            // Überprüfen, ob das Ergebnis bereits im Cache vorhanden ist
            if (chbCache.Checked && searchResultCache.Contains(searchTerm))
            {
                // Holen Sie die Ergebnisse aus dem Cache
                var cachedResults = searchResultCache.Get(searchTerm);
                LogDebugMessage($"Cache hit for search term: {searchTerm}");

                // Aktualisieren Sie die DataGridView auf dem UI-Thread
                dgvtablo.Invoke(new Action(() =>
                {
                    dgvtablo.DataSource = cachedResults;
                    dgvtablo.Columns["Findings"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvtablo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                    dgvtablo.DataBindingComplete += (s, e) =>
                    {
                        foreach (DataGridViewRow row in dgvtablo.Rows)
                        {
                            int maxLines = 3; // Begrenze die Anzahl der Zeilen
                            int charHeight = TextRenderer.MeasureText("A", dgvtablo.DefaultCellStyle.Font).Height;
                            int maxHeight = maxLines * charHeight;
                            row.Height = Math.Min(maxHeight, row.Height);
                        }
                    };

                    dgvtablo.Refresh();
                }));
                this.Invoke(new Action(() =>
                {
                    chbCache.Enabled = true;
                    lblChbox.Visible = false;
                }));

                return;
            }
            // Suche im Verzeichnis fortsetzen
            LogDebugMessage("Cache miss, searching in directory...");

            this.Invoke(new Action(() =>
            {
                lblChbox.Text = "Es wird weiter im Verzeichnis gesucht!";
                lblChbox.Visible = true;
                lblChbox.Font = new System.Drawing.Font(lblChbox.Font, FontStyle.Bold);
                StartBlinkingLabel(lblChbox);
            }));
            await System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    // Logge den Aufruf der Methode
                    LogDebugMessage($"SearchFiles method called with directory: {directory} and searchTerm: {searchTerm}");

                    // Dateien im Verzeichnis und Unterverzeichnissen abrufen und filtern
                    var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                        .Where(file => file.EndsWith(".docx") || file.EndsWith(".xlsx") || file.EndsWith(".pptx") ||
                                       file.EndsWith(".pdf") || file.EndsWith(".csv") || file.EndsWith(".xml") ||
                                       file.EndsWith(".html") || file.EndsWith(".css") || file.EndsWith(".txt")).ToList();

                    // Logge die Anzahl der gefundenen Dateien
                    LogDebugMessage($"Number of files found: {files.Count}");

                    // Initialisiere die ProgressBar
                    this.Invoke(new Action(() => InitializeProgressBar(files.Count)));

                    // Deklaration und Initialisierung von fileCount
                    int fileCount = 0;

                    // Erstelle eine DataTable für die Suchergebnisse
                    System.Data.DataTable searchResults = new System.Data.DataTable();
                    searchResults.Columns.Add("Index");
                    searchResults.Columns.Add("Path");
                    searchResults.Columns.Add("Typ");
                    searchResults.Columns.Add("Name");
                    searchResults.Columns.Add("Findings");

                    // Logge die Spalten der DataTable
                    LogDebugMessage("DataTable columns: " + string.Join(", ", searchResults.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

                    int index = 1; // Initialisiere den Index für die Suchergebnisse
                    object lockObject = new object(); // Erstelle ein Lock-Objekt für Thread-Sicherheit


                    // Durchsuche die Dateien parallel
                    await System.Threading.Tasks.Task.WhenAll(files.Select(async file =>



                    {
                        try
                        {
                            LogDebugMessage($"Searching in file: {file}");
                            var fileStopwatch = Stopwatch.StartNew(); // Starte eine Stoppuhr für die Durchsuchungszeit der Datei
                            List<string> findings = new List<string>();

                            string fileContent = "";


                            // Datei basierend auf der Dateierweiterung lesen
                            if (file.EndsWith(".docx"))
                            {
                                fileContent = await ReadWordFileAsync(file);
                            }
                            else if (file.EndsWith(".xlsx"))
                            {
                                fileContent = await ReadExcelFileAsync(file);
                            }
                            else if (file.EndsWith(".pptx"))
                            {
                                fileContent = await ReadPowerPointFileAsync(file, false); // Verwenden von false, um das Öffnen zu verhindern
                            }
                            else if (file.EndsWith(".pdf"))
                            {
                                fileContent = await ReadPdfFileAsync(file);
                            }
                            else if (file.EndsWith(".csv") || file.EndsWith(".txt"))
                            {
                                fileContent = await ReadTextFileAsync(file);
                            }
                            else if (file.EndsWith(".xml") || file.EndsWith(".html") || file.EndsWith(".css"))
                            {
                                fileContent = await ReadTextFileAsync(file);
                            }

                            // Wenn der Suchbegriff im Dateiinhalt gefunden wird
                            if (fileContent.Contains(searchTerm))

                            {
                                LogDebugMessage($"Search term found in file: {file}");
                                var sentences = GetSentencesWithTerm(fileContent, searchTerm);
                                findings.AddRange(sentences.Take(2)); // Nur die ersten 2 Sätze mit dem Suchbegriff nehmen

                                // Erhalte den Dateityp-Icon-Pfad
                                string iconName = GetIconForFile(file);
                                string iconPath = Path.Combine(iconsDirectory, iconName);

                                lock (lockObject)
                                {
                                    foreach (var snippet in findings)
                                    {
                                        var row = searchResults.NewRow();
                                        row["Index"] = index++;
                                        row["Path"] = file;
                                        row["Typ"] = iconPath;
                                        row["Name"] = Path.GetFileName(file);
                                        row["Findings"] = GetFirst20Words(snippet);

                                        searchResults.Rows.Add(row);
                                        LogDebugMessage($"Added search result for file: {file} - Index: {index - 1}, Path: {file}, Icon: {iconPath}, Name: {Path.GetFileName(file)}, Findings: {snippet}");
                                    }
                                }
                            }
                            else
                            {
                                LogDebugMessage($"Search term not found in file: {file}");
                            }

                            fileStopwatch.Stop();
                            LogDebugMessage($"Time spent searching in file {file}: {fileStopwatch.ElapsedMilliseconds} ms");

                            // Aktualisiere die ProgressBar
                            fileCount++;
                            this.Invoke(new Action(() => UpdateProgressBar(file, fileCount)));
                        }
                        catch (Exception ex)
                        {
                            LogDebugMessage($"Error processing file: {file}. Error: {ex.Message}");
                        }
                    }));

                    LogDebugMessage("Search results DataTable created");
                    LogDebugMessage("Number of rows in search results DataTable: " + searchResults.Rows.Count);

                    // Speichern der Ergebnisse im Cache
                    searchResultCache.Add(searchTerm, searchResults);

                    // Update der DataGridView auf dem UI-Thread
                    dgvtablo.Invoke(new Action(() =>
                    {
                        dgvtablo.DataSource = null; // Setze die DataSource auf null, um die DataGridView zu leeren
                        dgvtablo.Rows.Clear();

                        // Hinzufügen der Daten aus der DataTable
                        foreach (DataRow dr in searchResults.Rows)
                        {
                            dgvtablo.Rows.Add(dr["Index"], dr["Path"], LoadIcon(dr["Typ"].ToString()), dr["Name"], dr["Findings"]);
                        }

                        LogDebugMessage("DataGridView updated with search results");
                    }));

                    dgvtablo.Invoke(new Action(() =>
                    {
                        dgvtablo.DataSource = searchResults;
                        dgvtablo.Columns["Findings"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        dgvtablo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                        dgvtablo.DataBindingComplete += (s, e) =>
                        {
                            foreach (DataGridViewRow row in dgvtablo.Rows)
                            {
                                int maxLines = 3; // Begrenze die Anzahl der Zeilen
                                int charHeight = TextRenderer.MeasureText("A", dgvtablo.DefaultCellStyle.Font).Height;
                                int maxHeight = maxLines * charHeight;
                                row.Height = Math.Min(maxHeight, row.Height);
                            }
                        };

                        dgvtablo.Refresh();
                    }));

                    LogDebugMessage("Search results DataTable created");
                    LogDebugMessage("Number of rows in search results DataTable: " + searchResults.Rows.Count);
                    LogDebugMessage("Search results DataTable columns: " + String.Join(", ", searchResults.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
                    LogDebugMessage("Number of rows in search results DataTable: " + searchResults.Rows.Count);
                    LogDebugMessage("DataGridView columns: " + String.Join(", ", dgvtablo.Columns.Cast<DataGridViewColumn>().Select(c => c.Name)));
                    LogDebugMessage("Number of rows in DataGridView: " + dgvtablo.Rows.Count);
                    LogDebugMessage("DataGridView updated with search results");
                    LogDebugMessage($"Suche abgeschlossen. {index - 1} Dateien gefunden.");
                    SaveDebugMessagesToFile();

                    // Vervollständige die ProgressBar
                    this.Invoke(new Action(() => CompleteProgressBar()));
                }
                catch (Exception ex)
                {
                    LogDebugMessage($"Error in SearchFilesAsync method: {ex.Message}");
                }
            });
        }


        // Methode zum Initialisieren der ProgressBar
        private void InitializeProgressBar(int max)
        {
            // Setze das Maximum der ProgressBar auf die Anzahl der zu durchsuchenden Dateien
            pgBar.Minimum = 0;
            pgBar.Maximum = max;
            pgBar.Value = 0;
            pgBar.Step = 1;


            // Setze den Text des Labels auf "Suche läuft..."
            lblStatus.Text = "Die Suche läuft in diesen Dokumenten: ...";
            lblStatus.Visible = true;
        }

        // Methode zum Aktualisieren der ProgressBar
        private void UpdateProgressBar(string currentFile, int currentIndex)
        {
            // Inkrementiere den Wert der ProgressBar
            pgBar.PerformStep();

            // Setze den Text des Labels auf den aktuellen Fortschritt
            lblStatus.Text = $"Die Datei:  {currentIndex} von {pgBar.Maximum}: {Path.GetFileName(currentFile)}";
        }

        // Methode zum Zurücksetzen der ProgressBar nach Abschluss des Suchvorgangs
        private void CompleteProgressBar()
        {
            // Setze den Wert der ProgressBar auf Maximum
            pgBar.Value = pgBar.Maximum;

            // Setze den Text des Labels auf "Suche abgeschlossen"
            lblStatus.Text = "Die Suche ist abgeschlossen.";
        }

        // Asynchrone Methode zum Lesen von Textdateien (CSV, TXT, XML, HTML, CSS)
        private async System.Threading.Tasks.Task<string> ReadTextFileAsync(string filePath)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    return File.ReadAllText(filePath);
                }
                catch (Exception ex)
                {
                    LogDebugMessage($"Error reading text file: {ex.Message}");
                    return string.Empty;
                }
            });
        }


        // Asynchrone Methode zum Lesen von Word-Dateien
        private async Task<string> ReadWordFileAsync(string filePath)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                // Logik zum Lesen von Word-Dateien
                LogDebugMessage($"Reading Word file: {filePath}");
                string content = string.Empty;
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = null;

                try
                {
                    doc = wordApp.Documents.Open(filePath);
                    content = doc.Content.Text;
                }
                catch (Exception ex)
                {
                    LogDebugMessage($"Error reading Word file: {ex.Message}");
                }
                finally
                {
                    doc?.Close();
                    wordApp.Quit();
                }

                return content;
            });
        }

        private async Task<string> ReadExcelFileAsync(string filePath)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                // Logik zum Lesen von Excel-Dateien
                LogDebugMessage($"Reading Excel file: {filePath}");
                string content = string.Empty;
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook = null;

                try
                {
                    workbook = excelApp.Workbooks.Open(filePath);
                    foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in workbook.Sheets)
                    {
                        foreach (Microsoft.Office.Interop.Excel.Range row in sheet.UsedRange.Rows)
                        {
                            foreach (Microsoft.Office.Interop.Excel.Range cell in row.Cells)
                            {
                                content += cell.Text.ToString() + " ";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogDebugMessage($"Error reading Excel file: {ex.Message}");
                }
                finally
                {
                    workbook?.Close();
                    excelApp.Quit();
                }

                return content;
            });
        }

        private async Task<string> ReadPowerPointFileAsync(string filePath, bool open)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                // Logik zum Lesen von PowerPoint-Dateien
                LogDebugMessage($"Reading PowerPoint file: {filePath}");
                string content = string.Empty;
                Microsoft.Office.Interop.PowerPoint.Application pptApp = new Microsoft.Office.Interop.PowerPoint.Application();
                Microsoft.Office.Interop.PowerPoint.Presentation presentation = null;

                try
                {
                    presentation = pptApp.Presentations.Open(filePath, WithWindow: Microsoft.Office.Core.MsoTriState.msoFalse);
                    foreach (Microsoft.Office.Interop.PowerPoint.Slide slide in presentation.Slides)
                    {
                        foreach (Microsoft.Office.Interop.PowerPoint.Shape shape in slide.Shapes)
                        {
                            if (shape.HasTextFrame == Microsoft.Office.Core.MsoTriState.msoTrue)
                            {
                                content += shape.TextFrame.TextRange.Text + " ";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogDebugMessage($"Error reading PowerPoint file: {ex.Message}");
                }
                finally
                {
                    presentation?.Close();
                    pptApp.Quit();
                }

                return content;
            });
        }

        private async System.Threading.Tasks.Task<string> ReadPdfFileAsync(string filePath)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                // Logik zum Lesen von PDF-Dateien
                LogDebugMessage($"Reading PDF file: {filePath}");
                string content = string.Empty;

                try
                {
                    using (PdfDocument document = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
                    {
                        var text = new StringBuilder();
                        foreach (PdfPage page in document.Pages)
                        {
                            // Verwenden Sie einen PDF-Text-Extraktor oder eine eigene Logik zum Extrahieren von Text aus der Seite
                            string pageText = ExtractTextFromPage(page);
                            text.Append(pageText);
                        }
                        content = text.ToString();
                    }
                    LogDebugMessage($"PDF file read successfully: {filePath}");
                }
                catch (Exception ex)
                {
                    LogDebugMessage($"Error reading PDF file: {ex.Message}");
                }

                return content;
            });
        }

        // Methode zum Extrahieren von Text aus einer PDF-Seite
        private string ExtractTextFromPage(PdfPage page)
        {
            var text = new StringBuilder();
            var content = ContentReader.ReadContent(page);
            foreach (var item in content)
            {
                if (item is COperator textOperator && textOperator.OpCode.Name.ToString() == OpCodeName.Tj.ToString())
                {
                    var operands = textOperator.Operands;
                    if (operands[0] is CString cString)
                    {
                        text.Append(cString.Value);
                    }
                }
            }
            return text.ToString();
        }



        // Methode zum Lesen von Word-Dateien
        private string ReadWordFile(string filePath)
        {
            LogDebugMessage($"Reading Word file: {filePath}");
            var stopwatch = Stopwatch.StartNew();
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Document doc = null;
            string content = string.Empty;

            try
            {
                doc = wordApp.Documents.Open(filePath);
                content = doc.Content.Text;
                LogDebugMessage($"Word file read successfully: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Lesen der Word-Datei: " + ex.Message);
                LogDebugMessage($"Error reading Word file: {filePath}, Error: {ex.Message}");
            }
            finally
            {
                doc.Close();
                wordApp.Quit();
                LogDebugMessage($"Word application closed for file: {filePath}");
            }

            stopwatch.Stop();
            LogDebugMessage($"Time spent reading Word file {filePath}: {stopwatch.ElapsedMilliseconds} ms");
            return content;
        }

        // Methode zum Lesen von Excel-Dateien
        private string ReadExcelFile(string filePath)
        {
            LogDebugMessage($"Reading Excel file: {filePath}");
            var stopwatch = Stopwatch.StartNew();
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = null;
            string content = string.Empty;

            try
            {
                workbook = excelApp.Workbooks.Open(filePath);
                foreach (Excel.Worksheet sheet in workbook.Sheets)
                {
                    foreach (Excel.Range row in sheet.UsedRange.Rows)
                    {
                        foreach (Excel.Range cell in row.Cells)
                        {
                            content += cell.Text + " ";
                        }
                        content += "\n";
                    }
                }
                LogDebugMessage($"Excel file read successfully: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Lesen der Excel-Datei: " + ex.Message);
                LogDebugMessage($"Error reading Excel file: {filePath}, Error: {ex.Message}");
            }
            finally
            {
                workbook.Close();
                excelApp.Quit();
                LogDebugMessage($"Excel application closed for file: {filePath}");
            }

            stopwatch.Stop();
            LogDebugMessage($"Time spent reading Excel file {filePath}: {stopwatch.ElapsedMilliseconds} ms");
            return content;
        }

        // Methode zum Lesen von PowerPoint-Dateien
        private string ReadPowerPointFile(string filePath, bool openFile = true)
        {
            LogDebugMessage($"Reading PowerPoint file: {filePath}");
            var stopwatch = Stopwatch.StartNew();
            PowerPoint.Application pptApp = new PowerPoint.Application();
            PowerPoint.Presentation presentation = null;
            string content = string.Empty;

            try
            {
                if (openFile)
                {
                    presentation = pptApp.Presentations.Open(filePath);
                }
                else
                {
                    presentation = pptApp.Presentations.Open(filePath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);
                }

                foreach (PowerPoint.Slide slide in presentation.Slides)
                {
                    foreach (PowerPoint.Shape shape in slide.Shapes)
                    {
                        if (shape.HasTextFrame == Microsoft.Office.Core.MsoTriState.msoTrue)
                        {
                            content += shape.TextFrame.TextRange.Text + " ";
                        }
                    }
                    content += "\n";
                }
                LogDebugMessage($"PowerPoint file read successfully: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Lesen der PowerPoint-Datei: " + ex.Message);
                LogDebugMessage($"Error reading PowerPoint file: {filePath}, Error: {ex.Message}");
            }
            finally
            {
                presentation.Close();
                pptApp.Quit();
                LogDebugMessage($"PowerPoint application closed for file: {filePath}");
            }

            stopwatch.Stop();
            LogDebugMessage($"Time spent reading PowerPoint file {filePath}: {stopwatch.ElapsedMilliseconds} ms");
            return content;
        }

        // Methode zum Lesen von PDF-Dateien
        private string ReadPdfFile(string filePath)
        {
            LogDebugMessage($"Reading PDF file: {filePath}");
            var stopwatch = Stopwatch.StartNew();
            string content = string.Empty;

            try
            {
                using (var document = PdfiumViewer.PdfDocument.Load(filePath))
                {
                    var text = new System.Text.StringBuilder();
                    for (int page = 0; page < document.PageCount; page++)
                    {
                        var pageText = document.GetPdfText(page);
                        text.Append(pageText);
                    }
                    content = text.ToString();
                }
                LogDebugMessage($"PDF file read successfully: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Lesen der PDF-Datei: " + ex.Message);
                LogDebugMessage($"Error reading PDF file: {filePath}, Error: {ex.Message}");
            }

            stopwatch.Stop();
            LogDebugMessage($"Time spent reading PDF file {filePath}: {stopwatch.ElapsedMilliseconds} ms");
            return content;
        }


        // Methode zum Extrahieren von Sätzen mit dem Suchbegriff
        private List<string> GetSentencesWithTerm(string content, string searchTerm)
        {
            LogDebugMessage($"Extracting sentences with term: {searchTerm}");
            List<string> sentencesWithTerm = new List<string>();

            // Escape des Suchbegriffs für reguläre Ausdrücke
            string escapedSearchTerm = Regex.Escape(searchTerm);

            // Verwenden eines Regex, um Sätze zu trennen
            var sentenceEndings = new Regex(@"(?<=[.!?])\s+");
            string[] sentences = sentenceEndings.Split(content);

            // Regex zur Suche nach dem Suchbegriff
            var searchTermRegex = new Regex(escapedSearchTerm, RegexOptions.IgnoreCase);

            foreach (string sentence in sentences)
            {
                // Überprüft, ob der Satz den Suchbegriff enthält
                if (searchTermRegex.IsMatch(sentence))
                {
                    sentencesWithTerm.Add(sentence.Trim());
                }
            }

            LogDebugMessage($"Found {sentencesWithTerm.Count} sentences with term: {searchTerm}");
            return sentencesWithTerm;
        }


        // Methode zum Laden des Icons für die DataGridView
  /*      private Image LoadIcon(string iconName)
        {
            string iconPath = Path.Combine(iconsDirectory, iconName);
            if (File.Exists(iconPath))
            {
                return Image.FromFile(iconPath);
            }
            else
            {
                return null; // oder ein Standard-Icon verwenden
            }
        }*/

        // Methode zum Loggen von Debug-Nachrichten

        private static readonly object logLock = new object();

        private void LogDebugMessage(string message, string logFileName = "debug.log")
        {
            lock (logLock)
            {
                try
                {
                    string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string logDirectory = Path.Combine(projectDirectory, "Logs", "Debug");
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }

                    string logFilePath = Path.Combine(logDirectory, logFileName);
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Schreiben in die Log-Datei: {ex.Message}");
                }
            }
        }

        private void SaveDebugMessagesToFile(string logFileName = "debug.log")
        {
            lock (logLock)
            {
                try
                {
                    string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string logDirectory = Path.Combine(projectDirectory, "Logs", "Debug");
                    LogDebugMessage($"Logs Directory: {logDirectory}", logFileName);

                    if (!Directory.Exists(logDirectory))
                    {
                        LogDebugMessage("Logs directory does not exist, creating directory.", logFileName);
                        Directory.CreateDirectory(logDirectory);
                    }

                    string logFilePath = Path.Combine(logDirectory, logFileName);
                    LogDebugMessage($"Log file path: {logFilePath}", logFileName);

                    File.AppendAllLines(logFilePath, debugMessages);

                    debugMessages.Clear();
                    LogDebugMessage($"Debug-Nachrichten wurden in die Datei geschrieben: {logFilePath}", logFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Speichern der Log-Datei: {ex.Message}");
                }
            }
        }




        // Event-Handler für das CellPainting-Ereignis
        private void dgvtablo_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == dgvtablo.Columns["Findings"].Index && e.RowIndex >= 0)
            {
                e.Handled = true; // Verhindert das Standardzeichnen der Zelle
                e.PaintBackground(e.ClipBounds, true); // Zeichnet den Zellenhintergrund

                string cellText = e.FormattedValue.ToString();
                string searchTerm = txtSearch.Text;

                if (hoverRowIndex == e.RowIndex && hoverColumnIndex == e.ColumnIndex && !string.IsNullOrEmpty(searchTerm))
                {
                    int startIndex = cellText.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

                    if (startIndex >= 0)
                    {
                        string textBefore = cellText.Substring(0, startIndex);
                        System.Drawing.Size textBeforeSize = TextRenderer.MeasureText(e.Graphics, textBefore, e.CellStyle.Font, e.CellBounds.Size, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        TextRenderer.DrawText(e.Graphics, textBefore, e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                        string highlightedText = cellText.Substring(startIndex, searchTerm.Length);
                        System.Drawing.Font highlightFont = new System.Drawing.Font(e.CellStyle.Font.FontFamily, e.CellStyle.Font.Size + 2, System.Drawing.FontStyle.Bold);
                        System.Drawing.Color highlightColor = System.Drawing.Color.Blue;
                        System.Drawing.Color highlightBackground = System.Drawing.Color.Yellow;

                        System.Drawing.Rectangle highlightBounds = new System.Drawing.Rectangle(e.CellBounds.Left + textBeforeSize.Width, e.CellBounds.Top, TextRenderer.MeasureText(e.Graphics, highlightedText, highlightFont, e.CellBounds.Size, TextFormatFlags.Left | TextFormatFlags.VerticalCenter).Width, e.CellBounds.Height);
                        e.Graphics.FillRectangle(new System.Drawing.SolidBrush(highlightBackground), highlightBounds);
                        TextRenderer.DrawText(e.Graphics, highlightedText, highlightFont, highlightBounds, highlightColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                        string textAfter = cellText.Substring(startIndex + searchTerm.Length);
                        System.Drawing.Rectangle textAfterBounds = new System.Drawing.Rectangle(e.CellBounds.Left + textBeforeSize.Width + highlightBounds.Width, e.CellBounds.Top, e.CellBounds.Width - textBeforeSize.Width - highlightBounds.Width, e.CellBounds.Height);
                        TextRenderer.DrawText(e.Graphics, textAfter, e.CellStyle.Font, textAfterBounds, e.CellStyle.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                        return;
                    }
                }

                TextRenderer.DrawText(e.Graphics, cellText, e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        // Event-Handler für das MouseEnter-Ereignis
        private void dgvtablo_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvtablo.Columns["Findings"].Index && e.RowIndex >= 0)
            {
                hoverRowIndex = e.RowIndex;
                hoverColumnIndex = e.ColumnIndex;
                dgvtablo.InvalidateCell(e.ColumnIndex, e.RowIndex);
            }
        }

        // Event-Handler für das MouseLeave-Ereignis
        private void dgvtablo_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvtablo.Columns["Findings"].Index && e.RowIndex >= 0)
            {
                hoverRowIndex = -1;
                hoverColumnIndex = -1;
                dgvtablo.InvalidateCell(e.ColumnIndex, e.RowIndex);
            }
        }

        // Event-Handler für das Schließen des Menüs
        private void schToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogDebugMessage("Menu item 'Close' clicked");
            SaveDebugMessagesToFile();
            this.Close();
        }

        // Event-Handler für das Ändern des Textes in der TextBox
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LogDebugMessage("TextBox2 text changed");
        }

        // Event-Handler für das CellContentClick-Ereignis in der DataGridView
        private void dgvResultTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LogDebugMessage($"CellContentClick event triggered for row {e.RowIndex}, column {e.ColumnIndex}");
        }

        private void dgvtablo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        // Event-Handler für das Ändern des Zustands der Checkbox "Nur Cache lesen"
        private void chbCache_CheckedChanged(object sender, EventArgs e)
        {
            if (chbCache.Checked)
            {
                LogDebugMessage("Nur Cache lesen aktiviert");
            }
            else
            {
                LogDebugMessage("Nur Cache lesen deaktiviert");
            }
        }

        // Event-Handler für das Klicken des Cache-Lösch-Buttons
        private void btnCachErase_Click(object sender, EventArgs e)
        {
            LogDebugMessage("Cache-Leeren-Button geklickt");
            // Cache löschen
            searchResultCache.Clear();
            diskCache.SaveCache(searchResultCache.GetAll());
            LogDebugMessage("Cache erased");

            // CheckBox deaktivieren
            chbCache.Enabled = false;
            chbCache.Checked = false;
            diskCache.Clear();

            LogDebugMessage("Cache geleert");

            btnCachErase.Text = "Cache leeren";
            btnCachErase.BackColor = Color.SteelBlue;
            btnCachErase.Enabled = true;

            // Benutzer informieren
            lblChbox.Text = "Cache geleert.";
            lblChbox.Visible = true;
            lblChbox.Font = new System.Drawing.Font(lblChbox.Font, System.Drawing.FontStyle.Bold);
            StartBlinkingLabel(lblChbox);
        }

        private void StartBlinkingLabel(Label label)
        {
            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 500; // Intervall in Millisekunden
            bool labelVisible = true;

            blinkTimer.Tick += (s, e) =>
            {
                label.Visible = labelVisible;
                labelVisible = !labelVisible;
            };

            blinkTimer.Start();
        }

        private void StopBlinkingLabel()
        {
            if (blinkTimer != null)
            {
                blinkTimer.Stop();
                blinkTimer.Dispose();
                blinkTimer = null;
            }
        }

        private void InitializeComboBox()
        {
            cbColor.SelectedIndex = 0; // Standardmäßig das erste Theme auswählen
            cbColor.SelectedIndexChanged += cbColor_SelectedIndexChanged;
        }

        private void cbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbColor.SelectedItem.ToString())
            {
                case "Light":
                    ctmdgvTablos.ApplyTheme(ColorTheme.Light);
                    break;
                case "Dark":
                    ctmdgvTablos.ApplyTheme(ColorTheme.Dark);
                    break;
                case "Blue":
                    ctmdgvTablos.ApplyTheme(ColorTheme.Blue);
                    break;
                case "Custom":
                    ctmdgvTablos.ApplyTheme(ColorTheme.Green);
                    break;
            }
        }




        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }

        private void panelForCustomGrid_Enter(object sender, EventArgs e)
        {

        }
    }
}
