using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace PumpControl2023
{    
    public class DispenseSetting
    {
        int reps;
        int speed;
        float duration;
        float interval;


        public int Reps { get { return reps; } set { reps = value; } }
        public int Speed { get { return speed; } set { speed = value; } }
        public float Duration { get { return duration; } set { duration = value; } }
        public float Interval { get { return interval; } set { interval = value; } }
            


        public DispenseSetting()
        {
            reps = 10;
            speed = 12;
            duration = 1.5f;
            interval = 2.0f;
        }

    }
}
