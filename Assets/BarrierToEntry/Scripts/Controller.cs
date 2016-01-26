using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;
using BarrierToEntry;

namespace BarrierToEntry
{
    public class Controller : MonoBehaviour
    {
        private Wiimote wiimote;
        private Vector3 _pos = Vector3.zero;
        public Vector3 position
        {
            get
            {
                return new Vector3(_pos.x, _pos.y, _pos.z);
            }
            set
            {
                _pos = value;
            }
        }

        private Quaternion _rot = Quaternion.Euler(Vector3.zero);
        public Vector3 rotation
        {
            get
            {
                return _rot.eulerAngles;
            }
            set
            {
                _rot = Quaternion.Euler(value);
            }
        }

        private Vector3 velocity = Vector3.zero;

        void Start()
        {

        }
        
        void Update()
        {

        }
    }
}