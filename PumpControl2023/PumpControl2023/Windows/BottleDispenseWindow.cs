using System.Drawing;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;
using System.Threading;
using System;

namespace PumpControl2023
{
    public class BottleDispenseWindow : ApplicationWindow
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

        Text textV1;
        Text textV2;
        Text textV3;
        Text textV4;
        Text textV5;
        Text textSet;

        Button bigPlusButton;
        Button bigMinusButton;
        Button smallMinusButton;
        Button smallPlusButton;
        Button V1Button;
        Button V2Button;
        Button V3Button;
        Button V4Button;
        Button V5Button;
        Button SetButton;
        Button GoButton;
        Button StopButton;

        Button PrimerButton;
        Button DirectionButton;
        Text DirectionText;

        int editingMode;
        bool isDispensing;
        bool isInSettingMode;

        DispenseSetting currentSetting;
        PumpControl thePump;

        SPFEZBoard theBoard;

        Settings theSettings;


        bool cancelDispenseThread;

        #region Properties
        int CurrentSpeed
        {
            get
            {
                return thePump.Speed;
            }
            set
            {
                currentSetting.Speed = value;
                if (currentSetting.Speed < 0) currentSetting.Speed = 0;
                else if (currentSetting.Speed > 100) currentSetting.Speed = 100;
                if (thePump != null)
                    thePump.Speed = currentSetting.Speed;
                if (speedText != null)
                {
                    speedText.TextContent = CurrentSpeedString;
                    speedText.Invalidate();
                }
            }
        }

        string CurrentSpeedString
        {
            get
            {
                string tmp = CurrentSpeed.ToString("D");
                if (thePump.IsDirectionForward)
                    tmp = tmp + " (F)";
                else
                    tmp = tmp + " (R)";
                return tmp;
            }
        }

        double CurrentInterval
        {
            get
            {
                return currentSetting.Interval;
            }
            set
            {
                currentSetting.Interval = value;
                if (currentSetting.Interval < 0) currentSetting.Interval = 0.0f;
                if (intervalText != null)
                {
                    intervalText.TextContent = currentSetting.Interval.ToString("F1");
                    intervalText.Invalidate();
                }
            }
        }
        double CurrentDuration
        {
            get
            {
                return currentSetting.Duration;
            }
            set
            {
                currentSetting.Duration = value;
                if (currentSetting.Duration < 0) currentSetting.Duration = 0.0f;
                if (durationText != null)
                {
                    durationText.TextContent = currentSetting.Duration.ToString("F1");
                    durationText.Invalidate();
                }

            }
        }

        int CurrentReps
        {
            get { return currentSetting.Reps; }
            set
            {
                currentSetting.Reps = value;
                if (currentSetting.Reps < 0)
                    currentSetting.Reps = 0;
                if (repsText != null)
                {
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        repsText.TextContent = currentSetting.Reps.ToString("D");
                        repsText.Invalidate();
                        return null;
                    }, null);
                }
            }
        }

        string CurrentRepsString
        {
            get
            {
                return CurrentReps.ToString("D");
            }
        }

        string CurrentIntervalString
        {
            get
            {
                return CurrentInterval.ToString("F1");
            }
        }

        string CurrentDurationString
        {
            get
            {
                return CurrentDuration.ToString("F1");
            }
        }

        DispenseSetting CurrentSetting
        {
            get
            {
                return currentSetting;
            }
            set
            {
                CurrentInterval = value.Interval;
                CurrentSpeed = value.Speed;
                CurrentReps = value.Reps;
                CurrentDuration = value.Duration;
            }
        }

        #endregion

        #region UI
        public BottleDispenseWindow(Bitmap icon, string text, int width, int height, PumpControl pump, SPFEZBoard board, Settings settings) : base(icon, text, width, height)
        {
            var font2 = Resources.GetFont(Resources.FontResources.droid_reg11);
            thePump = pump;
            theBoard = board;
            theSettings = settings;
            isInSettingMode = false;
            OnScreenKeyboard.Font = font2;
        }

        private void CreateWindow()
        {
            currentSetting = new DispenseSetting();
            AddText();
            AddButtons();
            SetEditingMode(0);
            StopDispensing();

            CurrentSetting = new DispenseSetting();
            // Enable TopBar
            Canvas.SetLeft(this.TopBar, 0); Canvas.SetTop(this.TopBar, 0);
            this.canvas.Children.Add(this.TopBar);

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

            repsText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, CurrentRepsString)
            {
                ForeColor = Colors.White,
            };

            speedText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, CurrentSpeedString)
            {
                ForeColor = Colors.White,
            };

            durationText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, CurrentDurationString)
            {
                ForeColor = Colors.White,
            };

            intervalText = new GHIElectronics.TinyCLR.UI.Controls.Text(font, CurrentIntervalString)
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
            if (editingMode == 2)
            {
                SetEditingMode(0);
            }
            else if (editingMode == 0)
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

        private void IntervalText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isDispensing) return;
            if (editingMode == 4)
            {
                SetEditingMode(0);
            }
            else if (editingMode == 0)
            {
                SetEditingMode(4);
            }
        }

        private void DurationText_TouchUp(object sender, GHIElectronics.TinyCLR.UI.Input.TouchEventArgs e)
        {
            if (isDispensing) return;
            if (editingMode == 3)
            {
                SetEditingMode(0);

            }
            else if (editingMode == 0)
            {
                SetEditingMode(3);
            }
        }

        private void SetButtonText()
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

            textV1 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Bottle1))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV2 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Bottle2))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV3 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Bottle3))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV4 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Bottle4))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV5 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Bottle5))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };

        }


        private void AddButtons()
        {
            var font = Resources.GetFont(Resources.FontResources.droid_reg14);
            var font2 = Resources.GetFont(Resources.FontResources.droid_reg11);
            SetButtonText();
            textSet = new Text(font, "Set")
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

            V1Button = new Button()
            {
                Child = textV1,
                Width = 60,
                Height = 60,
            };
            V2Button = new Button()
            {
                Child = textV2,
                Width = 60,
                Height = 60,
            };
            V3Button = new Button()
            {
                Child = textV3,
                Width = 60,
                Height = 60,
            };
            V4Button = new Button()
            {
                Child = textV4,
                Width = 60,
                Height = 60,
            };
            V5Button = new Button()
            {
                Child = textV5,
                Width = 60,
                Height = 60,
            };
            SetButton = new Button()
            {
                Child = textSet,
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

            V1Button.Click += V1Button_Click;
            V2Button.Click += V2Button_Click;
            V3Button.Click += V3Button_Click;
            V4Button.Click += V4Button_Click;
            V5Button.Click += V5Button_Click;
            SetButton.Click += SetButton_Click;

            textGo.ForeColor = Colors.Green;
            textStop.ForeColor = Colors.Red;

            Canvas.SetLeft(bigPlusButton, 20); Canvas.SetTop(bigPlusButton, 140);
            Canvas.SetLeft(bigMinusButton, 20); Canvas.SetTop(bigMinusButton, 205);

            Canvas.SetLeft(smallPlusButton, 85); Canvas.SetTop(smallPlusButton, 140);
            Canvas.SetLeft(smallMinusButton, 85); Canvas.SetTop(smallMinusButton, 205);

            Canvas.SetLeft(V1Button, 285); Canvas.SetTop(V1Button, 140);
            Canvas.SetLeft(V2Button, 285); Canvas.SetTop(V2Button, 205);

            Canvas.SetLeft(V3Button, 350); Canvas.SetTop(V3Button, 140);
            Canvas.SetLeft(V4Button, 350); Canvas.SetTop(V4Button, 205);

            Canvas.SetLeft(V5Button, 415); Canvas.SetTop(V5Button, 140);
            Canvas.SetLeft(SetButton, 415); Canvas.SetTop(SetButton, 205);

            Canvas.SetLeft(GoButton, 170); Canvas.SetTop(GoButton, 140);
            Canvas.SetLeft(StopButton, 170); Canvas.SetTop(StopButton, 205);

            Canvas.SetLeft(PrimerButton, 425); Canvas.SetTop(PrimerButton, 35);
            Canvas.SetLeft(DirectionButton, 425); Canvas.SetTop(DirectionButton, 80);


            this.canvas.Children.Add(bigPlusButton);
            this.canvas.Children.Add(bigMinusButton);
            this.canvas.Children.Add(smallPlusButton);
            this.canvas.Children.Add(smallMinusButton);
            this.canvas.Children.Add(V1Button);
            this.canvas.Children.Add(V2Button);
            this.canvas.Children.Add(V3Button);
            this.canvas.Children.Add(V4Button);
            this.canvas.Children.Add(V5Button);
            this.canvas.Children.Add(SetButton);

            this.canvas.Children.Add(GoButton);
            this.canvas.Children.Add(StopButton);

            this.canvas.Children.Add(DirectionButton);
            this.canvas.Children.Add(PrimerButton);
        }

        private void V5Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Bottle5)] = new DispenseSetting(CurrentSetting);
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV5.TextContent = theSettings.GetButtonName(SettingsIndex.Bottle5);
                        textV5.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    SetCurrentSettingToDispenseSetting((int)(SettingsIndex.Bottle5));
                }
            }
        }

        private void V4Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Bottle4)] = new DispenseSetting(CurrentSetting);
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV4.TextContent = theSettings.GetButtonName(SettingsIndex.Bottle4);
                        textV4.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    SetCurrentSettingToDispenseSetting((int)(SettingsIndex.Bottle4));
                }
            }
        }

        private void V3Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Bottle3)] = new DispenseSetting(CurrentSetting);
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV3.TextContent = theSettings.GetButtonName(SettingsIndex.Bottle3);
                        textV3.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    SetCurrentSettingToDispenseSetting((int)(SettingsIndex.Bottle3));
                }
            }
        }

        private void V2Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Bottle2)] = new DispenseSetting(CurrentSetting);
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV2.TextContent = theSettings.GetButtonName(SettingsIndex.Bottle2);
                        textV2.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    SetCurrentSettingToDispenseSetting((int)(SettingsIndex.Bottle2));
                }
            }
        }

        void SetCurrentSettingToDispenseSetting(int i)
        {
            CurrentReps = theSettings.TheDispenseSettings[i].Reps;
            CurrentSpeed = theSettings.TheDispenseSettings[i].Speed;
            CurrentDuration = theSettings.TheDispenseSettings[i].Duration;
            CurrentInterval = theSettings.TheDispenseSettings[i].Interval;
        }

        private void V1Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Bottle1)] = new DispenseSetting(CurrentSetting);
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV1.TextContent = theSettings.GetButtonName(SettingsIndex.Bottle1);
                        textV1.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    SetCurrentSettingToDispenseSetting((int)(SettingsIndex.Bottle1));
                }
            }
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    isInSettingMode = false;
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textSet.Invalidate();
                        return null;
                    }, null);
                }
                else
                {
                    isInSettingMode = true;
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Red;
                        textSet.Invalidate();
                        return null;
                    }, null);
                }
            }
        }

        private void DirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                thePump.ToggleDirection();
                Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                    if (speedText != null)
                    {
                        speedText.TextContent = CurrentSpeedString;
                        speedText.Invalidate();
                    }
                    return null;
                }, null);
            }
        }

        private void PrimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                thePump.TurnPrimeOff();
            }
            else if (e.RoutedEvent.Name.CompareTo("TouchDownEvent") == 0)
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
                        CurrentReps -= 1;
                        break;
                    case 2:
                        CurrentSpeed -= 1;
                        break;
                    case 3:
                        CurrentDuration -= 0.1f;
                        break;
                    case 4:
                        CurrentInterval -= 0.1f;
                        break;
                }
            }
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
                        CurrentReps -= 10;
                        break;
                    case 2:
                        CurrentSpeed -= 10;
                        break;
                    case 3:
                        CurrentDuration -= 1.0f;
                        break;
                    case 4:
                        CurrentInterval -= 1.0f;
                        break;
                }
            }
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
                        CurrentReps += 1;
                        break;
                    case 2:
                        CurrentSpeed += 1;
                        break;
                    case 3:
                        CurrentDuration += .10f;
                        break;
                    case 4:
                        CurrentInterval += .10f;
                        break;
                }
            }
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
                        CurrentReps += 10;
                        break;
                    case 2:
                        CurrentSpeed += 10;
                        break;
                    case 3:
                        CurrentDuration += 1.0f;
                        break;
                    case 4:
                        CurrentInterval += 1.0f;
                        break;
                }
            }
        }

        #endregion

        #region General Functions
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

        void StopDispensing()
        {
            isDispensing = false;
            V1Button.IsEnabled = true;
            V2Button.IsEnabled = true;
            V3Button.IsEnabled = true;
            V4Button.IsEnabled = true;
            V5Button.IsEnabled = true;
            SetButton.IsEnabled = true;
            GoButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            V1Button.Invalidate();
            V2Button.Invalidate();
            V3Button.Invalidate();
            V4Button.Invalidate();
            V5Button.Invalidate();
            SetButton.Invalidate();
            GoButton.Invalidate();
            StopButton.Invalidate();

            cancelDispenseThread = true;
        }

        void StartDispensing()
        {
            SetEditingMode(0);
            isDispensing = true;
            V1Button.IsEnabled = false;
            V2Button.IsEnabled = false;
            V3Button.IsEnabled = false;
            V4Button.IsEnabled = false;
            V5Button.IsEnabled = false;
            SetButton.IsEnabled = false;
            GoButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            V1Button.Invalidate();
            V2Button.Invalidate();
            V3Button.Invalidate();
            V4Button.Invalidate();
            V5Button.Invalidate();
            SetButton.Invalidate();
            GoButton.Invalidate();

            var t = new Thread(DispenseThreadWorker);
            cancelDispenseThread = false;
            t.Start();
        }


        void DispenseThreadWorker()
        {
            bool manualBreak = false;
            int millisecDuration = (int)(currentSetting.Duration * 1000.0);
            int millisecInterval = (int)(currentSetting.Interval * 1000.0);
            int originalReps = currentSetting.Reps;
            while (CurrentReps > 0 && !manualBreak)
            {
                thePump.TurnDispenseOn();
                Thread.Sleep(millisecDuration);
                thePump.TurnDispenseOff();
                CurrentReps--;
                if (CurrentReps > 0)
                    Thread.Sleep(millisecInterval);
                if (cancelDispenseThread)
                {
                    theBoard.Beep(200);
                    manualBreak = true;
                }
            }
            CurrentReps = originalReps;
            theBoard.Beep(200);
            Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                StopDispensing();
                return null;
            }, null);

        }

        protected override void Active()
        {

            this.canvas = new Canvas();

            this.CreateWindow();

            this.Child = this.canvas;
        }

        protected override void Deactive() => this.canvas.Children.Clear();
        #endregion
    }
}
