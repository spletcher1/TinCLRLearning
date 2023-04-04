using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace PumpControl2023
{
    public class Settings
    {
        bool useSettingNamesOnButtons;
        bool useSettingValuesOnButtons;
        
        DispenseSetting[] theDispenserSettings;


        public DispenseSetting[] TheDispenseSettings
        {
            get { return theDispenserSettings; }
        }

        public Settings()
        {
            // First six are vial, second 6 for Bottles, and last 6 for timer.
            theDispenserSettings = new DispenseSetting[18];
            for(int i=0;i<theDispenserSettings.Length;i++)
                theDispenserSettings[i]=new DispenseSetting();
        }


    }
}
