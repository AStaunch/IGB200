using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;



public class DestructableDoor : AbstractLockedDoor
{
    // Start is called before the first frame update
    void Start() {
        isInvulnerable = false;
        if (switchGameObjects.Length > 0) {
            switchObjects_ = GetSwitches();
            initSwitches();
        }
        UpdateSprite();
    }
    public override void CheckSenders(iSenderObject iSenderObject) {
        for (int i = 0; i < switchObjects_.Length; i++) {
            if (switchObjects_[i].currentState_ == false) {
                currentState_ = false;
                Debug.Log($"{i}: {switchObjects_[i].currentState_}");
                return;
            }
        }
        currentState_ = true;
    }

    public override void ValidateFunction() {
        isInvulnerable = false;
        UpdateSprite();
        InitExitDoor();
    }
}
