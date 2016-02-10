//
// Copyright (C) 2014 Sixense Entertainment Inc.
// All Rights Reserved
//
// SixenseCore Unity Plugin
// Version 0.1
//

using UnityEngine;
using System.Collections;

namespace SixenseCore
{
    [SelectionBase]
    public class TrackerVisual : MonoBehaviour
    {
        #region Public Variables
        [Tooltip("Specifies the hardware identifying type for this tracking device.")]
        public SixenseCore.Hardware m_type;

        [Tooltip("3D Model for this tracked device.")]
        public MeshRenderer m_model;

        [Tooltip("Transform that defines the position and orientation of the sensor hardware within the tracked device.")]
        public Transform m_sensor;

        [Tooltip("If this is set, any object this is parented to will move with the specified tracking device.")]
        public SixenseCore.TrackerID m_trackerBind = TrackerID.NONE;
        #endregion

        #region Private Variables
        static Tracker ms_nullController = new Tracker(null, -1);

        Tracker m_controller = null;

        BaseVisual m_base = null;
        #endregion

        #region Properties
        
        public Tracker Input
        {
            get
            {
                if (m_controller != null)
                    return m_controller;

                return ms_nullController;
            }
            set
            {
                m_controller = value;
            }
        }

        public bool HasInput
        {
            get { return m_controller != null && m_controller.Connected; }
        }
        #endregion

        void Update()
        {
            if (m_trackerBind == TrackerID.NONE)
            {
                enabled = false;
                return;
            }

            if (m_base != null)
                return;

            var bases = FindObjectsOfType<BaseVisual>();

            foreach(var b in bases)
            {
                if(b.RegisterTracker(this))
                {
                    m_base = b;
                    break;
                }
            }

            if(m_base == null)
            {
                m_model.enabled = false;
                m_controller = null;
            }
            else
            {
                m_model.enabled = true;
            }
        }
    }
}
