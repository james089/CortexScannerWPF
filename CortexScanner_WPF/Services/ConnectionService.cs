

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;

namespace CortexScanner_WPF.Services
{
    public class ConnectionService
    {
        public static Capture mCamera = null;
        public static bool IsCameraConnected;

        public static bool ConnectCamera()
        {
            if (mCamera != null)                                           //if there is a camera, dispose and reconnect.
            {
                mCamera.Dispose();
                IsCameraConnected = false;
            }

            try
            {
                mCamera = new Capture();   // using Capture(0) / Capture(1) to switch between different webcams connected
                IsCameraConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                IsCameraConnected = false;
            }
            return IsCameraConnected;
        }

        public static Image<Bgr, byte> Capture()
        {
            if (IsCameraConnected)
                return mCamera.QueryFrame();
            else
                return null;
        }
    }
}
