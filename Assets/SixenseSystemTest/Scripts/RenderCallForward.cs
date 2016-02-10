//
// Copyright (C) 2014 Sixense Entertainment Inc.
// All Rights Reserved
//
// Sixense STEM Test Application
// Version 0.1
//

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class RenderCallForward : MonoBehaviour
{
    #region Public Members
    [Tooltip("The OnPostRender event will be forwarded from this camera to the specified GameObject.")]
    public GameObject m_call;
    #endregion

    #region Script Events
    void OnPostRender()
    {
        m_call.SendMessage("OnPostRender");
    }
    #endregion
}
