using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;
using BarrierToEntry;

namespace BarrierToEntry
{
    public class Controls : MonoBehaviour
    {
        ControlManager manager = new ControlManager();
        Controller primaryController;
        Controller secondaryController;

        public GameObject saber;
        public GameObject offhand;

        readonly Matrix4x4 scale = Matrix4x4.Scale(new Vector3(-10, -10, -2));

        private Vector3 saberOffset = new Vector3(0.5395362f, -0.5596324f, 0.1693648f);

        void Start()
        {
            manager.Start();
        }

        void Update()
        {
            manager.Update();
            if(manager.foundControllers)
            {
                primaryController   = primaryController   ?? manager.controllers[0];
                secondaryController = secondaryController ?? manager.controllers[1];

                saber.transform.localPosition = scale.MultiplyVector(primaryController.position) + saberOffset;

            }
        }
    }
}