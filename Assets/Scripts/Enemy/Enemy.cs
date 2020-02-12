using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{

    #region Vision

    [SerializeField]
    private GameObject visionObject;
    private Collider visionCollider;

    #endregion

    #region Targeting
    
    [SerializeField]
    private float rotationStrength = 1;
    [SerializeField]
    private float aimConeSize = 0.5f;
    [HideInInspector]
    public GameObject target;

    #endregion

    #region Movement

    [SerializeField]
    private float enemySpeed = 15;

    [SerializeField]
    private float idealDistance = 10;

    #endregion

    private Eyes eyes;
    private Rigidbody enemyRB;
    private EnemyGun enemyGun;

    // Start is called before the first frame update
    void Start(){
        visionCollider = visionObject.GetComponent<Collider>();
        eyes = gameObject.GetComponent<Eyes>();
        enemyRB = gameObject.GetComponent<Rigidbody>();
        enemyGun = gameObject.GetComponent<EnemyGun>();
    }

    // Update is called once per frame
    void Update(){
        if(target != null){
            RotateTowardsTarget();
            MoveToPosition();
        }
    }

    private void InitializeChecks(){
        if(visionObject == null){
            Debug.LogWarning("Enemy vision object missing!");
        }
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            SetTarget(other);
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other){
        if(other.tag == "Player"){
            SetTarget(null);
            eyes.SetTarget(null);
        }
    }

    private void SetTarget(Collider target){
        this.target = target.gameObject;
        // Set aim target (weird)
        enemyGun.SetAimTarget(target.transform.Find("AimTargetPointer").GetComponent<AimTargetPointer>().GetAimTarget());
        // Tell eyes to look at target
        eyes.SetTarget(target.gameObject);
    }

    private void RotateTowardsTarget(){
        //gameObject.transform.LookAt(eyes.restingPos.transform);
        //Vector3 dir = (this.transform.position - eyes.restingPos.transform.position).normalized;
        if(target.tag == "Player"){
            //Debug.Log("Enemy direction: " + dir);
            Vector3 dir = (this.transform.position - target.transform.position).normalized;

            Vector3 heading = target.transform.position - transform.position;
            float dirNum = AngleDir(transform.forward, heading, transform.up);

            if(dirNum > 0){
                enemyRB.AddRelativeTorque(dir * rotationStrength, ForceMode.Force);
            }
            else if(dirNum < 0){
                enemyRB.AddRelativeTorque(dir * -rotationStrength, ForceMode.Force);
            }
            
            
            if(dirNum > -aimConeSize && dirNum < aimConeSize){
                if(enemyGun.canFire){
                    enemyGun.Fire();
                }
            }
        }
    }

    private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);

        return dir;
		
		// if (dir > 0f) {
		// 	return 1f;
		// } else if (dir < 0f) {
		// 	return -1f;
		// } else {
		// 	return 0f;
		// }
	}

    private void MoveToPosition(){
        if(Vector3.Distance(this.transform.position, target.transform.position) > idealDistance){
            enemyRB.AddRelativeForce(Vector3.forward * enemySpeed, ForceMode.Force);
        }
        else if(Vector3.Distance(this.transform.position, target.transform.position) < idealDistance){
            enemyRB.AddRelativeForce(Vector3.forward * -enemySpeed, ForceMode.Force);
        }
    }
}


// TODO Damage