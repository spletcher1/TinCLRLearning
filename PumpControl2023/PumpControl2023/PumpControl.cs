using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace PumpControl2023
{
    public class PumpControl
    {
        SPFEZBoard theBoard;
        public PumpControl(SPFEZBoard board)
        {
            theBoard = board;
        }

        public void SetSpeed(int value)
        {
            if (value <= 0)
                theBoard.SetUserPWM(2000, 0.0);
            else if (value >= 100)
                theBoard.SetUserPWM(2000, 1.0);
            else
            {
                float tmp = value / 100.0f;
                theBoard.SetUserPWM(2000,tmp);
            }
        }

        public void SetForwardDirection()
        {
            theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
        }

        public void SetReverseDirection()
        {
            theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
        }

        public void TurnPrimeOn()
        {
            theBoard.PumpPrimePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
        }

        public void TurnPrimeOff()
        {
            theBoard.PumpPrimePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
        }

        public void ToggleDirection()
        {
            if(theBoard.PumpReversePin.Read()==GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High)
                theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
            else
                theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
        }

        public void TriggerDispense()
        {
            theBoard.PumpTriggerPin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
            Thread.Sleep(50);
            theBoard.PumpTriggerPin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);            
        }

    }
}
