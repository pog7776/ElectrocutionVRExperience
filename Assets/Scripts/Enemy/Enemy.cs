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
    private GameObject target;

    #endregion

    private Eyes eyes;
    private Rigidbody enemyRB;

    // Start is called before the first frame update
    void Start(){
        visionCollider = visionObject.GetComponent<Collider>();
        eyes = gameObject.GetComponent<Eyes>();
        enemyRB = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        if(target != null){
            RotateTowardsTarget();
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
        }
    }

    private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}
}
