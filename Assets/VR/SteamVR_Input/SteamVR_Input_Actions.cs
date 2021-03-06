//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Action_Boolean p_vRShooterKit_Grab;
        
        private static SteamVR_Action_Boolean p_vRShooterKit_Shoot;
        
        private static SteamVR_Action_Vector2 p_vRShooterKit_Joystick;
        
        private static SteamVR_Action_Boolean p_vRShooterKit_JoystickPress;
        
        private static SteamVR_Action_Boolean p_vRShooterKit_PrimaryButton;
        
        private static SteamVR_Action_Boolean p_vRShooterKit_SecondaryButton;
        
        private static SteamVR_Action_Pose p_vRShooterKit_LeftHand;
        
        private static SteamVR_Action_Pose p_vRShooterKit_RightHand;
        
        private static SteamVR_Action_Vibration p_vRShooterKit_Haptic;
        
        public static SteamVR_Action_Boolean vRShooterKit_Grab
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_Grab.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean vRShooterKit_Shoot
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_Shoot.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Vector2 vRShooterKit_Joystick
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_Joystick.GetCopy<SteamVR_Action_Vector2>();
            }
        }
        
        public static SteamVR_Action_Boolean vRShooterKit_JoystickPress
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_JoystickPress.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean vRShooterKit_PrimaryButton
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_PrimaryButton.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean vRShooterKit_SecondaryButton
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_SecondaryButton.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Pose vRShooterKit_LeftHand
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_LeftHand.GetCopy<SteamVR_Action_Pose>();
            }
        }
        
        public static SteamVR_Action_Pose vRShooterKit_RightHand
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_RightHand.GetCopy<SteamVR_Action_Pose>();
            }
        }
        
        public static SteamVR_Action_Vibration vRShooterKit_Haptic
        {
            get
            {
                return SteamVR_Actions.p_vRShooterKit_Haptic.GetCopy<SteamVR_Action_Vibration>();
            }
        }
        
        private static void InitializeActionArrays()
        {
            Valve.VR.SteamVR_Input.actions = new Valve.VR.SteamVR_Action[] {
                    SteamVR_Actions.vRShooterKit_Grab,
                    SteamVR_Actions.vRShooterKit_Shoot,
                    SteamVR_Actions.vRShooterKit_Joystick,
                    SteamVR_Actions.vRShooterKit_JoystickPress,
                    SteamVR_Actions.vRShooterKit_PrimaryButton,
                    SteamVR_Actions.vRShooterKit_SecondaryButton,
                    SteamVR_Actions.vRShooterKit_LeftHand,
                    SteamVR_Actions.vRShooterKit_RightHand,
                    SteamVR_Actions.vRShooterKit_Haptic};
            Valve.VR.SteamVR_Input.actionsIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.vRShooterKit_Grab,
                    SteamVR_Actions.vRShooterKit_Shoot,
                    SteamVR_Actions.vRShooterKit_Joystick,
                    SteamVR_Actions.vRShooterKit_JoystickPress,
                    SteamVR_Actions.vRShooterKit_PrimaryButton,
                    SteamVR_Actions.vRShooterKit_SecondaryButton,
                    SteamVR_Actions.vRShooterKit_LeftHand,
                    SteamVR_Actions.vRShooterKit_RightHand};
            Valve.VR.SteamVR_Input.actionsOut = new Valve.VR.ISteamVR_Action_Out[] {
                    SteamVR_Actions.vRShooterKit_Haptic};
            Valve.VR.SteamVR_Input.actionsVibration = new Valve.VR.SteamVR_Action_Vibration[] {
                    SteamVR_Actions.vRShooterKit_Haptic};
            Valve.VR.SteamVR_Input.actionsPose = new Valve.VR.SteamVR_Action_Pose[] {
                    SteamVR_Actions.vRShooterKit_LeftHand,
                    SteamVR_Actions.vRShooterKit_RightHand};
            Valve.VR.SteamVR_Input.actionsBoolean = new Valve.VR.SteamVR_Action_Boolean[] {
                    SteamVR_Actions.vRShooterKit_Grab,
                    SteamVR_Actions.vRShooterKit_Shoot,
                    SteamVR_Actions.vRShooterKit_JoystickPress,
                    SteamVR_Actions.vRShooterKit_PrimaryButton,
                    SteamVR_Actions.vRShooterKit_SecondaryButton};
            Valve.VR.SteamVR_Input.actionsSingle = new Valve.VR.SteamVR_Action_Single[0];
            Valve.VR.SteamVR_Input.actionsVector2 = new Valve.VR.SteamVR_Action_Vector2[] {
                    SteamVR_Actions.vRShooterKit_Joystick};
            Valve.VR.SteamVR_Input.actionsVector3 = new Valve.VR.SteamVR_Action_Vector3[0];
            Valve.VR.SteamVR_Input.actionsSkeleton = new Valve.VR.SteamVR_Action_Skeleton[0];
            Valve.VR.SteamVR_Input.actionsNonPoseNonSkeletonIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.vRShooterKit_Grab,
                    SteamVR_Actions.vRShooterKit_Shoot,
                    SteamVR_Actions.vRShooterKit_Joystick,
                    SteamVR_Actions.vRShooterKit_JoystickPress,
                    SteamVR_Actions.vRShooterKit_PrimaryButton,
                    SteamVR_Actions.vRShooterKit_SecondaryButton};
        }
        
        private static void PreInitActions()
        {
            SteamVR_Actions.p_vRShooterKit_Grab = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/VRShooterKit/in/Grab")));
            SteamVR_Actions.p_vRShooterKit_Shoot = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/VRShooterKit/in/Shoot")));
            SteamVR_Actions.p_vRShooterKit_Joystick = ((SteamVR_Action_Vector2)(SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/VRShooterKit/in/Joystick")));
            SteamVR_Actions.p_vRShooterKit_JoystickPress = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/VRShooterKit/in/JoystickPress")));
            SteamVR_Actions.p_vRShooterKit_PrimaryButton = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/VRShooterKit/in/PrimaryButton")));
            SteamVR_Actions.p_vRShooterKit_SecondaryButton = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/VRShooterKit/in/SecondaryButton")));
            SteamVR_Actions.p_vRShooterKit_LeftHand = ((SteamVR_Action_Pose)(SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/VRShooterKit/in/LeftHand")));
            SteamVR_Actions.p_vRShooterKit_RightHand = ((SteamVR_Action_Pose)(SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/VRShooterKit/in/RightHand")));
            SteamVR_Actions.p_vRShooterKit_Haptic = ((SteamVR_Action_Vibration)(SteamVR_Action.Create<SteamVR_Action_Vibration>("/actions/VRShooterKit/out/Haptic")));
        }
    }
}
