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

    // This class presents and interface to the pump using one of the two FEZ boards that I 
    // have for development and implementation (Portal)

    // For the Dev Board SCM20260D Dev with 480 x 272 Touchscreen
    // PI6 is pwm for output voltage control for pump speed
    // The remote wires black (+) and orange (ground) receive the 0V-10V output from the PWM to voltage converter.
    // PB1 is Buzzer
    // PB0 is UserLED
    // PH11 is LED2
    // PF6 is pump trigger (Open drain) - Green wire in remote control cable - Yellow wire is ground
    // PF7 is pump forward/reverse (open drain) - Blue wire in remote control cable - Yellow wire is ground
    // PF8 is pump prime (open drain) - Red/Yellow wire in remote control cable - Yellow wire is ground
    // PA14 is (pulled down) input for detecting motor on state - Utilizes Red/green and Pink wires on the remote cable to connect to 3.3 on NO switch.
    // PG7 is meant to read PWM from the tachometer when the motor is running. It is connected to the Tan wire on the remote (Grey wire on remote is connected to ground)

    abstract public class SPFEZBoard
    {
        protected GpioPin userLED;
        protected GpioPin LED2;
        protected GpioPin Button1;
        protected GpioPin Button2;
        protected GpioPin buzzer;

        protected GpioPin pumpTriggerPin;
        protected GpioPin pumpReversePin;
        protected GpioPin pumpPrimePin;

        protected GpioPin motorOnPin;

        protected PwmController buzzerPWMController;
        protected PwmChannel buzzerPWMChannel;

        protected PwmController userPWMController;
        protected PwmChannel userPWMChannel;

        public delegate void ButtonPressedHandler();

        public event ButtonPressedHandler Button1Pressed;
        public event ButtonPressedHandler Button2Pressed;
        
        protected RtcController theRTCController;


        bool isBuzzerRinging;
        Timer BuzzerTimer;

        bool IsBuzzerRining
        {
            get { return isBuzzerRinging; }
        }
        #region Properties

        public GpioPin PumpTriggerPin
        {
            get { return pumpTriggerPin; }  
        }

        public GpioPin PumpReversePin
        {
            get { return pumpReversePin; }
        }

        public GpioPin PumpPrimePin
        {
            get { return pumpPrimePin; }
        }

        public GpioPin MotorOnPin
        {
            get { return motorOnPin; }
        }

        public bool IsMotorOn
        {
            get
            {
                if(motorOnPin != null)
                {
                    if(motorOnPin.Read()==GpioPinValue.High)
                        return true;
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        public SPFEZBoard()
        {
            userLED = null;
            LED2 = null;
            buzzer = null;
            Button1 = Button2 =  null;
            buzzerPWMController = null;
            isBuzzerRinging = false;
            BuzzerTimer = null;
            BuzzerTimer = null;            
        }

        void BuzzerTimerTick(object o)
        {
            int i = (int)o;            
            if (i == 0)
                StopBuzzer();
            else
                ToggleBuzzer();
        }

        public abstract void SetLed(int led, bool on);
        public abstract void ToggleLed(int led);

        public void SetUserPWM(int freq, double dutyCycle)
        {
            if (userPWMController != null && userPWMChannel != null)
            {
                userPWMController.SetDesiredFrequency(freq);
                userPWMChannel.SetActiveDutyCyclePercentage(dutyCycle);
            }
        }

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

        protected void SetBuzzer(int freq, double dutyCycle)
        {
            if(buzzerPWMController != null && buzzerPWMChannel != null)
            {
                buzzerPWMController.SetDesiredFrequency(freq);
                buzzerPWMChannel.SetActiveDutyCyclePercentage(dutyCycle);
            }
        }

        public void StopBuzzer()
        {
            if(BuzzerTimer != null) { 
                BuzzerTimer.Dispose();
                BuzzerTimer = null;
            }
            Buzzer = false;
            isBuzzerRinging = false;

        }

        void StartBuzzer()
        {            
            BuzzerTimer = new Timer(BuzzerTimerTick, 0, 0, 500);
            isBuzzerRinging = true;
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

        public void Beep(int ms)
        {
            BuzzerTimer = new Timer(BuzzerTimerTick, 0, ms, ms);
            isBuzzerRinging = true;
        }

        public void PeriodicBeep()
        {
            BuzzerTimer = new Timer(BuzzerTimerTick, 1, 0, 500);
            isBuzzerRinging = true;
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
            InitializeUserPWM();
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

        void InitializeUserPWM()
        {
            userPWMController = GHIElectronics.TinyCLR.Devices.Pwm.PwmController.FromName(SC20260.Timer.Pwm.Controller2.Id);
            userPWMChannel = userPWMController.OpenChannel(SC20260.Timer.Pwm.Controller2.PB3);
            SetUserPWM(1000, 0.5);
            userPWMChannel.Start();
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
         
            pumpTriggerPin = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PF6);       
            pumpTriggerPin.SetDriveMode(GpioPinDriveMode.OutputOpenDrain);            
            pumpTriggerPin.Write(GpioPinValue.High);

            pumpReversePin= GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PF7);
            pumpReversePin.SetDriveMode(GpioPinDriveMode.OutputOpenDrain);
            pumpReversePin.Write(GpioPinValue.High);

            pumpPrimePin = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PF8);
            pumpPrimePin.SetDriveMode(GpioPinDriveMode.OutputOpenDrain);
            pumpPrimePin.Write(GpioPinValue.High);

            motorOnPin = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PA14);
            motorOnPin.SetDriveMode(GpioPinDriveMode.InputPullDown);            

            theDisplay = new SCM26260D_Display();
            Input.Touch.InitializeTouch();            

            InitializeRTC();
            InitializeBuzzer();
            InitializeUserPWM();
            
        }
        
        void InitializeBuzzer()
        {
            buzzerPWMController = GHIElectronics.TinyCLR.Devices.Pwm.PwmController.FromName(SC20260.Timer.Pwm.Controller3.Id);
            buzzerPWMChannel = buzzerPWMController.OpenChannel(SC20260.Timer.Pwm.Controller3.PB1);
            SetBuzzer(500, 0.5);
            Buzzer = false;
        }
        void InitializeUserPWM()
        {           
            userPWMController = GHIElectronics.TinyCLR.Devices.Pwm.PwmController.FromName(SC20260.Timer.Pwm.Controller8.Id);
            userPWMChannel = userPWMController.OpenChannel(SC20260.Timer.Pwm.Controller8.PI6);
            userPWMController.SetDesiredFrequency(2000);
            userPWMChannel.SetActiveDutyCyclePercentage(0.00);
            userPWMChannel.Start();            
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
