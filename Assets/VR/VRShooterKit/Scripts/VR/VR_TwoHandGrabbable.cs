using UnityEngine;
using UnityEngine.Events;

namespace VRShooterKit
{
    public class VR_TwoHandGrabbable : MonoBehaviour
    {
        [SerializeField] private VR_Grabbable dominantGrabbable = null;
        [SerializeField] private VR_Grabbable secondaryGrabbable = null;     
        [SerializeField] private UnityEvent onTwoHandGrabStart = null;
        [SerializeField] private UnityEvent onTwoHandGrabEnd = null;

        private VR_Controller secondaryController = null;        
        private Quaternion desireRotation = Quaternion.identity;
        private Vector3 dir = Vector3.zero;
        private bool applyTwoHandTransformManually = false;

        public UnityEvent OnTwoHandGrabStart { get { return onTwoHandGrabStart; } }
        public UnityEvent OnTwoHandGrabEnd { get { return onTwoHandGrabEnd; } }


        private void Awake()
        {
            //disable secondary grabbable while dominat grabbable is dropped
            secondaryGrabbable.enabled = false;

            dominantGrabbable.OnGrabStateChange.AddListener( OnDominantGrabbableGrabStateChange );
            secondaryGrabbable.OnGrabStateChange.AddListener( OnSecondaryGrabbableGrabStateChange );
            //prevent the VR_Grabbable for making changes in the object, so we can control it
            secondaryGrabbable.PreventDefault();
        }

        private void Update()
        {
            if (CanUpdate())
                return;

            //set the desire rotation to the dominanthand
            dominantGrabbable.GrabController.transform.rotation = desireRotation;
        }


        private void LateUpdate()
        {
            if (!applyTwoHandTransformManually)
            {
                TwoHandUpdate();
            }

            
        }

        public void TwoHandUpdate()
        {
            if (CanUpdate())
            {
                //for some reason the secondary controller should be dropped now
                if (SecondaryGrabbableShouldBeDropped())
                {
                    ReleaseSecondaryGrabbable();
                }

                //dominantGrabbable.IsUsingTwoHandGrabbable = false;
                return;
            }

            //dominantGrabbable.IsUsingTwoHandGrabbable = true;
            desireRotation = CalculateDesireRotation();

            if (dominantGrabbable.GrabMode == GrabMode.Physics)
            {
                dominantGrabbable.TwoHandGrabbableDesireRotation = desireRotation;
            }

            if (dominantGrabbable.GrabController.transform.parent == null)
            {
                return;
            }

            //add recoil if we are grabbing using joints
            if (dominantGrabbable.GrabMode == GrabMode.Joint)
            {
                desireRotation = desireRotation * dominantGrabbable.GrabController.RotationOffset;
            }

            //set the desire rotation to the dominanthand
            dominantGrabbable.GrabController.transform.rotation = desireRotation;
            dominantGrabbable.GrabController.transform.localPosition = dominantGrabbable.GrabController.InitialPosition + ( dominantGrabbable.GrabController.transform.parent.InverseTransformDirection( dir * -1.0f ) * ( dominantGrabbable.GrabController.PositionOffset.magnitude ) );
        }

        private bool CanUpdate()
        {
            return dominantGrabbable.CurrentGrabState != GrabState.Grab || secondaryGrabbable.CurrentGrabState != GrabState.Grab;
        }

       


        private Quaternion CalculateDesireRotation()
        {
            dir = ( secondaryGrabbable.GrabController.OriginalParent.transform.position - GetDominatGrabbableGrabPosition() ).normalized;


            Vector3 customDir = dominantGrabbable.GrabController.OriginalParent.forward;
            float angle = Vector2.Angle( new Vector2( dir.x, dir.z ), new Vector2( customDir.x, customDir.z ) );

            return Quaternion.LookRotation( dir ) * Quaternion.Euler( 0.0f, 0.0f, dominantGrabbable.GrabController.OriginalParent.localEulerAngles.z * ( angle > 90.0f && angle < 180.0f ? -1.0f : 1.0f ) );
        }

        private bool SecondaryGrabbableShouldBeDropped()
        {
            return dominantGrabbable.CurrentGrabState == GrabState.UnGrab && secondaryGrabbable.CurrentGrabState == GrabState.Grab;
        }

        private bool ShouldCalculateRotationFromOriginalPosition()
        {
            return dominantGrabbable.GrabMode == GrabMode.Physics && dominantGrabbable.PhysicsUpdateState == PhysicsUpdateState.InCollision;
        }

        private Vector3 GetDominatGrabbableGrabPosition()
        {
            return ShouldCalculateRotationFromOriginalPosition() ? dominantGrabbable.GrabController.OriginalParent.position : dominantGrabbable.GrabController.transform.position;
        }

        private void OnDominantGrabbableGrabStateChange(GrabState state)
        {
            if (state == GrabState.Drop)
            {
                if (secondaryGrabbable.CurrentGrabState == GrabState.Grab || secondaryGrabbable.CurrentGrabState == GrabState.Drop)
                {
                    ReleaseSecondaryGrabbable();
                }

                secondaryGrabbable.enabled = false;               
            }
            else if (state == GrabState.Grab)
            {
                secondaryGrabbable.enabled = true;                
            }               
        }

        private void ReleaseSecondaryGrabbable()
        {
            secondaryGrabbable.ForceDrop();

            
            secondaryController.SetVisibility( true );
                        
            //dominantGrabbable.GrabController.UsePositionOffset = true;

            secondaryController.SetPositionControlMode( MotionControlMode.Engine );
            secondaryController.SetRotationControlMode( MotionControlMode.Engine );

            if(OnTwoHandGrabEnd != null)
                OnTwoHandGrabEnd.Invoke();
        }
       
        private void OnSecondaryGrabbableGrabStateChange( GrabState state)
        {
            if (state == GrabState.Grab)
            {
                dominantGrabbable.GrabController.UsePositionOffset = false;
               

                secondaryController = secondaryGrabbable.GrabController;
                Transform interactPoint = secondaryGrabbable.GetCurrentHandInteractSettings().interactPoint;

                secondaryController.SetVisibility( !secondaryGrabbable.GetCurrentHandInteractSettings().hideHandOnGrab );

                secondaryController.SetPositionControlMode( MotionControlMode.Free );
                secondaryController.SetRotationControlMode( MotionControlMode.Free );

                secondaryController.transform.parent = interactPoint;
                secondaryController.transform.localPosition = Vector3.zero;
                secondaryController.transform.rotation = interactPoint.rotation * Quaternion.Euler( secondaryGrabbable.GetCurrentHandInteractSettings().rotationOffset );

                if (OnTwoHandGrabStart != null)
                    OnTwoHandGrabStart.Invoke();

            }

            else if(state == GrabState.Drop && dominantGrabbable.CurrentGrabState == GrabState.Grab)
            {
                
                dominantGrabbable.GrabController.UsePositionOffset = true;
                
                secondaryController.SetPositionControlMode( MotionControlMode.Engine );
                secondaryController.SetRotationControlMode( MotionControlMode.Engine );

                secondaryController.SetVisibility( true);

                if (OnTwoHandGrabEnd != null)
                    OnTwoHandGrabEnd.Invoke();
            }
           
        }

        public void SetApplyTwoHandTransformManually(bool value)
        {
            applyTwoHandTransformManually = value;
        }



    }
}

