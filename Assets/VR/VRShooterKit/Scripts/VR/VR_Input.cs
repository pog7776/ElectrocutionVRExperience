using UnityEngine;
#if SDK_STEAM_VR
using Valve.VR;
#endif

namespace VRShooterKit
{
    public class VR_Input 
    {
        private VR_Controller controller = null;

#if SDK_STEAM_VR
        private SteamVR_Action_Boolean grabAction = null;
        private SteamVR_Action_Boolean triggerAction = null;
        private SteamVR_Action_Boolean primaryButtonAction = null;
        private SteamVR_Action_Boolean secondaryButtonAction = null;
        private SteamVR_Action_Boolean joystickPressAction = null;
        private SteamVR_Action_Vector2 joystickInputAction = null;
#endif

        public VR_Input(VR_Controller controller)
        {
            this.controller = controller;

            #if SDK_STEAM_VR
            InitializeSteamVR_Actions();
            #endif
        }

#if SDK_STEAM_VR
        private void InitializeSteamVR_Actions()
        {
            grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>( "VRShooterKit", "Grab" );
            triggerAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>( "VRShooterKit", "Shoot" );
            primaryButtonAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>( "VRShooterKit", "PrimaryButton" );
            secondaryButtonAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>( "VRShooterKit", "SecondaryButton" );
            joystickPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>( "VRShooterKit", "JoystickPress" );
            joystickInputAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>( "VRShooterKit", "Joystick" );
        }
#endif

#if PLACEHOLDER_DEFINE
        this is a error
#endif

        /// <summary>
        /// Get input from this controller
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetButtonDown(VR_InputButton button)
        {

            switch (button)
            {
                case VR_InputButton.Button_Trigger:
                {
#if SDK_OCULUS
                    
                    OVRInput.Axis1D axis = OVRInput.Axis1D.PrimaryIndexTrigger;
                    axis = OVRInput.Axis1D.PrimaryIndexTrigger;

                    return ( OVRInput.Get( axis, controller.OVR_ControllerType ) > 0.75f );  


#endif
#if SDK_STEAM_VR
                    return triggerAction.GetState(controller.SteamControllerType);
#endif
                    return false;
                }
                case VR_InputButton.Button_Grip:
                {
#if SDK_OCULUS

                    OVRInput.Axis1D axis;
                    axis = OVRInput.Axis1D.PrimaryHandTrigger;

                    return ( OVRInput.Get( axis, controller.OVR_ControllerType ) > 0.75f );                  
#endif
#if SDK_STEAM_VR
                    return grabAction.GetState( controller.SteamControllerType );
#endif
                    return false;
                }

                case VR_InputButton.Button_Tumbstick:
                {
                    Vector2 joystickInput = GetJoystickInput();

                    return ( joystickInput.magnitude > 0.5f );
                }

                case VR_InputButton.Button_Thumbstick_Up:
                {
                    Vector2 joystickInput = GetJoystickInput();

                    return ( joystickInput.y > 0.5f );
                }

                case VR_InputButton.Button_Thumbstick_Down:
                {
                    Vector2 joystickInput = GetJoystickInput();

                    return ( joystickInput.y < -0.5f );
                }

                case VR_InputButton.Button_Thumbstick_Right:
                {
                    Vector2 joystickInput = GetJoystickInput();

                    return ( joystickInput.x > 0.5f );
                }

                case VR_InputButton.Button_Thumbstick_Left:
                {
                    Vector2 joystickInput = GetJoystickInput();

                    return ( joystickInput.x < -0.5f );
                }

                case VR_InputButton.Button_Primary:
                {
#if SDK_OCULUS
                    return OVRInput.Get( OVRInput.Button.One, controller.OVR_ControllerType );
#endif
#if SDK_STEAM_VR
                    return primaryButtonAction.GetState(controller.SteamControllerType);
#endif
                    return false;
                }

                case VR_InputButton.Button_Secondary:
                {
#if SDK_OCULUS
                    return OVRInput.Get( OVRInput.Button.Two, controller.OVR_ControllerType );
#endif
#if SDK_STEAM_VR
                    return secondaryButtonAction.GetState(controller.SteamControllerType);
#endif
                    return false;
                }
                case VR_InputButton.Button_TumbstickPress:
#if SDK_OCULUS
                return OVRInput.Get( OVRInput.Button.PrimaryThumbstick, controller.OVR_ControllerType );
#endif
#if SDK_STEAM_VR
                return joystickPressAction.GetState(controller.SteamControllerType);
#endif
                return false;


                default:
                return false;
            }
        }


        public Vector2 GetJoystickInput()
        {
#if SDK_OCULUS
            OVRInput.Axis2D axis = OVRInput.Axis2D.PrimaryThumbstick;
            return OVRInput.Get( axis, controller.OVR_ControllerType );
#endif

#if SDK_STEAM_VR
            return joystickInputAction.GetAxis(controller.SteamControllerType);
#endif

            return Vector2.zero;
        }

    }
}

