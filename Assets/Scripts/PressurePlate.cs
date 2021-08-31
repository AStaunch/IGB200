using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    bool PressOnce = true;
    public GameObject Target;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision) {
        if (PressOnce && collision.transform.CompareTag("Player")) {
            if (Target.TryGetComponent(out DoorScript ds)) {
                ds.SetDoor(true);
            }
        }
    }
}
