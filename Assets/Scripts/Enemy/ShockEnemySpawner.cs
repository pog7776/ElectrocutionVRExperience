using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockEnemySpawner : MonoBehaviour{

    [SerializeField]
    private float spawnRate = 7.5f;

    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private Transform spawnPoint;

    private bool canSpawn;
    // Start is called before the first frame update
    void Start(){
        EnemyManager.instance.AddToSpawnerList(this);

        canSpawn = true;

        if(enemy == null){
            Debug.LogWarning("Enemy to spawn not found.");
        }

        if(spawnPoint == null){
            Debug.LogWarning("Spawn point not found.");
        }
    }

    // Update is called once per frame
    void Update(){
        if(canSpawn){
            Spawn();
        }
    }

    public void Spawn(){
        canSpawn = false;
        StartCoroutine(SpawnTimer(spawnRate));
        Instantiate(enemy, spawnPoint.position, Quaternion.Euler(gameObject.transform.rotation.eulerAngles));
    }

    public IEnumerator SpawnTimer(float time){
        yield return new WaitForSeconds(time);
        canSpawn = true;
    }

    public void SetCanSpawn(bool state){
        canSpawn = state;
    }

    public float GetSpawnRate(){
        return spawnRate;
    }
}
