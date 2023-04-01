using System.Drawing;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;


namespace PumpControl2023
{
    public class VialDispenseWindow : ApplicationWindow
    {
        private Canvas canvas;
   
        Text textplus10;
        Text textminus10;
        Text textplus1;
        Text textM1;
        Text textminus1;

        Text durationText;
        Text repsText;
        Text speedText;
        Text intervalText;

        Button plus10Button;
        Button minus10Button;
        Button M1Button;

        bool isEditingSpeed;
        bool isEditingDuration;
        bool isEditingReps;
        bool isEditingInterval;

        DispenseSetting currentSetting;

        public VialDispenseWindow(Bitmap icon, string text, int width, int height) : base(icon, text, width, height)
        {
            isEditingDuration = isEditingInterval = isEditingReps = isEditingSpeed = false;
        }

        private void CreateWindow()
        {

            AddText();
            AddButtons();

            // Enable TopBar
            Canvas.SetLeft(this.TopBar, 0); Canvas.SetTop(this.TopBar, 0);
            this.canvas.Children.Add(this.TopBar);

        }


        void AddText()
        {          
            var font = Resources.GetFont(Resources.FontResources.droid_reg18);

            this.canvas.Children.Clear();

            repsText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, " Reps: ")
            {
                ForeColor = Colors.White,
            };

            speedText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, "Speed: ")
            {
                ForeColor = Colors.White,
            };

            durationText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, "Duration (sec): ")
            {
                ForeColor = Colors.White,
            };

            intervalText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, " Interval (sec): ")
            {
                ForeColor = Colors.White,
            };

            durationText.TouchUp += DurationText_TouchUp;
            intervalText.TouchUp += IntervalText_TouchUp;
            repsText.TouchUp += RepsText_TouchUp;
            speedText.TouchUp += SpeedText_TouchUp;            

            Canvas.SetLeft(repsText, 28); Canvas.SetTop(repsText, 40); 
            Canvas.SetLeft(speedText, 20); Canvas.SetTop(speedText, 80); 
            Canvas.SetLeft(durationText, 200); Canvas.SetTop(durationText, 40);
            Canvas.SetLeft(intervalText, 203); Canvas.SetTop(intervalText, 80);

            this.canvas.Children.Add(repsText);
            this.canvas.Children.Add(speedText);
            this.canvas.Children.Add(durationText);
            this.canvas.Children.Add(intervalText);
        }
        

        private void SpeedText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if(isEditingSpeed) { 
                speedText.ForeColor = Colors.White;
                isEditingSpeed = false;

            }
            else if(!isEditingInterval && !isEditingDuration && !isEditingReps)
            {
                isEditingSpeed = true;
                speedText.ForeColor = Colors.Red;
            }
        }

        private void RepsText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isEditingReps)
            {
                repsText.ForeColor = Colors.White;
                isEditingReps = false;

            }
            else if (!isEditingInterval && !isEditingDuration && !isEditingSpeed)
            {
                isEditingReps = true;
                repsText.ForeColor = Colors.Red;
            }
        }

        private void IntervalText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isEditingInterval)
            {
                intervalText.ForeColor = Colors.White;
                isEditingInterval = false;

            }
            else if (!isEditingReps && !isEditingDuration && !isEditingSpeed)
            {
                isEditingInterval = true;
                intervalText.ForeColor = Colors.Red;
            }
        }

        private void DurationText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isEditingDuration)
            {
                durationText.ForeColor = Colors.White;
                isEditingDuration = false;

            }
            else if (!isEditingReps && !isEditingReps && !isEditingSpeed)
            {
                isEditingDuration = true;
                durationText.ForeColor = Colors.Red;
            }
        }

        private void AddButtons()
        {
            var font = Resources.GetFont(Resources.FontResources.droid_reg14);
            textplus10= new Text(font, "+10x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            textminus10 = new Text(font, "-10x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            textplus1 = new Text(font, "+1x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            textminus1 = new Text(font, "-1x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            textM1 = new Text(font, "M1")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            var textM2 = new Text(font, "M2")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            var textM3 = new Text(font, "M3")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            var textM4 = new Text(font, "M4")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            var textM5 = new Text(font, "M5")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            var textM6 = new Text(font, "M6")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            var textGo = new Text(font, "GO")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            var textStop = new Text(font, "STOP")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            plus10Button= new Button()
            {
                Child = textplus10,
                Width = 60,
                Height = 60,
            };
            minus10Button = new Button()
            {
                Child = textminus10,
                Width = 60,
                Height = 60,
            };

            var minus1Button = new Button()
            {
                Child = textminus1,
                Width = 60,
                Height = 60,
            };
            var plus1Button = new Button()
            {
                Child = textplus1,
                Width = 60,
                Height = 60,
            };

            M1Button = new Button()
            {
                Child = textM1,
                Width = 60,
                Height = 60,
            };
            var M2Button = new Button()
            {
                Child = textM2,
                Width = 60,
                Height = 60,
            };
            var M3Button = new Button()
            {
                Child = textM3,
                Width = 60,
                Height = 60,
            };
            var M4Button = new Button()
            {
                Child = textM4,
                Width = 60,
                Height = 60,
            };
            var M5Button = new Button()
            {
                Child = textM5,
                Width = 60,
                Height = 60,
            };
            var M6Button = new Button()
            {
                Child = textM6,
                Width = 60,
                Height = 60,
            };

            var GoButton = new Button()
            {
                Child = textGo,
                Width = 60,
                Height = 60,
            };

            var StopButton = new Button()
            {
                Child = textStop,
                Width = 60,
                Height = 60,
            };


            M1Button.Click += M1Button_Click;


            Canvas.SetLeft(plus10Button, 20); Canvas.SetTop(plus10Button, 140);
            Canvas.SetLeft(minus10Button, 20); Canvas.SetTop(minus10Button, 205);

            Canvas.SetLeft(plus1Button, 85); Canvas.SetTop(plus1Button, 140);
            Canvas.SetLeft(minus1Button, 85); Canvas.SetTop(minus1Button, 205);

            Canvas.SetLeft(M1Button, 285); Canvas.SetTop(M1Button, 140);
            Canvas.SetLeft(M2Button, 285); Canvas.SetTop(M2Button, 205);

            Canvas.SetLeft(M3Button, 350); Canvas.SetTop(M3Button, 140);
            Canvas.SetLeft(M4Button, 350); Canvas.SetTop(M4Button, 205);

            Canvas.SetLeft(M5Button, 415); Canvas.SetTop(M5Button, 140);
            Canvas.SetLeft(M6Button, 415); Canvas.SetTop(M6Button, 205);

            Canvas.SetLeft(GoButton, 170); Canvas.SetTop(GoButton, 140);
            Canvas.SetLeft(StopButton, 170); Canvas.SetTop(StopButton, 205);

            this.canvas.Children.Add(plus10Button);
            this.canvas.Children.Add(minus10Button);
            this.canvas.Children.Add(plus1Button);
            this.canvas.Children.Add(minus1Button);
            this.canvas.Children.Add(M1Button);
            this.canvas.Children.Add(M2Button);
            this.canvas.Children.Add(M3Button);
            this.canvas.Children.Add(M4Button);
            this.canvas.Children.Add(M5Button);
            this.canvas.Children.Add(M6Button);

            this.canvas.Children.Add(GoButton);
            this.canvas.Children.Add(StopButton);
        }

        private void M1Button_Click(object sender, RoutedEventArgs e)
        {
            textplus10.TextContent = "Hi";
        }     

        protected override void Active()
        {

            this.canvas = new Canvas();

            this.CreateWindow();

            this.Child = this.canvas;
        }

        protected override void Deactive() => this.canvas.Children.Clear();
    }
}
