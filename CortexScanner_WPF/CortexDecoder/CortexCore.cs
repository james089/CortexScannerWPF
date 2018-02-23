using CortexScanner_WPF.Services;
using Emgu.CV.Structure;
using mUserControl_BSC_dll.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using static CortexScanner_WPF.CortexDecoder.CortexDecoderFunctions;

namespace CortexScanner_WPF.CortexDecoder
{
    public class CortexCore
    {
        public static CortexDecoderFunctions mCortexDecoder;
        public static bool IsCortexReady;
        public static string ResultString;
        public static Point ResultCenter;
        public static CortexResult FullResult;
        public static Rectangle BondRec;

        public static BackgroundWorker decodeRoutine = new BackgroundWorker();

        public static void DecoderSetup()
        {
            decodeRoutine.DoWork += new DoWorkEventHandler(decodeRoutine_doWork);
            decodeRoutine.ProgressChanged += new ProgressChangedEventHandler(decodeRoutine_ProgressChanged);
            decodeRoutine.RunWorkerCompleted += new RunWorkerCompletedEventHandler(decodeRoutine_WorkerCompleted);
            decodeRoutine.WorkerReportsProgress = true;

            mCortexDecoder = new CortexDecoderFunctions();
            if (mCortexDecoder.Initialize() <= 0)
            {
                mMessageBox.Show("Could not get handle");
                IsCortexReady = false;
                return;
            }
            IsCortexReady = true;
        }

        private static void decodeRoutine_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MainWindow.main.busyWindow.Visibility = System.Windows.Visibility.Collapsed;
            PreviewService.imgOriginal.Draw(CortexCore.BondRec, new Bgr(0, 0, 255), 3);
            if (ResultString != "NULL")
                MainWindow.player.Play();
            if (mMessageBox.Show("" + CortexCore.ResultString) == mDialogResult.yes)
            {
                PreviewService.startPreview(previewFPS.HIGH);
                CortexCore.mCortexDecoder.ResetResult();
            }
        }

        private static void decodeRoutine_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private static void decodeRoutine_doWork(object sender, DoWorkEventArgs e)
        {
            double timeout = 1.5;
            DateTime dateTimeStart = DateTime.Now;
            while (mCortexDecoder.GetResult().decodeData == null && (DateTime.Now - dateTimeStart).TotalSeconds < timeout)
            {
                Decode((Bitmap)e.Argument);
                Thread.Sleep(300);
            }
            if ((DateTime.Now - dateTimeStart).TotalSeconds >= timeout)
            {
                ResultString = "NULL";
            }
        }

        public static void Decode_async(Bitmap bmp)
        {
            mCortexDecoder.ResetResult();
            decodeRoutine.RunWorkerAsync(bmp);
            MainWindow.main.busyWindow.Visibility = System.Windows.Visibility.Visible;
        }

        public static void Decode(Bitmap bmp)
        {
            if (bmp == null) return;
            if (!IsCortexReady) return;

            try
            {
                mCortexDecoder.Decode(bmp);
            }
            catch (Exception)
            {
                ResultString = "-Decode Error-";
                return;
            }
            FullResult = mCortexDecoder.GetResult();
            ResultString = FullResult.decodeData;
            ResultString = (ResultString == null) ? ResultString = "NULL" : ResultString;
            ResultCenter = FullResult.center;
            BondRec = new Rectangle(FullResult.corner0.X, FullResult.corner0.Y,
                FullResult.corner1.X - FullResult.corner0.X, FullResult.corner2.Y - FullResult.corner0.Y);

            //mCortexDecoder.Close();
        }
    }
}
