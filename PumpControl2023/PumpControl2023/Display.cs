using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using System.Drawing;


namespace PumpControl2023
{

    static class TMPDisplay
    {
        public static DisplayController DisplayController { get; set; }

        public static void InitializeDisplay()
        {
            var backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);

            backlight.SetDriveMode(GpioPinDriveMode.Output);

            backlight.Write(GpioPinValue.High);

            DisplayController = GHIElectronics.TinyCLR.Devices.Display.DisplayController.GetDefault();

            var controllerSetting = new GHIElectronics.TinyCLR.Devices.Display.ParallelDisplayControllerSettings
            {
                // 480x272
                Width = 480,
                Height = 272,
                DataFormat = GHIElectronics.TinyCLR.Devices.Display.DisplayDataFormat.Rgb565,
                PixelClockRate = 10000000,
                PixelPolarity = false,
                DataEnablePolarity = false,
                DataEnableIsFixed = false,
                HorizontalFrontPorch = 2,
                HorizontalBackPorch = 2,
                HorizontalSyncPulseWidth = 41,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 2,
                VerticalBackPorch = 2,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = false,
            };

            DisplayController.SetConfiguration(controllerSetting);
            DisplayController.Enable();
        }

        public static int Width => 480;
        public static int Height => 272;
    }

    public abstract class Display
    {
        protected GpioPin backlight;
        protected DisplayController displayController;
        protected System.Drawing.Graphics screen;
        protected int width;
        protected int height;

        public int Width { get { return width; } }
        public int Height { get { return height; } }    

        public Graphics Screen
        {
            get { return screen; }
        }

        public DisplayController DisplayController
        {
            get { return displayController; }
        }

    }

    public class SCM26260D_Display : Display
    {
        public SCM26260D_Display()
        {
            backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);
            backlight.SetDriveMode(GpioPinDriveMode.Output);
            backlight.Write(GpioPinValue.High);
            width = 480;
            height = 270;
            InitializeDisplayController();
        }

        void InitializeDisplayController()
        {
            displayController = DisplayController.GetDefault();

            displayController.SetConfiguration(new ParallelDisplayControllerSettings
            {
                Width = 480,
                Height = 272,
                DataFormat = DisplayDataFormat.Rgb565,
                Orientation = DisplayOrientation.Degrees0, //Rotate display.
                PixelClockRate = 10000000,
                PixelPolarity = false,
                DataEnablePolarity = false,
                DataEnableIsFixed = false,
                HorizontalFrontPorch = 2,
                HorizontalBackPorch = 2,
                HorizontalSyncPulseWidth = 41,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 2,
                VerticalBackPorch = 2,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = false,
            });

            displayController.Enable();
            screen = Graphics.FromHdc(displayController.Hdc);
            screen.Clear();
        }
    }

    public class Portal_Display : Display
    {
        public Portal_Display()
        {
            backlight = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PA15);
            backlight.SetDriveMode(GpioPinDriveMode.Output);
            backlight.Write(GpioPinValue.High);
            width = 480;
            height = 270;
            InitializeDisplayController();
        }

        void InitializeDisplayController()
        {
            displayController = DisplayController.GetDefault();

            displayController.SetConfiguration(new ParallelDisplayControllerSettings
            {
                Width = 480,
                Height = 272,
                DataFormat = DisplayDataFormat.Rgb565,
                Orientation = DisplayOrientation.Degrees0, //Rotate display.
                PixelClockRate = 10000000,
                PixelPolarity = false,
                DataEnablePolarity = false,
                DataEnableIsFixed = false,
                HorizontalFrontPorch = 2,
                HorizontalBackPorch = 2,
                HorizontalSyncPulseWidth = 41,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 2,
                VerticalBackPorch = 2,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = false,
            });

            displayController.Enable();
            screen = Graphics.FromHdc(displayController.Hdc);
            screen.Clear();
        }
    }
}
