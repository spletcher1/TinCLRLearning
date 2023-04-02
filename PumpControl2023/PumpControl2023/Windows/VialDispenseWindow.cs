using System.Drawing;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;
using System.Threading;
using System;

namespace PumpControl2023
{
    public class VialDispenseWindow : ApplicationWindow
    {
        private Canvas canvas;
   
        Text textBigPlus;
        Text textBigMinus;
        Text textSmallPlus;        
        Text textSmallMinus;

        Text durationLabel;
        Text repsLabel;
        Text speedLabel;
        Text intervalLabel;

        Text textGo;
        Text textStop;

        Text durationText;
        Text repsText;
        Text speedText;
        Text intervalText;

        Button bigPlusButton;
        Button bigMinusButton;
        Button smallMinusButton;
        Button smallPlusButton; 
        Button M1Button;
        Button M2Button;
        Button M3Button;
        Button M4Button;
        Button M5Button;
        Button M6Button;
        Button GoButton;
        Button StopButton;

        Button PrimerButton;
        Button DirectionButton;
        Text DirectionText;

        int editingMode;
        bool isDispensing;

        DispenseSetting currentSetting;
        PumpControl thePump;

        bool cancelDispenseThread;

        public VialDispenseWindow(Bitmap icon, string text, int width, int height,PumpControl pump) : base(icon, text, width, height)
        {
            thePump = pump;
        }

        private void CreateWindow()
        {
            currentSetting = new DispenseSetting();

            AddText();
            AddButtons();
            SetEditingMode(0);
            StopDispensing();
            SetPumpValues();
            // Enable TopBar
            Canvas.SetLeft(this.TopBar, 0); Canvas.SetTop(this.TopBar, 0);
            this.canvas.Children.Add(this.TopBar);     
            
           
        }

        void SetPumpValues()
        {
            thePump.SetSpeed(currentSetting.Speed);
        }
        void AddText()
        {          
            var font = Resources.GetFont(Resources.FontResources.droid_reg18);

            this.canvas.Children.Clear();

            repsLabel = new GHIElectronics.TinyCLR.UI.Controls.Text(font, " Reps: ")
            {
                ForeColor = Colors.White,
            };

            speedLabel = new GHIElectronics.TinyCLR.UI.Controls.Text(font, "Speed: ")
            {
                ForeColor = Colors.White,
            };

            durationLabel = new GHIElectronics.TinyCLR.UI.Controls.Text(font, "Duration (sec): ")
            {
                ForeColor = Colors.White,
            };

            intervalLabel = new GHIElectronics.TinyCLR.UI.Controls.Text(font, " Interval (sec): ")
            {
                ForeColor = Colors.White,
            };

            durationLabel.TouchUp += DurationText_TouchUp;
            intervalLabel.TouchUp += IntervalText_TouchUp;
            repsLabel.TouchUp += RepsText_TouchUp;
            speedLabel.TouchUp += SpeedText_TouchUp;            

            Canvas.SetLeft(repsLabel, 28); Canvas.SetTop(repsLabel, 45); 
            Canvas.SetLeft(speedLabel, 20); Canvas.SetTop(speedLabel, 85); 
            Canvas.SetLeft(durationLabel, 180); Canvas.SetTop(durationLabel, 45);
            Canvas.SetLeft(intervalLabel, 183); Canvas.SetTop(intervalLabel, 85);

            this.canvas.Children.Add(repsLabel);
            this.canvas.Children.Add(speedLabel);
            this.canvas.Children.Add(durationLabel);
            this.canvas.Children.Add(intervalLabel);

            repsText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, currentSetting.Reps.ToString("D"))
            {
                ForeColor = Colors.White,
            };

            speedText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, currentSetting.Speed.ToString("D"))
            {
                ForeColor = Colors.White,
            };

            durationText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, currentSetting.Duration.ToString("F1"))
            {
                ForeColor = Colors.White,
            };

            intervalText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, currentSetting.Interval.ToString("F1"))
            {
                ForeColor = Colors.White,
            };

            durationText.TouchUp += DurationText_TouchUp;
            intervalText.TouchUp += IntervalText_TouchUp;
            repsText.TouchUp += RepsText_TouchUp;
            speedText.TouchUp += SpeedText_TouchUp;

            Canvas.SetLeft(repsText, 100); Canvas.SetTop(repsText, 45);
            Canvas.SetLeft(speedText, 100); Canvas.SetTop(speedText, 85);
            Canvas.SetLeft(durationText, 345); Canvas.SetTop(durationText, 45);
            Canvas.SetLeft(intervalText, 345); Canvas.SetTop(intervalText, 85);

            this.canvas.Children.Add(repsText);
            this.canvas.Children.Add(speedText);
            this.canvas.Children.Add(durationText);
            this.canvas.Children.Add(intervalText);

        }



        private void SpeedText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isDispensing) return;
            if (editingMode == 2) {
                SetEditingMode(0);
            }
            else if(editingMode == 0)
            {
                SetEditingMode(2);
            }
        }

        private void RepsText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isDispensing) return;
            if (editingMode == 1)
            {
                SetEditingMode(0);

            }
            else if (editingMode == 0)
            {
                SetEditingMode(1);
            }
        }

        void SetEditingMode(int mode)
        {
            switch (mode)
            {
                case 0: //Clear editing mode                    
                    repsLabel.ForeColor = Colors.White;
                    durationLabel.ForeColor = Colors.White;
                    speedLabel.ForeColor = Colors.White;
                    intervalLabel.ForeColor = Colors.White;
                    repsText.ForeColor = Colors.White;
                    speedText.ForeColor = Colors.White;
                    intervalText.ForeColor = Colors.White;
                    durationText.ForeColor = Colors.White;
                    bigMinusButton.IsEnabled = false;
                    bigPlusButton.IsEnabled = false;
                    smallMinusButton.IsEnabled = false;
                    smallPlusButton.IsEnabled = false;                    
                    editingMode = 0;
                    break;
                case 1: //Reps                    
                    textBigPlus.TextContent = "+10x";
                    textSmallPlus.TextContent = "+1x";
                    textBigMinus.TextContent = "-10x";
                    textSmallMinus.TextContent = "-1x";
                    repsLabel.ForeColor = Colors.Red;
                    repsText.ForeColor = Colors.Red;
                    bigMinusButton.IsEnabled = true;
                    bigPlusButton.IsEnabled = true;
                    smallMinusButton.IsEnabled = true;
                    smallPlusButton.IsEnabled = true;
                    editingMode = 1;
                    bigPlusButton.Invalidate();
                    break;
                case 2: //Speed                    
                    textBigPlus.TextContent = "+10x";
                    textSmallPlus.TextContent = "+1x";
                    textBigMinus.TextContent = "-10x";
                    textSmallMinus.TextContent = "-1x";
                    speedLabel.ForeColor = Colors.Red;
                    speedText.ForeColor = Colors.Red;
                    bigMinusButton.IsEnabled = true;
                    bigPlusButton.IsEnabled = true;
                    smallMinusButton.IsEnabled = true;
                    smallPlusButton.IsEnabled = true;
                    bigPlusButton.Invalidate();
                    editingMode = 2;
                    break;
                case 3: //Duration                    
                    textBigPlus.TextContent = "+1s";
                    textSmallPlus.TextContent = "+0.1s";
                    textBigMinus.TextContent = "-1s";
                    textSmallMinus.TextContent = "-0.1s";
                    durationLabel.ForeColor = Colors.Red;
                    durationText.ForeColor = Colors.Red;
                    bigMinusButton.IsEnabled = true;
                    bigPlusButton.IsEnabled = true;
                    smallMinusButton.IsEnabled = true;
                    smallPlusButton.IsEnabled = true;
                    bigPlusButton.Invalidate();
                    editingMode = 3;
                    break;
                case 4: //Interval                  
                    textBigPlus.TextContent = "+1s";
                    textSmallPlus.TextContent = "+0.1s";
                    textBigMinus.TextContent = "-1s";
                    textSmallMinus.TextContent = "-0.1s";
                    intervalLabel.ForeColor = Colors.Red;
                    intervalText.ForeColor = Colors.Red;
                    bigMinusButton.IsEnabled = true;
                    bigPlusButton.IsEnabled = true;
                    smallMinusButton.IsEnabled = true;
                    smallPlusButton.IsEnabled = true;
                    bigPlusButton.Invalidate();
                    editingMode = 4;
                    break;                 
            }
            bigPlusButton.Invalidate();
            smallPlusButton.Invalidate();
            bigMinusButton.Invalidate();
            smallMinusButton.Invalidate();
        }


        private void IntervalText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isDispensing) return;
            if (editingMode==4)
            {
                SetEditingMode(0);
            }
            else if (editingMode==0)
            {
                SetEditingMode(4);
            }
        }

        private void DurationText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isDispensing) return;
            if (editingMode==3)
            {
                SetEditingMode(0);

            }
            else if (editingMode == 0)
            {
                SetEditingMode(3);
            }
        }

        private void AddButtons()
        {
            var font = Resources.GetFont(Resources.FontResources.droid_reg14);
            var font2 = Resources.GetFont(Resources.FontResources.droid_reg11);
            textBigPlus = new Text(font, "+10x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            textBigMinus = new Text(font, "-10x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            textSmallPlus = new Text(font, "+1x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            textSmallMinus = new Text(font, "-1x")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            
            var textM1 = new Text(font2, "52-1\n1-43")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            textM1.TextWrap = true;
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

            var textPrimer = new Text(font, "P")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            DirectionText = new Text(font, "F/D")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            textGo = new Text(font, "GO")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            textStop = new Text(font, "STOP")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            DirectionButton = new Button()
            {
                Child = DirectionText,
                Width = 50,
                Height = 40,
            };

            PrimerButton = new Button()
            {
                Child = textPrimer,
                Width = 50,
                Height = 40,
            };



            bigPlusButton = new Button()
            {
                Child = textBigPlus,
                Width = 60,
                Height = 60,
            };
            bigMinusButton = new Button()
            {
                Child = textBigMinus,
                Width = 60,
                Height = 60,
            };

            smallMinusButton = new Button()
            {
                Child = textSmallMinus,
                Width = 60,
                Height = 60,
            };
            smallPlusButton = new Button()
            {
                Child = textSmallPlus,
                Width = 60,
                Height = 60,
            };

            M1Button = new Button()
            {
                Child = textM1,
                Width = 60,
                Height = 60,
            };
            M2Button = new Button()
            {
                Child = textM2,
                Width = 60,
                Height = 60,
            };
            M3Button = new Button()
            {
                Child = textM3,
                Width = 60,
                Height = 60,
            };
            M4Button = new Button()
            {
                Child = textM4,
                Width = 60,
                Height = 60,
            };
            M5Button = new Button()
            {
                Child = textM5,
                Width = 60,
                Height = 60,
            };
            M6Button = new Button()
            {
                Child = textM6,
                Width = 60,
                Height = 60,
            };

            GoButton = new Button()
            {
                Child = textGo,
                Width = 60,
                Height = 60,
            };

            StopButton = new Button()
            {
                Child = textStop,
                Width = 60,
                Height = 60,
            };

            bigPlusButton.Click += BigPlusButton_Click;
            smallPlusButton.Click += SmallPlusButton_Click;
            bigMinusButton.Click += BigMinusButton_Click;
            smallMinusButton.Click += SmallMinusButton_Click;
            GoButton.Click += GoButton_Click;
            StopButton.Click += StopButton_Click;
            PrimerButton.Click += PrimerButton_Click;
            DirectionButton.Click += DirectionButton_Click;            

            M1Button.Click += M1Button_Click;

            textGo.ForeColor = Colors.Green;
            textStop.ForeColor = Colors.Red;

            Canvas.SetLeft(bigPlusButton, 20); Canvas.SetTop(bigPlusButton, 140);
            Canvas.SetLeft(bigMinusButton, 20); Canvas.SetTop(bigMinusButton, 205);

            Canvas.SetLeft(smallPlusButton, 85); Canvas.SetTop(smallPlusButton, 140);
            Canvas.SetLeft(smallMinusButton, 85); Canvas.SetTop(smallMinusButton, 205);

            Canvas.SetLeft(M1Button, 285); Canvas.SetTop(M1Button, 140);
            Canvas.SetLeft(M2Button, 285); Canvas.SetTop(M2Button, 205);

            Canvas.SetLeft(M3Button, 350); Canvas.SetTop(M3Button, 140);
            Canvas.SetLeft(M4Button, 350); Canvas.SetTop(M4Button, 205);

            Canvas.SetLeft(M5Button, 415); Canvas.SetTop(M5Button, 140);
            Canvas.SetLeft(M6Button, 415); Canvas.SetTop(M6Button, 205);

            Canvas.SetLeft(GoButton, 170); Canvas.SetTop(GoButton, 140);
            Canvas.SetLeft(StopButton, 170); Canvas.SetTop(StopButton, 205);

            Canvas.SetLeft(PrimerButton, 425); Canvas.SetTop(PrimerButton, 35);
            Canvas.SetLeft(DirectionButton, 425); Canvas.SetTop(DirectionButton, 80);


            this.canvas.Children.Add(bigPlusButton);
            this.canvas.Children.Add(bigMinusButton);
            this.canvas.Children.Add(smallPlusButton);
            this.canvas.Children.Add(smallMinusButton);
            this.canvas.Children.Add(M1Button);
            this.canvas.Children.Add(M2Button);
            this.canvas.Children.Add(M3Button);
            this.canvas.Children.Add(M4Button);
            this.canvas.Children.Add(M5Button);
            this.canvas.Children.Add(M6Button);

            this.canvas.Children.Add(GoButton);
            this.canvas.Children.Add(StopButton);

            this.canvas.Children.Add(DirectionButton);
            this.canvas.Children.Add(PrimerButton);
        }

        private void DirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                thePump.ToggleDirection();
            }
        }

        private void PrimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                thePump.TurnPrimeOff();
            } else if(e.RoutedEvent.Name.CompareTo("TouchDownEvent") == 0)
            {
                thePump.TurnPrimeOn();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                StopDispensing();
            }
        }

        void StopDispensing()
        {
            isDispensing = false;
            M1Button.IsEnabled = true;
            M2Button.IsEnabled = true;
            M3Button.IsEnabled = true;
            M4Button.IsEnabled = true;
            M5Button.IsEnabled = true;
            M6Button.IsEnabled = true;
            GoButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            M1Button.Invalidate();
            M2Button.Invalidate();
            M3Button.Invalidate();
            M4Button.Invalidate();
            M5Button.Invalidate();
            M6Button.Invalidate();
            GoButton.Invalidate();
            StopButton.Invalidate();

            cancelDispenseThread = true;

        }

        void StartDispensing()
        {
            SetEditingMode(0);
            isDispensing = true;
            M1Button.IsEnabled = false;
            M2Button.IsEnabled = false;
            M3Button.IsEnabled = false;
            M4Button.IsEnabled = false;
            M5Button.IsEnabled = false;
            M6Button.IsEnabled = false;
            GoButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            M1Button.Invalidate();
            M2Button.Invalidate();
            M3Button.Invalidate();
            M4Button.Invalidate();
            M5Button.Invalidate();
            M6Button.Invalidate();
            GoButton.Invalidate();

            var t = new Thread(DispenseThreadWorker);
            cancelDispenseThread = false;
            t.Start();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDispensing) return;
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                StartDispensing();
            }
        }

        private void SmallMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDispensing) return;
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                switch (editingMode)
                {
                    case 0:
                        break;
                    case 1:
                        currentSetting.Reps -= 1;
                        if (currentSetting.Reps < 0) currentSetting.Reps = 0;
                        repsText.TextContent = currentSetting.Reps.ToString("D");
                        break;
                    case 2:
                        currentSetting.Speed -= 1;
                        if (currentSetting.Speed < 0) currentSetting.Speed = 0;
                        speedText.TextContent = currentSetting.Speed.ToString("D");
                        break;
                    case 3:
                        currentSetting.Duration -= 0.1f;
                        if (currentSetting.Duration < 0) currentSetting.Duration = 0.0f;
                        durationText.TextContent = currentSetting.Duration.ToString("F1");
                        break;
                    case 4:
                        currentSetting.Interval -= 0.1f;
                        if (currentSetting.Interval < 0) currentSetting.Interval = 0.0f;
                        intervalText.TextContent = currentSetting.Interval.ToString("F1");
                        break;
                }
            }
            SetPumpValues();
        }

        private void BigMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDispensing) return;
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                switch (editingMode)
                {
                    case 0:
                        break;
                    case 1:
                        currentSetting.Reps -= 10;
                        if (currentSetting.Reps < 1) currentSetting.Reps = 1;
                        repsText.TextContent = currentSetting.Reps.ToString("D");
                        break;
                    case 2:
                        currentSetting.Speed -= 10;
                        if (currentSetting.Speed < 1) currentSetting.Speed = 1;
                        speedText.TextContent = currentSetting.Speed.ToString("D");
                        break;
                    case 3:
                        currentSetting.Duration -= 1.0f;
                        if (currentSetting.Duration < 0.10f) currentSetting.Duration = 0.10f;
                        durationText.TextContent = currentSetting.Duration.ToString("F1");
                        break;
                    case 4:
                        currentSetting.Interval -= 1.0f;
                        if (currentSetting.Interval < 0.10f) currentSetting.Interval = 0.10f;
                        intervalText.TextContent = currentSetting.Interval.ToString("F1");
                        break;
                }
            }
            SetPumpValues();
        }

        private void SmallPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDispensing) return;
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                switch (editingMode)
                {
                    case 0:
                        break;
                    case 1:
                        currentSetting.Reps += 1;
                        if (currentSetting.Reps > 200) currentSetting.Reps = 200;
                        repsText.TextContent = currentSetting.Reps.ToString("D");
                        break;
                    case 2:
                        currentSetting.Speed += 1;
                        if (currentSetting.Speed > 100) currentSetting.Speed = 100;
                        speedText.TextContent = currentSetting.Speed.ToString("D");
                        break;
                    case 3:
                        currentSetting.Duration += .10f;
                        if (currentSetting.Duration > 100.0f) currentSetting.Duration = 100.0f;
                        durationText.TextContent = currentSetting.Duration.ToString("F1");
                        break;
                    case 4:
                        currentSetting.Interval += .10f;
                        if (currentSetting.Interval > 100.0f) currentSetting.Interval = 100.0f;
                        intervalText.TextContent = currentSetting.Interval.ToString("F1");
                        break;
                }
            }
            SetPumpValues();
        }

        private void BigPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDispensing) return;
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                switch (editingMode)
                {
                    case 0:
                        break;
                    case 1:
                        currentSetting.Reps += 10;
                        if (currentSetting.Reps > 200) currentSetting.Reps = 200;
                        repsText.TextContent = currentSetting.Reps.ToString("D");
                        break;
                    case 2:
                        currentSetting.Speed += 10;
                        if (currentSetting.Speed > 100) currentSetting.Speed = 100;
                        speedText.TextContent = currentSetting.Speed.ToString("D");
                        break;
                    case 3:
                        currentSetting.Duration += 1.0f;
                        if (currentSetting.Duration > 100.0f) currentSetting.Duration = 100.0f;
                        durationText.TextContent = currentSetting.Duration.ToString("F1");
                        break;
                    case 4:
                        currentSetting.Interval += 1.0f;
                        if (currentSetting.Interval > 100.0f) currentSetting.Interval = 100.0f;
                        intervalText.TextContent = currentSetting.Interval.ToString("F1");
                        break;
                }
            }
            SetPumpValues();
        }

        private void M1Button_Click(object sender, RoutedEventArgs e)
        {
            if (isDispensing) return;
            textBigPlus.TextContent = "Hi";
        }     


        void DispenseThreadWorker()
        {
            bool manualBreak = false;
            int millisecDuration = (int) currentSetting.Duration * 1000;
            int millisecInterval = (int)currentSetting.Interval * 1000;
            int repsToPump = currentSetting.Reps;
            while (repsToPump > 0 && !manualBreak)
            {
                thePump.TriggerDispense();
                Thread.Sleep(millisecDuration);
                thePump.TriggerDispense();
                repsToPump--;
                Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {                  
                    repsText.TextContent = repsToPump.ToString("D");
                    repsText.Invalidate();
                    return null;
                }, null);                
                Thread.Sleep(millisecInterval);
                if(cancelDispenseThread)
                {
                    manualBreak = true;
                }   
            }
            Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                repsText.TextContent = currentSetting.Reps.ToString("D");
                repsText.Invalidate();
                return null;
            }, null);
            StopDispensing();
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
