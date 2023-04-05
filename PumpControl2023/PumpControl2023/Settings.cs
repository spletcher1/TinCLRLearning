using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.IO.TinyFileSystem;

namespace PumpControl2023
{
    public enum SettingsIndex
    {
        Vial1=0,
        Vial2=1,   
        Vial3=2,
        Vial4=3,
        Vial5=4,
        Bottle1=5,
        Bottle2=6,
        Bottle3=7,
        Bottle4=8,
        Bottle5=9,
        Timer1=10,
        Timer2=11,
        Timer3=12,
        Timer4=13,
        Timer5=14
    }

    public class Settings
    {
        bool useSettingNamesOnButtons;
        bool useSettingValuesOnButtons;
        
        DispenseSetting[] theDispenserSettings;
        MyFileSystem theFileSystem;

        public bool UseSettingNamesOnButtons
        {
            get { return useSettingNamesOnButtons; }
        }

        public string GetButtonName(int button)
        {
            if (button < 0 || button >= theDispenserSettings.Length)
                return "NA";
            if (button < (int)(SettingsIndex.Timer1))
            {
                if (useSettingValuesOnButtons)
                {
                    return theDispenserSettings[button].VB_ButtonString;
                }
                else if (UseSettingNamesOnButtons)
                {
                    return theDispenserSettings[button].Name;
                }
                else
                {
                    return theDispenserSettings[button].Name;
                }
            }
            else
            {
                return theDispenserSettings[(int)button].T_ButtonString;
            }
        }

        public string GetButtonName(SettingsIndex i)
        {
            return GetButtonName((int)(i));
        }

        public bool UseSettingValuesOnButtons
        {
            get { return useSettingValuesOnButtons; }
        }
        public DispenseSetting[] TheDispenseSettings
        {
            get { return theDispenserSettings; }
        }

        public Settings()
        {
            theFileSystem = new MyFileSystem();
            // First six are vial, second 6 for Bottles, and last 6 for timer.
            theDispenserSettings = new DispenseSetting[15];
            for(int i=0;i<theDispenserSettings.Length;i++)
                theDispenserSettings[i]=new DispenseSetting();

            useSettingNamesOnButtons = false;
            useSettingValuesOnButtons = true;
        }

        public void SaveSettings()
        {
            theFileSystem.SaveSettings(this);
        }

        public void LoadSettings()
        {
            Settings tmp = theFileSystem.LoadSettings();
            for(int i = 0; i < theDispenserSettings.Length; i++)
            {
                theDispenserSettings[i] = tmp.theDispenserSettings[i];
            }

            useSettingNamesOnButtons = tmp.UseSettingNamesOnButtons;
            useSettingValuesOnButtons = tmp.UseSettingValuesOnButtons;
        }

    }
}
