using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour{

    [SerializeField]
    private HitboxSide side;

    [SerializeField]
    private GameObject player;

    private VRShooterKit.DamageSystem.DamageableManager playerHealth;

    // Start is called before the first frame update
    void Start(){
        playerHealth = player.GetComponent<VRShooterKit.DamageSystem.DamageableManager>();
    }

    // Update is called once per frame
    void Update(){
        
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "projectile"){
            // Do damage and shock
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            
            // Damage
            playerHealth.ModifyHP(-bullet.GetDamage());
            
            // Trigger shock
            if(side == HitboxSide.Left){
                HitBoxController.Instance.ChangeChannel(1);
                HitBoxController.Instance.Trigger();
            }
            else{
                HitBoxController.Instance.ChangeChannel(2);
                HitBoxController.Instance.Trigger();
            }
        }
    }
}

public enum HitboxSide{
    Left,
    Right
}
