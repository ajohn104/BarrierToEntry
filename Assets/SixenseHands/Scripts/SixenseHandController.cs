
//
// Copyright (C) 2013 Sixense Entertainment Inc.
// All Rights Reserved
//

using UnityEngine;
using System.Collections;

public class SixenseHandController : MonoBehaviour
{
	protected Animator			m_animator = null;
	protected float				m_fLastTriggerVal = 0.0f;
    protected SixenseCore.TrackerVisual[] m_trackers;
	
	void Start() 
	{
		// get the Animator
		m_animator = gameObject.GetComponent<Animator>();
        m_trackers = gameObject.GetComponentsInChildren<SixenseCore.TrackerVisual>();
	}
	
	// Updates the animated object from controller input.
	void Update()
	{
        //Debug.Log(m_trackers.Length);
        SixenseCore.TrackerVisual tracker = null;
        foreach(var t in m_trackers)
        {
            //Debug.Log(t.HasInput);
            if (t.HasInput)
            {
                tracker = t;
                break;
            }
        }
        //Debug.Log(tracker);

        if (tracker == null)
            return;

        var controller = tracker.Input;
        var id = tracker.m_trackerBind;

		// Point
        if (id == SixenseCore.TrackerID.CONTROLLER_RIGHT ? controller.GetButton(SixenseCore.Buttons.A) : controller.GetButton(SixenseCore.Buttons.X))
		{
			m_animator.SetBool( "Point", true );
		}
		else
		{
			m_animator.SetBool( "Point", false );
		}
        
        // Grip Ball
        if (id == SixenseCore.TrackerID.CONTROLLER_RIGHT ? controller.GetButton(SixenseCore.Buttons.X) : controller.GetButton(SixenseCore.Buttons.A))
        {
            m_animator.SetBool( "GripBall", true );
        }
        else
        {
            m_animator.SetBool( "GripBall", false );
        }
				
        // Hold Book
        if (id == SixenseCore.TrackerID.CONTROLLER_RIGHT ? controller.GetButton(SixenseCore.Buttons.B) : controller.GetButton(SixenseCore.Buttons.Y))
        {
            m_animator.SetBool( "HoldBook", true );
        }
        else
        {
            m_animator.SetBool( "HoldBook", false );
        }
				
        // Fist
        float fTriggerVal = controller.Trigger;
        fTriggerVal = Mathf.Lerp( m_fLastTriggerVal, fTriggerVal, 0.1f );
        m_fLastTriggerVal = fTriggerVal;
		
        if ( fTriggerVal > 0.01f )
        {
            m_animator.SetBool( "Fist", true );
        }
        else
        {
            m_animator.SetBool( "Fist", false );
        }
		
        m_animator.SetFloat("FistAmount", fTriggerVal);
		
        // Idle
        if ( m_animator.GetBool("Fist") == false &&  
             m_animator.GetBool("HoldBook") == false && 
             m_animator.GetBool("GripBall") == false && 
             m_animator.GetBool("Point") == false )
        {
            m_animator.SetBool("Idle", true);
        }
        else
        {
            m_animator.SetBool("Idle", false);
        }
    }
}

