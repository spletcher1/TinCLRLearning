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
            SCM20260D theBoard = new SCM20260D();
           
            MainApp = new Program(theBoard.DisplayController);
            MainWindow mainWindow = new MainWindow(theBoard.DisplayWidth, theBoard.DisplayHeight);


            // Create System Window            
            Bitmap iconImageSystem = Resources.GetBitmap(Resources.BitmapResources.AnotherVial);
            string iconTextSystem = "Vials";
            var systemWindow = new SystemWindow(iconImageSystem, iconTextSystem, theBoard.DisplayWidth, theBoard.DisplayHeight);

            mainWindow.RegisterWindow(systemWindow); // Register to MainWindow            


            Bitmap iconImageSystem2 = Resources.GetBitmap(Resources.BitmapResources.AnotherBottle);
            string iconTextSystem2 = "Bottles";
           var systemWindow2 = new SystemWindow(iconImageSystem2, iconTextSystem2, theBoard.DisplayWidth, theBoard.DisplayHeight);

            Bitmap iconImageSystem3 = Resources.GetBitmap(Resources.BitmapResources.AnotherPlate);
            string iconTextSystem3 = "Plates";
            var systemWindow3= new SystemWindow(iconImageSystem3, iconTextSystem3, theBoard.DisplayWidth, theBoard.DisplayHeight);

            Bitmap iconImageSystem4 = Resources.GetBitmap(Resources.BitmapResources.AnotherHands);
            string iconTextSystem4 = "Manual";
            var systemWindow4 = new SystemWindow(iconImageSystem4, iconTextSystem4, theBoard.DisplayWidth, theBoard.DisplayHeight);

            Bitmap iconImageSystem5 = Resources.GetBitmap(Resources.BitmapResources.Timer);
            string iconTextSystem5 = "Timer";
            var timerWindow = new TimerWindow(iconImageSystem5, iconTextSystem5, theBoard.DisplayWidth, theBoard.DisplayHeight);


            mainWindow.RegisterWindow(systemWindow2);
            mainWindow.RegisterWindow(systemWindow3);
            mainWindow.RegisterWindow(systemWindow4);
            mainWindow.RegisterWindow(timerWindow);
            MainApp.Run(mainWindow);            
        }



    }
}
