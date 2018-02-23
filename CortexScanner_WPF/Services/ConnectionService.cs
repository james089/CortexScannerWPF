using CameraToImage_dll_x64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CortexScanner_WPF.Services
{
    public class ConnectionService
    {
        public static CameraConnection mCamera = new CameraConnection();

        public static bool ConnectCamera()
        {
            if (mCamera != null)                                           //if there is a camera, dispose and reconnect.
                mCamera.disposeCam();

            if (mCamera.connect(camType.WebCam))
            {
                return true;
            }
            else
                return false;
        }
    }
}
