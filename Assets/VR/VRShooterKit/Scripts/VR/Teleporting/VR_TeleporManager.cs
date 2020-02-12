using UnityEngine;
using System.Collections.Generic;

namespace VRShooterKit.Locomotion
{
    public enum TeleporState
    {
        WaitingInput,
        PreTeleport,
        PostTeleport
    }

    //this script makes use of all the teleport components and handles the logic for teleporting
    public class VR_TeleporManager : MonoBehaviour
    {
        [SerializeField] private VR_TeleportLineRenderer teleportLineRender = null;
        [SerializeField] private VR_TeleportAimHandler aimHandler = null;
        [SerializeField] private VR_AimRaycaster aimRaycaster = null;
        [SerializeField] private VR_TeleportMarker teleportMarker = null;
        [SerializeField] private VR_TeleportHandler teleportHandler = null;
        [SerializeField] private CharacterController characterController = null;
        [SerializeField] private VR_InputButton teleportEnablerButton = VR_InputButton.Button_Trigger;    
        

        private VR_Controller leftController = null;
        private VR_Controller rightController = null;
        private VR_Controller activeController = null;
        private VR_Controller lastActiveController = null;
        private AimRaycastInfo lastRaycastInfo = null;
        private TeleporState currentTeleportState = TeleporState.WaitingInput;
        private bool isTeleporting = false;

        private void Start()
        {
            rightController = VR_Manager.instance.RightController;
            leftController = VR_Manager.instance.LeftController;
        }       

        private void LateUpdate()
        {           

            switch (currentTeleportState)
            {
                case TeleporState.WaitingInput:
                WaitingInputUpdate();
                break;

                case TeleporState.PreTeleport:
                PreTeleportUpdate();
                break;

                case TeleporState.PostTeleport:
                PostTeleportUpdate();
                break;
            }
        }

        private void WaitingInputUpdate()
        {
            if (isTeleporting)
                return;

            //wait for the teleport to start
            if (leftController.Input.GetButtonDown( teleportEnablerButton ) || rightController.Input.GetButtonDown( teleportEnablerButton ))
            {
                UpdateActiveController();
                currentTeleportState = TeleporState.PreTeleport;
            }
        }

        //set the controller that makes the teleport intent
        private void UpdateActiveController()
        {
            activeController = GetActiveController();
            lastActiveController = activeController;
        }

        private VR_Controller GetActiveController()
        {
            if (leftController.Input.GetButtonDown( teleportEnablerButton ) && rightController.Input.GetButtonDown( teleportEnablerButton ))
            {
                return lastActiveController == null ? rightController : lastActiveController;
            }

            if (leftController.Input.GetButtonDown( teleportEnablerButton ))
                return leftController;


            if (rightController.Input.GetButtonDown( teleportEnablerButton ))
                return rightController;

            return null;

        }

        //wait for the player to decide where to teleport
        private void PreTeleportUpdate()
        {
            UpdateActiveController();

            //there is no active controller try to do a teleport
            if (activeController == null)
            {
                //if we can teleport to the last AimRaycast
                if (IsAimRaycastInfoSuitableForTeleporting(lastRaycastInfo))
                {
                    DoTeleport( lastRaycastInfo );
                }

                //clean the line inmediatly
                teleportLineRender.CleanRender();

                //go to post teleport
                currentTeleportState = TeleporState.PostTeleport;
                return;
            }

            Ray controllerRay = new Ray( activeController.transform.position, activeController.transform.forward );

            //use the aimhandler to generate all the line points
            List<Vector3> points = aimHandler.GetAllPoints( controllerRay );
            //use the raycaster
            AimRaycastInfo info = aimRaycaster.Raycast( points , activeController.transform );

            if (info != null)
            {
                teleportLineRender.Render( info.validPoints, info.suitableForTeleporting );
            }
            else
            {
                teleportLineRender.Render( points , false );
            }           

            if (IsAimRaycastInfoSuitableForTeleporting( info ))
            {
                teleportMarker.UpdatePositionAndRotation( activeController, info );
               
            }
            else
            {
                teleportMarker.Hide();
            }

            lastRaycastInfo = info;
        }

        private bool IsAimRaycastInfoSuitableForTeleporting(AimRaycastInfo info)
        {
            return info != null && info.suitableForTeleporting;
        }

        private void DoTeleport(AimRaycastInfo info)
        {
            isTeleporting = true;
            teleportHandler.DoTeleport( characterController , teleportMarker.Marker.transform , delegate { isTeleporting = false; } );
        }

        private void PostTeleportUpdate()
        {            
            teleportMarker.Hide();
            lastActiveController = null;
            lastRaycastInfo = null;

            currentTeleportState = TeleporState.WaitingInput;
        }
       
    }

}

