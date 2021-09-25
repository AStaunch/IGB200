using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public interface iEntityInterface
{

    public Properties[] EntityProperties { get; set; }
    public EntityType EntityType { get; set; }
    Directions CurrentDirection { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }



    #region Entity Health and Death
    public void TakeDamage(float damage);

    public void EntityDeath();
    #endregion


}

public interface iObjectBase : iEntityInterface
{
    
    public float Deceleration { get; set; }
    public void UpdateVelocity(Vector3 change);
    public void Decelerate(Vector2 velocity);
}

public interface iCreatureBase : iEntityInterface
{

    public float EntitySpeed { get; set; }
    
    public float Deceleration { get; set; }
    Animator Anim { get; set; }
    Rigidbody2D RB { get; set; }

    #region Update Variables Externally
    public void UpdateVelocity(Vector3 change);
    public void Decelerate(Vector2 velocity);
    public void UpdateAnimation(Vector3 change);
    public void UpdateDirection(Vector3 change);

    #endregion
    #region Facing Getters
    public Directions GetEntityDirectionEnum();
    public int GetEntityFacing();
    public Vector2 GetEntityDirection();
    #endregion
}

public interface EnemyBase : iCreatureBase
{
    public float EntityDamage { get; set; }
}

public class IKDYet
{
    #region IDK Management
    /*
    private Vector2 Decelerate(Vector2 velocity) {
        //Debug.Log(vector2);
        if (velocity == Vector2.zero) {
            return velocity;
        }

        velocity -= Deceleration * Time.deltaTime * velocity;

        if (velocity.magnitude < 0.25f) {
            velocity *= 0f;
        }
        if (anim && velocity == Vector2.zero) {
            UpdateAnimation(velocity);
        }
        return velocity;
    }


    public void AddProperty(Properties property) {
        if (!entityProperties.Contains(property)) {
            //Add Property
            Array.Resize(ref entityProperties, entityProperties.Length + 1);
            entityProperties[entityProperties.Length - 1] = property;
        }
    }

    public void RemovePropery(Properties property) {
        if (entityProperties.Contains(property)) {
            int index = Array.FindIndex(entityProperties, 0, entityProperties.Length, entityProperties.Contains);
            for (; index < entityProperties.Length - 1; index++) {
                entityProperties[index] = entityProperties[index + 1];
            }
            Array.Resize(ref entityProperties, entityProperties.Length - 1);
        }
    }

    public void AddProperty(Properties property, float duration) {
        if (!entityProperties.Contains(property)) {
            StartCoroutine(AddPropertyForDuration(property, duration));
        }
    }

    private IEnumerator AddPropertyForDuration(Properties property, float duration) {
        float t = 0;
        Array.Resize(ref entityProperties, entityProperties.Length + 1);
        entityProperties[entityProperties.Length - 1] = property;

        while (t < duration) {
            t += Time.deltaTime;
            yield return null;
        }
        Array.Resize(ref entityProperties, entityProperties.Length - 1);

    }
    */
    #endregion
}
