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

    public Directions exitDirection;
    public Sprite[] OpenDoorSprites;
    public Sprite[] WoodenDoorSprites;
    public Sprite[] FireproofDoorSprites;

    private static readonly Dictionary<Directions, Directions> directionOpposites = new Dictionary<Directions, Directions> {
        { Directions.Up,      Directions.Down},
        { Directions.Down,    Directions.Up},
        { Directions.Left,    Directions.Right },
        { Directions.Right,   Directions.Left },
    };

    // Start is called before the first frame update
    void Awake() {

    }

    private void Start() {
        if(isOpen || isFireproof) {
            OnValidate();
        }
        //Sets Initial Sprite
        UpdateSprite();
        //Initialises Door Timer
        delayTimer = Time.timeSinceLevelLoad;
    }

    public override void  EntityDeath() {
        SetDoorState(true);
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision.transform.name + " entered");
        if (collision.gameObject.TryGetComponent(out EntityManager em) && isOpen) {
            if(em.GetEntityDirectionEnum() == exitDirection) {
                if (sceneIndex < 0) {
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
    }

    public void SetDoorState(bool state) {
        isOpen = state;
        GetComponent<Collider2D>().isTrigger = state;
        UpdateSprite();
        SyncExitDoor();
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
        ExitDoor.isOpen = isOpen;
        ExitDoor.isFireproof = isFireproof;
        ExitDoor.entityProperties = entityProperties;
        ExitDoor.GetComponent<Collider2D>().isTrigger = GetComponent<Collider2D>().isTrigger;
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
        SetDoorState(isOpen);
        SetDoorProperties();
        SyncExitDoor();
        UpdateSprite();
    }
}
