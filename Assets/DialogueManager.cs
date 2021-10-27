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
    private bool IsOpen;

    private float textSpeed = 0.075f;
    private bool donePrinting = false;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        EndDialogue();
    }

    private void Update() {
        if(IsOpen && Input.GetKeyUp(KeyCode.Space) && donePrinting) {
            DisplayNextSentence();
        } else if (IsOpen && Input.GetKeyUp(KeyCode.Space) && !donePrinting) {
            textSpeed = 0f;
        }
    }

    internal void StartDialogue(Dialogue dialogue) {
        OpenDialogue();
        sentences.Clear();
        nameText.text = dialogue.name;
        portrait.sprite = dialogue.CharacterPortrait;
        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    private void DisplayNextSentence() {
        //StopCoroutine("WriteSentence");
        Debug.Log("Display Next Dialogue");
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogText.text = "";
        StartCoroutine(WriteSentence(sentence));
    }

    IEnumerator WriteSentence(string sentence) {
        donePrinting = false;
        textSpeed = 0.075f;
        foreach (char Char in sentence.ToCharArray()) {
            dialogText.text += Char;
            if(Char == ' ') {
                yield return null;
            } else {
                yield return new WaitForSecondsRealtime(textSpeed);
            }
        }
        donePrinting = true;
    }

private void OpenDialogue() {
        Time.timeScale = 0;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        Debug.Log("Open Dialogue");
        //GetComponent<Animator>().SetTrigger("Open");
        IsOpen = true;
    }

    private void EndDialogue() {
        Time.timeScale = 1;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        Debug.Log("End Dialogue"); 
        Time.timeScale = 1;
        //GetComponent<Animator>().SetTrigger("Close");
        IsOpen = false;
    }

    //IEnumerator LerpToPosition(KeyCode keyCode) {
        
    //}
}
