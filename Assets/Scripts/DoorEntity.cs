using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static EntityManager;
using static EnumsAndDictionaries;



[ExecuteInEditMode]
public class DoorEntity : EntityManager
{
    public DoorEntity ExitDoor;

    protected float delayTimer;
    public bool isOpen = false;
    public bool isFireproof = false;
    [Range(-1,10)]
    public int sceneIndex = -1;

    [HideInInspector]
    public Directions exitDirection;
    public Sprite[] OpenDoorSprites;
    public Sprite[] WoodenDoorSprites;
    public Sprite[] FireproofDoorSprites;
    // Start is called before the first frame update

    void Awake() {
        if(isOpen){
            ExitDoor.isOpen = isOpen;
        }
    }

    private void Start() {
        //Sets Initial Sprite
        UpdateSprite();
        //Initialises Door Timer
        delayTimer = Time.timeSinceLevelLoad;
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision.transform.name + " entered");
        if (collision.gameObject.TryGetComponent(out EntityManager _) && isOpen) {
            if(sceneIndex < 0){
                if (delayTimer < Time.timeSinceLevelLoad) {
                    Vector3 offset = VectorDict[exitDirection];
                    collision.gameObject.transform.position = ExitDoor.transform.position + offset;
                    delayTimer = Time.timeSinceLevelLoad + 1f;
                    ExitDoor.GetComponent<DoorEntity>().delayTimer = this.delayTimer;
                }
            } else {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            }
        }
    }

    public void ToggleDoor() {
        isOpen = !isOpen;
    }

    public void SetDoor(bool state) {
        isOpen = state;
        ExitDoor.isOpen = state;
        GetComponent<Collider2D>().isTrigger = state;
        ExitDoor.GetComponent<Collider2D>().isTrigger = state;
        UpdateSprite();
        ExitDoor.UpdateSprite();
    }

    public void SetDoor() {
        entityType = EntityType.Object;
        if (isFireproof) {
            entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Fireproof };
        } else {
            entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Flamable };
        }
    }

    public void SyncDoor() {
        ExitDoor.isOpen = isOpen;
        ExitDoor.isFireproof = isFireproof;
        ExitDoor.entityProperties = entityProperties;
        ExitDoor.UpdateSprite();
    }

    public void UpdateSprite() {
        if (isOpen) {
            GetComponent<SpriteRenderer>().sprite = OpenDoorSprites[IntDict[exitDirection]];
        } else {
            if (isFireproof) {
                GetComponent<SpriteRenderer>().sprite = FireproofDoorSprites[IntDict[exitDirection]];
            } else {
                GetComponent<SpriteRenderer>().sprite = WoodenDoorSprites[IntDict[exitDirection]];
            }
        }
    }

    public void OnValidate() {
        SetDoor();
        if (ExitDoor) {
            SyncDoor();
        }
        UpdateSprite();
    }
}
