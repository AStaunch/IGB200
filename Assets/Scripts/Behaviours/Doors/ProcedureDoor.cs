using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;



public class ProcedureDoor : AbstractLockedDoor, iRecieverObject
{
    [SerializeField]
    public SwitchObjects[] Switches;
    private SwitchOrders[] Solution;
    private SwitchOrders[] Input;

    private iSenderObject[] switchObjects;
    private int CorrectAnswers;

    public override void CheckSenders(iSenderObject iSenderObject) {
        string msg = "";
        //Update the List of Inputs
        for (int j = CorrectAnswers; j < Solution.Length - 1; j++) {
            Input[j] = Input[j + 1];
            //msg += j + " [" + Input[j].DesiredState == Solution[j].DesiredState + "] : ";            
        }
        Input[Input.Length - 1] = new SwitchOrders(iSenderObject, iSenderObject.currentState_);
        //msg += Input.Length - 1 + " [" + Input[Input.Length - 1].DesiredState == Solution[Input.Length - 1].DesiredState + "] \n";

        //Check for Correct Answers
        CorrectAnswers = 0;
        for (int j = 0; j < Solution.Length; j++) {
            bool SameState = Input[j].DesiredState == Solution[j].DesiredState;
            bool SameObject = Input[j].SwitchObject == Solution[j].SwitchObject;
            //msg += j + " [" + SameState + SameObject + "]";
            if (SameState && SameObject) {
                CorrectAnswers = j;//Mathf.Max(j, CorrectAnswers);
                msg += CorrectAnswers + ", ";
            } else {
                break;
            }
        }
        //Make sure CorrectAnswers doesnt exceed the solution and make sure it doesnt go below 0
        CorrectAnswers = Mathf.Clamp(CorrectAnswers, 0, Solution.Length);

        if (CorrectAnswers == Solution.Length - 1) {
            currentState_ = true;
        } else {
            currentState_ = false;
        }
        msg += "\n " + currentState_;
        Debug.Log(msg);
    }

    // Start is called before the first frame update
    void Start() {
        isInvulnerable = true;
        CorrectAnswers = 0;
        if (Switches.Length > 0) {
            switchObjects_ = GetSwitches();
            foreach (iSenderObject iSender in switchObjects_) {
                iSender.targetObjects_.Add(this);
            }
        }
        Solution = new SwitchOrders[Switches.Length];
        Input = new SwitchOrders[Switches.Length];
        for (int i = 0; i < Switches.Length; i++) {
            Solution[i].SwitchObject = Switches[i].SwitchObject.GetComponent<iSenderObject>();
            Solution[i].DesiredState = Switches[i].DesiredState;
        }
    }

    protected new iSenderObject[] GetSwitches() {
        iSenderObject[] returnValue = new iSenderObject[Switches.Length];
        for (int i = 0; i < Switches.Length; i++) {
            if(Switches[i].SwitchObject.TryGetComponent(out iSenderObject si))
            returnValue[i] = si;
        }
        return returnValue;
    }

    public override void ValidateFunction() {
        isInvulnerable = true;
        UpdateSprite();
        SetDoorProperties();
        InitExitDoor();
    }
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
[System.Serializable]
public struct SwitchObjects
{
    public GameObject SwitchObject;
    public bool DesiredState;
}
