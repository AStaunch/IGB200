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
    private List<GameObject> RoomVoid_ = new List<GameObject>();
    public bool isSolved_ {get{ return isSolved; } set{ isSolved = value; } }
    private bool isSolved = false;
    public SpriteRenderer spriteRenderer { get => GetComponent<SpriteRenderer>(); }
    public List<GameObject> LoadedObjects = new List<GameObject>();

    public bool isLoaded_ { get { return isLoaded; } set { isLoaded = value; } }
    private bool isLoaded = false;
    public bool hasVisited_ { get { return hasVisited; } set { hasVisited = value; } }
    private bool hasVisited = false;
    public bool hasChest_ { get { return hasChest; } set { hasChest = value; } }
    private bool hasChest = false;
    private int m_LayerMask = ~ (1 << 8);
    [HideInInspector]
    public GameObject Icon;
    void Start() {
        //PlayerRef = PlayerEntity.Instance.gameObject;
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        spriteRenderer.enabled = false;
        CreateData();

        Icon = new GameObject(transform.name +" icon");
        Icon.transform.position = transform.position;

        float smallestSide = 0.5f * (Mathf.Min(transform.localScale.x, transform.localScale.y));
        Icon.layer = 5;
        Icon.gameObject.transform.localScale *= smallestSide;
        Icon.transform.parent = transform;
        Icon.AddComponent<SpriteRenderer>().material = spriteRenderer.material;
        Icon.SetActive(false);
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
                }
            }
        }
        GetComponent<Collider2D>().enabled = false;
        Unload();
    }

    private void AddObject(Collider2D collider) {
        if(collider.transform.TryGetComponent(out ChestScript _)) {
            hasChest = true;
            return;
        }
        if(collider.TryGetComponent(out EmptySpaceScript _)) {
            if (!RoomVoid_.Contains(collider.gameObject)) {
                RoomVoid_.Add(collider.gameObject);
                collider.gameObject.SetActive(false);
            }
            return;
        }
        if(collider.transform.TryGetComponent(out PlayerEntity playerEntity)) {
            playerEntity.StartRoomData_ = this;
            return;
        }

        bool BannedTypes = collider.transform.TryGetComponent(out iRecieverObject _) || collider.transform.TryGetComponent(out iSenderObject _);
        BannedTypes = BannedTypes || collider.transform.TryGetComponent(out RoomData _);
        if (!RoomObjects_.Contains(collider.gameObject) && collider.transform.TryGetComponent(out iPropertyInterface _) && !(BannedTypes)) {
            RoomObjects_.Add(collider.gameObject);
            collider.gameObject.SetActive(false);
        }
    }

    public void Load() {
        Unload();
        hasVisited_ = true;
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
        foreach(GameObject emptySpace in RoomVoid_) {
            GameObject clone = Instantiate(emptySpace, emptySpace.transform.position, emptySpace.transform.rotation);
            clone.transform.parent = this.transform;
            clone.SetActive(true);
            clone.AddComponent<LoadedObject>().roomDataParent = this;
            LoadedObjects.Add(clone);
        }
        isLoaded_ = true;
    }

    public void Unload() {
        hasVisited_ = true;
        foreach (GameObject gameObject in LoadedObjects.ToArray()) {
            //LoadedObjects.Remove(gameObject);
            Destroy(gameObject);
        }
        isLoaded_ = false;
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
