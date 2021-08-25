using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityManager : MonoBehaviour
{
    [System.Serializable]
    public enum Properties
    {
        Heavy, Light, Flamable, Fireproof, Metal, Insulated,
    }
    public Properties[] entityProperties;

    [System.Serializable]
    public enum EntityType
    {
        Object,
        Creature
    }
    public EntityType entityType;

    private Rigidbody2D rb;
    private float health = 10;
    private float maxHealth;

    //[SerializeField]
    protected float entitySpeed;
    [SerializeField]
    private Sprite[] EntitySpriteSheet;
    private int EntityFacing;

    // Start is called before the first frame update
    void Awake()
    {
        if(!TryGetComponent<Rigidbody2D>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        maxHealth = health;

    }

    public void ChangeSprite(Vector3 movement)
    { 
        if (Mathf.Abs(movement.x) < Mathf.Abs(movement.y)) {
            if (movement.y > 0) {
                EntityFacing = 0;
            }
            else {
                EntityFacing = 2;
            }
        }
        else {
            if (movement.x > 0) {
                EntityFacing = 1;
            }
            else {
                EntityFacing = 3;
            }
        }
        GetComponent<SpriteRenderer>().sprite = EntitySpriteSheet[EntityFacing];
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (0 >= health) {
            EntityDeath();
        }
    }

    public void MoveEntity(Vector3 difference)
    {
        Vector3 position = transform.position;
        rb.MovePosition(position + difference);
        ChangeSprite(difference);
    }
    private void EntityDeath()
    {
        // TODO: Impliment Colour change
        // SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(this.gameObject, Time.deltaTime);
    }

    public int GetEntityFacing()
    {
        return EntityFacing;
    }
}
