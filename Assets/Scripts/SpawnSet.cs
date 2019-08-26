using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnSet : MonoBehaviour {
    
    bool triggered = false;
    public ParticleSystem particle;

    public Quaternion rot;
    public Vector3 scale;

    void Start() {
        GameManager.instance.restart += Restart;
        rot = transform.rotation;
        scale = transform.localScale;
    }

    private void Restart() {
        particle.Play();
        transform.rotation = rot;
        transform.localScale = scale;
        triggered = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!triggered && other.CompareTag("Player")) {
            triggered = true;
            //GetComponent<SpriteRenderer>().enabled = false;
            transform.DOScale(Vector3.zero, .5f);
            transform.DORotate(new Vector3(0,0,90),.5f);
            PlayerController.instance.SetSpawn(GetComponent<SpawnPoint>().spawnName);
            particle.Stop();
        }
    }
}
