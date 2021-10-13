using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour, iSenderObject
{
    public GameObject buttonPressSound;
    public bool PressOnce = true;
    public Sprite[] sprites;
    private bool currentState;
    public bool currentState_ {
        get {
            return currentState;
        }
        set {
            currentState = value;
            UpdateReciever();
        }
    }
    private List<iRecieverObject> targetObjects = new List<iRecieverObject>();
    public List<iRecieverObject> targetObjects_ { get => targetObjects; set => targetObjects = value; }

    private void UpdateReciever() {
        foreach (iRecieverObject target in targetObjects_) {
            target.CheckSenders(this);
        }
    }
    // Start is called before the first frame update
    private void Start() {
        UpdateSprite();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (currentState_ == false) { Instantiate(buttonPressSound); }
        if (PressOnce && collision.transform.CompareTag("Player")) {
            currentState_ = true;
        }
        UpdateSprite();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!PressOnce && collision.transform.TryGetComponent<iPhysicsInterface>(out _) && !collision.isTrigger) {
            currentState_ = true;
            UpdateSprite();            
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!PressOnce && collision.transform.TryGetComponent<iPhysicsInterface>(out _) && !collision.isTrigger) {
            currentState_ = false;
            UpdateSprite();
        }
    }

    private void UpdateSprite() {
        if (currentState_) {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        } else {
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
    }
}
