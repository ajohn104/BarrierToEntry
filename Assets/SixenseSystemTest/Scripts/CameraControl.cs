//
// Copyright (C) 2014 Sixense Entertainment Inc.
// All Rights Reserved
//
// Sixense STEM Test Application
// Version 0.1
//

using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    #region Public Variables
    [Tooltip("List of cameras that the user can cycle through.")]
    public Camera[] m_cameras;

    [Tooltip("List of clip space rectangles [0,1] that each camera will render to in splitscreen mode.")]
    public Rect[] m_viewports;
    #endregion

    #region Private Variables
    int m_selected = 0;
    float m_scale = 0;
    #endregion

    #region GUI
    void OnGUI()
    {
        float scale = Mathf.Max(Screen.height, Screen.width) / 1000.0f;
        int fontsize = Mathf.FloorToInt(13.0f * scale);
        GUI.skin.label.fontSize = fontsize;
        GUI.skin.toggle.fontSize = fontsize;
        GUI.skin.textField.fontSize = fontsize;
        GUI.skin.textArea.fontSize = fontsize;
        GUI.skin.button.fontSize = fontsize;

        float width = Screen.width * 0.3f;
        var rect = new Rect(0, 0, width, Screen.height);
        GUILayout.BeginArea(rect);
        
        GUILayout.Label("Camera Angle");

        for(int i=0; i<m_cameras.Length; i++)
        {
            var c = m_cameras[i];

            c.orthographicSize = Mathf.Lerp(1.1f, 3.0f, m_scale);
            c.fieldOfView = Mathf.Lerp(10, 25, m_scale);

            if (GUILayout.Toggle(m_selected == i, c.name))
                m_selected = i;

            c.gameObject.SetActive(m_selected == i);

            if(m_selected == i)
            {
                c.rect = new Rect(0, 0, 1, 1);
            }
        }

        if (GUILayout.Toggle(m_selected == -1, "Split View"))
            m_selected = -1;

        if(m_selected == -1)
        {
            int w = Screen.width;
            int h = Screen.height;
            for (int i = 0; i < m_cameras.Length; i++)
            {
                var c = m_cameras[i];
                c.gameObject.SetActive(true);

                var r = m_viewports[i];
                c.pixelRect = new Rect(Mathf.Floor(r.xMin * w), Mathf.Floor(r.yMin * h), Mathf.Ceil(r.width * w), Mathf.Ceil(r.height * h));
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("Camera Distance");

        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
        if(rect.Contains(Input.mousePosition))
            m_scale += Input.GetAxis("Mouse ScrollWheel") * -0.25f;

        m_scale = Mathf.Clamp01(m_scale);
        m_scale = GUILayout.VerticalSlider(m_scale, 0, 1);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.EndArea();
    }
    #endregion
}
