using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hibox : MonoBehaviour{

    private Enemy enemy;

    // Start is called before the first frame update
    void Start(){
        enemy = gameObject.GetComponentInParent<Enemy>();
        if(enemy == null){
            Debug.LogWarning("Enemy Hitbox cannot find Enemy script in parent.");
        }
    }

    
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "PlayerProjectile"){
            enemy.DoDamage(enemy.defaultDamageTaken);
            Destroy(other.gameObject);
        }

        if(other.gameObject.tag == "PlayerMelee"){
            enemy.DoDamage(300f);
        }
        
    }
}
