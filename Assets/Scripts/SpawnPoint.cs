using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    
    public static Dictionary<string, SpawnPoint> spawns;

    public string spawnName;
    public bool firstSpawn;

    void Start() {
        GameManager.instance.restart += Restart;
        if (firstSpawn) {
            PlayerController.instance.SetSpawn(spawnName);
        }
    }

    private void Restart() {
        if (firstSpawn) {
            PlayerController.instance.SetSpawn(spawnName);
        }
    }

    void Awake() {
        if (spawns == null) {
            spawns = new Dictionary<string, SpawnPoint>();
        }
        spawns.Add(spawnName, this);
        
    }

    public static SpawnPoint GetSpawnPoint(string name) {
        return spawns[name];
    }
}
