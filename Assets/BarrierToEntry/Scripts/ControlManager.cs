using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;
using BarrierToEntry;

namespace BarrierToEntry
{
    public class ControlManager : MonoBehaviour
    {
        public Controller[] controllers;
        public bool foundControllers = false;
        public bool allowInput = false;
        
        public void Start()
        {
            WiimoteManager.FindWiimotes();
        }
        
        public void Update()
        {

        }
    }
}