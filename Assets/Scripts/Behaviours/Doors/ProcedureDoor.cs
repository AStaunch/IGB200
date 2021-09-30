using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;



public class ProcedureDoor : AbstractLockedDoor
{
    // Start is called before the first frame update
    void Start() {
        isInvulnerable = true;
        if (switchGameObjects.Length > 0) {
            switchObjects_ = GetSwitches();
            initSwitches();
        }
    }

    
    public override void CheckSenders() {
        for (int i = 0; i < switchObjects_.Length; i++) {
            if (switchObjects_[i].currentState_ == false) {
                currentState_ = false;
                Debug.Log($"{i}: {switchObjects_[i].currentState_}");
                return;
            }
        }
        currentState_ = true;
    }

    public override void OnValidate() {
        isInvulnerable = true;
        UpdateSprite();
        SetDoorProperties();
        initExitDoor();
    }
}
