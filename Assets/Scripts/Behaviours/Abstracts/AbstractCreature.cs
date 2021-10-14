using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;

public abstract class AbstractCreature : MonoBehaviour, iHealthInterface, iCreatureInterface, iPhysicsInterface, iPropertyManager, iFacingInterface
{
    public int Health_ { get => Health; set => Health = value; }
    private int Health;
    public int MaxHealth_ { get => MaxHealth; set => MaxHealth = value; }
    private int MaxHealth;
    public Elements[] DamageImmunities_ { get => DamageImmunities; set => DamageImmunities = value; }
    private Elements[] DamageImmunities = new Elements[0];

    public Properties[] EntityProperties_ { get => EntityProperties; set => EntityProperties = value; }
    [SerializeField]
    private Properties[] EntityProperties = new Properties[0];
    public EntityTypes EntityType_ { get => EntityType; set => EntityType = value; }
    [SerializeField]
    private EntityTypes EntityType = EntityTypes.Creature;
    public float EntitySpeed_ { get => EntitySpeed; set => EntitySpeed = value; }
    [SerializeField]
    private float EntitySpeed;
    public Animator Anim_ { get => GetComponent<Animator>();}
    public Directions CurrentDirection_ { get => CurrentDirection; set => CurrentDirection = value; }
    private Directions CurrentDirection;
    public Rigidbody2D RB_ { get => GetComponent<Rigidbody2D>();}
    public float Deceleration;
    public float Deceleration_ { get => Deceleration; set => Deceleration = value; }

    public GameObject damageSound;
    private void FixedUpdate() {
        Decelerate();
        UpdateSorting();
    }

    private void Awake() {
        GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
    }

    public abstract void Decelerate();
    public abstract void EntityDeath();
    public Vector2 GetEntityDirection() {
        return VectorDict[CurrentDirection_];
    }

    public Directions GetEntityDirectionEnum() {
        return CurrentDirection_;
    }

    public int GetEntityFacing() {
        return IntDict[CurrentDirection_];
    }

    public void TakeDamage(float damage, Elements damageType) {
        if (DamageImmunities_.Contains(damageType) || Mathf.RoundToInt(damage) == 0) {
            return;
        }
        SpellRenderer hitDrawer = FindObjectOfType<SpellRenderer>();
        hitDrawer.CreateBurstFX(transform.position, ColourDict[damageType]);
        Instantiate(damageSound);
        Health_ -= Mathf.RoundToInt(damage);
        Health_ = Mathf.Clamp(Health_, 0, MaxHealth_);
        if (0 >= Health_) {
            EntityDeath();
        }
    }

    public abstract void UpdateAnimation(Vector3 change);

    public void UpdateDirection(Vector3 change) {
        if (change != Vector3.zero) {
            CurrentDirection_ = VectorToDirection(change);
        }
    }
    public void UpdateSorting() {
        GetComponent<Renderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }

    public abstract void  UpdateVelocity(float magnitude, Vector3 direction);
    public abstract void UpdateForce(float magnitude, Vector3 direction);
    #region Property Management
    public void AddProperty(Properties property) {
        if (!EntityProperties_.Contains(property)) {
            //Add Property
            Array.Resize(ref EntityProperties, EntityProperties_.Length + 1);
            EntityProperties_[EntityProperties_.Length - 1] = property;
        }
    }

    public void RemovePropery(Properties property) {
        if (EntityProperties_.Contains(property)) {
            int index = Array.FindIndex(EntityProperties, 0, EntityProperties_.Length, EntityProperties_.Contains);
            for (; index < EntityProperties_.Length - 1; index++) {
                EntityProperties_[index] = EntityProperties_[index + 1];
            }
            Array.Resize(ref EntityProperties, EntityProperties_.Length - 1);
        }
    }

    public void AddProperty(Properties property, float duration) {
        if (!EntityProperties_.Contains(property)) {
            StartCoroutine(AddPropertyForDuration(property, duration));
        }
    }

    private IEnumerator AddPropertyForDuration(Properties property, float duration) {
        float t = 0;
        Array.Resize(ref EntityProperties, EntityProperties_.Length + 1);
        EntityProperties_[EntityProperties_.Length - 1] = property;

        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        Array.Resize(ref EntityProperties, EntityProperties_.Length - 1);

    }


    public void AddImmunity(Elements element) {
        if (!DamageImmunities_.Contains(element)) {
            //Add Property
            Array.Resize(ref DamageImmunities, DamageImmunities_.Length + 1);
            DamageImmunities_[DamageImmunities_.Length - 1] = element;
        }
    }

    public void RemoveImmunity(Elements property) {
        if (DamageImmunities_.Contains(property)) {
            int index = Array.FindIndex(DamageImmunities, 0, DamageImmunities_.Length, DamageImmunities_.Contains);
            for (; index < DamageImmunities_.Length - 1; index++) {
                DamageImmunities_[index] = DamageImmunities_[index + 1];
            }
            Array.Resize(ref EntityProperties, EntityProperties_.Length - 1);
        }
    }

    public void AddImmunity(Elements element, float duration) {
        if (!DamageImmunities_.Contains(element)) {
            StartCoroutine(AddImmunityForDuration(element, duration));
        }
    }

    private IEnumerator AddImmunityForDuration(Elements element, float duration) {
        float t = 0;
        Array.Resize(ref DamageImmunities, DamageImmunities_.Length + 1);
        DamageImmunities_[DamageImmunities_.Length - 1] = element;

        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        Array.Resize(ref DamageImmunities, DamageImmunities_.Length - 1);
    }
    public IEnumerator MovePause(float Wait) {
        float SpeedStore = EntitySpeed_;
        float time = 0;
        while (time < Wait) {
            EntitySpeed_ = 0f;
            time += Time.deltaTime;
            yield return null;
        }
        EntitySpeed_ = SpeedStore;
    }
    #endregion
}