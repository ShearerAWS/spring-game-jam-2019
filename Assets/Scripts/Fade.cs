using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fade : MonoBehaviour {
    
    public static Fade instance;
    public Color alpha, full;

    void Awake() {
        GetComponent<Image>().color = alpha;
        instance = this;
    }

    public void Trigger() {
        GetComponent<Image>().color = full;
        DOTween.To(()=>full,x=>GetComponent<Image>().color = x,alpha,.4f).SetUpdate(true);
    }

    public void FadeOut() {
        GetComponent<Image>().color = alpha;
        DOTween.To(()=>alpha,x=>GetComponent<Image>().color = x,full,3f).SetUpdate(true);
    }

    public void FadeIn() {
        GetComponent<Image>().color = full;
        DOTween.To(()=>full,x=>GetComponent<Image>().color = x,alpha,3f).SetUpdate(true);
    }
}
