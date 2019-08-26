using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTrigger : MonoBehaviour {
    
    public Script script;
    public bool triggered = false;

    void Start() {
        GameManager.instance.restart += Restart;
    }

    private void Restart() {
        triggered = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!triggered && other.CompareTag("Player")) {
            triggered = true;
            UIController.instance.LoadScript(script);
        }
    }
}
