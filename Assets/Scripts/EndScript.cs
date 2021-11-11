using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour, iSenderObject
{
    public bool currentState_ {
        get {
            return currentState;
        }
        set {
            currentState = value;
            UpdateReciever();
        }
    }
    public bool currentState = false;
    public List<iRecieverObject> targetObjects_ { get => targetObjects; set => targetObjects = value; }
    private List<iRecieverObject> targetObjects = new List<iRecieverObject>();
    public void UpdateReciever() {
        foreach (iRecieverObject target in targetObjects_) {
            target.CheckSenders(this);
        }
    }

    public void Start() {
        Destroy(this.GetComponent<SpriteRenderer>());
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out PlayerEntity _) && collision.isTrigger) {
            Debug.Log("EndGame");
            currentState_ = true;
            StartCoroutine(MainMenuScript.FadeMenu(2, MainMenuScript.endID));
            Destroy(this.GetComponent<Collider2D>());
        }
    }

    public void ResetSender() {
        currentState_ = false;
    }
}
