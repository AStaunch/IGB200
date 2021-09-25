using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public abstract class DoorBase : EntityManager
{
    public DoorBase ExitDoor;
    public Directions exitDirection;
    public bool isOpen = false;
    [Range(-1, 10)]
    public int sceneIndex = -1;

    protected float delayTimer;

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision.transform.name + " entered");
        if (collision.gameObject.TryGetComponent(out EntityManager em) && isOpen) {
            if (em.GetEntityDirectionEnum() == exitDirection) {
                if (sceneIndex < 0) {
                    if (delayTimer < Time.timeSinceLevelLoad) {
                        Vector3 offset = VectorDict[exitDirection];
                        collision.gameObject.transform.position = ExitDoor.transform.position + offset;
                        delayTimer = Time.timeSinceLevelLoad + 1f;
                        ExitDoor.GetComponent<NormalDoor>().delayTimer = this.delayTimer;
                    }
                } else {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
                }
            }

        }
    }

    public abstract void UpdateSprite();
}
