using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorScript : MonoBehaviour  
{
    public DoorScript ExitDoor;
    protected Transform ExitPoint;

    [HideInInspector]
    public float delayTimer;
    public bool isOpen = false;

    [Range(0, 3)]
    [Header("0 - Up, 1 - Right, 2 - Down, 3 - Left")]
    public int FacingIndex;
    public Sprite[] DoorSprites; // 0 - Closed, 1 - Open
    // Start is called before the first frame update


    void UpdateSpriteInEditor() {
        UpdateSprite();
    }

    void Awake() {
        ExitPoint = GetComponentInChildren<Transform>();
        //Just Redunant Code to make Level Devs life easier
        if (ExitDoor.ExitDoor == null) {
            ExitDoor.ExitDoor = this;
        }
        ExitDoor.isOpen = isOpen;

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
            if (delayTimer < Time.timeSinceLevelLoad) {

                collision.gameObject.transform.position = ExitDoor.ExitPoint.position;
                delayTimer = Time.timeSinceLevelLoad + 1f;
                ExitDoor.GetComponent<DoorScript>().delayTimer = this.delayTimer;
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
            GetComponent<SpriteRenderer>().sprite = DoorSprites[FacingIndex*2 + 1];
        } else {
            GetComponent<SpriteRenderer>().sprite = DoorSprites[FacingIndex*2];
        }
    }

}
