using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public GameObject Exit;

    [HideInInspector]
    public float delayTimer;
    private List<GameObject> entitiesInside;
    // Start is called before the first frame update
    void Awake()
    {
        delayTimer = Time.timeSinceLevelLoad;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision.transform.name + " entered");
        if (collision.gameObject.TryGetComponent<EntityManager>(out EntityManager otherEnt)) {

            //entitiesInside.Add(collision.gameObject);
            //if
            if (delayTimer < Time.timeSinceLevelLoad) {
                collision.gameObject.transform.position = Exit.transform.position;

                delayTimer = Time.timeSinceLevelLoad + 5f;
                Exit.GetComponent<PortalScript>().delayTimer = this.delayTimer;
            }

        }
    }
}
