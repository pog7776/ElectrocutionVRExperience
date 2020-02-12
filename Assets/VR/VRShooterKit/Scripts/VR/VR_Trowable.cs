using UnityEngine;

namespace VRShooterKit
{   
    public class VR_Trowable : MonoBehaviour
    {
        [SerializeField] private float speedModifier = 1.5f;
        [SerializeField] private float aungularSpeedModifier = 1.5f;

        private Rigidbody rb = null;

        public float SpeedModifier { get { return speedModifier; } }
        public float AngularSpeedModifier { get { return aungularSpeedModifier; } }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            VR_Grabbable grabbable = GetComponent<VR_Grabbable>();

            if (grabbable == null)
            {
                Debug.LogError( "Trowable needs VR_Grabbable script in order to work!" );
                return;
            }

            GetComponent<VR_Grabbable>().OnGrabStateChange.AddListener( OnThisGrabbableStateChange );
        }

        private void OnThisGrabbableStateChange(GrabState grabState)
        {
            if (grabState == GrabState.Drop && rb != null)
            {              
                rb.velocity *= speedModifier;
                rb.angularVelocity *= aungularSpeedModifier;

            }
        }
    }

}

