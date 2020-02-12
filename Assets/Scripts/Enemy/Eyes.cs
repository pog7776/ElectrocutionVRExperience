using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour{

    #region Eyes

    [SerializeField]
    private GameObject LeftEye;
    private Rigidbody LeftEyeRB;
    [SerializeField]
    private GameObject RightEye;
    private Rigidbody RightEyeRB;

    #endregion

    #region Lookat

    [SerializeField]
    public GameObject restingPos;
    private Rigidbody restingPosRB;
    [SerializeField]
    private float lookatStrength = 10f;

    [HideInInspector]
    public GameObject target;
    
    #endregion

    #region Private Variables

    private GameObject lookatTarget;

    #endregion

    // Start is called before the first frame update
    void Start(){
        restingPosRB = restingPos.GetComponent<Rigidbody>();
        LeftEyeRB = LeftEye.GetComponent<Rigidbody>();
        RightEyeRB = RightEye.GetComponent<Rigidbody>();
        InitializeChecks();
    }

    // Update is called once per frame
    void Update(){
        EyesLookat();
        RestingPostoTarget();
    }

    private void InitializeChecks(){
        // Check for eyes
        if(LeftEye == null){
            Debug.LogWarning("Enemy left eye not found.");
        }
        if(RightEye == null){
            Debug.LogWarning("Enemy right eye not found.");
        }
        // Check for resting pos
        if(restingPos == null){
            Debug.LogWarning("Enemy resting position not found.");
        }
    }

    private void EyesLookat(){
        LeftEye.transform.LookAt(restingPos.transform);
        RightEye.transform.LookAt(restingPos.transform);
    }

    private void RestingPostoTarget(){
        if(target != null){
            Vector3 dir = (this.transform.position - target.transform.position).normalized;
            restingPosRB.AddForce(dir * (Vector3.Distance(target.transform.position, this.transform.position)) * -lookatStrength , ForceMode.Acceleration);
        }
    }

    public void SetTarget(GameObject target){
        this.target = target;
        restingPosRB.AddForce(target.transform.position, ForceMode.Impulse);
    }
}
