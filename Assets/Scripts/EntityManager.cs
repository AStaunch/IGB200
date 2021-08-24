using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Properties
{
    Heavy, Light, Flamable, Fireproof, Metal, Insulated,
}
public class EntityManager : MonoBehaviour
{
    [System.Serializable]
    public enum EntityType
    {
        Object,
        Creature
    }
    public EntityType TypeOfEntity;

    private Rigidbody2D rb;
    private float health = 10;
    private float maxHealth;

    //[SerializeField]
    protected float entitySpeed;
    [SerializeField]
    private Sprite[] EntitySpriteSheet;
    public int EntityFacing { set => EntityFacing = value; get => EntityFacing; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeSprite(Vector3 movement)
    {
        if (Mathf.Abs(movement.x) < Mathf.Abs(movement.y))
        {
            if (movement.y > 0)
            {
                EntityFacing = 0;
            }
            else
            {
                EntityFacing = 2;
            }
        }
        else
        {
            if (movement.x > 0)
            {
                EntityFacing = 1;
            }
            else
            {
                EntityFacing = 3;
            }
        }
        GetComponent<SpriteRenderer>().sprite = EntitySpriteSheet[EntityFacing];
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (0 >= health)
        {
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
        Destroy(this.gameObject, Time.deltaTime);
    }
}
