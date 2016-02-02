using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;
using BarrierToEntry;

namespace BarrierToEntry
{
    public class ControlManager
    {
        public Controller[] controllers;

        private bool _foundControllers = false;
        public bool foundControllers
        {
            get
            {
                return _foundControllers;
            }
        }
        
        public bool allowInput
        {
            get
            {
                return foundControllers && controllers[0].allowTracking && controllers[1].allowTracking;
            }
        }

        public static bool lockPosition = true;

        private Transform target1;
        private Transform target2;

        public void Start()
        {
            WiimoteManager.FindWiimotes();
        }

        public void assignTargets(Transform target1, Transform target2)
        {
            this.target1 = target1;
            this.target2 = target2;
        }
        
        public void Update()
        {
            if (!WiimoteManager.HasWiimote())
            {
                return;
            } else if (!foundControllers) // Meaning you just gained access to them.
            {
                _foundControllers = true;
                controllers = new Controller[2];
                controllers[0] = new Controller(WiimoteManager.Wiimotes[0], target1, 1);
                controllers[1] = new Controller(WiimoteManager.Wiimotes[1], target2, 2);

                controllers[0].SetLED(4); // 4th led is the rightmost light and therefore the right hand, for now.
                controllers[1].SetLED(1); // 1st led is the leftmost light and therefore the left hand, for now.
            }
            bool readyBefore = allowInput;

            controllers[0].UpdateData();
            controllers[1].UpdateData();

            bool readyAfter = allowInput;

            if (readyBefore != readyAfter)
            {
                controllers[0].ResetData();
                controllers[1].ResetData();
            }
        }
    }
}