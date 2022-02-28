﻿namespace Meadow.Foundation.Sensors.Motion
{
    public partial class Mma7660fc
    {
        public enum SensorMode : byte
        {
            Standby = 0,
            Active = 1,
        }

        public enum SampleRate : byte
        {
            _120 = 0,
            _64 = 1,
            _32 =2,
            _16 = 3,
            _8 = 4,
            _4 = 5,
            _2 = 6,
            _1 = 7,
        }

        //check the TILT registe
        public enum Orientation : byte
        {
            Shake = 0b10000000,
            Alert = 0b01000000,
            Tap   = 0b00100000,
            Up,
            Down,
            Right,
            Left,
            Back,
            Front,
        }
    }
}
