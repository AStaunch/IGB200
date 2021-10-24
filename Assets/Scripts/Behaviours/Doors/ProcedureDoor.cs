using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;

public class ProcedureDoor : AbstractLockedDoor, iRecieverObject
{
    [SerializeField]
    public GameObject[] Switches;
    private bool[] InputBools;
    private iSenderObject[] InputSenders;
    private iSenderObject[] SolutionSenders;
    // Start is called before the first frame update
    void Start() {
        isInvulnerable = true;
        if (Switches.Length > 0) {
            switchObjects_ = GetSwitches();
            foreach (iSenderObject iSender in switchObjects_) {
                iSender.targetObjects_.Add(this);
            }
        }
        isSolved = new bool[Switches.Length];
        InputBools = new bool[Switches.Length];
        InputSenders = new iSenderObject[Switches.Length];
        SolutionSenders = new iSenderObject[Switches.Length];

        for (int i = 0; i < Switches.Length; i++) {
            SolutionSenders[i] = Switches[i].GetComponent<iSenderObject>();
        }
    }
    public override void CheckSenders(iSenderObject iSender) {
        UpdateInput(iSender);
        UpdateStates(iSender);
        ResetSwitches();
        if (!isSolved.Contains(false)) {
            currentState_ = true;
        }
    }

    private void ResetSwitches() {
        for (int i = 0; i < Switches.Length; i++) {
            if (!isSolved[i] && InputSenders[i] != null) {
                InputSenders[i].ResetSender();
            }
        }
    }

    bool[] isSolved;
    public void UpdateInput(iSenderObject iSender) {
        for (int i = 0; i < Switches.Length; i++) {
            if (InputSenders[i] == iSender) {
                return;
            }
            if (!isSolved[i]) {
                InputBools[i] = iSender.currentState_;
                InputSenders[i] = iSender;
                return;


                if (i < Switches.Length - 1) {
                    InputBools[i] = InputBools[i + 1];
                    InputSenders[i] = InputSenders[i + 1];
                } else {

                }
            }
        }
    }
    public void UpdateStates(iSenderObject iSender) {
        for (int i = 0; i < Switches.Length; i++) {
            //if (isSolved[i]) {
            //    if (InputSenders[i] == iSender) {
            //        return;
            //    }
            //} else {
            bool SameState = InputBools[i];
            bool SameObject = InputSenders[i] == SolutionSenders[i];
            isSolved[i] = SameState && SameObject;
            if (SolutionSenders[i] == iSender) {
                return;
            }
            //}            
        }
    }
    protected new iSenderObject[] GetSwitches() {
        iSenderObject[] returnValue = new iSenderObject[Switches.Length];
        for (int i = 0; i < Switches.Length; i++) {
            if(Switches[i].TryGetComponent(out iSenderObject si))
            returnValue[i] = si;
        }
        return returnValue;
    }
    public override void ValidateFunction() {
        isInvulnerable = true;
        UpdateSprite();
        InitExitDoor(); 
    }
}

[System.Serializable]
public struct SwitchObjects
{
    public GameObject SwitchObject;
    public bool DesiredState;
}

public struct SwitchOrders
{
    public iSenderObject SwitchObject;
    public bool DesiredState;
    public SwitchOrders(iSenderObject iSenderObject, bool State) {
        this.SwitchObject = iSenderObject;
        this.DesiredState = State;
    }
}
