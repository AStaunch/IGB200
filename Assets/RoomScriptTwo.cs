using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScriptTwo : MonoBehaviour
{
    private RoomData RoomData_;
    void Start() {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        if(TryGetComponent(out Renderer renderer)) {
            Destroy(renderer);
        }
        RoomData_ = new RoomData();
        CreateData();
    }
    void CreateData() {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        int m_LayerMask = 1 << 6 | 1 << 7;
        //m_LayerMask = ~ m_LayerMask;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(gameObject.transform.position, transform.localScale / 2, 0f, m_LayerMask);
        string msg = "";
        foreach(Collider2D collider in hitColliders) {
            if(collider.transform.TryGetComponent(out AbstractDoor ab)) {
                RoomData_.RoomDoors_.Add(ab);
                ab.RoomData_ = this.RoomData_;
            } else {
                if (collider.gameObject != this.gameObject ) {
                    GameObject clone = Instantiate(collider.gameObject, collider.transform.position, collider.transform.rotation);
                    clone.transform.parent = this.transform;
                    RoomData_.RoomObjects_.Add(clone);
                    clone.SetActive(false);
                }
            }
           msg += (collider.transform.name) + " ";
        }
        Debug.Log(msg);
    }
}

public class RoomData
{
    public List<AbstractDoor> RoomDoors_ { get => RoomDoors; set {
            RoomDoors = value;
        } 
    }
    private List<AbstractDoor> RoomDoors;
    public List<GameObject> RoomObjects_ { get => RoomObjects; set => RoomObjects = (value); }
    private List<GameObject>  RoomObjects;

    public RoomData() {
        RoomObjects_  = new List<GameObject>();
        RoomDoors_ = new List<AbstractDoor>();
    }
}
