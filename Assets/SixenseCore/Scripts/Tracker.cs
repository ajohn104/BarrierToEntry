//
// Copyright (C) 2014 Sixense Entertainment Inc.
// All Rights Reserved
//
// SixenseCore Unity Plugin
// Version 0.1
//

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace SixenseCore
{
    /// <summary>
    /// Tracker objects provide access to Sixense tracking and hardware data.
    /// </summary>
    public class Tracker
    {
        #region Properties
        /// <summary>
        /// The tracker enabled state.
        /// </summary>
        public bool Connected { get { return m_Connected; } }

        /// <summary>
        /// The tracker enabled state.
        /// </summary>
        public bool Enabled {  get { /*Debug.Log(m_Enabled); */return m_Connected && m_Enabled; } }

        /// <summary>
        /// Hardware model identifier
        /// </summary>
        public Hardware HardwareType { get { return m_HardwareType; } }

        /// <summary>
        /// Dock the tracker bound to, which could be UNKNOWN.
        /// Can change if the tracker is replaced in a different dock.
        /// </summary>
        public TrackerID ID { get { return m_TrackerID; } }

        /// <summary>
        /// Constant index of the tracking device, determined by wireless channel assignment
        /// </summary>
        public int DeviceIndex { get { return m_Index; } }

        /// <summary>
        /// The tracker docked state.
        /// </summary>
        public bool Docked { get { return m_Docked; } }

        /// <summary>
        /// Value of trigger from released (0.0) to pressed (1.0).
        /// </summary>
        public float Trigger { get { return m_Trigger; } }

        /// <summary>
        /// Value of joystick X axis from left (-1.0) to right (1.0).
        /// </summary>
        public float JoystickX { get { return m_JoystickX; } }

        /// <summary>
        /// Value of joystick Y axis from bottom (-1.0) to top (1.0).
        /// </summary>
        public float JoystickY { get { return m_JoystickY; } }

        /// <summary>
        /// The tracker position in Unity coordinates.
        /// </summary>
        public Vector3 Position { get { return m_Position; } }

        /// <summary>
        /// The tracker rotation in Unity coordinates.
        /// </summary>
        public Quaternion Rotation { get { return m_Rotation; } }

        /// <summary>
        /// Local gravity vector measured by IMU
        /// </summary>
        public Vector3 LocalGravity { get { return m_Gravity; } }

        /// <summary>
        /// Hardware version number
        /// </summary>
        public string HardwareVersion { get { return m_Hardware; } }

        /// <summary>
        /// Firmware revision number
        /// </summary>
        public string FirmwareVersion { get { return m_Firmware; } }

        /// <summary>
        /// Firmware revision number
        /// </summary>
        public string RuntimeVersion { get { return m_Runtime; } }

        /// <summary>
        /// EM Frequency ID used by this tracker
        /// </summary>
        public int SerialNumber { get { return m_SerialNumber; } }

        /// <summary>
        /// EM Frequency ID used by this tracker
        /// </summary>
        public int MagneticFrequency { get { return m_MagneticFrequency; } }

        /// <summary>
        /// DSP Gain mode
        /// </summary>
        public int Gain { get { return m_gain; } }

        /// <summary>
        /// If false, position may be mirrored, and tracker may need to be docked
        /// </summary>
        public bool HemiTrackingEnabled { get { return m_HemiTrackingEnabled; } }

        /// <summary>
        /// Battery life percentage
        /// </summary>
        public int BatteryPercentage { get { return m_batteryPercentage; } }

        /// <summary>
        /// Battery voltage level
        /// </summary>
        public float BatteryVoltage { get { return m_batteryVoltage; } }

        /// <summary>
        /// True if low battery warning should be displayed
        /// </summary>
        public bool BatteryLow { get { return m_batteryLow; } }

        /// <summary>
        /// Is this tracker's battery currently charging
        /// </summary>
        public bool Charging { get { return m_charging; } }

        /// <summary>
        /// Is this tracker currently receiving power
        /// </summary>
        public bool ExternalPower { get { return m_powered; } }

        /// <summary>
        /// Is this tracker communicating with the host over USB
        /// </summary>
        public bool WiredConnection { get { return m_wired; } }

        /// <summary>
        /// 8 bit sequential index for incoming sensor packets
        /// </summary>
        public int SequenceNumber { get { return m_SequenceNumber; } }
        #endregion

        #region Filtering
        /// <summary>
        /// Enable smoothing of the position and orientation based on distance to reduce noise, on by default
        /// </summary>
        public bool FilterJitterEnabled
        {
            get
            {
                if (!m_Enabled || m_Index < 0)
                    return false;

                int e = 0;
                Plugin.sxCoreGetDeviceFilterEnabled((uint)Filter.JITTER, (uint)m_Index, out e);
                return e != 0;
            }
            set
            {
                if (!m_Enabled || m_Index < 0)
                    return;

                Plugin.sxCoreSetDeviceFilterEnabled((uint)Filter.JITTER, (uint)m_Index, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Enable smoothing of the position and orientation based on distance to reduce noise, on by default
        /// </summary>
        public bool FilterMovingAverageEnabled
        {
            get
            {
                if (!m_Enabled || m_Index < 0)
                    return false;

                int e = 0;
                Plugin.sxCoreGetDeviceFilterEnabled((uint)Filter.MOVING_AVERAGE, (uint)m_Index, out e);
                return e != 0;
            }
            set
            {
                if (!m_Enabled || m_Index < 0)
                    return;

                Plugin.sxCoreSetDeviceFilterEnabled((uint)Filter.MOVING_AVERAGE, (uint)m_Index, value ? 1 : 0);
            }
        }

        /// <summary>
        /// The parameters of the noise filter
        /// </summary>
        public JitterFilter FilterJitterSettings
        {
            get
            {
                if (!m_Enabled || m_Index < 0)
                    return new JitterFilter();

                JitterFilter p;
                Plugin.sxCoreGetDeviceFilterParams((uint)Filter.JITTER, (uint)m_Index, out p);
                return p;
            }
            set
            {
                if (!m_Enabled || m_Index < 0)
                    return;

                Plugin.sxCoreSetDeviceFilterParams((uint)Filter.JITTER, (uint)m_Index, ref value);
            }
        }

        /// <summary>
        /// The parameters of the noise filter
        /// </summary>
        public MovingAverageFilter FilterMovingAverageSettings
        {
            get
            {
                if (!m_Enabled || m_Index < 0)
                    return new MovingAverageFilter();

                MovingAverageFilter p;
                Plugin.sxCoreGetDeviceFilterParams((uint)Filter.MOVING_AVERAGE, (uint)m_Index, out p);
                return p;
            }
            set
            {
                if (!m_Enabled || m_Index < 0)
                    return;

                Plugin.sxCoreSetDeviceFilterParams((uint)Filter.MOVING_AVERAGE, (uint)m_Index, ref value);
            }
        }

        /// <summary>
        /// Uses an accelerometer and gyroscope to correct for magnetic distortion in the environment
        /// </summary>
        public bool DistortionCorrectionEnabled
        {
            get
            {
                if (!m_Enabled || m_Index < 0)
                    return false;

                int e = 0;
                Plugin.sxCoreGetDeviceFilterEnabled((uint)Filter.IMU_DISTORTION_CORRECTION, (uint)m_Index, out e);
                return e != 0;
            }
            set
            {
                if (!m_Enabled || m_Index < 0)
                    return;

                Plugin.sxCoreSetDeviceFilterEnabled((uint)Filter.IMU_DISTORTION_CORRECTION, (uint)m_Index, value ? 1 : 0);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns true if the button parameter is being pressed.
        /// </summary>
        public bool GetButton(Buttons button)
        {
            return ((button & m_Buttons) != 0);
        }

        /// <summary>
        /// Returns true if the button parameter was pressed this frame.
        /// </summary>
        public bool GetButtonDown(Buttons button)
        {
            return ((button & m_Buttons) != 0) && ((button & m_ButtonsPrevious) == 0);
        }

        /// <summary>
        /// Returns true if the button parameter was released this frame.
        /// </summary>
        public bool GetButtonUp(Buttons button)
        {
            return ((button & m_Buttons) == 0) && ((button & m_ButtonsPrevious) != 0);
        }

        /// <summary>
        /// Returns true if any button is being pressed.
        /// </summary>
        public bool GetAnyButton()
        {
            return (m_Buttons != 0);
        }

        /// <summary>
        /// Returns true if any button was pressed this frame.
        /// </summary>
        public bool GetAnyButtonDown()
        {
            return (m_Buttons != 0) && (m_ButtonsPrevious == 0);
        }

        /// <summary>
        /// Returns true if any button was released this frame.
        /// </summary>
        public bool GetAnyButtonUp()
        {
            return (m_Buttons == 0) && (m_ButtonsPrevious != 0);
        }

        /// <summary>
        /// Turns on the vibration motor for a given number of milliseconds with a magnitude between 0 to 1
        /// </summary>
        public void Vibrate(int milliseconds, float magnitude)
        {
            if (m_Enabled && !m_Docked && m_Index >= 0)
            {
                m_vibrateTime = milliseconds * 0.001f;

                Plugin.sxCoreSetVibration((uint)m_Index, 0, magnitude);
            }
        }

        /// <summary>
        /// Directly sets the speed of the vibration motor in a range of 0 to 1
        /// </summary>
        public void SetVibration(float magnitude)
        {
            if (m_Enabled && !m_Docked && m_Index >= 0)
            {
                Plugin.sxCoreSetVibration((uint)m_Index, 0, magnitude);
            }
        }

        public float GetVibration() {
            float mag;
            if (Plugin.sxCoreGetVibration((uint)m_Index, 0, out mag) == PluginTypes.Result.SUCCESS)
            {
                return mag;
            }
            else {
                Debug.Log("Get failure");
                return 0f;
            }
        }
        /// <summary>
        /// Get device info
        /// </summary>
        public void UpdateInfo()
        {
            m_hasInfo = false;

            PluginTypes.TrackedDeviceInfo i;
            if (Plugin.sxCoreGetInfo((uint)m_Index, out i) == PluginTypes.Result.SUCCESS)
            {
                ProcessInfo(i);
            }
        }

        /// <summary>
        /// Turns this tracker off
        /// </summary>
        public void PowerOff()
        {
            if (m_Index >= 0)
            {
                Debug.Log("here");
                m_Enabled = false;
                Plugin.sxCorePowerOffDevice((uint)m_Index);
            }
        }

        /// <summary>
        /// Syncs and sets hemisphere tracking
        /// </summary>
        public void Sync()
        {
            if (m_Index >= 0)
            {
                Plugin.sxCoreSyncDevice((uint)m_Index);
            }
        }

        /// <summary>
        /// Syncs and sets hemisphere tracking
        /// </summary>
        public void AutoEnableHemisphereTracking()
        {
            if (m_Index >= 0)
            {
                Plugin.sxCoreAutoEnableHemisphereTracking((uint)m_Index);
            }
        }
        #endregion

        #region Input Processing
        private bool GetFlag(uint mask, Status s)
        {
            return ((Status)mask & s) == s;
        }

        internal void ProcessInfo(PluginTypes.TrackedDeviceInfo info)
        {
            const byte gpio_id_mask = 0x3;
            const byte gpio_rev_mask = 0xC;

            m_SerialNumber = info.serial_number;
            m_HardwareType = (Hardware)info.device_type;
            // m_Index = info.tracked_device_index; // Redundant, and screws up Hydra
            
            byte bid = (byte)(info.board_hardware_gpio & gpio_id_mask);
            byte brev = (byte)(info.board_hardware_gpio & gpio_rev_mask);

            m_Hardware = info.hardware_revision.ToString("X2") + "-" + bid.ToString("X") + "r" + brev.ToString("X");
            m_Firmware = info.mcu_firmware_version_major.ToString("X2") + "." + info.mcu_firmware_version_minor.ToString("X2") + "-" +
                            info.dsp_firmware_version_major.ToString("X2") + "." + info.dsp_firmware_version_minor.ToString("X2");
            m_Runtime = info.runtime_version_major + "." + info.runtime_version_minor.ToString("00");

            m_MagneticFrequency = info.magnetic_frequency;
            m_batteryVoltage = info.battery_voltage;

            m_hasInfo = (m_HardwareType != Hardware.NONE);
            
            if (m_hasInfo)
                m_Connected = true;
        }

        internal void ProcessData(PluginTypes.TrackedDeviceData data)
        {
            float scale = 0.001f;
            if (m_device != null)
                scale = 1.0f / m_device.m_worldUnitScaleInMillimeters;
            
            if (data.tracked_device_index != m_Index)
            {
                //Debug.Log("yo: " + m_Connected);
                m_hasInfo = false;
                m_Connected = false;
            }
            
            // m_Index = data.tracked_device_index;
            m_SequenceNumber = data.sequence_number;
            m_Time = data.packet_time;
            m_TrackerID = (m_Index == 0)?TrackerID.CONTROLLER_LEFT:TrackerID.CONTROLLER_RIGHT;//(TrackerID)data.tracker_id; // ***altered***
            m_batteryPercentage = data.battery_percent;
            m_gain = data.dsp_gain;

            
            m_Enabled = true;//GetFlag(data.status, Status.ENABLED); // ***altered***
            m_Docked = GetFlag(data.status, Status.DOCKED);
            m_powered = GetFlag(data.status, Status.HAS_EXTERNAL_POWER);
            m_charging = GetFlag(data.status, Status.BATTERY_CHARGING);
            m_batteryLow = GetFlag(data.status, Status.BATTERY_LOW);
            m_wired = GetFlag(data.status, Status.MODE_WIRED);
            m_HemiTrackingEnabled = GetFlag(data.status, Status.FILTER_HEMI_TRACKING_ENABLED);

            m_Buttons = (Buttons)data.buttons;
            m_Trigger = data.trigger;
            m_JoystickX = data.joystick_x;
            m_JoystickY = data.joystick_y;
            //Debug.Log("yo: " + data.tracker_pos[0]);
            m_Position.Set(data.tracker_pos[0] * scale, data.tracker_pos[1] * scale, -data.tracker_pos[2] * scale);
            m_Rotation.Set(data.tracker_rot_quat[0], data.tracker_rot_quat[1], -data.tracker_rot_quat[2], -data.tracker_rot_quat[3]);

            if (data.imu_gravity[0] != 0 && data.imu_gravity[1] != 0 && data.imu_gravity[2] != 0)
                m_Gravity.Set(data.imu_gravity[0], data.imu_gravity[1], -data.imu_gravity[2]);

            //Debug.Log("yo: " + m_Connected);
            if (!m_hasInfo)
            {
                //Debug.Log("updating");
                UpdateInfo();
            }
        }

        /// <summary>
        /// Update the tracking data once per physics update at 240hz, called by SixenseCore.Device
        /// </summary>
        public void FixedUpdate()
        {
            //Debug.Log("trying to process");
            if (m_Index < 0)
                return;

            if (m_firstUpdate)
            {
                PreUpdate();

                int firstSequence = -1;

                for (uint h = 0; h < Device.HistorySize; h++)
                {
                    Profiler.BeginSample("Sixense Driver");
                    {
                        if (Plugin.sxCoreGetDataPrevious((uint)m_Index, h, out m_pastData[h]) == PluginTypes.Result.FAILURE)
                        {
                            Profiler.EndSample();

                            break;
                        }
                    }
                    Profiler.EndSample();

                    if (h == 0)
                        firstSequence = m_pastData[h].sequence_number;

                    if (m_pastData[h].sequence_number == m_lastSequence)
                        break;

                    m_pastDataCursor = (int)h;
                }

                m_lastSequence = firstSequence;

                m_firstUpdate = false;
            }

            if (m_pastDataCursor == -1)
            {
                return;
            }

            var data = m_pastData[m_pastDataCursor--];
            //Debug.Log("about to process");
            ProcessData(data);
        }

        /// <summary>
        /// Reset the button state prior to update, called by SixenseCore.Device
        /// </summary>
        public void PreUpdate()
        {
            m_ButtonsPrevious = m_Buttons;

            if (m_Index < 0)
                return;
        }

        /// <summary>
        /// Handles the vibration timer
        /// </summary>
        public void UpdateVibration()
        {
            if (m_vibrateTime > 0)
            {
                m_vibrateTime -= Time.deltaTime;

                if (m_vibrateTime <= 0)
                {
                    Plugin.sxCoreSetVibration((uint)m_Index, 0, 0);
                }
            }
        }

        /// <summary>
        /// Update the tracking data once per frame, called by SixenseCore.Device
        /// </summary>
        public void Update()
        {
            m_firstUpdate = true;

            if (m_Index < 0)
                return;

            UpdateVibration();
            
            m_Connected = (Plugin.sxCoreIsTrackedDeviceConnected((uint)m_Index) != 0);
            //Debug.Log("yo: " + m_Connected + ", index: " + m_Index);
            if (!m_Connected)
            {
                m_hasInfo = false;
                return;
            }

            PluginTypes.TrackedDeviceData d;
            Profiler.BeginSample("Sixense Driver");
            {
                if (Plugin.sxCoreGetData((uint)m_Index, out d) == PluginTypes.Result.SUCCESS)
                    ProcessData(d);
            }
            Profiler.EndSample();
        }
        #endregion

        #region Private Variables
        Device m_device;

        private int m_Index = -1;
        private int m_SequenceNumber = -1;
        private ulong m_Time = 0;
        private bool m_Connected = false;
        private bool m_Enabled = false;
        private bool m_Docked = false;
        private TrackerID m_TrackerID = TrackerID.NONE;
        private Buttons m_Buttons = 0;
        private Buttons m_ButtonsPrevious = 0;
        private float m_Trigger = 0;
        private float m_JoystickX = 0;
        private float m_JoystickY = 0;
        private Vector3 m_Position = Vector3.zero;
        private Quaternion m_Rotation = Quaternion.identity;
        private Vector3 m_Gravity = Vector3.zero;
        private string m_Hardware = "0";
        private string m_Firmware = "0";
        private string m_Runtime = "0";
        private Hardware m_HardwareType = Hardware.NONE;
        private int m_SerialNumber = 0;
        private int m_MagneticFrequency = 0;
        private bool m_HemiTrackingEnabled = false;
        private int m_batteryPercentage = 0;
        private float m_batteryVoltage = 0;
        private bool m_batteryLow = false;
        private bool m_charging = false;
        private bool m_powered = false;
        private bool m_wired = false;
        private int m_gain = 0;
        int m_lastSequence = -1;
        bool m_firstUpdate = true;
        bool m_hasInfo = false;
        PluginTypes.TrackedDeviceData[] m_pastData = null;
        int m_pastDataCursor = -1;
        float m_vibrateTime = 0;
        #endregion

        #region Construction
        /// <summary>
        /// Called by SixenseCore.Device
        /// </summary>
        internal Tracker(Device device, int index)
        {
            m_device = device;
            m_Index = index;

            m_pastData = new PluginTypes.TrackedDeviceData[Device.HistorySize];
        }
        #endregion

        #region Native Interface
        private partial class Plugin
        {
            const string module = "sxCore";

            [DllImport(module)]
            public static extern int sxCoreIsTrackedDeviceEnabled(uint tracked_device_index);          // connected and ready to use? (based on status)
            [DllImport(module)]
            public static extern int sxCoreIsTrackedDeviceConnected(uint tracked_device_index);          // connected

            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetInfo(uint tracked_device_index, out PluginTypes.TrackedDeviceInfo info);

            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetData(uint tracked_device_index, out PluginTypes.TrackedDeviceData data); // get packet data, this should be fast, and locking should only be on accessing the packet queue, not on device reading/writing/open/close
            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetDataPrevious(uint tracked_device_index, uint history_index_back, out PluginTypes.TrackedDeviceData data);
            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetDataAtTime(uint tracked_device_index, UInt64 time, out PluginTypes.TrackedDeviceData data);	// returns an interpolated position and orientation at a specified time in the past

            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetVibration(uint tracked_device_index, uint motor_index, out float magnitude); // get device rumble vibration magnitude, per motor
            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreSetVibration(uint tracked_device_index, uint motor_index, float magnitude); // set device rumble vibration, per motor, to a set magnitude 0.0-1.0

            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreSetDeviceFilterEnabled(uint filter_type, uint tracked_device_index, int on_or_off);      // enable/disable application controllable filters
            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetDeviceFilterEnabled(uint filter_type, uint tracked_device_index, out int on_or_off);     // enable/disable application controllable filters

            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetDeviceFilterParams(uint filter_type, uint tracked_device_index, out MovingAverageFilter filter_params);     // get application controllable filter parameters, "filter_params" can be NULL for filters that don't use it
            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreSetDeviceFilterParams(uint filter_type, uint tracked_device_index, ref MovingAverageFilter filter_params);     // set application controllable filter parameters, "filter_params" can be NULL for filters that don't use it

            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreGetDeviceFilterParams(uint filter_type, uint tracked_device_index, out JitterFilter filter_params);     // get application controllable filter parameters, "filter_params" can be NULL for filters that don't use it
            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreSetDeviceFilterParams(uint filter_type, uint tracked_device_index, ref JitterFilter filter_params);     // set application controllable filter parameters, "filter_params" can be NULL for filters that don't use it
            
            [DllImport(module)]
            public static extern PluginTypes.Result sxCorePowerOffDevice(uint tracked_device_index);                  // turn off the device

            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreSyncDevice(uint tracked_device_index);                      // sync the device, sends sync command
            [DllImport(module)]
            public static extern PluginTypes.Result sxCoreAutoEnableHemisphereTracking(uint tracked_device_index);    // reset hemisphere tracking
        }
        #endregion
    }
}