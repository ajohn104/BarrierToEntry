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
    public class SystemVisualizer : MonoBehaviour
    {
        #region Public Variables
        [Tooltip("Set of prefabs for each tracking system type.")]
        public BaseVisual[] m_basePrefabs;
        #endregion

        #region Private Variables
        BaseVisual m_base = null;
        #endregion

        #region 3D Visualization
        void Start()
        {
            if (!SixenseCore.Device.Initialized)
            {
                Debug.LogWarning("SixenseCore not initialized.");
                enabled = false;
                return;
            }
        }

        void UpdateVisual()
        {
            SixenseCore.System ourSystem = SixenseCore.Device.SystemType;
          
            if (!SixenseCore.Device.BaseConnected)
            {
                if (m_base != null)
                {
                    Destroy(m_base.gameObject);
                }
                return;
            }

            if (m_base == null)
            {
                int index = -1;
                for (int j = 0; j < m_basePrefabs.Length; j++)
                {
                    if (m_basePrefabs[j] == null)
                        continue;

                    if (m_basePrefabs[j].m_systemType == ourSystem)
                    {
                        index = j;
                        break;
                    }
                }
                if (index == -1)
                    return;

                var go = GameObject.Instantiate(m_basePrefabs[index].gameObject) as GameObject;
                var bas = go.GetComponent<BaseVisual>();

                go.layer = gameObject.layer;
                go.GetComponentInChildren<Renderer>().gameObject.layer = gameObject.layer;
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;

                m_base = bas;
            }
            else if (m_base.m_systemType != ourSystem && ourSystem != System.NONE)
            {
                Debug.LogWarning("Device type mismatch: Expected " + m_base.m_systemType + " Got " + ourSystem);
                Destroy(m_base.gameObject);
                return;
            }
        }

        void Update()
        {
            if (!SixenseCore.Device.Initialized)
            {
                return;
            }

            UpdateVisual();
        }
        #endregion
    }
}
