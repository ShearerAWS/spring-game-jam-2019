using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance;
    public bool playStart;
    public Script startScript;
    public CinemachineVirtualCamera startCam;
    public Transform playerStart;
    public Color startColor, mainColor, startAlpha;
    public GameObject startDoor;
    public ParticleSystem absorb;
    public GameObject spike1, spike2, spike3;
    public GameObject spikes;
    public Vector3 startScale;

 
    public delegate void Event();
    public Event restart;

    void Awake() {
        instance = this;
    }

    void Start() {
        if (playStart) {
            startCam.m_Priority = 10;
            PlayerController.instance.GetComponent<SpriteRenderer>().color = startColor;
            startDoor.transform.localScale = startScale;
            PlayerController.instance.gameObject.SetActive(false);
            PlayerController.instance.transform.position = playerStart.position;
            PlayerController.instance.allowWallJump = false;
            spike1.GetComponent<SpriteRenderer>().color = startAlpha;
            spike2.GetComponent<SpriteRenderer>().color = startAlpha;
            spike3.GetComponent<SpriteRenderer>().color = startAlpha;
            spikes.SetActive(false);
            StartCoroutine("WaitThenStart");
        } else {
            startCam.m_Priority = -10;
            startDoor.SetActive(false);
       }
    }
    
    IEnumerator WaitThenStart() {
        yield return new WaitForSeconds(.6f);
        UIController.instance.LoadScript(startScript);
    }
    public void SpecialEvent(string name) {
        StartCoroutine(name);
    }

    private IEnumerator FixColor() {
        absorb.transform.position = PlayerController.instance.transform.position;
        absorb.Play();
        yield return new WaitForSecondsRealtime(.5f);
        DOTween.To(()=>startColor,x=>PlayerController.instance.GetComponent<SpriteRenderer>().color = x,mainColor,1).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1.5f);
        UIController.instance.ResumeScript();
    }

    private IEnumerator RespawnPlayer() {
        yield return null;
        Fade.instance.Trigger();
        PlayerController.instance.Respawn();
        UIController.instance.ResumeScript();
    }

    private IEnumerator SpawnPlayer() {
        PlayerController.instance.gameObject.SetActive(true);
        PlayerController.instance.GetComponent<Rigidbody2D>().isKinematic = true;
        PlayerController.instance.GetComponent<SpriteRenderer>().color = startAlpha;
        DOTween.To(()=>startAlpha,x=>PlayerController.instance.GetComponent<SpriteRenderer>().color = x,startColor,1).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1);
        PlayerController.instance.GetComponent<Rigidbody2D>().isKinematic = false;
        UIController.instance.ResumeScript();
    }

    private IEnumerator MainCameraSwitch() {
        startDoor.transform.DOScaleX(0,1);
        yield return new WaitForSecondsRealtime(1f);
        startDoor.SetActive(false);
        startCam.m_Priority = -10;
        UIController.instance.ResumeScript();
    }

    private IEnumerator DangerReveal() {
        spike1.GetComponent<SpriteRenderer>().color = startAlpha;
        spike2.GetComponent<SpriteRenderer>().color = startAlpha;
        spike3.GetComponent<SpriteRenderer>().color = startAlpha;
        spikes.SetActive(true);
        DOTween.To(()=>startAlpha,x=>spike1.GetComponent<SpriteRenderer>().color = x,startColor,1).SetUpdate(true);
        DOTween.To(()=>startAlpha,x=>spike2.GetComponent<SpriteRenderer>().color = x,startColor,1).SetUpdate(true);
        DOTween.To(()=>startAlpha,x=>spike3.GetComponent<SpriteRenderer>().color = x,startColor,1).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1f);
        UIController.instance.ResumeScript();
    }

    private IEnumerator WallJumpAdd() {
        absorb.transform.position = PlayerController.instance.transform.position;
        absorb.Play();
        yield return new WaitForSecondsRealtime(2f);
        PlayerController.instance.allowWallJump = true;
        UIController.instance.ResumeScript();
    }

     private IEnumerator FadeBackIn() {
        startCam.m_Priority = 10;
        PlayerController.instance.GetComponent<SpriteRenderer>().color = startColor;
        PlayerController.instance.gameObject.SetActive(false);
        PlayerController.instance.transform.position = playerStart.position;
        PlayerController.instance.allowWallJump = false;
        startDoor.transform.localScale = startScale;
        startDoor.SetActive(true);
        spike1.GetComponent<SpriteRenderer>().color = startAlpha;
        spike2.GetComponent<SpriteRenderer>().color = startAlpha;
        spike3.GetComponent<SpriteRenderer>().color = startAlpha;
        spikes.SetActive(false);
        Fade.instance.FadeIn();
        yield return new WaitForSecondsRealtime(3f);
        UIController.instance.ResumeScript();
    }

    private IEnumerator EndEnd() {
        UIController.instance.ResumeScript();
        yield return null;
        restart();
        StartCoroutine("WaitThenStart");
    }
}

    
