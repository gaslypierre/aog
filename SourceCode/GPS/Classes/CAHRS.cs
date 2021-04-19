﻿
namespace AgOpenGPS
{
    public class CAHRS
    {
        //private readonly FormGPS mf;

        //Roll and heading from the IMU
        public double imuHeading = 99999, prevIMUHeading = 0, imuRoll = 88888;

        //actual value in degrees
        public double rollZero;

        //Roll Filter Value
        public double rollFilter;

        //is the auto steer in auto turn on mode or not
        public bool isAutoSteerAuto, isRollInvert;

        //the factor for fusion of GPS and IMU
        public double fusionWeight;

        //constructor
        public CAHRS()
        {
            //mf = _f;

            rollZero = Properties.Settings.Default.setIMU_rollZero;
            //pitchZeroX16 = Properties.Settings.Default.setIMU_pitchZeroX16;

            rollFilter = Properties.Settings.Default.setIMU_rollFilter;

            isAutoSteerAuto = Properties.Settings.Default.setAS_isAutoSteerAutoOn;

            fusionWeight = Properties.Settings.Default.setIMU_fusionWeight;
            isRollInvert = Properties.Settings.Default.setIMU_invertRoll;

        }
    }
}

