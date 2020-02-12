using UnityEngine;

namespace VRShooterKit.Locomotion
{
    //this scripts handles teleport marker position and rotation
    public class VR_TeleportMarker : MonoBehaviour
    {
        [SerializeField] private GameObject marker = null;       

        public GameObject Marker { get { return marker; } }

        private void Awake()
        {
            //disable marker 
            marker.gameObject.SetActive(false);
        }

        public void Hide()
        {
            marker.SetActive( false );
        }

        public void UpdatePositionAndRotation(VR_Controller controller, AimRaycastInfo info)
        {
            if (!marker.activeInHierarchy)
                marker.SetActive( true );

            marker.transform.position = info.hitPoint;
            marker.transform.up = info.normal;


            Vector2 controllerInput = controller.Input.GetJoystickInput().normalized;
            Vector3 controllerDirection = new Vector3( controllerInput.x, 0.0f, controllerInput.y );

            //get controller pointing direction in world space
            controllerDirection = controller.transform.TransformDirection( controllerDirection );                   
           
            //get marker forward in local space
            Vector3 forward = marker.transform.InverseTransformDirection( marker.transform.forward);
           
            //find the angle diference betwen the controller pointing direction and marker current forward
            float angle = Vector2.SignedAngle( new Vector2( controllerDirection.x , controllerDirection.z ) , new Vector2( forward.x , forward.z ) );

            //rotate marker in local space to match controller pointing direction
            marker.transform.Rotate(Vector3.up , angle , Space.Self);          

        }
        
    }
}

