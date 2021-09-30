using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;

public abstract class AbstractDoor : MonoBehaviour, iHealthInterface
{
    public AbstractDoor ExitDoor;
    public Directions exitDirection;
    [Range(-1, 10)]
    public int sceneIndex = -1;
    protected bool isInvulnerable;
    private float delayTimer = 0;
    public bool IsOpen { get; set; }

    public Properties[] EntityProperties_ { get => EntityProperties; set => EntityProperties = value; }
    private Properties[] EntityProperties;
    public EntityTypes EntityType_ { get => EntityType; set => EntityType = value; }
    private EntityTypes EntityType = EntityTypes.Object;
    public Directions CurrentDirection_ { get => CurrentDirection; set => CurrentDirection = value; }
    private Directions CurrentDirection;
    public int Health_ { get => Health; set => Health = value; }
    private int Health;
    public int MaxHealth_ { get => MaxHealth; set => MaxHealth = value; }
    private int MaxHealth;
    public Elements[] DamageImmunities_ { get => null; set => _ = value; }



    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision.transform.name + " entered");
        if (collision.gameObject.TryGetComponent(out iCreatureInterface em)) {
            if (em.GetEntityDirectionEnum() == exitDirection && IsOpen) {
                if (sceneIndex < 0) {
                    if (delayTimer < Time.timeSinceLevelLoad) {
                        Vector3 offset = VectorDict[exitDirection];
                        collision.gameObject.transform.position = ExitDoor.transform.position + offset;
                    }
                } else {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
                }
            }

        }
    }
    public void initExitDoor() {
        if (!ExitDoor)
            return;
        if (IsOpen) {
            ExitDoor.IsOpen = IsOpen;
            ExitDoor.GetComponent<Collider2D>().isTrigger = GetComponent<Collider2D>().isTrigger;
        }
        if (isInvulnerable) {
            ExitDoor.isInvulnerable = isInvulnerable;
        }
        ExitDoor.UpdateSprite();
    }

    public void syncExitDoor() {
        if (!ExitDoor)
            return;
        ExitDoor.IsOpen = IsOpen;
        ExitDoor.GetComponent<Collider2D>().isTrigger = GetComponent<Collider2D>().isTrigger;
        ExitDoor.isInvulnerable = isInvulnerable;
        ExitDoor.UpdateSprite();
    }

    public void OpenCloseDoor(bool newState) {
        IsOpen = newState;
        GetComponent<Collider2D>().isTrigger = newState;
        UpdateSprite();
        syncExitDoor();
    }

    public void SetDoorProperties() {
        EntityType_ = EntityTypes.Object;
        if (isInvulnerable) {
            EntityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Indestructable };
        } else {
            EntityProperties = new Properties[] { Properties.Door, Properties.Immovable, Properties.Flamable };
        }
    }

    public void UpdateSprite() {
        try {
            Sprite currentSprite;
            if (IsOpen) {
                currentSprite = SpriteDict["OpenDoor"][IntDict[exitDirection]];
            } else if (isInvulnerable) {
                currentSprite = SpriteDict["MetalDoor"][IntDict[exitDirection]];
            } else {
                currentSprite = SpriteDict["WoodDoor"][IntDict[exitDirection]];
            }
            GetComponent<SpriteRenderer>().sprite = currentSprite;
        }
        catch (System.Exception) {
            //Debug.LogWarning(ex.Message);
        }
    }

    public void TakeDamage(float damage, Elements damageType) {
        Debug.Log("isHurt");
        if (EntityProperties.Contains(Properties.Indestructable) || damageType != Elements.Fire) {
            return;
        }
        Health_ -= Mathf.RoundToInt(damage);
        Health_ = Mathf.Clamp(Health_, 0, MaxHealth_);
        if (0 >= Health_) {
            EntityDeath();
        }
    }

    public virtual void EntityDeath() {
        OpenCloseDoor(true);
        // TODO: Impliment Colour change
    }

    public abstract void OnValidate();
}