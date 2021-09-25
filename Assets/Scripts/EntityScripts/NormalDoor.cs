using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static EntityManager;
using static EnumsAndDictionaries;

using static SpriteManager;



[ExecuteInEditMode]
public class NormalDoor : DoorBase
{
    private DoorBase normalExitDoor;
    public bool isFireproof = false;

    // Start is called before the first frame update
    void Awake() {
        normalExitDoor = ExitDoor;
    }

    private void Start() {
        OnValidate();
        //Sets Initial Sprite
        UpdateSprite();
        //Initialises Door Timer
        delayTimer = Time.timeSinceLevelLoad;
    }

    public override void  EntityDeath() {
        SetDoorState(true);
    }

    public void SetDoorState(bool state) {
        isOpen = state;
        GetComponent<Collider2D>().isTrigger = state;
    }

    public void SetDoorProperties() {
        entityType = EntityType.Object;
        if (isFireproof) {
            entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Fireproof };
        } else {
            entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Flamable };
        }
    }

    public void SyncExitDoor() {
        if (!ExitDoor)
            return;
        if (isOpen) {
            normalExitDoor.isOpen = isOpen;
            normalExitDoor.GetComponent<Collider2D>().isTrigger = GetComponent<Collider2D>().isTrigger;
        }
        if(isFireproof){
            NormalDoor normalDoor = (NormalDoor) normalExitDoor;
            normalDoor.isFireproof = isFireproof;
            normalExitDoor.entityProperties = entityProperties;
        }
        normalExitDoor.UpdateSprite();
    }
    public override void UpdateSprite() {
        try{
        Sprite currentSprite;
        if (isOpen) {
            currentSprite = SpriteDict["OpenDoor"][IntDict[exitDirection]];
        }
        else if (isFireproof) {
            currentSprite = SpriteDict["MetalDoor"][IntDict[exitDirection]];
        } 
        else {
            currentSprite = SpriteDict["WoodenDoor"][IntDict[exitDirection]];
        }
        GetComponent<SpriteRenderer>().sprite = currentSprite;
        }
        catch (System.Exception ex) {
            Debug.LogWarning(ex.Message);
        }
    }

    public void OnValidate() {
        SetDoorState(isOpen);
        SetDoorProperties();
        SyncExitDoor();
        UpdateSprite();
    }
}

//public class DoorHandler : EntityManager, iRecieverObject
//{
//    public Dictionary<iRecieverObject, bool> Switches { get => Switches; set => Switches = value; }
//    public bool alwaysOn;
//    private bool isFireproof;

//    public bool checkRecievedSignals() {
//        if (alwaysOn) {
//            return true;
//        }
//        bool rf = true;
//        if (Switches.ContainsValue(false)) {
//            rf = false;
//        }
//        return rf;
//    }

//    public override void EntityDeath() {
//        alwaysOn = true;
//    }

//    void SetProperties() {
//        entityType = EntityType.Object;
//        if (isFireproof) {
//            entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Fireproof };
//        } else {
//            entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Flamable };

//        }

//    }
//}
