using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Goal : MonoBehaviour {
    
    public Script ending;
    public ParticleSystem p;
    public float speed;
    public Quaternion rot;
    public Vector3 scale;

    void Start() {
        GameManager.instance.restart += Restart;
        rot = transform.rotation;
        scale = transform.localScale;
    }

    private void Restart() {
        p.Play();
        transform.rotation = rot;
        transform.localScale = scale;
        speed = -25f;
    }

    void Update() {
        transform.Rotate(0,0,Time.deltaTime * speed);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            p.Stop();
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOScale(new Vector3(2.0f,2.0f,2.0f), 1f));
            s.Join(DOTween.To(()=>speed,x=>speed=x,0,1.5f));
            s.Append(transform.DOScale(Vector3.zero, 1.5f));
            s.Join(transform.DORotate(new Vector3(0,0,-45),1.5f));
            s.AppendCallback(PostAnimation);
            s.Play();
        }
    }

    private void PostAnimation() {
        StartCoroutine("Ending");
        
    }

    IEnumerator Ending() {
        Fade.instance.FadeOut();
        yield return new WaitForSecondsRealtime(3f);
        UIController.instance.LoadScript(ending);
    }
}
