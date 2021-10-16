using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public interface iPropertyInterface {    
    public Properties[] EntityProperties_ { get; set; }
    public EntityTypes EntityType_ { get;}
}
public interface iHealthInterface : iPropertyInterface {
    public int Health_ { get; set; }
    public int MaxHealth_ { get; set; }
    public Elements[] DamageImmunities_ { get; set; }
    public void TakeDamage(float damage, Elements damageType);
    public void EntityDeath();
}

public interface iPhysicsInterface : iPropertyInterface {
    public Rigidbody2D RB_ { get;}
    public float Deceleration_ { get; set; }
    public void UpdateVelocity(float magnitude, Vector3 direction);
    public void UpdateForce(float magnitude, Vector3 direction, Elements elementType = Elements.Pull);
    public void Decelerate();
    public void UpdateSorting();
}

public interface iCreatureInterface : iAnimationInterface {
    public float EntitySpeed_ { get; set; }
    public void UpdateDirection(Vector3 change);
}

public interface iAnimationInterface
{
    public void AlertObservers(AnimationEvents message);
    Animator Anim_ { get; }
    public void UpdateAnimation(Vector3 change);

}

public interface iFacingInterface {
    Directions CurrentDirection_ { get; set; }
    public Directions GetEntityDirectionEnum();
    public int GetEntityFacing();
    public Vector2 GetEntityDirection();
}

public interface iEnemyInterface {
    public float EntityDamage_ { get; set; }
}

public interface iPropertyManager {
    public void AddProperty(Properties property);
    public void RemovePropery(Properties property);
    public void AddProperty(Properties property, float duration);
    public void AddImmunity(Elements element);
    public void RemoveImmunity(Elements property);
    public void AddImmunity(Elements element, float duration);
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
