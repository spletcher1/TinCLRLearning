using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Devices.Rtc;
using GHIElectronics.TinyCLR.Native;
using System.Drawing;
using System.Diagnostics;


namespace PumpControl2023
{
    abstract public class SPFEZBoard
    {
        protected GpioPin userLED;
        protected GpioPin LED2;
        protected GpioPin Button1;
        protected GpioPin Button2;
        protected GpioPin buzzer;

        protected PwmController buzzerPWMController;
        protected PwmChannel buzzerPWMChannel;

        public delegate void ButtonPressedHandler();

        public event ButtonPressedHandler Button1Pressed;
        public event ButtonPressedHandler Button2Pressed;
        
        protected RtcController theRTCController;


        public SPFEZBoard()
        {
            userLED = null;
            LED2 = null;
            buzzer = null;
            Button1 = Button2 =  null;
            buzzerPWMController = null;
        }

        public abstract void SetLed(int led, bool on);
        public abstract void ToggleLed(int led);

        public bool UserLED
        {
            get
            {
                if (userLED == null) return false;
                if(userLED.Read()== GpioPinValue.High)
                    return true;
                else
                    return false;
            }
            set
            {
                if (userLED == null) return;
                if (value)
                    userLED.Write(GpioPinValue.High);
                else
                    userLED.Write(GpioPinValue.Low);
            }
        }

        public bool Buzzer
        {
            get
            {
                if(buzzerPWMChannel == null) return false;
                if (buzzerPWMChannel.IsStarted)
                    return true;
                else
                    return false;
            }
            set
            {
                if (buzzerPWMChannel == null) return;
                if (value)
                    buzzerPWMChannel.Start();
                else
                    buzzerPWMChannel.Stop();
            }
        }

        public void SetBuzzer(int freq, double dutyCycle)
        {
            if(buzzerPWMController != null && buzzerPWMChannel != null)
            {
                buzzerPWMController.SetDesiredFrequency(freq);
                buzzerPWMChannel.SetActiveDutyCyclePercentage(dutyCycle);
            }
        }
        public void ToggleBuzzer()
        {
            if (buzzerPWMController == null || buzzerPWMChannel == null) return;
            if (Buzzer)
                Buzzer = false;
            else
                Buzzer = true;
        }
        public void ToggleUserLED()
        {
            if (userLED != null)
                userLED.Toggle();
        }

        protected void OnButton1Pressed()
        {
            if (Button1 != null)
                Button1Pressed?.Invoke();
        }

        protected void OnButton2Pressed()
        {
            if (Button2 != null)
                Button2Pressed?.Invoke();
        }

    }

    public class Portal : SPFEZBoard
    {
        SCM26260D_Display theDisplay;

        public int DisplayWidth
        {
            get { return theDisplay.Width; }
        }

        public int DisplayHeight
        {
            get { return theDisplay.Height; }
        }

        public DisplayController DisplayController
        {
            get { return theDisplay.DisplayController; }
        }

        public Portal() : base()
        {
            userLED = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PB0);
            userLED.SetDriveMode(GpioPinDriveMode.Output);
            UserLED = true;

            buzzer = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PB1);
            buzzer.SetDriveMode(GpioPinDriveMode.Output);
            Buzzer = false;

            Button1 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PE3);
            Button1.SetDriveMode(GpioPinDriveMode.InputPullUp);
            Button1.ValueChangedEdge = GpioPinEdge.RisingEdge;
            Button1.ValueChanged += Button1_ValueChanged;
            Button2 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PB7);
            Button2.SetDriveMode(GpioPinDriveMode.Input);

            theDisplay = new SCM26260D_Display();
            Input.Touch.InitializeTouch();

            InitializeRTC();
            InitializeBuzzer();
        }

        void InitializeRTC()
        {
            theRTCController = RtcController.GetDefault();

            if (theRTCController.IsValid)
            {
                Debug.WriteLine("RTC is Valid");
                // RTC is good so let's use it
                // If we need to set it....
                //var MyTime = new DateTime(2023, 3, 27,11, 26, 00);
                //theRTCController.Now = MyTime;
                SystemTime.SetTime(theRTCController.Now);
                theRTCController.SetChargeMode(BatteryChargeMode.Fast);
            }
            else
            {
                Debug.WriteLine("RTC is Invalid");
                // RTC is not valid. Get user input to set it
                // This example will simply set it to January 1st 2020 at 11:11:11
                var MyTime = new DateTime(2020, 1, 1, 11, 11, 11);
                theRTCController.Now = MyTime;
                SystemTime.SetTime(MyTime);
            }

        }
        void InitializeBuzzer()
        {
            buzzerPWMController = GHIElectronics.TinyCLR.Devices.Pwm.PwmController.FromName(SC20260.Timer.Pwm.Controller3.Id);
            buzzerPWMChannel = buzzerPWMController.OpenChannel(SC20260.Timer.Pwm.Controller3.PB1);
            SetBuzzer(500, 0.5);
            Buzzer = false;
        }


        public override void SetLed(int num, bool on)
        {
            if (num != 1)
                return;

            if (on)
                userLED.Write(GpioPinValue.Low);
            else
                userLED.Write(GpioPinValue.High);

        }

        public override void ToggleLed(int num)
        {
            if (num != 1)
                return;

            userLED.Toggle();

        }

       
        private void Button1_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            base.OnButton1Pressed();
        }

    }

    public class SCM20260D : SPFEZBoard
    {
        SCM26260D_Display theDisplay;

        public int DisplayWidth
        {
            get { return theDisplay.Width; }
        }

        public int DisplayHeight
        {
            get { return theDisplay.Height; }
        }

        public DisplayController DisplayController
        {
            get { return theDisplay.DisplayController; }
        }

     
        public override void SetLed(int num, bool on)
        {
            if (num > 2 || num < 1)
                return;
            if (num == 1)
            {
                if (on)
                    userLED.Write(GpioPinValue.Low);
                else
                    userLED.Write(GpioPinValue.High);
            }
            else
            {
                if (on)
                    LED2.Write(GpioPinValue.Low);
                else
                    LED2.Write(GpioPinValue.High);
            }
        }

        public override void ToggleLed(int num)
        {
            if (num > 2 || num < 1)
                return;
            if (num == 1)
            {
                userLED.Toggle();
            }
            else
                LED2.Toggle();
        }


        public SCM20260D() : base()
        {
            userLED = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PB0);
            userLED.SetDriveMode(GpioPinDriveMode.Output);

            LED2 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PH11);
            LED2.SetDriveMode(GpioPinDriveMode.Output);
            UserLED = true;
            SetLed(2, true);

            Button1 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PE3);
            Button1.SetDriveMode(GpioPinDriveMode.InputPullUp);
            Button1.ValueChangedEdge = GpioPinEdge.RisingEdge;
            Button1.ValueChanged += Button1_ValueChanged;
            Button2 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PB7);
            Button2.SetDriveMode(GpioPinDriveMode.Input);
       
            theDisplay = new SCM26260D_Display();
            Input.Touch.InitializeTouch();

            InitializeRTC();
            InitializeBuzzer();
            
        }



        void InitializeBuzzer()
        {
            buzzerPWMController = GHIElectronics.TinyCLR.Devices.Pwm.PwmController.FromName(SC20260.Timer.Pwm.Controller3.Id);
            buzzerPWMChannel = buzzerPWMController.OpenChannel(SC20260.Timer.Pwm.Controller3.PB1);
            SetBuzzer(500, 0.5);
            Buzzer = false;
        }


        void InitializeRTC()
        {
            theRTCController = RtcController.GetDefault();

            if (theRTCController.IsValid)
            {
                Debug.WriteLine("RTC is Valid");
                // RTC is good so let's use it
                // If we need to set it....
                //var MyTime = new DateTime(2023, 3, 27,11, 26, 00);
                //theRTCController.Now = MyTime;
                SystemTime.SetTime(theRTCController.Now);
                theRTCController.SetChargeMode(BatteryChargeMode.Fast);
            }
            else
            {
                Debug.WriteLine("RTC is Invalid");
                // RTC is not valid. Get user input to set it
                // This example will simply set it to January 1st 2020 at 11:11:11
                var MyTime = new DateTime(2020, 1, 1, 11, 11, 11);
                theRTCController.Now = MyTime;
                SystemTime.SetTime(MyTime);
            }

        }

        private void Button1_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            base.OnButton1Pressed();
        }
    }
}
