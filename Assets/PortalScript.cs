using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private GameObject Entry;
    public GameObject Exit;

    private Transform ExitTransform;
    [HideInInspector]
    public float delayTimer;
    // Start is called before the first frame update
    void Awake()
    {
        Entry = this.gameObject;
        delayTimer = Time.timeSinceLevelLoad;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision.transform.name + " entered");
        if (collision.gameObject.TryGetComponent<EntityManager>(out EntityManager otherEnt)) {
            if(delayTimer < Time.timeSinceLevelLoad) {
                collision.gameObject.transform.position = Exit.transform.position;

                delayTimer = Time.timeSinceLevelLoad + 5f;
                Exit.GetComponent<PortalScript>().delayTimer = this.delayTimer;
            }

        }
    }
}
