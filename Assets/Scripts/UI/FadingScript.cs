using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingScript : MonoBehaviour
{
    private bool isActive = false;
    public float Duration = 2f;
    public bool ToggleButton;

    #region Singleton Things
    private static FadingScript _instance;
    public static FadingScript Instance { get { return _instance; } }
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeFromBlack(Duration));
    }

    public void FadeScreen() {
        if (isActive) {
            StartCoroutine(FadeToBlack(Duration));
        } else {
            StartCoroutine(FadeFromBlack(Duration));
        }
        isActive = !isActive;
    }

    public void FadeScreen(bool isActive, float duration = 2f) {
        if (isActive) {
            StartCoroutine(FadeToBlack(duration));
        } else {
            StartCoroutine(FadeFromBlack(duration));
        }
        this.isActive = !isActive;
    }
    float timeStep = 0.01f;
    private IEnumerator FadeToBlack(float duration) {
        Debug.Log("StartFade");
        Time.timeScale = 0f;
        Image sr = GetComponent<Image>();
        float t = 0;
        Color color = sr.color;
        sr.enabled = true;
        while (t < duration) {
            color.a = Mathf.Clamp01(t / duration );
            sr.color = color;
            t += timeStep;
            yield return new WaitForSecondsRealtime(timeStep);
        }
        color.a = 1;
        sr.color = color;
        sr.enabled = true;
        Debug.Log("EndFade");
    }
    
    private IEnumerator FadeFromBlack (float duration) {
        Debug.Log("StartFade");
        Time.timeScale = 0f;
        Image sr = GetComponent<Image>();
        sr.enabled = true;
        float t = 0;
        Color color = sr.color;
        sr.enabled = true;
        while (t < duration) {
            color.a = 1 - Mathf.Clamp01(t / duration);
            sr.color = color;
            t += timeStep;
            yield return new WaitForSecondsRealtime(timeStep);
        }
        color.a = 0;
        sr.color = color;
        sr.enabled = false;
        Time.timeScale = 1f;
        Debug.Log("EndFade");
    }
     
}
