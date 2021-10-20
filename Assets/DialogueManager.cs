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
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    internal void StartDialogue(Dialogue dialogue) {
        OpenDialogue();
        sentences.Clear();
        nameText.text = dialogue.name ;
        portrait.sprite = dialogue.CharacterPortrait;
        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    private void DisplayNextSentence() {
        Debug.Log("Display Next Dialogue");
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogText.text = sentence;
        StartCoroutine(WaitForKeyPress(KeyCode.Space));
    }

    private void OpenDialogue() {
        Debug.Log("Open Dialogue");
        GetComponent<Animator>().SetTrigger("Open");
    }

    private void EndDialogue() {
        Debug.Log("End Dialogue");
        Time.timeScale = 1;
        GetComponent<Animator>().SetTrigger("Close");
    }

    IEnumerator WaitForKeyPress(KeyCode keyCode) {

        while (!Input.GetKeyUp(keyCode)) {
            //Wait
            yield return null;
        }
        //Display Next Sentence
        DisplayNextSentence();
    }
}
