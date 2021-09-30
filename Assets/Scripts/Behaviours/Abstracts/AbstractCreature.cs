using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;

public abstract class AbstractCreature : MonoBehaviour, iHealthInterface, iCreatureInterface, iPhysicsInterface, iPropertyManager
{
    public int Health_ { get => Health; set => Health = value; }
    private int Health;
    public int MaxHealth_ { get => MaxHealth; set => MaxHealth = value; }
    public int MaxHealth;
    public abstract Elements[] DamageImmunities_ { get; set; }


    public Properties[] EntityProperties_ { get => EntityProperties; set => EntityProperties = value; }
    public Properties[] EntityProperties;
    public EntityTypes EntityType_ { get => EntityType; set => EntityType = value; }
    private EntityTypes EntityType = EntityTypes.Creature;
    public float EntitySpeed_ { get => EntitySpeed; set => EntitySpeed = value; }
    public float EntitySpeed;
    public Animator Anim_ { get => GetComponent<Animator>();}
    public Directions CurrentDirection_ { get => CurrentDirection; set => CurrentDirection = value; }
    private Directions CurrentDirection;


    public Rigidbody2D RB_ { get => GetComponent<Rigidbody2D>();}
    public float Deceleration;
    public float Deceleration_ { get => Deceleration; set => Deceleration = value; }


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
        if (DamageImmunities_.Contains(damageType)) {
            return;
        }
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

    public abstract void  UpdateVelocity(float magnitude, Vector3 direction);

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
    #endregion
}