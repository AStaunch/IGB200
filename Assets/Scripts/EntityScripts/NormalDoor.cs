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
    public bool isFireproof = false;

    // Start is called before the first frame update

    private void Start() {
        //Sets Initial Sprite
        UpdateSprite();
        //Initialises Door Timer
        delayTimer = Time.timeSinceLevelLoad;

        GameObject CP = new GameObject(transform.name + " Checkpoint");
        CP.transform.parent = this.transform;
        CP.AddComponent<CheckPoint>();
        CP.transform.position = transform.position - convert2to3(VectorDict[exitDirection]);
        BoxCollider2D box = CP.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(0.5f, 0.5f);
    }

    Vector3 convert2to3(Vector2 v) {

        return new Vector3(v.x, v.y, 0.0f);
    }

    public override void  EntityDeath() {
        SetDoorState(true);
        OnValidate();
    }

    public void SetDoorState(bool state) {
        isOpen = state;
        GetComponent<Collider2D>().isTrigger = isOpen;
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
        if (!base.ExitDoor)
            return;
        if (isOpen) {
            ExitDoor.isOpen = isOpen;
            ExitDoor.GetComponent<Collider2D>().isTrigger = GetComponent<Collider2D>().isTrigger;
        }
        if(isFireproof){
            NormalDoor normalDoor = (NormalDoor) ExitDoor;
            normalDoor.isFireproof = isFireproof;
            ExitDoor.entityProperties = entityProperties;
        }
        ExitDoor.UpdateSprite();
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
            currentSprite = SpriteDict["WoodDoor"][IntDict[exitDirection]];
        }
        GetComponent<SpriteRenderer>().sprite = currentSprite;
        }
        catch (System.Exception ex) {
            //Debug.LogWarning(ex.Message);
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
