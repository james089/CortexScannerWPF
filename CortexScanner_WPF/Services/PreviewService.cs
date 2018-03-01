using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CortexScanner_WPF.CortexDecoder;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CortexScanner_WPF.Services
{
    public enum previewFPS
    {
        LOW = 5,
        MEDIUM = 15,
        HIGH = 30
    }

    public class PreviewService
    {
        public static Image<Bgr, byte> imgOriginal;
        public static BackgroundWorker previewRoutine = new BackgroundWorker();
        public static bool IsCapturing = false;
        public static previewFPS _previewFPS;

        public static void previewSetup()
        {
            previewRoutine.DoWork += new DoWorkEventHandler(previewRoutine_doWork);
            previewRoutine.ProgressChanged += new ProgressChangedEventHandler(previewRoutine_ProgressChanged);
            previewRoutine.RunWorkerCompleted += new RunWorkerCompletedEventHandler(previewRoutine_WorkerCompleted);
            previewRoutine.WorkerReportsProgress = true;
            previewRoutine.WorkerSupportsCancellation = true;
        }

        public static void startPreview(previewFPS previewFPS)
        {
            IsCapturing = true;
            previewRoutine.RunWorkerAsync(previewFPS);
        }

        public static void stopPreview()
        {
            IsCapturing = false;
            previewRoutine.CancelAsync();
        }

        private static void previewRoutine_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsCapturing = false;
        }

        private static void previewRoutine_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (MainWindow.main.DecodeSwitch)
            {
                imgOriginal.Draw(CortexCore.BondRec, new Bgr(0, 0, 255), 3);
                Helpers.drawString(CortexCore.ResultString, imgOriginal,
                    new Point(CortexCore.BondRec.X, CortexCore.BondRec.Y - 20), 1, Color.Red);
                MainWindow.main.lbl_codeResult.Content = CortexCore.ResultString;
            }

            MainWindow.main.ibOriginal.Source = Helpers.ToBitmapSource(imgOriginal);
        }

        private static void previewRoutine_doWork(object sender, DoWorkEventArgs e)
        {
            IsCapturing = true; previewFPS FPS = (previewFPS)e.Argument;
            while (!previewRoutine.CancellationPending)
            {
                if (ConnectionService.mCamera == null)
                {
                    previewRoutine.CancelAsync();
                    return;
                }

                imgOriginal = ConnectionService.Capture();

                if (MainWindow.main.DecodeSwitch)
                {
                    CortexCore.Decode(imgOriginal.ToBitmap());
                }
                previewRoutine.ReportProgress(0);
                Thread.Sleep(1000 / (int)FPS);
            }
        }
    }
}
