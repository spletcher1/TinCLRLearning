using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace PumpControl2023
{
    public class PumpControl
    {
        SPFEZBoard theBoard;
        int speed;
        public PumpControl(SPFEZBoard board)
        {
            theBoard = board;
        }

        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                if (value <= 0) {
                    speed = 0;
                    theBoard.SetUserPWM(2000, 0.0);
                }
                else if (value >= 100) { 
                    speed = 100;
                    theBoard.SetUserPWM(2000, 1.0);
                }
                else
                {
                    speed = value;
                    float tmp = value / 100.0f;
                    theBoard.SetUserPWM(2000, tmp);
                }
                
            }
        }
        
        public bool IsPriming
        {
            get
            {
                if (theBoard.PumpPrimePin.Read() == GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low)                
                    return true;                
                else
                    return false;
            }
        }

        public bool IsDirectionForward
        {
            get
            {
                return (theBoard.PumpReversePin.Read() == GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
            }
        }

        public string CurrentDirection
        {
            get
            {
                if (theBoard.PumpReversePin.Read() == GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low)
                    return "Forward";
                else
                    return "Reverse";
            }
        }       

        public void SetForwardDirection()
        {
            theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
        }

        public void SetReverseDirection()
        {
            theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
        }

        public void TurnPrimeOn()
        {
            theBoard.PumpPrimePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
        }

        public void TurnPrimeOff()
        {
            theBoard.PumpPrimePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
            // After the prime is toggled off, the pump seems to be in the ON state, so turn it off.
            TurnDispenseOn();
            Thread.Sleep(50);
            TurnDispenseOff();
        }

        public void ToggleDirection()
        {
            if(theBoard.PumpReversePin.Read()==GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High)
                theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
            else
                theBoard.PumpReversePin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
        }

        public void TurnDispenseOn()
        {
            theBoard.PumpTriggerPin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
        }

        public void TurnDispenseOff()
        {
            theBoard.PumpTriggerPin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
        }

        public void TriggerDispensePress()
        {
            theBoard.PumpTriggerPin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.Low);
            Thread.Sleep(50);
            theBoard.PumpTriggerPin.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);            
        }
    }
}
