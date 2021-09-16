using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool PressOnce = true;
    public GameObject Target;
    public Sprite[] sprites;
    private bool State = false;
    // Start is called before the first frame update
    private void Awake() {
        UpdateSprite();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (PressOnce && collision.transform.CompareTag("Player")) {
            if (Target.TryGetComponent(out DoorEntity ds)) {
                State = true;
                UpdateSprite();
                ds.SetDoor(true);
            }
        }
        UpdateSprite();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!PressOnce && collision.transform.TryGetComponent<EntityManager>(out _)) {
            if (Target.TryGetComponent(out DoorEntity ds)) {
                State = true;
                UpdateSprite();
                ds.SetDoor(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!PressOnce && collision.transform.TryGetComponent<EntityManager>(out _)) {
            if (Target.TryGetComponent(out DoorEntity ds)) {
                State = false;
                UpdateSprite();
                ds.SetDoor(true);
            }
        }
    }

    private void UpdateSprite() {
        if (State) {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        } else {
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
    }
}
