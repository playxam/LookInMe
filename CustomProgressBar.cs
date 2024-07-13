using System;
using System.Drawing;
using System.Windows.Forms;

public class CustomProgressBar : ProgressBar
{
    public CustomProgressBar()
    {
        // Aktivieren Sie den benutzerdefinierten Zeichenmodus
        this.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Rectangle rec = e.ClipRectangle;

        // Berechnen Sie die Breite des Fortschrittsbalkens
        rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
        if (ProgressBarRenderer.IsSupported)
        {
            ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
        }

        // Definieren Sie die Farben
        Color backgroundColor = Color.FromArgb(198, 226, 255); // SlateGray1
        Color progressColor = Color.FromArgb(54, 100, 139); // SteelBlue4

        // Füllen Sie den Hintergrund
        e.Graphics.FillRectangle(new SolidBrush(backgroundColor), 0, 0, Width, Height);

        // Füllen Sie den Fortschrittsbalken
        e.Graphics.FillRectangle(new SolidBrush(progressColor), 2, 2, rec.Width, rec.Height);
    }
}
