﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour{

    [SerializeField]
    private HitboxSide side;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other){
        //if(other.gameObject.)
        //if(other.gameObject.GetComponent<bulletwhatever>.type == bullet){
            // Do damage and shock
        //}
    }
}

public enum HitboxSide{
    Left,
    Right
}