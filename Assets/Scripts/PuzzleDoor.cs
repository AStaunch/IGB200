using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;

public class PuzzleDoor : DoorBase, iRecieverObject
{
    public Dictionary<iRecieverObject, bool> Switches { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool isOpen { get => checkRecievedSignals(); }

    void Awake() {

    }
    public bool checkRecievedSignals() {
        if (Switches.ContainsValue(false)) {
            return false;
        }
        return true;
    }

    public override void UpdateSprite() {
        Sprite currentSprite;
        if (isOpen) {
            currentSprite = SpriteDict["OpenDoor"][IntDict[exitDirection]];
        } else {
            currentSprite = SpriteDict["MetalDoor"][IntDict[exitDirection]];
        }
        GetComponent<SpriteRenderer>().sprite = currentSprite;
        
        ExitDoor.UpdateSprite();
}


    public void OnValidate() {
        UpdateSprite();
    }
}
