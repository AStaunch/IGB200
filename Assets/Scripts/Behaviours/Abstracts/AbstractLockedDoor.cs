using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;

public abstract class AbstractLockedDoor : AbstractDoor, iRecieverObject
{
    public GameObject[] switchGameObjects;
    private iSenderObject[] switchObjects;
    public iSenderObject[] switchObjects_ { get => this.switchObjects; set => this.switchObjects = value; }
    private bool currentState;
    public bool currentState_ { get { return currentState; } set { OpenCloseDoor(value); currentState = value; } }

    protected iSenderObject[] GetSwitches() {
        iSenderObject[] returnValue = new iSenderObject[switchGameObjects.Length];
        for (int i = 0; i < switchGameObjects.Length; i++) {
            returnValue[i] = switchGameObjects[i].GetComponent<iSenderObject>();
        }
        return returnValue;
    }

    protected void initSwitches() {
        foreach (iSenderObject iSender in switchObjects_) {
            iSender.targetObjects_.Add(this);
        }
    }

    public abstract void CheckSenders();

    public override void OnValidate() {
        throw new NotImplementedException();
    }
}
