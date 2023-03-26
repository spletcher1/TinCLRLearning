using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;


namespace PumpControl2023
{
    public class MyApp
    {
        SCM20260D theBoard;
        double currentBrightness = 10;
        public MyApp()
        {
            theBoard = new SCM20260D();
            theBoard.Button1Pressed += TheBoard_Button1Pressed;
        }

        public void Run()
        {

            TestScreen();
            while (true)
            {

            }
        }

        private void TheBoard_Button1Pressed()
        {
            currentBrightness += .10;
            if (currentBrightness > 1.00) currentBrightness = .10;
            theBoard.SetLED1Brightness(currentBrightness);
        }

        private void TestScreen()
        {

        }

    }

    internal class Program
    {
        static void Main()
        {
            var app = new MyApp();
            app.Run();
        }



    }
}
