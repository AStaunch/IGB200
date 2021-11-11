using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public Image portrait;
    public AudioClip clip;
    public AudioSource sound;
    #region Singleton Things
    private static DialogueManager _instance;
    public static DialogueManager Instance { get { return _instance; } }
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion
    private Queue<string> sentences;
    public static bool isActive;

    private float textSpeed = 0.075f;
    private bool donePrinting = false;
    internal bool PAK = false;
    // Start is called before the first frame update
    void Start() {
        sentences = new Queue<string>();
        EndDialogue();
        if (PAK) {
            FadingScript.Instance.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (isActive && Input.GetKeyUp(KeyCode.Space)) {
            if (isActive && Input.GetKeyUp(KeyCode.Space) && donePrinting) {
                DisplayNextSentence();
            } else if (isActive && Input.GetKeyUp(KeyCode.Space) && !donePrinting) {
                textSpeed = 0f;
                sound.clip = null;
            }
        }

        if (isActive && Input.GetKeyUp(KeyCode.Escape)) {
            sentences.Clear();
            StopAllCoroutines();
            EndDialogue();
        }
    }

    internal void StartDialogue(Dialogue dialogue) {
        OpenDialogue();
        sentences.Clear();
        if (nameText) {
            nameText.text = dialogue.name;
        }
        if (portrait) {
            portrait.sprite = dialogue.CharacterPortrait;
        }
        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    private void DisplayNextSentence() {
        //StopCoroutine("WriteSentence");
        //Debug.Log("Display Next Dialogue");
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogText.text = "";
        StartCoroutine(WriteSentence(sentence));
    }
    string waitCmd = "/w=N";
    IEnumerator WriteSentence(string sentence) {
        Time.timeScale = 0;
        donePrinting = false;
        textSpeed = 0.075f;
        sound.clip = clip;

        //for (int i = 0; i < sentence.Length; i++) { }
        int i = 0;
        while (i < sentence.Length) {
            if (!sound.isPlaying) {
                sound.Play();
            }

            if (sentence.Length - i >= waitCmd.Length) {
                if (sentence.Substring(i, waitCmd.Length).StartsWith("/w=")) {
                    string str = sentence.Substring(i, waitCmd.Length).Replace("/w=", "");
                    int waitTime = int.Parse(str);
                    i += waitCmd.Length + 1;
                    i = Mathf.Clamp(i, 0, sentence.Length);
                    dialogText.text += "\n";
                    yield return new WaitForSecondsRealtime(waitTime);
                }
            }
            if (!(i < sentence.Length)) {
                break;
            }
            if (textSpeed < 0) {
                for (; i < sentence.Length; i++) {
                    char Char = sentence.ToCharArray()[i];
                    dialogText.text += Char;
                }
                yield return null;
            } else {
                char Char = sentence.ToCharArray()[i];
                dialogText.text += Char;

                if (Char == ' ') {
                    yield return null;
                } else {
                    yield return new WaitForSecondsRealtime(textSpeed);
                }
                i++;
            }
        }
        if (PAK) {
            StartCoroutine(PrintPAK());
        } else {
            donePrinting = true;
        }
    }

    IEnumerator PrintPAK() {
        if (dialogText.text.ToCharArray()[dialogText.text.Length - 1] != '\n') {
            dialogText.text += '\n';
        }
        dialogText.text += "<color=#3A4466>[</color>";
        dialogText.text += "<color=#F77A22>";

        string sentence = "PRESS SPACE TO CONTINUE";
        int i = 0;
        while (i < sentence.Length) {
            if (!sound.isPlaying) {
                sound.Play();
            }
            char Char = sentence.ToCharArray()[i];
            dialogText.text += Char;
            if (Char == ' ') {
                yield return null;
            } else {
                yield return new WaitForSecondsRealtime(textSpeed);
            }
            i++;
        }
        dialogText.text += "</color>";
        dialogText.text += "<color=#3A4466>]</color>";
        donePrinting = true;
    }

    private void OpenDialogue() {
        Time.timeScale = 0;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        //Debug.Log("Open Dialogue");
        //GetComponent<Animator>().SetTrigger("Open");
        isActive = true;
    }

    private void EndDialogue() {
        if (PAK) {
            FadingScript.Instance.gameObject.SetActive(true);
            StartCoroutine(MainMenuScript.FadeMenu(2, MainMenuScript.menuID));
            return;
        }
        Time.timeScale = 1;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        //Debug.Log("End Dialogue"); 
        Time.timeScale = 1;
        //GetComponent<Animator>().SetTrigger("Close");
        isActive = false;
    }


}
