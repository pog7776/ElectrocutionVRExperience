using UnityEngine;
using UnityEngine.Events;
using VRShooterKit.Events;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace VRShooterKit
{
    public enum GrabState
    {
        UnGrab,
        Flying,
        Grab,
        Drop,
        None
    }

    public enum GrabMode
    {
        Joint,
        Physics
    }

    
    public enum PhysicsUpdateState
    {
        NoCollision,
        InCollision
    }


    //this script handles the grabbables
    public class VR_Grabbable : VR_Interactable
    {
        #region INSPECTOR        
        [SerializeField] protected GrabMode grabMode = GrabMode.Joint;
        [SerializeField] protected float jointBreakForce = 0.0f;
        [SerializeField] protected float jointBreakTorque = 0.0f;
        [SerializeField] protected OnGrabStateChangeEvent onGrabStateChange = null;
        [SerializeField] protected bool perfectGrab = false;
        [SerializeField] protected float grabFlyTime = 0.5f;
        [SerializeField] protected bool shouldFly = true;
        [SerializeField] protected bool startOnRightcController = false;
        [SerializeField] protected bool startOnLeftController = false;
        [SerializeField] protected bool autoGrab = false;
        [SerializeField] protected bool enableColliderOnGrab = false;
        [SerializeField] protected int grabLayer = 0;
        [SerializeField] protected int unGrabLayer = 0;
        [SerializeField] protected int bulletMaxBounce = 0;
        [SerializeField] private bool setJointSettings = false;
        [SerializeField] private bool preserveKinematicState = false;
        [SerializeField] private bool toggleGrab = false;
        [SerializeField] private Vector3 recoilDirection = Vector3.zero;
        [SerializeField] protected List<Collider> ignoreColliderList = new List<Collider>();
                
#if SDK_STEAM_VR
        [SerializeField] protected bool useSteamRotationOffset = false;
#endif
#endregion

        #region protected        
        [SerializeField]protected List<Collider> colliderList = null;
        protected CollisionInfo thisCollisionInfo = null;
        
        protected Rigidbody rb = null;
        protected GrabState currentGrabState = GrabState.UnGrab;
        protected VR_Controller activeController = null;
        protected float grabStartTime = 0.0f;
        protected Vector3 grabStartPosition = Vector3.zero;
        protected Quaternion grabStartRotation = Quaternion.identity;
        protected Vector3 childrenPosition = Vector3.zero;       
        protected VR_Tag grabbableTag = null;
        protected Vector3 initialPosition = Vector3.zero;
        protected const float activeDistance = 5.0f;
        protected bool isHighLight = false;       
        protected RigidbodyInterpolation originalInterpolateMode = RigidbodyInterpolation.None;
        protected PhysicsUpdateState physicsUpdateState = PhysicsUpdateState.NoCollision;
        protected bool previusKinematicValue = false;
        protected bool preventDefault = false;
        protected float velocityChangeThreshold = 10f;       
        protected float angularVelocityChangeThreshold = 20f;
        protected CollisionPredictor collisionPredictor = null;
        private bool previusUseGravityState = false;
        private bool previusGravityState = false;
        private PhysicMaterial zeroFrictionOrBouncinessMaterial = null;
        private VR_TwoHandGrabbable twoHandGrabbable = null;        
        private bool isUsingTwoHandGrabbable = false;
        private const float defaultMaxAngularVelocity = 10.0f;
        private VR_Controller lastInteractController = null;
        private bool objectWasThrow = false;
        protected bool canUseDropZone = true;
        #endregion

        #region PUBLIC    
        public Rigidbody RB { get { return rb; } }
        public List<Collider> ColliderList { get { return colliderList; } }
        public VR_DropZone AffectedDropZone { get; set; }
        public VR_Tag GrabbableTag { get { return grabbableTag; } }
        public float GrabDistance { get { return interactDistance; } }
        public bool IsGrabbed { get { return activeController != null; } }
        public bool IsHighLight { get { return isHighLight; } }
        public VR_Controller GrabController { get { return activeController; } }
        public OnGrabStateChangeEvent OnGrabStateChange { get { return onGrabStateChange; } }
        public Vector3 PositionOffset { get; set; }
        public Quaternion RotationOffset { get; set; }
        public VR_Controller LastInteractController { get { return lastInteractController; } }
        public GrabMode GrabMode { get { return grabMode; } }
        public PhysicsUpdateState PhysicsUpdateState { get { return physicsUpdateState; } }        
        public Quaternion TwoHandGrabbableDesireRotation { get; set; }
        public bool ObjectWasThrow { get { return objectWasThrow; } }      
        public bool CanUseDropZone { get { return canUseDropZone; } }
        public GrabState CurrentGrabState
        {
            get
            {
                return currentGrabState;
            }

            private set
            {
                currentGrabState = value;
            }
        }
        public Transform CurrentInteractPoint
        {
            get
            {
                return activeController.ControllerType == VR_ControllerType.Right ? rightHandSettings.interactPoint : leftHandSettings.interactPoint;
            }
        }
        public float GrabFlyTime
        {
            get
            {
                if (shouldFly)
                {
                    return grabFlyTime;
                }

                return 0.0f;
            }
        }
        private bool IsInCollision
        {
            get
            {               
                return thisCollisionInfo.IsInCollision();
            }
        }
       

        
        #endregion

        #region UNITY_CALLBACKS
        protected override void Awake()
        {
            base.Awake();

            Construct();
            SetLayer( unGrabLayer );
            SetJointBreakForces();
            SetTwoHandedGrabbableCallbacks();
            AddRigidBodyIfNeccesary();
            SaveCurrentKinematicState();            
        }

        private void Construct()
        {           
            grabbableTag = GetComponent<VR_Tag>();
            colliderList = GetComponentsInChildren<Collider>().ToList();
            rb = GetComponent<Rigidbody>();
            thisCollisionInfo = gameObject.AddComponent<CollisionInfo>();
            collisionPredictor = GetComponent<CollisionPredictor>();
            twoHandGrabbable = GetComponent<VR_TwoHandGrabbable>();
            zeroFrictionOrBouncinessMaterial = Resources.Load( "ZeroFrictionOrBounciness" ) as PhysicMaterial;
        }


        private void SetLayer(int layer)
        {
            gameObject.layer = layer;

            for (int n = 0; n < colliderList.Count; n++)
            {
                if (colliderList[n] != null && !ignoreColliderList.Contains( colliderList[n] ))
                {
                    colliderList[n].gameObject.layer = layer;
                }
                
            }
        }

        private void SetJointBreakForces()
        {
            //set joint break forces just if we want to grab using joints
            if (grabMode == GrabMode.Joint)
            {
                //if we dont want to set the joint settings just use the max values
                if (!setJointSettings)
                {
                    jointBreakForce = float.MaxValue;
                    jointBreakTorque = float.MaxValue;
                }
            }     
        }

        private void SetTwoHandedGrabbableCallbacks()
        {
            if (twoHandGrabbable != null && grabMode == GrabMode.Physics)
            {
                twoHandGrabbable.OnTwoHandGrabStart.AddListener( OnTwoHandGrabStart );
                twoHandGrabbable.OnTwoHandGrabEnd.AddListener( OnTwoHandGrabGrabEnd );
            }
        }

        private void OnTwoHandGrabStart()
        {
            isUsingTwoHandGrabbable = true;
        }

        private void OnTwoHandGrabGrabEnd()
        {
            isUsingTwoHandGrabbable = false;
        }

        private void AddRigidBodyIfNeccesary()
        {
            if (rb == null)
            {
                Debug.LogWarning( "Grabbable component needs Rigidbody in order to work, adding one" );
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            rb.maxAngularVelocity = defaultMaxAngularVelocity;
        }

        private void SaveCurrentKinematicState()
        {
            if (rb != null)
                previusKinematicValue = rb.isKinematic;
        }

        protected override void Start()
        {

            base.Start();

            //should this grabbable start on the right controller?
            if (startOnRightcController)
            {
                VR_Manager.instance.RightController.ForceGrab( this );
            }

            //should this grabbable start on left controller?
            else if (startOnLeftController)
            {               
                VR_Manager.instance.LeftController.ForceGrab( this );
            }
        }       
        
       
        protected override void Update()
        {
            base.Update();

            //call the update
            switch (CurrentGrabState)
            {
                case GrabState.UnGrab:
                UngrabUpdate();
                break;
                case GrabState.Flying:
                FlyUpdate();
                break;
                case GrabState.Grab:
                GrabUpdate();
                break;
                case GrabState.Drop:
                DropUpdate();
                break;
            }

        }

        /// <summary>
        /// in this update the object will be making distance check to the controllers, and check if can be grabbed
        /// </summary>
        protected virtual void UngrabUpdate()
        {
            //wait to be grabbed
            //handle by the VR_Interactable
        }

        /// <summary>
        /// this update is called after a object has been grabbed so it will fly to the hand
        /// </summary>
        protected virtual void FlyUpdate()
        {           
            if ( ShouldFlyToHandPositionAndRotation() )
            {
                MoveToHandPositionAndRotation();
                return;
            }
           
            SetFinalGrabState();

            CurrentGrabState = GrabState.Grab;
            RaiseOnGrabStateChangeEvent( GrabState.Grab );
        }

        private bool ShouldFlyToHandPositionAndRotation()
        {
            float flyPercent = ( Time.time - grabStartTime ) / grabFlyTime;

            return flyPercent < 1 && shouldFly;
        }

        private void MoveToHandPositionAndRotation()
        {
            float flyPercent = ( Time.time - grabStartTime ) / grabFlyTime;

            transform.rotation = Quaternion.Slerp( grabStartRotation , CalculateGrabRotation() , flyPercent );
            transform.position = Vector3.Lerp( grabStartPosition, CalculateGrabPosition(), flyPercent );
        }

        private Quaternion CalculateGrabRotation()
        {
#if SDK_STEAM_VR            
                return activeController.GrabPoint.transform.rotation * Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset ) *  (useSteamRotationOffset ? Quaternion.Euler( 0.0f, 0.0f, 0.0f ) : Quaternion.identity);
#endif

#if SDK_OCULUS
            return activeController.GrabPoint.transform.rotation * Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset );
#endif

            return Quaternion.identity;

        }

        public VR_HandInteractSettings GetCurrentHandInteractSettings()
        {
            return activeController.ControllerType == VR_ControllerType.Right ? rightHandSettings : leftHandSettings;
        }
             
        private Vector3 CalculateGrabPosition()
        {
            Vector3 dir = ( GetCurrentHandInteractSettings().interactPoint.position - transform.position ).normalized;
            float d = Vector3.Distance( GetCurrentHandInteractSettings().interactPoint.position, transform.position );


            return activeController.GrabPoint.transform.position + ( dir * d * -1.0f );
        }        

        private void SetFinalGrabState()
        {
            ChangeCollidersEnable( enableColliderOnGrab );
            SetFinalHandPositionAndRotation();            

            if (grabMode == GrabMode.Joint)
            {
                SetupFixedJoint();
            }

            //should the hand be hide?
            GrabController.SetVisibility( !GetCurrentHandInteractSettings().hideHandOnGrab );

            //parent the objects so they exist on the same space
            transform.parent = activeController.transform;
        }

        private void ChangeCollidersEnable(bool enable)
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                if ( CanWeControlThisCollider( colliderList[n] ) )
                {
                    colliderList[n].enabled = enable;
                }
            }
        }

        private bool CanWeControlThisCollider(Collider collider)
        {
            return collider != null && !ignoreColliderList.Contains( collider ) && collider.GetComponent<IgnoreColliderActivationFromGrabbable>() == null;
        }

        private void SetFinalHandPositionAndRotation()
        {
            transform.rotation = CalculateGrabRotation();
            transform.position = CalculateGrabPosition();
        }

        protected void SetupFixedJoint()
        {
            DestroyCurrentJoint();
            CreateNewGrabJoint();            

            activeController.OnJointBreakListener.SetListener( OnJointBreak );
            rb.isKinematic = false;
        }

        private void DestroyCurrentJoint()
        {
            FixedJoint joint = activeController.GrabPoint.gameObject.GetComponent<FixedJoint>();

            if (joint != null)
                Destroy( joint );
        }

        private void CreateNewGrabJoint()
        {
            FixedJoint joint = activeController.GrabPoint.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rb;
            joint.breakForce = jointBreakForce;
            joint.breakTorque = jointBreakTorque;
        }

        /// <summary>
        /// update when a this object is grabbed
        /// </summary>
        protected virtual void GrabUpdate()
        {
            //check if we should drop this grabbable
            if (ShouldDropObject())
            {
                CurrentGrabState = GrabState.Drop;
                return;
            }

            UpdatePositionAndRotationOffset();

        }

        private bool ShouldDropObject()
        {
            if (toggleGrab)
            {
                return activeController.Input.GetButtonDown( interactButton );
            }

            return !autoGrab && !activeController.Input.GetButtonDown( interactButton );
        }

        private void UpdatePositionAndRotationOffset()
        {
            activeController.PositionOffset = PositionOffset;
            activeController.RotationOffset = RotationOffset;
        }

        /// <summary>
        /// called when the object is dropped, this is just called a frame mainly it is a excuse to call RaiseOnGrabStateChangeEvent
        /// </summary>
        protected virtual void DropUpdate()
        {
            ResetActiveControllerState();
            ResetRigidBodyState();

            isUsingTwoHandGrabbable = false;

            if (grabMode == GrabMode.Physics)
            {
                ResetPhysicsState();
            }            

            //some componets stop the default behaivour of this component like the VR_TwoHandGrabbable.cs
            if (!preventDefault)
            {
                ApplyControllerVelocity();
                EnableHandCollision();
                ChangeCollidersEnable( true );
                transform.SetParent( null );
            }

            lastInteractController = activeController;
            activeController = null;

            //can be grabbed aigan
            CanInteract = true;   
            TransitionToUnGrabState();            
        }

        private void ResetActiveControllerState()
        {
            activeController.UsePositionOffset = true;
            activeController.UseRotationOffset = true;

            activeController.SetPositionAndRotationControlMode( MotionControlMode.Engine, MotionControlMode.Engine );

            GrabController.SetVisibility( true );

            if (activeController != null)
            {
                ResetControllerState( activeController );
            }
        }

        private void ResetPhysicsState()
        {
            SetRigidbodyVelocityToZero();
            rb.useGravity = previusUseGravityState;
            rb.useGravity = previusGravityState;
            OverridePhysicsMaterial( null );
        }

        private void ResetRigidBodyState()
        {
            if (rb != null)
                rb.interpolation = originalInterpolateMode;

            if (preserveKinematicState && rb != null)
            {
                rb.isKinematic = previusKinematicValue;
            }
            else if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        private void EnableHandCollision()
        {
            if (activeController.Velocity.magnitude > 0.1f && activeController.Collider != null)
            {
                StartCoroutine( EnableCollisionRoutine( activeController.Collider, 0.1f ) );
            }
            else if (activeController.Collider != null)
            {
                EnableCollision( activeController.Collider );
            }
        }


        private void TransitionToUnGrabState()
        {
            RaiseOnGrabStateChangeEvent( GrabState.Drop );
            CurrentGrabState = GrabState.UnGrab;
            RaiseOnGrabStateChangeEvent( GrabState.UnGrab );
        }

        private void FixedUpdate()
        {
            if (objectWasThrow && rb.velocity.magnitude <= 0.01f)
            {
                objectWasThrow = false;
            }

            if ( !CanUpdatePhysicsMovement() )
                return;
            
            switch (physicsUpdateState)
            {
                case PhysicsUpdateState.InCollision:
                    physicsUpdateState = InCollisionPhysicsUpdateState();
                break;
                case PhysicsUpdateState.NoCollision:
                    physicsUpdateState = NoCollisionPhysicsUpdate();
                break;
            }     

        }

        private PhysicsUpdateState InCollisionPhysicsUpdateState()
        {

            Vector3 desirePosition;
            Quaternion desireRotation;
            CalculatePhysicsDesirePositionAndRotation( out desirePosition, out desireRotation );

            if ( IsTheHandInValidPosition(desirePosition , desireRotation) )
            {
                SetRigidbodyVelocityToZero();

                transform.parent = activeController.transform;
                activeController.SetPositionAndRotationControlMode( MotionControlMode.Engine, MotionControlMode.Engine );
                rb.rotation = desireRotation;
                rb.position = desirePosition;
                StartCoroutine( DisableCollidersForOneFrame() );

                return PhysicsUpdateState.NoCollision;
            }

            UpdatePhysicsVelocity();

            return PhysicsUpdateState.InCollision;
        }

        private void CalculatePhysicsDesirePositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            position = CalculatePhysicsDesirePosition();
            rotation = CalculatePhysicsDesireRotation();
        }

        private Vector3 CalculatePhysicsDesirePosition()
        {

            if (physicsUpdateState == PhysicsUpdateState.InCollision)
            {
                Vector3 dir = ( GetCurrentHandInteractSettings().interactPoint.position - rb.position ).normalized;
                float d = Vector3.Distance( GetCurrentHandInteractSettings().interactPoint.position, rb.position );

                return activeController.OriginalParent.position + ( dir * d * -1.0f ) + activeController.PositionOffset;
            }
            else if (physicsUpdateState == PhysicsUpdateState.NoCollision)
            {
                return CalculateGrabPosition();
            }
            return Vector3.zero;
        }

        private Quaternion CalculatePhysicsDesireRotation()
        {
            if (isUsingTwoHandGrabbable)
            {
                return TwoHandGrabbableDesireRotation * Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset );
            }

            return activeController.OriginalParent.rotation * Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset );
        }

        private bool IsTheHandInValidPosition(Vector3 desirePosition , Quaternion desireRotation)
        {
            return !IsHandInsideCollider() && !collisionPredictor.WillCollisionAtPositionAndRotation( desirePosition, desireRotation );
        }

        //is the hand already inside a collider?
        private bool IsHandInsideCollider()
        {
            List<Collider> contactColliders = thisCollisionInfo.GetCurrentContactColliders();

            for (int n = 0; n < contactColliders.Count; n++)
            {
                if (contactColliders[n] != null && contactColliders[n].gameObject.tag != "Player" && contactColliders[n].bounds.Contains( activeController.OriginalParent.position ))
                    return true;
                if (contactColliders[n] == null)
                {
                    contactColliders.RemoveAt( n );
                    n--;
                }
            }

            return false;
        }

        private IEnumerator DisableCollidersForOneFrame()
        {
            ChangeCollidersEnable( false );
            yield return new WaitForFixedUpdate();
            ChangeCollidersEnable( true );
        }

        private void UpdatePhysicsVelocity()
        {
            SetRigidBodyDesireVelocity();
            SetRigidBodyDesireAngularVelocity();          

        }

        private void SetRigidBodyDesireVelocity()
        {
            Vector3 desirePosition = CalculatePhysicsDesirePosition();

            Vector3 velocity = ( desirePosition - rb.position ) / Time.fixedUnscaledDeltaTime;
            rb.velocity = Vector3.MoveTowards( rb.velocity, velocity, velocityChangeThreshold );
        }

        private void SetRigidBodyDesireAngularVelocity()
        {
            Quaternion desireRotation = CalculatePhysicsDesireRotation();
            desireRotation = ApplyRecoilRotation( desireRotation );
            desireRotation = ApplySteamVROffset(desireRotation);

            Quaternion angularVelocity = desireRotation * Quaternion.Inverse( rb.rotation );

            float angle = 0.0f;
            Vector3 axis = Vector3.zero;

            angularVelocity.ToAngleAxis( out angle, out axis );



            if (angle > 180f)
                angle -= 360f;

            angle *= Mathf.Deg2Rad;

            axis = ( axis * angle ) / Time.deltaTime;

            if (( float.IsNaN( axis.x ) || float.IsNaN( axis.y ) || float.IsNaN( axis.z ) )
                || float.IsInfinity( axis.x ) || float.IsInfinity( axis.y ) || float.IsInfinity( axis.z ))
                axis = Vector3.zero;


            rb.angularVelocity = axis;

        }

        private Quaternion ApplyRecoilRotation(Quaternion rotation)
        {
            Vector3 recoilEuler = activeController.RotationOffset.eulerAngles;
            float recoilMagnitude = recoilEuler.magnitude;

            return rotation * ( Quaternion.Euler( recoilDirection * recoilMagnitude ) );
        }

        private Quaternion ApplySteamVROffset(Quaternion rotation)
        {
#if SDK_STEAM_VR
            if (!isUsingTwoHandGrabbable)
            {
                //rotation = desireRotation * Quaternion.Euler( steamRotationOffset );
            }
#endif
            return rotation;
        }


        private PhysicsUpdateState NoCollisionPhysicsUpdate()
        {

            if (collisionPredictor.WillCollisionAtPositionAndRotation( transform.position, transform.rotation ))
            {
                activeController.SetPositionAndRotationControlMode( MotionControlMode.Free, MotionControlMode.Free );
                transform.parent = null;
                return PhysicsUpdateState.InCollision;
            }

            UpdatePhysicsVelocity();

            return PhysicsUpdateState.NoCollision;
        }

        private bool CanUpdatePhysicsMovement()
        {
            return CurrentGrabState == GrabState.Grab && grabMode == GrabMode.Physics;
        }

        private void LateUpdate()
        {
            if (!CanUpdatePhysicsMovement() || physicsUpdateState != PhysicsUpdateState.InCollision)
                return;

            UpdateVisualControllerPositionAndRotation();
            
        }
        
        private void UpdateVisualControllerPositionAndRotation()
        {
            activeController.transform.position = GetCurrentHandInteractSettings().interactPoint.position;
            activeController.transform.rotation = GetCurrentHandInteractSettings().interactPoint.rotation * Quaternion.Inverse( Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset ) );
        }  

      

        private void SetRigidbodyVelocityToZero()
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
        
#endregion

        //this is a feature what i am working you can ignore this function :)
        public void SetEditorGrabPositionAndRotation(VR_Controller controller)
        {
            activeController = controller;

            if (!usePerHandSettings)
            {
                leftHandSettings = handSettings;
                rightHandSettings = handSettings;
            }

            transform.position = CalculateGrabPosition();
            transform.rotation = activeController.GrabPoint.transform.rotation * Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset );
                     
            //SetupFixedJoint();
            transform.parent = activeController.transform;
        }

        //this is a feature what i am working you can ignore this function :)
        public void CopySettingsTo(VR_Grabbable grabbable)
        {
            grabbable.handSettings.rotationOffset = handSettings.rotationOffset;
            grabbable.rightHandSettings.rotationOffset = rightHandSettings.rotationOffset;
            grabbable.leftHandSettings.rotationOffset = leftHandSettings.rotationOffset;
        }
        
        private void ApplyControllerVelocity()
        {
            if (rb != null && activeController != null)
            {
                StartCoroutine(ApplyControllerVelocityRoutine(activeController));                
            }
        }

        private IEnumerator ApplyControllerVelocityRoutine(VR_Controller controller)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            yield return new WaitForFixedUpdate();
           
            controller.ApplyThrowVelocity(this);

            while (rb.velocity.magnitude < 0.25f)
                yield return new WaitForFixedUpdate();

            objectWasThrow = true;
        }

        private void ResetControllerState(VR_Controller controller)
        {
            //recenter controller
            controller.Recenter();

            controller.OnJointBreakListener.RemoveAllListeners();
            FixedJoint joint = controller.GrabPoint.gameObject.GetComponent<FixedJoint>();

            if (joint != null)
                Destroy( joint );

            controller.CleanCurrentGrab();
        }
       
        protected void RaiseOnGrabStateChangeEvent(GrabState grabState)
        {
            SetLayer( grabState == GrabState.Grab ? grabLayer : unGrabLayer );
            onGrabStateChange.Invoke( grabState );
        }       

        public void ForceDrop()
        {
            m_buttonWasPressedLeft = VR_Manager.instance.LeftController.Input.GetButtonDown( interactButton );
            m_buttonWasPressedRight = VR_Manager.instance.RightController.Input.GetButtonDown( interactButton );

            DropUpdate();
        }

        public override void Interact(VR_Controller controller)
        {
            OnGrabSuccess( controller );
        }

        /// <summary>
        /// Called by VR_Input, to let know what we are grabbing this object
        /// </summary>
        /// <param name="controller"></param>
        public virtual void OnGrabSuccess(VR_Controller controller)
        {
            if (preventDefault)
            {
                activeController = controller;
                CurrentGrabState = GrabState.Grab;                
                RaiseOnGrabStateChangeEvent(CurrentGrabState);
                return;
            }

            previusKinematicValue = rb.isKinematic;

            //stop this object to be interactable
            CanInteract = false;

            //set the active controller
            activeController = controller;

           
            if (rb != null && grabMode == GrabMode.Joint)
            {
                rb.isKinematic = true;
                originalInterpolateMode = rb.interpolation;
                rb.interpolation = RigidbodyInterpolation.None;                
            }

            if (grabMode == GrabMode.Physics)
            {
                physicsUpdateState = PhysicsUpdateState.NoCollision;
                previusUseGravityState = rb.useGravity;
                previusGravityState = rb.useGravity;
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.useGravity = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                originalInterpolateMode = rb.interpolation;
                OverridePhysicsMaterial( zeroFrictionOrBouncinessMaterial );

                activeController.UseRotationOffset = false;
                activeController.UsePositionOffset = false;
            }

            

            //disable collision with the grabbable and the hand
            if (activeController.Collider != null)
            {
                IgnoreCollision(activeController.Collider);
            }           


            //if this object shoudl fly to hand disable colliders while flying otherwise set desire collider state
            if (shouldFly)
            {
                //disable colliders while flying
                ChangeCollidersEnable( false );
            }
            else
            {
                ChangeCollidersEnable( enableColliderOnGrab );
            }

            //if we are using a perfect grab
            if (perfectGrab)
            {
                //parent the objects so they exist on the same space
                transform.parent = activeController.transform;
                GrabController.SetVisibility( !GetCurrentHandInteractSettings().hideHandOnGrab );
                SetupFixedJoint();               

                //set the current grab state
                CurrentGrabState = GrabState.Grab;
                //raise grab state change event
                RaiseOnGrabStateChangeEvent( CurrentGrabState );
                return;
            }
            else
            {

                //set fly values
                if (shouldFly)
                {
                    grabStartTime = Time.time;
                    grabStartPosition = transform.position;
                    grabStartRotation = transform.rotation;
                }

                else
                {
                    SetFinalGrabState();
                }

                CurrentGrabState = shouldFly ? GrabState.Flying : GrabState.Grab;

            }


            //raise the event
            RaiseOnGrabStateChangeEvent( ( shouldFly ? GrabState.Flying : GrabState.Grab ) );

        }

        public void PreventDefault()
        {
            preventDefault = true;
        }

        private void OverridePhysicsMaterial(PhysicMaterial newMaterial)
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                colliderList[n].material = newMaterial;
            }
        }

       

        /// <summary>
        /// Called whena grabbed joints breaks
        /// </summary>
        private void OnJointBreak(float f)
        {
            //if the object is no grabbed ignore jojntbreak
            if (CurrentGrabState != GrabState.Grab)
                return;

            FixedJoint joint = activeController.GrabPoint.gameObject.GetComponent<FixedJoint>();

            if (joint != null && joint.connectedBody != null)
                return;

            m_buttonWasPressedLeft = VR_Manager.instance.LeftController.Input.GetButtonDown( interactButton );
            m_buttonWasPressedRight = VR_Manager.instance.RightController.Input.GetButtonDown( interactButton );


            CurrentGrabState = GrabState.Drop;
        }

        private void IgnoreCollision(Collider c)
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                if (colliderList[n] != null && c != null)
                {
                    Physics.IgnoreCollision( colliderList[n], c );
                }
                
            }
        }

        private IEnumerator EnableCollisionRoutine(Collider c, float t)
        {
            yield return new WaitForSeconds( t );
            IgnoreCollision( c );
        }

        private void EnableCollision(Collider c)
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                if (colliderList[n] != null)
                {
                    Physics.IgnoreCollision( colliderList[n], c, false );
                }
               
            }
        }


        public void OnDropSuccess()
        {
            ChangeCollidersEnable( false );
        }        

        public void IgnoreCollider(Collider c)
        {
            ignoreColliderList.Add( c );
        }        
        

    }
}