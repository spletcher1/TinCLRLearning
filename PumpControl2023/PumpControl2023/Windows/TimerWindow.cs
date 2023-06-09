﻿using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Threading;
using System;
using System.Threading;
using System.Globalization;
using System.Drawing;


namespace PumpControl2023
{
    public class TimerWindow : ApplicationWindow
    {
        private Canvas canvas;
        private Font font;
        private Font fontB;
        Text timerText;

        Text plusHour;
        Text minusHour;
        Text plusMin;
        Text minusMin;
        Text plusSec;
        Text minusSec;
        Text textGo;
        Text textStop;

        Text textV1;
        Text textV2;
        Text textV3;
        Text textV4;
        Text textV5;
        Text textSet;

        Button plusHourButton;
        Button minusHourButton;
        Button plusMinButton;
        Button minusMinButton;
        Button plusSecButton;
        Button minusSecButton;
        Button goButton;
        Button stopButton;

        Button T1Button;
        Button T2Button;
        Button T3Button;
        Button T4Button;
        Button T5Button;
        Button SetButton;

        Button resetButton;

        TimeSpan timerTimeSpan;
        TimeSpan beginningTimeSpan;

        SPFEZBoard theBoard;
        Settings theSettings;
        bool isInSettingMode;

        private DispatcherTimer timerTimer;

        private void CreateTimerTimer()
        {
            timerTimer = new DispatcherTimer();
            
            timerTimer.Tick += TimerTimer_Tick; ;
            timerTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timerTimer.Stop();
        }


        void RingBuzzer()
        {
            theBoard.ToggleBuzzer();
            Thread.Sleep(2000);
            theBoard.ToggleBuzzer();
        }

        private void TimerTimer_Tick(object sender, EventArgs e)
        {            
            timerTimeSpan -= TimeSpan.FromMilliseconds(500);
            UpdateTimerText();
            if(timerTimeSpan.TotalMilliseconds <= 0)
            {
                timerTimer.Stop();
                theBoard.PeriodicBeep();                
            }
        }

        public TimerWindow(Bitmap icon, string text, int width, int height, SPFEZBoard board, Settings settings) : base(icon, text, width, height)
        {
            theBoard = board;
            theSettings = settings;
            this.font = Resources.GetFont(Resources.FontResources.droid_reg48);
            this.fontB = Resources.GetFont(Resources.FontResources.droid_reg12);
            OnScreenKeyboard.Font = this.fontB;
            CreateTimerTimer();          
            timerTimeSpan = TimeSpan.FromSeconds(theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer1)].Duration);
            isInSettingMode = false;
        }

        private void CreateWindow()
        {
            this.canvas.Children.Clear();
            timerTimeSpan = TimeSpan.FromSeconds(theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer1)].Duration);
            timerText = new GHIElectronics.TinyCLR.UI.Controls.Text(this.font, timerTimeSpan.ToString("c", new CultureInfo("en-US")))
            {
                ForeColor = Colors.White,
            };
            Canvas.SetLeft(timerText, 100);
            Canvas.SetTop(timerText, 40);
            this.canvas.Children.Add(timerText);
            UpdateTimerText();

            AddButtons();

            // Enable TopBar
            Canvas.SetLeft(this.TopBar, 0); Canvas.SetTop(this.TopBar, 0);
            this.canvas.Children.Add(this.TopBar);
        }

        private void SetButtonText()
        {
            var font = Resources.GetFont(Resources.FontResources.droid_reg14);
            var font2 = Resources.GetFont(Resources.FontResources.droid_reg11);
            plusHour = new Text(font, "+ Hour")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            minusHour = new Text(font, "- Hour")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            plusMin = new Text(font, "+ Min")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            minusMin = new Text(font, "- Min")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            textV1 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Timer1))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV2 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Timer2))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV3 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Timer3))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV4 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Timer4))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textV5 = new Text(font2, theSettings.GetButtonName(SettingsIndex.Timer5))
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrap = true,
            };
            textSet = new Text(font, "Set")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

        }

        private void AddButtons()
        {
            var font = Resources.GetFont(Resources.FontResources.droid_reg14);
            var font2 = Resources.GetFont(Resources.FontResources.droid_reg11);
            SetButtonText(); 

            var textReset = new Text(font, "Reset")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            plusSec = new Text(font, "+ Sec")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            minusSec = new Text(font, "- Sec")
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };


            plusHourButton = new Button()
            {
                Child = plusHour,
                Width = 60,
                Height = 60,
            };
            minusHourButton = new Button()
            {
                Child = minusHour,
                Width = 60,
                Height = 60,
            };

            plusMinButton= new Button()
            {
                Child = plusMin,
                Width = 60,
                Height = 60,
            };
            minusMinButton = new Button()
            {
                Child = minusMin,
                Width = 60,
                Height = 60,
            };

            plusSecButton = new Button()
            {
                Child = plusSec,
                Width = 60,
                Height = 60,
            };

            minusSecButton= new Button()
            {
                Child = minusSec,
                Width = 60,
                Height = 60,
            };


            T1Button = new Button()
            {
                Child = textV1,
                Width = 60,
                Height = 60,
            };
            T2Button = new Button()
            {
                Child = textV2,
                Width = 60,
                Height = 60,
            };
            T3Button = new Button()
            {
                Child = textV3,
                Width = 60,
                Height = 60,
            };
            T4Button = new Button()
            {
                Child = textV4,
                Width = 60,
                Height = 60,
            };
            T5Button = new Button()
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

            resetButton = new Button()
            {
                Child = textReset,
                Width = 70,
                Height = 50,
            };



            plusHourButton.Click += PlusHourButton_Click;
            minusHourButton.Click += MinusHourButton_Click;
            plusMinButton.Click += PlusMinButton_Click;
            minusMinButton.Click += MinusMinButton_Click;
            plusSecButton.Click += PlusSecButton_Click;
            minusSecButton.Click += MinusSecButton_Click;
            resetButton.Click += ResetButton_Click;

            T1Button.Click += T1Button_Click;
            T2Button.Click += T2Button_Click;
            T3Button.Click += T3Button_Click;
            T4Button.Click += T4Button_Click;
            T5Button.Click += T5Button_Click;
            SetButton.Click += SetButton_Click;
            

            Canvas.SetLeft(plusHourButton, 15); Canvas.SetTop(plusHourButton, 140);
            Canvas.SetLeft(minusHourButton, 15); Canvas.SetTop(minusHourButton, 205);

            Canvas.SetLeft(plusMinButton, 80); Canvas.SetTop(plusMinButton, 140);
            Canvas.SetLeft(minusMinButton, 80); Canvas.SetTop(minusMinButton, 205);

            Canvas.SetLeft(plusSecButton, 145); Canvas.SetTop(plusSecButton, 140);
            Canvas.SetLeft(minusSecButton, 145); Canvas.SetTop(minusSecButton, 205);

            Canvas.SetLeft(T1Button, 285); Canvas.SetTop(T1Button, 140);
            Canvas.SetLeft(T2Button, 285); Canvas.SetTop(T2Button, 205);

            Canvas.SetLeft(T3Button, 350); Canvas.SetTop(T3Button, 140);
            Canvas.SetLeft(T4Button, 350); Canvas.SetTop(T4Button, 205);

            Canvas.SetLeft(T5Button, 415); Canvas.SetTop(T5Button, 140);
            Canvas.SetLeft(SetButton, 415); Canvas.SetTop(SetButton, 205);

            Canvas.SetLeft(resetButton, 395); Canvas.SetTop(resetButton, 53);

            this.canvas.Children.Add(plusHourButton);
            this.canvas.Children.Add(minusHourButton);
            this.canvas.Children.Add(plusMinButton);
            this.canvas.Children.Add(minusMinButton);
            this.canvas.Children.Add(plusSecButton);
            this.canvas.Children.Add(minusSecButton);
            this.canvas.Children.Add(T1Button);
            this.canvas.Children.Add(T2Button);
            this.canvas.Children.Add(T3Button);
            this.canvas.Children.Add(T4Button);
            this.canvas.Children.Add(T5Button);
            this.canvas.Children.Add(SetButton);
            this.canvas.Children.Add(resetButton);

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
            textGo.ForeColor = Colors.Green;
            textStop.ForeColor = Colors.Red;

            goButton = new Button()
            {
                Child = textGo,
                Width = 60,
                Height = 60,
            };

            stopButton = new Button()
            {
                Child = textStop,
                Width = 60,
                Height = 60,
            };

            goButton.Click += GoButton_Click1;
            stopButton.Click += StopButton_Click;
            Canvas.SetLeft(goButton, 210); Canvas.SetTop(goButton, 140);
            Canvas.SetLeft(stopButton, 210); Canvas.SetTop(stopButton, 205);
            this.canvas.Children.Add(goButton);
            this.canvas.Children.Add(stopButton);

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

        private void T1Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    DispenseSetting tmp = new DispenseSetting();
                    tmp.Duration = timerTimeSpan.TotalSeconds;
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer1)] = tmp;
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV1.TextContent = theSettings.GetButtonName(SettingsIndex.Timer1);
                        textV1.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {                                 
                    timerTimeSpan = TimeSpan.FromSeconds(theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer1)].Duration);
                    UpdateTimerText();
                }
            }
        }
        private void T2Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    DispenseSetting tmp = new DispenseSetting();
                    tmp.Duration = timerTimeSpan.TotalSeconds;
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer2)] = tmp;
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV2.TextContent = theSettings.GetButtonName(SettingsIndex.Timer2);
                        textV2.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    timerTimeSpan = TimeSpan.FromSeconds(theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer2)].Duration);
                    UpdateTimerText();
                }
            }
        }
        private void T3Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    DispenseSetting tmp = new DispenseSetting();
                    tmp.Duration = timerTimeSpan.TotalSeconds;
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer3)] = tmp;
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV3.TextContent = theSettings.GetButtonName(SettingsIndex.Timer3);
                        textV3.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    timerTimeSpan = TimeSpan.FromSeconds(theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer3)].Duration);
                    UpdateTimerText();
                }
            }
        }
        private void T4Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    DispenseSetting tmp = new DispenseSetting();
                    tmp.Duration = timerTimeSpan.TotalSeconds;
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer4)] = tmp;
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV4.TextContent = theSettings.GetButtonName(SettingsIndex.Timer4);
                        textV4.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    timerTimeSpan = TimeSpan.FromSeconds(theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer4)].Duration);
                    UpdateTimerText();
                }
            }
        }
        private void T5Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (isInSettingMode)
                {
                    DispenseSetting tmp = new DispenseSetting();
                    tmp.Duration = timerTimeSpan.TotalSeconds;
                    theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer5)] = tmp;
                    theSettings.SaveSettings();
                    Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                        textSet.ForeColor = Colors.Black;
                        textV5.TextContent = theSettings.GetButtonName(SettingsIndex.Timer5);
                        textV5.Invalidate();
                        textSet.Invalidate();
                        return null;
                    }, null);
                    isInSettingMode = false;
                }
                else
                {
                    timerTimeSpan = TimeSpan.FromSeconds(theSettings.TheDispenseSettings[(int)(SettingsIndex.Timer5)].Duration);
                    UpdateTimerText();
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (timerTimer.IsEnabled)
            {
                timerTimer.Stop();
            }
            theBoard.StopBuzzer();
            timerTimeSpan = TimeSpan.FromSeconds(beginningTimeSpan.TotalSeconds);
            UpdateTimerText();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (timerTimer.IsEnabled)
                {
                    timerTimer.Stop();
                }
                else
                {
                    theBoard.StopBuzzer();
                    timerTimeSpan = TimeSpan.FromSeconds(beginningTimeSpan.TotalSeconds);
                    UpdateTimerText();
                }
            }
        }

        private void GoButton_Click1(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                if (timerTimer.IsEnabled)
                    return;
                else
                {
                    beginningTimeSpan = timerTimeSpan;
                    timerTimer.Start();
                }
            }                
        }

        void UpdateTimerText()
        {
            Application.Current.Dispatcher.Invoke(TimeSpan.FromMilliseconds(10), _ => {
                if (timerText != null)
                {
                    timerText.TextContent = TimeSpan.FromSeconds((int)timerTimeSpan.TotalSeconds).ToString("c", new CultureInfo("en-US"));
                    timerText.Invalidate();
                }
                return null;
            }, null);
            
        }

        private void MinusSecButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                timerTimeSpan -= new TimeSpan(0, 0, 1);
                if (timerTimeSpan.TotalSeconds < 0)
                    timerTimeSpan = TimeSpan.Zero;
                UpdateTimerText();
            }
        }

        private void PlusSecButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                timerTimeSpan += new TimeSpan(0, 0, 1);
                UpdateTimerText();
            }
        }

        private void MinusMinButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                timerTimeSpan -= new TimeSpan(0, 1, 0);
                if (timerTimeSpan.TotalSeconds < 0)
                    timerTimeSpan = TimeSpan.Zero;
                UpdateTimerText();
            }
        }

        private void PlusMinButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                timerTimeSpan += new TimeSpan(0, 1, 0);
                UpdateTimerText();
            }
        }

        private void MinusHourButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                timerTimeSpan -= new TimeSpan(1, 0, 0);
                if (timerTimeSpan.TotalSeconds < 0)
                    timerTimeSpan = TimeSpan.Zero;
                UpdateTimerText();
            }
        }

        private void PlusHourButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                timerTimeSpan += new TimeSpan(1, 0, 0);
                UpdateTimerText();
            }
        }

        protected override void Active()
        {

            this.canvas = new Canvas();

            this.CreateWindow();

            this.Child = this.canvas;
        }

        protected override void Deactive() => this.canvas.Children.Clear();
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
