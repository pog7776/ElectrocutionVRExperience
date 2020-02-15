using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Platinio.Singleton<EnemyManager>{

    [SerializeField]
    private int maxEnemies = 10;

    private List<GameObject> enemyList;
    private List<ShockEnemySpawner> enemySpawners;


    protected override void Awake(){
        enemyList = new List<GameObject>();
        enemySpawners = new List<ShockEnemySpawner>();
        
        base.Awake();  // make sure base init stuff gets done
    }

    protected override void Start(){
        base.Start();  // make sure base init stuff gets done
    }

    // Update is called once per frame
    void Update(){

        if(enemyList.Count >= maxEnemies){
            foreach(ShockEnemySpawner spawner in enemySpawners){
                spawner.SetCanSpawn(false);
            }
        }
        else{
            foreach(ShockEnemySpawner spawner in enemySpawners){
                //StartCoroutine(spawner.SpawnTimer(spawner.GetSpawnRate()));
                spawner.SetCanSpawn(true);
            }
        }

    }

    public void AddToEnemyList(GameObject enemy){
        enemyList.Add(enemy);
    }

    public void RemoveFromEnemyList(GameObject enemy){
        if(enemyList.Contains(enemy)){
            enemyList.Remove(enemy);
        }
    }


    public void AddToSpawnerList(ShockEnemySpawner spawner){
        enemySpawners.Add(spawner);
    }

    public void RemoveFromSpawnerList(ShockEnemySpawner spawner){
        if(enemySpawners.Contains(spawner)){
            enemySpawners.Remove(spawner);
        }
    }
}
