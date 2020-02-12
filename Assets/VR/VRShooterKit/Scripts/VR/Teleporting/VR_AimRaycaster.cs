using UnityEngine;
using System.Collections.Generic;

namespace VRShooterKit.Locomotion
{
    //this scripts handle the raycasting part for teleporting
    public class VR_AimRaycaster : MonoBehaviour
    {
        [Tooltip( "How many subdivisions will have the teleport ray?, this could increase the numbers of linecast per frame, be careful." )]
        [SerializeField] [Range(1 , 25)] private int collisionAccuracy = 8;
        [SerializeField] private float characterRadiusOffset = 1.5f;
        [SerializeField] private CharacterController characterController = null;
        [SerializeField] private float slopeLimit = 20.0f;
        [SerializeField] private float validAngle = 60.0f;
        [SerializeField] private float invalidRayDistance = 2.0f;
        [SerializeField] private LayerMask validLayerMask;
       
        public virtual AimRaycastInfo Raycast(List<Vector3> points , Transform rayController)
        {
            //get rayController angle
            Vector3 rayControllerForward = rayController.forward;
            Vector3 unalteredForward = new Vector3( rayController.forward.x, 0.0f, rayController.forward.z ).normalized;
            float angle = Vector3.Angle( rayController.forward, unalteredForward );

            //check if we are on a valid angle
            if (angle > validAngle)
            {
                AimRaycastInfo info = new AimRaycastInfo();
                info.hitPoint = Vector3.zero;
                info.normal = Vector3.zero;          
                //clamp ray to a distance
                info.validPoints = ClampToDistance(points , invalidRayDistance );               
                info.suitableForTeleporting = false;

                return info;

            }

            //the points are valid?
            if (points.Count <= 1)
                return null;

            RaycastHit hitInfo;

            if (points.Count == 2)
            {
                if (Physics.Linecast( points[0], points[1] , out hitInfo , validLayerMask.value , QueryTriggerInteraction.Ignore))
                {
                    return ProcessHitInfo(hitInfo , new List<Vector3> { points[0] , hitInfo.point } );
                }

                return null;
            }

            
            List<Vector3> validPoints = new List<Vector3>();            
            
            int subdivision = Mathf.CeilToInt( points.Count / collisionAccuracy);
            int currentIndex = 0;
            int nextIndex = subdivision;

            for (int n = 0; n < subdivision; n++)
            {
                //check for collision in this segment
                if (Physics.Linecast( points[currentIndex], points[nextIndex] , validLayerMask.value , QueryTriggerInteraction.Ignore))
                {
                    //found a collision in this segment
                    //find collision in all points inside this segment
                    for (int j = currentIndex; j < nextIndex; j++)
                    {
                        validPoints.Add( points[j] );

                        if (Physics.Linecast( points[j], points[j + 1], out hitInfo , validLayerMask.value , QueryTriggerInteraction.Ignore))
                        {                          
                           validPoints.Add( hitInfo.point );
                           return ProcessHitInfo(hitInfo , validPoints);                                                      
                        }

                        
                    }

                }
                else
                {
                    //this segment dont has a collision, addall as valid points
                    for (int j = currentIndex; j <= nextIndex; j++)
                    {
                        validPoints.Add( points[j] );
                    }
                }                

                //move to the next subdivision if we have one
                currentIndex += subdivision; 
                nextIndex += subdivision;

                //we found the end
                if (currentIndex >= points.Count)
                {
                    return null;
                }
                if (nextIndex >= points.Count)
                    nextIndex = points.Count - 1;
            }
            
            return null;
        }

        private AimRaycastInfo ProcessHitInfo(RaycastHit hitInfo , List<Vector3> validPoints)
        {
            Vector3 start = hitInfo.point + ( hitInfo.normal * characterController.radius * characterRadiusOffset ) + hitInfo.normal;
            Vector3 end = start + ( hitInfo.normal * characterController.height );
                       
            AimRaycastInfo info = new AimRaycastInfo();
            info.hitPoint = hitInfo.point;
            info.normal = hitInfo.normal;
            info.validPoints = validPoints;
            //check if the character can fit in this place
            info.suitableForTeleporting = GetSlopeAngle(info.normal) < slopeLimit && !Physics.CheckCapsule( start, end, characterController.radius * characterRadiusOffset , validLayerMask.value , QueryTriggerInteraction.Ignore );
                       
            return info;
        }


        private List<Vector3> ClampToDistance(List<Vector3> points , float d)
        {
            List<Vector3> validPoints = new List<Vector3>();
                        
            float currentSqrDistance = 0.0f;

            for (int n = 0; n < points.Count - 1; n++)
            {
                float distance = (points[n] - points[n + 1]).sqrMagnitude;

                validPoints.Add( points[n] );

                if (distance + currentSqrDistance < d * d)
                {
                    currentSqrDistance += distance;
                }
                else
                {
                    Vector3 dir = ( points[n + 1] - points[n] ).normalized;
                    float diff = Mathf.Abs( Mathf.Abs( Mathf.Sqrt( currentSqrDistance ) ) - Mathf.Abs( d ) );

                    validPoints.Add( points[n] + (dir * diff) );

                    return validPoints;
                }

                

            }

            return validPoints;

        }

        private float GetSlopeAngle(Vector3 surfaceNormal)
        {
            return Vector3.Angle(surfaceNormal , Vector3.up);
        }


    }

    public class AimRaycastInfo
    {
        public Vector3 hitPoint = Vector3.zero;
        public Vector3 normal = Vector3.zero;
        public List<Vector3> validPoints = new List<Vector3>();
        public bool suitableForTeleporting = false;
    }

}

