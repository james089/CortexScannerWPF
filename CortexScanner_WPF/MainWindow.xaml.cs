using CortexScanner_WPF.CortexDecoder;
using CortexScanner_WPF.Services;
using Emgu.CV.Structure;
using mUserControl_BSC_dll.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CortexScanner_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow main = null;

        public bool DecodeSwitch = false;
        public static System.Media.SoundPlayer player = new System.Media.SoundPlayer(
            System.Environment.CurrentDirectory + @"\Resources\beep.wav");

        public MainWindow()
        {
            main = this;
            InitializeComponent();
        }

        private void Btn_minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PreviewService.previewSetup();
            CortexCore.DecoderSetup();

            if (ConnectionService.ConnectCamera())
                PreviewService.startPreview(previewFPS.HIGH);
        }

        private void Chk_continueDetect_Checked(object sender, RoutedEventArgs e)
        {
            Chk_continueDetect.Content = "ON";
            DecodeSwitch = true;
        }

        private void Chk_continueDetect_Unchecked(object sender, RoutedEventArgs e)
        {
            Chk_continueDetect.Content = "OFF";
            lbl_codeResult.Content = "";
            CortexCore.mCortexDecoder.ResetResult();
            DecodeSwitch = false;
        }

        private void Chk_stopAtDetect_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void Chk_stopAtDetect_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void Btn_scanOnce_Click(object sender, RoutedEventArgs e)
        {
            PreviewService.stopPreview();
            CortexCore.Decode_async(PreviewService.imgOriginal.ToBitmap());
            //PreviewService.imgOriginal.Draw(CortexCore.BondRec, new Bgr(0, 0, 255), 3);
            //if (mMessageBox.Show("" + CortexCore.ResultString) == mDialogResult.yes)
            //{
            //    PreviewService.startPreview(previewFPS.HIGH);
            //    CortexCore.mCortexDecoder.ResetResult();
            //}
        }

    }
}
