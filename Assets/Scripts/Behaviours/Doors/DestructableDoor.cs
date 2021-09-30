using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;



public class DestructableDoor : AbstractDoor, iRecieverObject
{
    public GameObject[] switchGameObjects;
    private iSenderObject[] switchObjects;
    public iSenderObject[] switchObjects_ { get => this.switchObjects; set => this.switchObjects = value; }
    private bool currentState;
    public bool currentState_ { get { return currentState; } set { OpenCloseDoor(value); currentState = value; } }
    //public override bool IsOpen { get { return currentState; } set { OpenCloseDoor(value); currentState = value; } }

    public bool isActive;

    // Start is called before the first frame update
    void Start() {
        isInvulnerable = false;
        if (switchGameObjects.Length > 0) {
            switchObjects_ = GetSwitches();
            initSwitches();
        }
        UpdateSprite();
        SetDoorProperties();
    }

    iSenderObject[] GetSwitches() {
        iSenderObject[] returnValue = new iSenderObject[switchGameObjects.Length];
        for (int i = 0; i < switchGameObjects.Length; i++) {
            returnValue[i] = switchGameObjects[i].GetComponent<iSenderObject>();
        }
        return returnValue;
    }

    void initSwitches() {
        foreach (iSenderObject iSender in switchObjects_) {
            iSender.targetObjects_.Add(this);
        }
    }
    public void CheckSenders() {
        for (int i = 0; i < switchObjects_.Length; i++) {
            if (switchObjects_[i].currentState_ == false) {
                currentState_ = false;
                Debug.Log($"{i}: {isActive}");
                return;
            }
        }
        isActive = true;
    }

    public override void OnValidate() {
        isInvulnerable = false;
        UpdateSprite();
        SetDoorProperties();
        initExitDoor();
    }


}
