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
        private ControlManager manager = new ControlManager();
        private Controller primaryController;
        private Controller secondaryController;

        public GameObject saber;
        GameObject leftObj
        {
            get
            {
                return usingHand ? hand : altHand;
            }
        }

        public GameObject hand;
        public GameObject altHand;

        bool usingHand = true;

        private readonly Matrix4x4 scale = Matrix4x4.Scale(new Vector3(-10f, -10f, -2f));

        private readonly Vector3 saberOffset = new Vector3(0.5395362f, -0.5596324f, 0.1693648f);
        private readonly Vector3 handOffset = new Vector3(-0.5395362f, -0.381f, 0.1693648f);

        private Vector3 primaryOffset
        {
            get
            {
                return primaryUseSaber ? saberOffset : new Vector3(saberOffset.x, handOffset.y, saberOffset.z);
            }
        }

        private Vector3 secondaryOffset
        {
            get
            {
                return secondaryUseSaber ? new Vector3(handOffset.x, saberOffset.y, handOffset.z) : handOffset;
            }
        }

        private bool primaryUseSaber = true;
        private bool secondaryUseSaber = false;

        public Transform target1;
        public Transform target2;

        void Start()
        {
            manager.Start();
            manager.assignTargets(target1, target2);
        }

        void Update()
        {
            manager.Update();
            if(manager.foundControllers)
            {
                primaryController   = primaryController   ?? manager.controllers[0];
                secondaryController = secondaryController ?? manager.controllers[1];

                // Face screen with Rift and press "A" on the primary controller (currently primary is always in your right hand) to reset and calibrate the Oculus
                if (VRCenter.VREnabled && primaryController.buttons.a)
                {
                    VRCenter.Recenter();
                }

                if (manager.allowInput)
                {
                    if (primaryController.getButtonDown(Controller.Button.HOME))
                    {
                        Debug.Log("Primary Controller battery level: " + primaryController.getBattery());
                    }

                    if (secondaryController.getButtonDown(Controller.Button.HOME))
                    {
                        Debug.Log("Secondary Controller battery level: " + secondaryController.getBattery());
                    }

                    if (primaryController.getButtonDown(Controller.Button.PLUS))
                    {
                        primaryUseSaber = !primaryUseSaber;
                    }
                    
                    if (secondaryController.getButtonDown(Controller.Button.PLUS))
                    {
                        secondaryUseSaber = !secondaryUseSaber;
                    }

                    if(secondaryController.getButtonDown(Controller.Button.MINUS))
                    {
                        leftObj.SetActive(false);
                        usingHand = !usingHand;
                        leftObj.SetActive(true);
                    }

                    saber.transform.localPosition = scale.MultiplyVector(primaryController.position) + primaryOffset;
                    saber.transform.localRotation = Quaternion.Euler(primaryController.rotation);

                    leftObj.transform.localPosition = scale.MultiplyVector(secondaryController.position) + secondaryOffset;
                    leftObj.transform.localRotation = Quaternion.Euler(secondaryController.rotation);
                }
            }
        }
    }
}