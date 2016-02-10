//
// Copyright (C) 2014 Sixense Entertainment Inc.
// All Rights Reserved
//
// SixenseCore Unity Plugin
// Version 0.1
//

using UnityEngine;
using System.Collections.Generic;

namespace SixenseCore
{
    [SelectionBase]
    public class BaseVisual : MonoBehaviour
    {
        #region Public Variables
        [Tooltip("Type of system for this prefab.")]
        public SixenseCore.System m_systemType;

        [Tooltip("Set of prefabs for each tracking device type.")]
        public TrackerVisual[] m_trackerPrefabs;

        [Tooltip("Transform defining the position and orientation of the sensor hardware within the base station.")]
        public Transform m_emitter;
        #endregion

        #region Private Variables
        TrackerVisual[] m_trackers = null;

        Dictionary<SixenseCore.TrackerID, TrackerVisual> m_sceneTrackers = new Dictionary<SixenseCore.TrackerID, TrackerVisual>();
        #endregion

        #region Public Methods
        public bool RegisterTracker(TrackerVisual t)
        {
            foreach(var p in m_trackerPrefabs)
            {
                if(p.m_type == t.m_type)
                {
                    m_sceneTrackers[t.m_trackerBind] = t;
                    t.gameObject.layer = gameObject.layer;
                    t.gameObject.GetComponentInChildren<Renderer>().gameObject.layer = gameObject.layer;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 3D Visualization
        void Start()
        {
            if(!SixenseCore.Device.Initialized)
            {
                Debug.LogWarning("SixenseCore not initialized.");
                enabled = false;
                return;
            }

            m_trackers = new TrackerVisual[SixenseCore.Device.MaxNumberTrackers];
        }

        void UpdateVisual()
        {
            SixenseCore.System ourSystem = SixenseCore.Device.SystemType;
            bool isValid = (SixenseCore.Device.BaseConnected && SixenseCore.Device.SystemType == m_systemType);

            for (int i = 0; i < m_trackers.Length; i++)
            {
                var c = Device.GetTrackerByIndex(i);
                if (!c.Enabled || !isValid)
                {
                    if (m_trackers[i] != null)
                    {
                        Destroy(m_trackers[i].gameObject);
                    }
                    continue;
                }

                if (m_trackers[i] == null && isValid)
                {
                    int index = -1;
                    for(int j=0; j<m_trackerPrefabs.Length;j++)
                    {
                        if (m_trackerPrefabs[j] == null)
                            continue;

                        if(m_trackerPrefabs[j].m_type == c.HardwareType)
                        {
                            index = j;
                            break;
                        }
                    }
                    if (index == -1)
                        continue;

                    var go = GameObject.Instantiate(m_trackerPrefabs[index].gameObject) as GameObject;
                    var con = go.GetComponent<TrackerVisual>();

                    go.layer = gameObject.layer;
                    go.GetComponentInChildren<Renderer>().gameObject.layer = gameObject.layer;
                    go.transform.parent = transform;
                    m_trackers[i] = con;

                    con.Input = c;
                }
                else if(!isValid)
                {
                    continue;
                }
                else if (m_trackers[i].m_type != c.HardwareType && c.HardwareType != Hardware.NONE)
                {
                    Debug.LogWarning("Device type mismatch: " + i + " Expected " + m_trackers[i].m_type + " Got " + c.HardwareType);
                    Destroy(m_trackers[i].gameObject);
                    continue;
                }

                {
                    var sensor = m_trackers[i].m_sensor;
                    var rot = Quaternion.Inverse(sensor.rotation) * (m_trackers[i].transform.rotation);

                    m_trackers[i].transform.rotation = m_emitter.rotation * c.Rotation * rot;

                    var pos = sensor.position - m_trackers[i].transform.position;

                    m_trackers[i].transform.position = m_emitter.TransformPoint(c.Position) - pos;
                }

                if(m_sceneTrackers.ContainsKey(c.ID))
                {
                    var t = m_sceneTrackers[c.ID];

                    if (t != null)
                    {
                        var root = t.transform;
                        if (root.parent != null)
                            root = root.parent;

                        var sensor = t.m_sensor;
                        var rot = Quaternion.Inverse(sensor.rotation) * (root.rotation);

                        root.rotation = m_emitter.rotation * c.Rotation * rot;

                        var pos = sensor.position - root.position;

                        root.position = m_emitter.TransformPoint(c.Position) - pos;

                        t.Input = c;
                    }
                    else
                    {
                        m_sceneTrackers.Remove(c.ID);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (!SixenseCore.Device.Initialized)
            {
                return;
            }

            UpdateVisual();
        }
        void Update()
        {
            if (!SixenseCore.Device.Initialized)
            {
                return;
            }

            for (int i = 0; i < m_trackers.Length; i++)
            {
                if (m_trackers[i] == null)
                    continue;

                var c = Device.GetTrackerByIndex(i);
                m_trackers[i].gameObject.SetActive(c.Enabled);

                if (!c.Enabled)
                    continue;
            }

            UpdateVisual();
        }
        #endregion
    }
}
