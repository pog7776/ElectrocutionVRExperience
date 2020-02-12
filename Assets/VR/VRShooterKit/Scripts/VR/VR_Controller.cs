using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if SDK_STEAM_VR
using Valve.VR;
#endif


namespace VRShooterKit
{

    public enum VR_ControllerType
    {
        Left,
        Right
    }

    public enum MotionControlMode
    {
        Free,
        Engine
    }

    /// <summary>
    /// Supported Inputs
    /// </summary>
    public enum VR_InputButton
    {
        Button_None,
        Button_Primary,
        Button_Secondary,
        Button_Trigger,
        Button_Grip,
        Button_Thumbstick_Left,
        Button_Thumbstick_Right,
        Button_Thumbstick_Down,
        Button_Thumbstick_Up,
        Button_Tumbstick,
        Button_TumbstickPress
    }



    /// class to asbtrac controller input    
    public class VR_Controller : MonoBehaviour
    {
        #region INSPECTOR
        [SerializeField] private Transform grabPoint = null;
        [SerializeField] private VR_ControllerType controllerType = VR_ControllerType.Right;
        [SerializeField] private Animator animator = null;
        [SerializeField] private AnimationClip defaultInteractAnimationClip = null;
        [SerializeField] private GameObject handGO = null;

        #endregion

        #region PUBLIC
        public Transform GrabPoint { get { return grabPoint; } }
        public Vector3 Position { get { return transform.position; } }
        public bool IsConnected
        {
            get
            {
#if SDK_OCULUS
                return ( ( OVRInput.GetConnectedControllers() & OVR_ControllerType ) == OVR_ControllerType );
#endif

#if SDK_STEAM_VR
                return isConnected;
#endif

                return false;
            }
        }
#if SDK_OCULUS
        public OVRInput.Controller OVR_ControllerType { get { return controllerType == VR_ControllerType.Right ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch; } }
#endif
#if SDK_STEAM_VR
        public SteamVR_Input_Sources SteamControllerType { get { return controllerType == VR_ControllerType.Right ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand; } }
#endif
        public Quaternion Rotation { get { return transform.rotation; } }
        public VR_Grabbable CurrentGrab { get { return currentGrab; } private set { currentGrab = value; } }
        public Vector3 AngularVelocity { get { return handPhysics.AngularVelocity; } }
        public Vector3 Velocity { get { return handPhysics.Velocity; } }
        public VR_ControllerType ControllerType { get { return controllerType; } }
        public VR_ControllerGesture GestureScript { get { return gestureScript; } }
        public OnJointBreakListener OnJointBreakListener { get { return onJointBreakListener; } }
        public Animator Animator { get { return animator; } }
        public Vector3 PositionOffset
        {
            set
            {
                positionOffset = value;

                if (UsePositionOffset && controlPositionMode == MotionControlMode.Engine)
                {
                    transform.localPosition = initialPosition + value;
                }
            }
            get { return positionOffset; }
        }

        public Quaternion RotationOffset
        {
            set
            {
                rotationOffset = value;

                if (UseRotationOffset && controlRotationMode == MotionControlMode.Engine)
                {
                    transform.localRotation = initialRotation * value;
                }

            }
            get { return rotationOffset; }
        }

        public Rigidbody RigidBody { get { return rb; } }
        public Rigidbody OriginalParentRB { get { return originalParentRB; } }

        public MotionControlMode ControlPositionMode { get { return controlPositionMode; } }
        public MotionControlMode ControlRotationMode { get { return controlRotationMode; } }
        public Transform OriginalParent { get { return originalParent; } }
        public Collider Collider { get { return thisCollider; } }
        public bool UsePositionOffset { get; set; }
        public bool UseRotationOffset { get; set; }
        public Vector3 InitialPosition { get { return initialPosition; } }
        public VR_Input Input { get; private set; }
        #endregion

        #region PRIVATE
#if SDK_STEAM_VR
        private SteamVR_Behaviour_Pose steamController = null;
        private bool isConnected = false;       
#endif
        private HandPhysics handPhysics = null;
        private Vector3 initialPosition = Vector3.zero;
        private Quaternion initialRotation = Quaternion.identity;
        private List<VR_Interactable> interactList = null;
        private List<VR_Highlight> highlightList = null;
        private VR_Grabbable currentGrab = null;
        private VR_Highlight currentHighlight = null;
        private OnJointBreakListener onJointBreakListener = null;
        private AnimatorOverrideController overrideAnimator = null;
        private VR_ControllerGesture gestureScript = null;
        private Transform originalParent = null;
        private string currentInteractAnimationName = null;
        private MotionControlMode controlPositionMode = MotionControlMode.Engine;
        private MotionControlMode controlRotationMode = MotionControlMode.Engine;
        private Collider thisCollider = null;
        private Vector3 positionOffset = Vector3.zero;
        private Quaternion rotationOffset = Quaternion.identity;
        private VR_Grabbable activeDistanceGrabbable = null;
        private VR_Highlight activeDistanceHighlight = null;
        private Rigidbody rb = null;
        private Rigidbody originalParentRB = null;
        private HistoryBuffer historyBuffer = null;
        private VR_Controller otherController
        {
            get
            {
                if (controllerType == VR_ControllerType.Right)
                    return VR_Manager.instance.LeftController;
                else
                    return VR_Manager.instance.RightController;
            }
        }

        #endregion

        private const int historySize = 20;


        #region ANIMATION_HASHES        
        private int isGrabbingHash = -1;
        #endregion


        private void Awake()
        {

            if (FindObjectOfType<VR_Manager>() == null)
            {
                Debug.LogError("you need a VR_Manager active in the scene in order to use VR Shooter Kit");
            }

            Setup();

#if SDK_STEAM_VR           
            SteamVR_Behaviour_Pose controller = transform.GetComponentInParent<SteamVR_Behaviour_Pose>();
            controller.onConnectedChanged.AddListener(delegate (SteamVR_Behaviour_Pose poseController , SteamVR_Input_Sources sources , bool state ) { OnControlllerConnectedChangeEvent( state ); } );
#endif

            gestureScript = GetComponent<VR_ControllerGesture>();
            thisCollider = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
            Input = new VR_Input(this);


            if (animator != null)
            {
                CreateOverrideAnimator();
                SetupAnimatiorHashes();
            }

            SaveLocalPositionAndRotation();
            originalParent = transform.parent;

            originalParentRB = originalParent.GetComponent<Rigidbody>();

        }

        private void Start()
        {
            onJointBreakListener = FindJointBreakListener();

            historyBuffer = transform.parent.GetComponent<HistoryBuffer>();

            handPhysics = new HandPhysics( historyBuffer );
        }
#if SDK_STEAM_VR
        private void OnControlllerConnectedChangeEvent(bool state)
        {
            isConnected = state;
        }       
#endif
        private void Setup()
        {
            UsePositionOffset = true;
            UseRotationOffset = true;

            controlPositionMode = MotionControlMode.Engine;
            controlRotationMode = MotionControlMode.Engine;

#if SDK_STEAM_VR
            SteamVR_Behaviour_Pose[] controllers = FindObjectsOfType<SteamVR_Behaviour_Pose>();

            for (int n = 0; n < controllers.Length; n++)
            {
                if (controllers[n].inputSource == SteamVR_Input_Sources.RightHand && controllerType == VR_ControllerType.Right)
                    steamController = controllers[n];
                else if (controllers[n].inputSource == SteamVR_Input_Sources.LeftHand && controllerType == VR_ControllerType.Left)
                    steamController = controllers[n];
            }
#endif
        }


        private void CreateOverrideAnimator()
        {
            if (animator == null)
                return;

            //create override animator controller so we can change the grab animations at running time
            overrideAnimator = new AnimatorOverrideController( animator.runtimeAnimatorController );
            animator.runtimeAnimatorController = overrideAnimator;

            currentInteractAnimationName = defaultInteractAnimationClip.name;
        }

        private void SetupAnimatiorHashes()
        {
            isGrabbingHash = Animator.StringToHash( "IsGrabbing" );
        }

        private void SaveLocalPositionAndRotation()
        {
            //we save this for recentering controllers back later
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }

        private OnJointBreakListener FindJointBreakListener()
        {
            OnJointBreakListener listener = grabPoint.GetComponent<OnJointBreakListener>();

            if (listener == null)
                listener = grabPoint.gameObject.AddComponent<OnJointBreakListener>();

            return listener;
        }

        private void Update()
        {

            UpdateHighlightState();

            if (animator != null)
                UpdateAnimator();
        }


        private void UpdateHighlightState()
        {
            if (CanHighlight())
            {
                VR_Highlight highlight = FindNearHighlight();

                //if we lost the nearest object
                if (highlight == null && currentHighlight != null)
                {
                    currentHighlight.UnHighlight( this );
                    currentHighlight = null;
                }

                //if we found a new object and we dont have highlight
                if (currentHighlight == null && highlight != null)
                {
                    currentHighlight = highlight;
                    highlight.Highlight( this );
                }

                //if we found a new closer object
                else if (highlight != null && highlight != currentHighlight)
                {
                    currentHighlight.UnHighlight( this );
                    highlight.Highlight( this );
                    currentHighlight = highlight;
                }

                //update the current higlight object, be sure that it is always on
                else if (highlight != null && currentHighlight == highlight && !currentHighlight.IsHighlight)
                {
                    currentHighlight.Highlight( this );
                }
            }
            else if (currentHighlight != null)
            {
                currentHighlight.UnHighlight( this );
                currentHighlight = null;
            }
        }


        private void UpdateAnimator()
        {
            if (animator.gameObject.activeInHierarchy)
                animator.SetBool( isGrabbingHash, currentGrab != null );
        }

        private bool CanHighlight()
        {
            return currentGrab == null;
        }

        //change the interact animation in running time
        public void OverrideInteractAnimation(AnimationClip animation)
        {
            if (animator == null)
                return;

            if (overrideAnimator == null)
            {
                CreateOverrideAnimator();
            }

            overrideAnimator[currentInteractAnimationName] = animation;
        }

        //back to the default grab animation
        public void SetDefaultInteractAnimation()
        {
            if (animator == null)
                return;

            if (overrideAnimator == null)
            {
                CreateOverrideAnimator();
            }

            overrideAnimator[currentInteractAnimationName] = defaultInteractAnimationClip;
        }

       

        public void ApplyThrowVelocity(VR_Grabbable grabbable)
        {
            handPhysics.ApplyThrowVelocity(grabbable);
        }

                
        public void Recenter()
        {
            PositionOffset = Vector3.zero;
            RotationOffset = Quaternion.identity;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="grabbableList"></param>
        public void Construct(List<VR_Interactable> interactList, List<VR_Highlight> highlightList , ControllerGestureConfig config)
        {
            //set controller type and get a refenrece to the grabbableList
            this.interactList = interactList;
            this.highlightList = highlightList;

            VR_ControllerGesture controllerGesture = GetComponent<VR_ControllerGesture>();

            if (controllerGesture != null)
                controllerGesture.Construct(config);
        }


        public void InteractWithNearesObject()
        {
            //we have something grabbe we should no interact
            if (currentGrab != null)
                return;            
            SetDefaultInteractAnimation();

            //get the near interact object to this controller
            VR_Interactable interact = FindNearInteract();
                        

            if (interact == null && activeDistanceGrabbable != null && (activeDistanceGrabbable != otherController.activeDistanceGrabbable || !ThereIsNearbyControllerInteraction(activeDistanceGrabbable)))
            {              
                interact = activeDistanceGrabbable as VR_Interactable;
            }

            if (interact != null && interact.enabled && interact.CanInteractUsingController( this ))
            {
                VR_HandInteractSettings settings = interact.GetHandInteractionSettings( this );
                ProcessInteractSettings( settings );
                ProcessInteraction( interact );
            }
        }

        private bool ThereIsNearbyControllerInteraction(VR_Interactable interactable)
        {
            
            if (!otherController.Input.GetButtonDown( interactable.InteractButton ))
            {
                return false;
            }
           
            float thisDistance = CalculateDistanceToInteractable(this , interactable );
            float otherDistance = CalculateDistanceToInteractable( otherController, interactable );

            return thisDistance > otherDistance;
        }

        private float CalculateDistanceToInteractable(VR_Controller controller , VR_Interactable interactable)
        {
            return Vector3.Distance( controller.OriginalParent.position , interactable.transform.position );

        }

        private void ProcessInteractSettings(VR_HandInteractSettings settings)
        {            
            if (animator != null && settings != null && settings.animation != null)
            {
                //override the grabbing animation
                OverrideInteractAnimation( settings.animation );
            }

        }

        private void ProcessInteraction(VR_Interactable interact)
        {
            interact.Interact( this );

            if (interact is VR_Grabbable)
            {
                currentGrab = interact as VR_Grabbable;
            }
                
        }
        

        //force a grab no distance check, and drop whathever you have on the hand
        public void ForceGrab(VR_Grabbable grabbable)
        {
            if (grabbable == null)
                return;

            if (currentGrab != null)
                CleanCurrentGrab();

            currentGrab = grabbable;
            currentGrab.OnGrabSuccess( this );
        }       
       


        public void CleanCurrentGrab()
        {
            currentGrab = null;
        }
       
        public List<Quaternion> GetRotationHistorySample(int sampleCount)
        {
            if (historyBuffer == null)
                return null;

            //return rotationHistory.GetRange( 0, sampleCount > rotationHistory.Count ? rotationHistory.Count : sampleCount);
            return historyBuffer.RotationHistory.Sample(sampleCount);
        }


        /// <summary>
        /// Find the near avalible grabbable to this controller
        /// </summary>
        /// <returns></returns>
        private VR_Interactable FindNearInteract()
        {

            if (interactList.Count == 0)
                return null;

            VR_Interactable interact = null;
            float minDistance = float.MaxValue;

            for (int n = 0; n < interactList.Count; n++)
            {
                if (interactList[n].enabled && interactList[n].CanInteract && interactList[n].CanInteractUsingController( this ))
                {
                    Transform highlightPoint = ( ControllerType == VR_ControllerType.Right ? interactList[n].HighlightPointRightHand : interactList[n].HighlightPointLeftHand );

                    if (highlightPoint != null)
                    {
                        float d = ( Position - highlightPoint.position ).magnitude;

                        if (d < minDistance && d <= interactList[n].InteractDistance)
                        {
                            interact = interactList[n];
                            minDistance = d;
                        }
                    }

                }

            }


            return interact;
        }

        /// <summary>
        /// Find the near avalible grabbable to this controller
        /// </summary>
        /// <returns></returns>
        private VR_Highlight FindNearHighlight()
        {

            if (highlightList.Count == 0)
                return null;

            VR_Highlight highlight = null;
            float minDistance = float.MaxValue;

            for (int n = 0; n < highlightList.Count; n++)
            {
                if (highlightList[n].enabled && highlightList[n].CanHighlight() && highlightList[n].CanHighlightUsingController( this ))
                {
                    Transform highlightPoint = ControllerType == VR_ControllerType.Right ? highlightList[n].HighlightPointRightHand : highlightList[n].HighlightPointLeftHand;

                    if (highlightPoint != null)
                    {
                        float d = ( Position - highlightPoint.position ).magnitude;

                        if (d < minDistance && d <= highlightList[n].HighlightDistance)
                        {
                            highlight = highlightList[n];
                            minDistance = d;
                        }
                    }

                }

            }

            if (highlight == null)
            {
                highlight = activeDistanceHighlight;
            }
            else if(activeDistanceHighlight != null)
            {
                activeDistanceHighlight.UnHighlight(this);
            }


            return highlight;
        }

        //should the position be controller by the engine or you want to control it manually,
        //useful for snap the hand to certain positions
        public void SetPositionControlMode(MotionControlMode controlMode)
        {
            controlPositionMode = controlMode;

            transform.parent = controlMode == MotionControlMode.Free ? null : originalParent;

            if (controlMode == MotionControlMode.Engine)
            {
                transform.localPosition = initialPosition;
                transform.localRotation = initialRotation;
            }

        }

        public void SetRotationControlMode(MotionControlMode controlMode)
        {
            controlRotationMode = controlMode;
        }

        public void SetPositionAndRotationControlMode(MotionControlMode positionControlMode , MotionControlMode rotationControlMode)
        {
            SetPositionControlMode(positionControlMode);
            SetRotationControlMode(rotationControlMode);
        }

        public void SetVisibility(bool visibility)
        {
            handGO.SetActive(visibility);
        }

        public void SetActiveDistanceGrabbable(VR_Grabbable grabbable)
        {
            activeDistanceGrabbable = grabbable;

            if(activeDistanceHighlight != null)
                activeDistanceHighlight.UnHighlight( this );


            if (grabbable != null)
            {
                activeDistanceHighlight = grabbable.GetComponent<VR_Highlight>();
            }
            else
            {
                activeDistanceHighlight = null;
            }
           
        }

    }

}

