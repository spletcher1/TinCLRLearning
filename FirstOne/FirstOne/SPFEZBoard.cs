using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Drawing;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Devices.Display;


namespace FirstOne
{
    abstract public class SPFEZBoard
    {
        protected GpioPin LED1;
        protected GpioPin LED2;
        protected GpioPin Button1;
        protected GpioPin Button2;
        protected GpioPin Button3;



        public delegate void ButtonPressedHandler();

        public event ButtonPressedHandler Button1Pressed;
        public event ButtonPressedHandler Button2Pressed;
        public event ButtonPressedHandler Button3Pressed;

        public SPFEZBoard()
        {
            LED1 = null;
            LED2 = null;
            Button1 = Button2 = Button3 = null;
        }

        public abstract void SetLed(int led, bool on);
        public abstract void ToggleLed(int led);

        protected void OnButton1Pressed()
        {
            if(Button1 != null)
                Button1Pressed?.Invoke();
        }

        protected void OnButton2Pressed()
        {
            if (Button2 != null)
                Button2Pressed?.Invoke();
        }

        protected void OnButton3Pressed()
        {
            if (Button3 != null)
                Button3Pressed?.Invoke();
        }
    }


    public class SCM20260D : SPFEZBoard
    {
        PwmController LED1BrightnessController;
        PwmChannel LED1BrightnessChannel;
        SCM26260D_Display theDisplay;

        public Graphics Screen
        {
            get { return theDisplay.Screen; }
        }

        public void SetLED1Brightness(double dutyCycle)
        {
            if(dutyCycle > 1) dutyCycle = 1;
            if(dutyCycle < 0) dutyCycle = 0;
            LED1BrightnessChannel.SetActiveDutyCyclePercentage(dutyCycle);

        }

        public override void SetLed(int num, bool on)
        {
            if (num > 2 || num < 0)
                return;
            if(num ==1)
            {
                if(on)
                    LED1.Write(GpioPinValue.Low);
                else
                    LED1.Write(GpioPinValue.High);
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
            if (num > 2 || num < 0)
                return;
            if (num == 1)
            {
                LED1.Toggle();
            }
            else
                LED2.Toggle();  
        }



        public SCM20260D() { 
            //LED1 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PB0);
            //LED1.SetDriveMode(GpioPinDriveMode.Output);
            LED1BrightnessController = PwmController.FromName(SC20260.Timer.Pwm.Controller3.Id);
            LED1BrightnessChannel = LED1BrightnessController.OpenChannel(SC20260.Timer.Pwm.Controller3.PB0);
            LED1BrightnessController.SetDesiredFrequency(10000);
            SetLED1Brightness(0.10);
            LED1BrightnessChannel.Start();

            LED2 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PH11);
            LED2.SetDriveMode(GpioPinDriveMode.Output);
            //SetLed(1, false);
            SetLed(2, true);
            Button1 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PE3);            
            Button1.SetDriveMode(GpioPinDriveMode.InputPullUp);
            Button1.ValueChangedEdge = GpioPinEdge.RisingEdge;
            Button1.ValueChanged += Button1_ValueChanged;
            Button2 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PB7);
            Button2.SetDriveMode(GpioPinDriveMode.Input);
            Button3 = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.SC20260.GpioPin.PD7);
            Button3.SetDriveMode(GpioPinDriveMode.Input);

            theDisplay = new SCM26260D_Display();
        }

        private void Button1_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {           
            base.OnButton1Pressed();
        }
    }
}
