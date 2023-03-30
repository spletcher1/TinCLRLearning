using System.Drawing;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;

namespace PumpControl2023
{
    public class TimerWindow : ApplicationWindow
    {
        private Canvas canvas;
        private Font font;
        private Font fontB;

        public TimerWindow(Bitmap icon, string text, int width, int height) : base(icon, text, width, height)
        {
            this.font = Resources.GetFont(Resources.FontResources.droid_reg48);
            this.fontB = Resources.GetFont(Resources.FontResources.droid_reg12);
            OnScreenKeyboard.Font = this.fontB;
            
        }

        private void CreateWindow()
        {
            this.canvas.Children.Clear();

            var timerText = new GHIElectronics.TinyCLR.UI.Controls.Text(this.font, "00:00:00")
            {
                ForeColor = Colors.White,
            };
            Canvas.SetLeft(timerText, 10);
            Canvas.SetTop(timerText, 60);
            this.canvas.Children.Add(timerText);

            var addHourButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "+ HOUR")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var subtractHourButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "- HOUR")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var addMinuteButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "+ MIN")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var subtractMinuteButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "- MIN")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var subtractSecondButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "- SEC")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var addSecondButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "+ SEC")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var startButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "START")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var stopButton = new Button()
            {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "STOP")
                {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            Canvas.SetLeft(addHourButton, 10);
            Canvas.SetTop(addHourButton, 170);
            this.canvas.Children.Add(addHourButton);

            Canvas.SetLeft(subtractHourButton, 10);
            Canvas.SetTop(subtractHourButton, 220);
            this.canvas.Children.Add(subtractHourButton);

            Canvas.SetLeft(addMinuteButton, 120);
            Canvas.SetTop(addMinuteButton, 170);
            this.canvas.Children.Add(addMinuteButton);

            Canvas.SetLeft(subtractMinuteButton, 120);
            Canvas.SetTop(subtractMinuteButton, 220);
            this.canvas.Children.Add(subtractMinuteButton);

            Canvas.SetLeft(addSecondButton, 230);
            Canvas.SetTop(addSecondButton, 170);
            this.canvas.Children.Add(addSecondButton);

            Canvas.SetLeft(subtractSecondButton, 230);
            Canvas.SetTop(subtractSecondButton, 220);
            this.canvas.Children.Add(subtractSecondButton);

            Canvas.SetLeft(startButton, 375);
            Canvas.SetTop(startButton, 170);
            this.canvas.Children.Add(startButton);

            Canvas.SetLeft(stopButton, 375);
            Canvas.SetTop(stopButton, 220);
            this.canvas.Children.Add(stopButton);

            // Enable TopBar
            Canvas.SetLeft(this.TopBar, 0); Canvas.SetTop(this.TopBar, 0);
            this.canvas.Children.Add(this.TopBar);
        }

        protected override void Active()
        {

            this.canvas = new Canvas();

            this.CreateWindow();

            this.Child = this.canvas;
        }

        protected override void Deactive() => this.canvas.Children.Clear();

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {


            }

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
