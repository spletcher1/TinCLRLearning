using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace PumpControl2023
{    
    public class DispenseSetting
    {
        string name;
        int reps;
        int speed;
        float duration;
        float interval;


        public string Name { get { return name; } set { name = value; } }
        public int Reps { get { return reps; } set { reps = value; } }
        public int Speed { get { return speed; } set { speed = value; } }
        public float Duration { get { return duration; } set { duration = value; } }
        public float Interval { get { return interval; } set { interval = value; } }
            
        public DispenseSetting()
        {
            name = "none";
            reps = 10;
            speed = 12;
            duration = 1.5f;
            interval = 2.0f;
        }

        public DispenseSetting(string setting)
        {
            name = "none";
            reps = 10;
            speed = 12;
            duration = 1.5f;
            interval = 2.0f;
            try
            {
                string[] tmp = setting.Split(',');
                if (tmp.Length != 5)
                {
                    Debug.WriteLine("Setting error: " + setting);
                    return;
                }
                name = tmp[0];
                reps = Convert.ToInt32(tmp[1]);
                speed = Convert.ToInt32(tmp[2]);
                duration = Convert.ToInt32(tmp[3]);
                interval = Convert.ToInt32(tmp[4]);
            }
            catch
            {
                return;
            }

        }
        public override string ToString()
        {            
             return name+","+ Reps.ToString("D")+","+ Speed.ToString("D")+","+ Duration.ToString("F1")+","+ Interval.ToString("F1");
        }

    }
}
