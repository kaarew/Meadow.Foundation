﻿using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Foundation.Sensors.Environmental
{
    public partial class AnalogWaterLevel
        : SensorBase<float>
    {
        //==== internals
        protected IAnalogInputPort AnalogInputPort { get; }
        protected int sampleCount = 5;
        protected int sampleIntervalMs = 40;

        //==== properties
        public Calibration LevelCalibration { get; protected set; }

        public float WaterLevel { get; protected set; }

        /// <summary>
        ///     New instance of the AnalogWaterLevel class.
        /// </summary>
        /// <param name="analogPin">Analog pin the temperature sensor is connected to.</param>
        /// <param name="sensorType">Type of sensor attached to the analog port.</param>
        /// <param name="calibration">Calibration for the analog temperature sensor. Only used if sensorType is set to Custom.</param>
        public AnalogWaterLevel(
            IAnalogInputController device,
            IPin analogPin,
            Calibration? calibration = null,
            int updateIntervalMs = 1000,
            int sampleCount = 5, int sampleIntervalMs = 40)
                : this(device.CreateAnalogInputPort(analogPin), calibration)
        {
            base.UpdateInterval = TimeSpan.FromMilliseconds(updateIntervalMs);
            this.sampleCount = sampleCount;
            this.sampleIntervalMs = sampleIntervalMs;
        }

        public AnalogWaterLevel(IAnalogInputPort analogInputPort,
                                 Calibration? calibration = null)
        {
            AnalogInputPort = analogInputPort;

            //
            //  If the calibration object is null use the defaults for TMP35.
            //
            LevelCalibration = calibration ?? new Calibration();

            // wire up our observable
            AnalogInputPort.Subscribe
            (
                IAnalogInputPort.CreateObserver(
                    h => {
                        // capture the old water leve.
                        var oldWaterLevel = WaterLevel;
                        //var oldWaterLevel = VoltageToWaterLevel(h.Old);

                        // get the new one
                        var newWaterLevel = VoltageToWaterLevel(h.New);
                        WaterLevel = newWaterLevel; // save state

                        base.RaiseEventsAndNotify(
                            new ChangeResult<float>(newWaterLevel, oldWaterLevel)
                        );
                    }
                )
           );
        }
        
        /// <summary>
        /// Convenience method to get the current temperature. For frequent reads, use
        /// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take. 
        /// Must be greater than 0. These samples are automatically averaged.</param>
        /// <param name="sampleIntervalDuration">The time, in milliseconds,
        /// to wait in between samples during a reading.</param>
        /// <returns>A float value that's ann average value of all the samples taken.</returns>
        protected override async Task<float> ReadSensor()
        {
            // read the voltage
            Voltage voltage = await AnalogInputPort.Read();

            // convert and save to our temp property for later retreival
            WaterLevel = VoltageToWaterLevel(voltage);

            // return
            return WaterLevel;
        }

        /// <summary>
        /// Starts continuously sampling the sensor.
        ///
        /// This method also starts raising `Changed` events and IObservable
        /// subscribers getting notified. Use the `readIntervalDuration` parameter
        /// to specify how often events and notifications are raised/sent.
        /// </summary>
        public void StartUpdating()
        {
            AnalogInputPort.StartUpdating();
        }

        /// <summary>
        /// Stops sampling the temperature.
        /// </summary>
        public void StopUpdating()
        {
            AnalogInputPort.StopUpdating();
        }

        /// <summary>
        /// Converts a voltage value to a level in centimeters, based on the current
        /// calibration values.
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        protected float VoltageToWaterLevel(Voltage voltage)
        {
            if(voltage <= LevelCalibration.VoltsAtZero)
            {
                return 0;
            }
            return (float)((voltage.Volts - LevelCalibration.VoltsAtZero.Volts) / LevelCalibration.VoltsPerCentimeter.Volts);
        }
    }
}