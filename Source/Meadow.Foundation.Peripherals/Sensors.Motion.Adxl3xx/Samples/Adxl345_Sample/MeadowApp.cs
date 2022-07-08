﻿using System;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Motion;

namespace Sensors.Motion.Adxl345_Sample
{
    public class MeadowApp : App<F7FeatherV2>
    {
        //<!=SNIP=>

        Adxl345 sensor;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            sensor = new Adxl345(Device.CreateI2cBus());
            sensor.SetPowerState(false, false, true, false, Adxl345.Frequencies.TwoHz);

            // classical .NET events can also be used:
            sensor.Updated += (sender, result) =>
            {
                Console.WriteLine($"Accel: [X:{result.New.X.MetersPerSecondSquared:N2}," +
                    $"Y:{result.New.Y.MetersPerSecondSquared:N2}," +
                    $"Z:{result.New.Z.MetersPerSecondSquared:N2} (m/s^2)]");
            };

            return Task.CompletedTask;
        }

        public async override Task Run()
        {
            var result = await sensor.Read();
            Console.WriteLine("Initial Readings:");
            Console.WriteLine($"Accel: [X:{result.X.MetersPerSecondSquared:N2}," +
                $"Y:{result.Y.MetersPerSecondSquared:N2}," +
                $"Z:{result.Z.MetersPerSecondSquared:N2} (m/s^2)]");

            sensor.StartUpdating(TimeSpan.FromMilliseconds(500));
        }
    }
}