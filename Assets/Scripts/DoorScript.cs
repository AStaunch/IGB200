using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static EntityManager;
using static EnumsAndDictionaries;

public class DoorScript : MonoBehaviour  
{
    public DoorScript ExitDoor;

    [HideInInspector]
    public float delayTimer;
    public bool isOpen = false;
    public bool isFireproof = false;
    public Directions exitDirection;
    public Sprite[] OpenDoorSprites;
    public Sprite[] WoodenDoorSprites;
    public Sprite[] FireproofDoorSprites;
    public int sceneIndex = -1;
    // Start is called before the first frame update

    void Awake() {
        EntityManager em = gameObject.AddComponent<EntityManager>();
        em.entityType = EntityType.Object;
        
        //em.AddProperty(Properties.Door);
        //em.AddProperty(Properties.Immovable);

        if (isFireproof) {
            ExitDoor.isFireproof = isFireproof;
            em.entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Fireproof};
            //em.AddProperty(Properties.Fireproof);
        } else {
            em.entityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Flamable};
            //em.AddProperty(Properties.Flamable);
        }

        if(isOpen){
            ExitDoor.isOpen = isOpen;
        }
        //GetComponent<Collider2D>().bounds.size = Vector2()
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
                    ExitDoor.GetComponent<DoorScript>().delayTimer = this.delayTimer;
                }
            } else {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            }
        }
    }

    public void SetDoor(bool state) {
        isOpen = state;
        ExitDoor.isOpen = state;
        GetComponent<Collider2D>().isTrigger = state;
        ExitDoor.GetComponent<Collider2D>().isTrigger = state;
        UpdateSprite();
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
}
