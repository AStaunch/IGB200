using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;
public abstract class AbstractDoor : MonoBehaviour, iHealthInterface
{
    public GameObject walkThroughSoundEffect;
    public bool isTriggerDoor;
    public bool isSolveTrigger;
    public bool isException;
    public GameObject nextRoomContents;
    public int nextRoomId = 0;
    private bool nextExists;
    private GameObject spawnedContents;

    public AbstractDoor ExitDoor;
    [Range(-1, 10)]
    public int sceneIndex = -1;
    protected bool isInvulnerable;
    private float delayTimer_ {get => delayTimer; set => delayTimer = Time.timeSinceLevelLoad + value; }
    private float delayTimer;
    public bool IsOpen { get; set; }
    public Properties[] EntityProperties_ { get => EntityProperties; set => EntityProperties = value; }
    private Properties[] EntityProperties;
    public EntityTypes EntityType_ { get => EntityType; set => EntityType = value; }
    private EntityTypes EntityType = EntityTypes.Object;
    public Directions CurrentDirection_ { get => CurrentDirection; set => CurrentDirection = value; }
    public Directions CurrentDirection;
    public int Health_ { get => Health; set => Health = value; }
    private int Health;
    public int MaxHealth_ { get => MaxHealth; set => MaxHealth = value; }
    private int MaxHealth;
    public Elements[] DamageImmunities_ { get => null; set => _ = value; }
    private void Awake() {
        ValidateFunction();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        //Debug.Log(collision.transform.name + " entered");
        if (collision.gameObject.TryGetComponent(out iFacingInterface em) && !collision.isTrigger) {
            if (em.GetEntityDirectionEnum() == CurrentDirection_ && IsOpen) {
                if (sceneIndex < 0 && delayTimer_ < Time.timeSinceLevelLoad) {
                    delayTimer_ = 2 * Time.deltaTime;
                    Vector3 offset = VectorDict[CurrentDirection_];
                    collision.gameObject.transform.position = ExitDoor.transform.position + offset;
                    Instantiate(walkThroughSoundEffect);
                    if (collision.TryGetComponent(out PlayerEntity player)) {
                        player.LastDoor_ = this.ExitDoor;
                        if (isSolveTrigger) {
                            foreach (GameObject gong in GameObject.FindGameObjectsWithTag("Room")) { gong.GetComponent<RoomScript>().isSolved = true; }
                        }

                        ReloadRoom();

                        if (isTriggerDoor){
                            GameObject.FindGameObjectWithTag("MovingDoor").transform.position = ExitDoor.transform.position;
                            Destroy(ExitDoor);
                            Destroy(this);
                        }
                    }                    
                } else {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
                }
            }
        }
    }

    public void ReloadRoom() {
        delayTimer_ = 2 * Time.deltaTime;
        if (isException == false) {
            foreach (GameObject roomContents in GameObject.FindGameObjectsWithTag("Room")) {
                if (roomContents.GetComponent<RoomScript>().isSolved == false) {
                    Destroy(roomContents);
                } else if (roomContents.GetComponent<RoomScript>().roomId == nextRoomId) {
                    nextExists = true;
                }
            }
        }

        if (nextRoomContents != null && nextExists == false) {
            spawnedContents = Instantiate(nextRoomContents);
            spawnedContents.transform.position = new Vector3(0, 0, 0);
        }
    }

    public Vector3 RespawnPoint {get => transform.position - (Vector3) VectorDict[CurrentDirection_]; }
    public void InitExitDoor() {
        if (!ExitDoor)
            return;
        if (IsOpen) {
            ExitDoor.IsOpen = IsOpen;
            ExitDoor.GetComponent<Collider2D>().isTrigger = true;
        }
        if (isInvulnerable) {
            ExitDoor.isInvulnerable = isInvulnerable;
        }
        ExitDoor.UpdateSprite();
    }
    public void SyncExitDoor() {
        if (!ExitDoor)
            return;
        ExitDoor.IsOpen = IsOpen;
        ExitDoor.GetComponent<Collider2D>().isTrigger = true;
        ExitDoor.isInvulnerable = isInvulnerable;
        ExitDoor.UpdateSprite();
    }
    public void OpenCloseDoor(bool newState) {
        IsOpen = newState;
        GetComponent<Collider2D>().isTrigger = true;
        UpdateSprite();
        SyncExitDoor();
    }
    public void SetDoorProperties() {
        EntityType_ = EntityTypes.Object;
        if (isInvulnerable) {
            EntityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Indestructable };
        } else {
            EntityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Flamable };
        }
    }
    protected Sprite currentSprite;
    public void UpdateSprite() {
        try {
            
            if (IsOpen || ExitDoor.IsOpen) {
                currentSprite = SpriteDict["OpenDoor"][IntDict[CurrentDirection_]];
            } else if (isInvulnerable || ExitDoor.isInvulnerable) {
                currentSprite = SpriteDict["MetalDoor"][IntDict[CurrentDirection_]];
            } else {
                currentSprite = SpriteDict["WoodDoor"][IntDict[CurrentDirection_]];
            }
            if (TryGetComponent(out BoxCollider2D boxCollider2D)) {
                Vector2 SpriteSize = GetComponent<SpriteRenderer>().bounds.size;
                Vector2 Mag = new Vector2(Mathf.Abs(VectorDict[CurrentDirection_].x), Mathf.Abs(VectorDict[CurrentDirection_].y));
                boxCollider2D.size = 0.5f * Mag * SpriteSize;
                boxCollider2D.size += 1.0f * SpriteSize;
            }
            GetComponent<SpriteRenderer>().sprite = currentSprite;
        }
        catch (System.Exception) {
            //Debug.LogWarning(ex.Message);
        }
    }
    public void TakeDamage(float damage, Elements damageType) {
        if (EntityProperties.Contains(Properties.Indestructable) || damageType != Elements.Fire) {
            return;
        }
        Health_ -= Mathf.RoundToInt(damage);
        Health_ = Mathf.Clamp(Health_, 0, MaxHealth_);
        if (0 >= Health_) {
            EntityDeath();
        }
    }
    public void EntityDeath() {
        OpenCloseDoor(true);
        // TODO: Impliment Colour change
    }

    public abstract void ValidateFunction();
}