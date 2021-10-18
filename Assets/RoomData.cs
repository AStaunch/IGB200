using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    #region DebugArea
    public bool LoadDataBool;
    public bool DestroyDataBool;
    private bool HasPlayer;
    public bool ContainsPlayer {
        get {
            foreach (Collider2D collider in Physics2D.OverlapAreaAll(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.max, PlayerRef.layer)) {
                if (collider == PlayerRef.GetComponent<Collider2D>()) {
                    return true;
                }
            }
            return false;
        }
    }
    private void Update() {
        if (LoadDataBool) {
            Load();
            LoadDataBool = false;
        }
        if (DestroyDataBool) {
            Unload();
            DestroyDataBool = false;
        }
    }
    #endregion

    public GameObject[] SolveObjects;
    private List<GameObject> RoomObjects_  = new List<GameObject>();
    private List<AbstractDoor> RoomDoors_ = new List<AbstractDoor>();
    GameObject PlayerRef;
    public bool isSolved_ {get{ return isSolved;} set{ isSolved = value; } }// if(value) UpdateData(); } }

    public List<GameObject> LoadedObjects = new List<GameObject>();
    public bool isLoaded;
    private bool isSolved;
    void Start() {
        PlayerRef = FindObjectOfType<PlayerEntity>().gameObject;
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        if(TryGetComponent(out Renderer renderer)) {
            Destroy(renderer);
        }
        CreateData();
    }
    private void CreateData() {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        GetComponent<Collider2D>().enabled = true;
        int m_LayerMask = 1 << 6 | 1 << 7 | 1 << 0;
        //m_LayerMask = ~ m_LayerMask;
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.max, m_LayerMask);
        string msg = "";
        foreach(Collider2D collider in hitColliders) {
            if(!collider.transform.TryGetComponent(out AbstractDoor ab)) {
                AddObject(collider);
            } else {
                if (!RoomDoors_.Contains(ab) && !ab.isException) {
                    RoomDoors_.Add(ab);
                }
            }
           msg += (collider.transform.name) + " ";
        }
        GetComponent<Collider2D>().enabled = false;
        UpdateDoors();
        //Debug.Log(msg);
    }

    private void UpdateData() {
        foreach (GameObject gameObject in RoomObjects_.ToArray()) {
            //LoadedObjects.Remove(gameObject);
            Destroy(gameObject);
        }
        GetComponent<Collider2D>().enabled = true;
        RoomObjects_ = new List<GameObject>();
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        int m_LayerMask = 1 << 6 | 1 << 7 | 1 << 0;
        //m_LayerMask = ~ m_LayerMask;
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.max, m_LayerMask);
        foreach (Collider2D collider in hitColliders) {
            if (!collider.transform.TryGetComponent(out AbstractDoor ab)){
                AddObject(collider);
            }
        }
        GetComponent<Collider2D>().enabled = false;
        Unload();
        UpdateDoors();
    }

    private void AddObject(Collider2D collider) {
        bool BannedTypes = collider.transform.TryGetComponent(out iRecieverObject _) || collider.transform.TryGetComponent(out iRecieverObject _);
        BannedTypes = BannedTypes || collider.transform.TryGetComponent(out PlayerEntity _) || collider.transform.TryGetComponent(out RoomData _);
        if (!RoomObjects_.Contains(collider.gameObject) && collider.transform.TryGetComponent(out iPropertyInterface _) && !(BannedTypes)) {
            RoomObjects_.Add(collider.gameObject);
            collider.gameObject.SetActive(false);
        }
    }

    private void UpdateDoors() {
        foreach(AbstractDoor ab in RoomDoors_) {
            ab.RoomData_ = this;
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

        foreach(AbstractDoor abstractDoor in RoomDoors_) {
            if(abstractDoor.TryGetComponent(out AbstractLockedDoor lockedDoor) && abstractDoor.isException) {
                abstractDoor.IsOpen = false;
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

    public void CheckSolved() {
        foreach(GameObject gameObject in SolveObjects) {

        }
    }
}

public class LoadedObject : MonoBehaviour
{
    public RoomData roomDataParent;
    private void OnDestroy() {
        roomDataParent.LoadedObjects.Remove(this.gameObject);
    }
}
