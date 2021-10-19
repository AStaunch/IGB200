using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    #region DebugArea
    //public bool LoadDataBool;
    //public bool DestroyDataBool;
    //private bool HasPlayer;
    //public bool ContainsPlayer {
    //    get {
    //        foreach (Collider2D collider in Physics2D.OverlapAreaAll(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.max, PlayerRef.layer)) {
    //            if (collider == PlayerRef.GetComponent<Collider2D>()) {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //}
    //private void Update() {
    //    if (LoadDataBool) {
    //        Load();
    //        LoadDataBool = false;
    //    }
    //    if (DestroyDataBool) {
    //        Unload();
    //        DestroyDataBool = false;
    //    }
    //}
    #endregion

    public GameObject[] SolveObjects;
    private List<GameObject> RoomObjects_  = new List<GameObject>();
    private List<AbstractDoor> RoomDoors_ = new List<AbstractDoor>();
    private List<GameObject> RoomSwitches_ = new List<GameObject>();
    public bool IsSolved_ {get{ return isSolved;} set{ isSolved = value; } }// if(value) UpdateData(); } }

    public List<GameObject> LoadedObjects = new List<GameObject>();
    public bool isLoaded;
    private bool isSolved;
    private int m_LayerMask = ~ (1 << 8);
    void Start() {
        //PlayerRef = FindObjectOfType<PlayerEntity>().gameObject;
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        if(TryGetComponent(out Renderer renderer)) {
            Destroy(renderer);
        }
        CreateData();
    }
    private void CreateData() {
        GetComponent<Collider2D>().enabled = true;
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.max, m_LayerMask);
        foreach(Collider2D collider in hitColliders) {
            if(!collider.transform.TryGetComponent(out AbstractDoor ab)) {
                AddObject(collider);
            } else {
                if (!RoomDoors_.Contains(ab)) {
                    RoomDoors_.Add(ab);
                    if (!ab.isException) {
                        ab.RoomData_ = this;
                    }
                }
            }

            if (collider.transform.TryGetComponent(out iSenderObject _)) {
                if (!RoomSwitches_.Contains(collider.gameObject)) {
                    RoomSwitches_.Add(collider.gameObject);
                    //collider.gameObject.layer = SortingLayer.NameToID("IgnoreRaycast");
                    Debug.Log(RoomSwitches_.Count);
                }
            }
        }   
        GetComponent<Collider2D>().enabled = false;
    }

    private void UpdateData() {
        foreach (GameObject gameObject in RoomObjects_.ToArray()) {
            //LoadedObjects.Remove(gameObject);
            Destroy(gameObject);
        }
        GetComponent<Collider2D>().enabled = true;
        RoomObjects_ = new List<GameObject>();

        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.max, m_LayerMask);
        foreach (Collider2D collider in hitColliders) {
            if (!collider.transform.TryGetComponent(out AbstractDoor ab)) {
                AddObject(collider);
            }
        }
        GetComponent<Collider2D>().enabled = false;
        Unload();
    }

    private void AddObject(Collider2D collider) {
        bool BannedTypes = collider.transform.TryGetComponent(out iRecieverObject _) || collider.transform.TryGetComponent(out iRecieverObject _) || collider.transform.TryGetComponent(out ChestScript _);
        BannedTypes = BannedTypes || collider.transform.TryGetComponent(out PlayerEntity _) || collider.transform.TryGetComponent(out RoomData _) || collider.transform.TryGetComponent(out EmptySpaceScript _);
        if (!RoomObjects_.Contains(collider.gameObject) && collider.transform.TryGetComponent(out iPropertyInterface _) && !(BannedTypes)) {
            RoomObjects_.Add(collider.gameObject);
            collider.gameObject.SetActive(false);
        }
    }

    public void Load() {
        Unload();
        foreach (GameObject gameObject in RoomObjects_) {
            GameObject clone = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
            clone.transform.parent = this.transform;
            clone.SetActive(true);
            clone.AddComponent<LoadedObject>().roomDataParent = this;
            LoadedObjects.Add(clone);
        }

        foreach (GameObject iSender in RoomSwitches_) {
            iSender.GetComponent<iSenderObject>().currentState_ = false;
        }

        foreach(AbstractDoor abstractDoor in RoomDoors_) {
            if(abstractDoor.TryGetComponent(out AbstractLockedDoor lockedDoor) && abstractDoor.isException) {
                abstractDoor.OpenCloseDoor(false);
                abstractDoor.Health_ = abstractDoor.MaxHealth_;
            }
        }
        isLoaded = true;
    }

    public void Unload() {
        foreach(GameObject gameObject in LoadedObjects.ToArray()) {
            //LoadedObjects.Remove(gameObject);
            Destroy(gameObject);
        }
        isLoaded = false;
    }

    //public void CheckSolved() {
    //    foreach(GameObject gameObject in SolveObjects) {

    //    }
    //}
}

public class LoadedObject : MonoBehaviour
{
    public RoomData roomDataParent;
    private void OnDestroy() {
        roomDataParent.LoadedObjects.Remove(this.gameObject);
    }
}
