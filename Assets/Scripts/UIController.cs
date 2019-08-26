using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour {
    
    public static UIController instance;
    public TextMeshProUGUI banner;
    public List<Color> speakerColors;

    public Script currentScript;
    public int scriptIndex;


    public bool scriptActive = false;
    private bool textAdvanced;
    private Animator anim;

    public enum TimeMode {idle, stop, resume};
    public TimeMode timeMode = TimeMode.idle;
    public AnimationCurve timeCurve;
    public float timeScaleTime;
    private float timeScaleTimer;

    void Awake() {
        instance = this;
        anim = GetComponent<Animator>();
        banner.enabled = false;
        timeScaleTimer = timeScaleTime;
    }

    void Update() {
        if (scriptActive) {
            if (Input.GetKey(KeyCode.Space) && textAdvanced) {
                scriptIndex++;
                textAdvanced = false;
                if (scriptIndex == currentScript.events.Count) {
                     anim.SetTrigger("EndScript");
                 } else {
                    if (currentScript.events[scriptIndex].eventName == "") {
                        anim.SetTrigger("AdvanceText");
                    } else {
                        anim.SetBool("LastEvent", scriptIndex == currentScript.events.Count - 1);
                        anim.SetTrigger("FadeOut");
                    }
                }
                
            }
        }

        if (timeMode == TimeMode.stop && timeScaleTimer < timeScaleTime) {
            timeScaleTimer += Time.unscaledDeltaTime;
            Time.timeScale = timeCurve.Evaluate(timeScaleTimer / timeScaleTime);
            Time.fixedDeltaTime = timeCurve.Evaluate(timeScaleTimer / timeScaleTime) * 0.02f;
            if (timeScaleTimer >= timeScaleTime) {
                Time.timeScale = 0;
                SetBannerText(currentScript.events[scriptIndex]);
                scriptActive = true;
                anim.SetTrigger("LoadScript");
                banner.enabled = true;
            }
        }

        if (timeMode == TimeMode.resume) {
            timeScaleTimer -= Time.unscaledDeltaTime;
            Time.timeScale = timeCurve.Evaluate(timeScaleTimer / timeScaleTime);
            Time.fixedDeltaTime = timeCurve.Evaluate(timeScaleTimer / timeScaleTime) * 0.02f;
            if (timeScaleTimer < 0) {
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f;
                timeMode = TimeMode.idle;
            }
        }
    }

    public void SetBannerText(ScriptEvent e) {
        string text = "<color=#" + ColorUtility.ToHtmlStringRGB(speakerColors[e.speaker]) + ">" + e.text + "</color>";
        banner.SetText(text);
    }

    public void AdvanceText() {
        SetBannerText(currentScript.events[scriptIndex]);
    }

    public void LoadScript(Script s) {
        currentScript = s;
        scriptIndex = 0;
        if (!currentScript.pauseTime) {
            SetBannerText(currentScript.events[scriptIndex]);
            scriptActive = true;
            anim.SetTrigger("LoadScript");
            banner.enabled = true;
        } else {
            timeScaleTimer = 0f;
            timeMode = TimeMode.stop;
        }
        
    }

    public void EndScript() {
        banner.enabled = false;
        if (currentScript.pauseTime) {
            timeMode = TimeMode.resume;
            timeScaleTimer = timeScaleTime;
        }
        
    }

    public void StartSpecialEvent() {
        banner.enabled = false;
        GameManager.instance.SpecialEvent(currentScript.events[scriptIndex].eventName);
    }

    public void ResumeScript() {
        scriptIndex++;
        if (scriptIndex == currentScript.events.Count) {
            EndScript();
            
        } else {
            SetBannerText(currentScript.events[scriptIndex]);
            anim.SetTrigger("AdvanceText");
            banner.enabled = true;
        }
    }

    public void AllowAdvance() {
        textAdvanced = true;
    }

}
