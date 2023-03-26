using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Drawing;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace FirstOne
{
    public static class ScreenDraw
    {
        public static void DrawIntroScreen(Graphics Screen)
        {
            var image = Resource1.GetBitmap(Resource1.BitmapResources.pump2);
            var font = Resource1.GetFont(Resource1.FontResources.droid_reg24);

            Screen.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb(255, 255, 0, 0)), 0, 0, 240, 136);

            Screen.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb
                (255, 0, 0, 255)), 240, 0, 240, 136);

            Screen.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb
                (128, 0, 255, 0)), 120, 0, 240, 136);

            Screen.DrawRectangle(new Pen(Color.Yellow), 10, 150, 140, 100);
            Screen.DrawEllipse(new Pen(Color.Purple), 170, 150, 140, 100);
            Screen.FillRectangle(new SolidBrush(Color.Teal), 330, 150, 140, 100);

            Screen.DrawImage(image, 110, 110);

            Screen.DrawString("Hello world!", font, new SolidBrush(Color.Blue), 210, 255);

            Screen.Flush();
        }
    }
}
