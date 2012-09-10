using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using GenArt.AST;
using GenArt.Classes;
using System.IO;
using GenArt.Core.Classes;
using System.Collections.Generic;
using GenArt.Core.AST;
namespace GenArt
{
    public partial class MainForm : Form
    {
        const int EVOLVER_COUNT = 4;
        int currIndex = 0;

        public static Settings Settings;

        private AbstractDnaDrawing currentDrawing;
        private AbstractDnaDrawing guiDrawing;
        private DateTime lastRepaint = DateTime.MinValue;

        private int lastSelected;
        private TimeSpan repaintIntervall = new TimeSpan(0, 0, 0, 0, 0);
        private int repaintOnSelectedSteps = 3;

        private SettingsForm settingsForm;

        private List<Evolver> evolvers = new List<Evolver>();

        Pixel[] sourceColours = null;
  

        public MainForm()
        {
            InitializeComponent();
            Settings = Serializer.DeserializeSettings();
            if (Settings == null)
                Settings = new Settings();

            seedTxtBox.Text = new Random().Next() + "";
        }

        public static Pixel[] SetupSourceColorMatrix(Bitmap sourceImage)
        {
            if (sourceImage == null)
                throw new NotSupportedException("A source image of Bitmap format must be provided");

            BitmapData bd = sourceImage.LockBits(
            new Rectangle(0, 0, Tools.MaxWidth, Tools.MaxHeight),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
            Pixel[] sourcePixels = new Pixel[Tools.MaxWidth * Tools.MaxHeight];
            unsafe
            {
                fixed (Pixel* psourcePixels = sourcePixels)
                {
                    Pixel* pSrc = (Pixel*)bd.Scan0.ToPointer();
                    Pixel* pDst = psourcePixels;
                    for (int i = sourcePixels.Length; i > 0; i--)
                        *(pDst++) = *(pSrc++);
                }
            }
            sourceImage.UnlockBits(bd);

            return sourcePixels;
        }

        public bool isRunning
        {
            get { return evolvers.Count > 0 && evolvers.TrueForAll(evolver => evolver.isRunning); }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Console.WriteLine("isRunning> " + isRunning);
            if (isRunning)
                Stop();
            else
                Start();
        }

        private void Start()
        {
            btnStart.Text = "Stop";
            tmrRedraw.Enabled = true;

            for (int i = Int32.Parse(seedTxtBox.Text); evolvers.Count < EVOLVER_COUNT; i++)
            {
                Evolver evolver = new VectorEvolver(i, (Pixel[])sourceColours.Clone());
                evolvers.Add(evolver);
            }

            evolvers.ForEach(evolver => evolver.start());
        }

        private void Stop()
        {
            evolvers.ForEach(evolver => evolver.stop());

            btnStart.Text = "Start";
            tmrRedraw.Enabled = false;
        }

        private Evolver currentEvolver
        {
            get
            {
                return evolvers[currIndex];
            }
        }

        private void tmrRedraw_Tick(object sender, EventArgs e)
        {
            // select current;
            Evolver evolver = currentEvolver;
            if (evolver == null)
            {
                Console.WriteLine("evolver fail");
                return;
            }

            // TODO: this should be part of Evolver
            lock (evolver.evolverLock)
            {
                currentDrawing = evolver.currentDrawing.Clone();
            }

            if (currentDrawing == null)
            {
                Console.WriteLine("currentDrawing fail");
                return;
            }

            String txt = String.Format("Current     : {0}\r\n", currIndex);

            for (int i = 0; i < EVOLVER_COUNT; i++)
            {
                txt += String.Format("Evolver {0} : {1}\r\n", i, evolvers[i].errorLevel);
            }

            textBox1.Text = txt;

            toolStripStatusLabelFitness.Text = evolver.errorLevel.ToString();
            toolStripStatusLabelGeneration.Text = evolver.generation.ToString();
            toolStripStatusLabelSelected.Text = evolver.selected.ToString();

            if (EVOLVER_COUNT > 1)
            {
                Evolver e1 = evolvers.Find(ev => ev.readyToBreed);
                if (e1 != null)
                {
                    Evolver e2 = evolvers.Find(ev => ev.readyToBreed && ev != e1);
                    if(e2 != null)
                        e1.breed(e2);

                }
            }

            bool shouldRepaint = false;
            if (repaintIntervall.Ticks > 0)
                if (lastRepaint < DateTime.Now - repaintIntervall)
                    shouldRepaint = true;

            if (repaintOnSelectedSteps > 0)
                if (lastSelected + repaintOnSelectedSteps < evolver.selected)
                    shouldRepaint = true;

            if (shouldRepaint)
            {
                redraw();
            }
        }

        private void redraw()
        {
            guiDrawing = currentDrawing;
            pnlCanvas.Invalidate();
            lastRepaint = DateTime.Now;
            lastSelected = currentEvolver.selected;
        }
       
        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (guiDrawing == null)
            {
                e.Graphics.Clear(Color.Black);
                return;
            }

            using (Bitmap backBuffer = new Bitmap(trackBarScale.Value * picPattern.Width, trackBarScale.Value * picPattern.Height, PixelFormat.Format24bppRgb))
            {
                guiDrawing.Render(backBuffer, e, trackBarScale.Value);

            }
        }

        private void OpenImage()
        {
            Stop();

            string fileName = FileUtil.GetOpenFileName(FileUtil.ImgExtension);
            if (string.IsNullOrEmpty(fileName))
                return;

            picPattern.Image = Image.FromFile(fileName);

            Tools.MaxHeight = picPattern.Height;
            Tools.MaxWidth = picPattern.Width;

            SetCanvasSize();

            splitContainer1.SplitterDistance = picPattern.Width + 30;

            sourceColours = SetupSourceColorMatrix(picPattern.Image as Bitmap);
            DnaBrush.colours = sourceColours;
        }

        private void SetCanvasSize()
        {
            pnlCanvas.Height = trackBarScale.Value*picPattern.Height;
            pnlCanvas.Width = trackBarScale.Value*picPattern.Width;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void savePNG()
        {
            MessageBox.Show("NOT IMPLEMENTED");
            //string fileName = FileUtil.GetSaveFileName(FileUtil.PNGExtension);
            //if (string.IsNullOrEmpty(fileName) == false && currentDrawing != null)
            //{
            //    MessageBox.Show("NOT IMPLEMENTED");

            //}
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settingsForm != null)
                if (settingsForm.IsDisposed)
                    settingsForm = null;

            if (settingsForm == null)
                settingsForm = new SettingsForm();

            settingsForm.Show();
        }

        private void sourceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImage();
        }

        private void trackBarScale_Scroll(object sender, EventArgs e)
        {
            SetCanvasSize();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.allowPolygons = allowPolygons.Checked;
        }

        private void allowCircles_CheckedChanged(object sender, EventArgs e)
        {
            Settings.allowCircles = allowCircles.Checked;
        }

        private void allowEllipses_CheckedChanged(object sender, EventArgs e)
        {
            Settings.allowEllipses = allowEllipses.Checked;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Settings.allowTinyCircles = onlyTiny.Checked;
        }

        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savePNG();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DnaBrush.colours = sourceColours = SetupSourceColorMatrix(picPattern.Image as Bitmap);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currIndex++;
            currIndex = Math.Min(currIndex, EVOLVER_COUNT - 1);
            redraw();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            currIndex--;
            currIndex = Math.Max(currIndex, 0);
            redraw();
        }
    }
}