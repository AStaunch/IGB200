using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;

public class PuzzleDoor : DoorBase, iRecieverObject
{
    public Dictionary<iSenderObjects, bool> Switches = new Dictionary<iSenderObjects, bool>();
    public bool opened { set { isOpen = value; opened = value; } get { return opened; } }

    void Awake() {

    }
    public void checkRecievedSignals() {
        if (Switches.ContainsValue(false)) {
            opened = false;
            return;
        }
        opened = true;
    }

    public override void UpdateSprite() {
        Sprite currentSprite;
        checkRecievedSignals();
        if (opened) {
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
