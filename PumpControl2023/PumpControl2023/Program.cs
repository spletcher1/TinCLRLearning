using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Media;


namespace PumpControl2023
{
    internal class Program : Application
    {
        public static Program MainApp;
      
        public Program(DisplayController d) : base(d)
        {

        }
        static void Main()
        {
         //   SCM20260D theBoard = new SCM20260D();
            SCM26260D_Display theDisplay = new SCM26260D_Display();            
            Input.Touch.InitializeTouch();
            MainApp = new Program(theDisplay.DisplayController);
            MainWindow mainWindow = new MainWindow(theDisplay.Width, theDisplay.Height);
           

            // Create System Window            
            Bitmap iconImageSystem = Resources.GetBitmap(Resources.BitmapResources.Hands);
            string iconTextSystem = "System Information";
            var systemWindow = new SystemWindow(iconImageSystem, iconTextSystem, TMPDisplay.Width, TMPDisplay.Height);

            mainWindow.RegisterWindow(systemWindow); // Register to MainWindow            


            Bitmap iconImageSystem2 = Resources.GetBitmap(Resources.BitmapResources.settingImage);
            string iconTextSystem2 = "System Information";
            var systemWindow2 = new SystemWindow(iconImageSystem2, iconTextSystem2, TMPDisplay.Width, TMPDisplay.Height);
            
            mainWindow.RegisterWindow(systemWindow2);

            MainApp.Run(mainWindow);            
        }



    }
}
