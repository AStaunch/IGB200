using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Properties
{
    Heavy, Light, Flamable, Fireproof, 
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
    private float entitySpeed;
    [SerializeField]
    private Sprite[] EntitySpriteSheet;


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
        int currentSpriteIndex;
        if (Mathf.Abs(movement.x) < Mathf.Abs(movement.y))
        {
            if (movement.y > 0)
            {
                currentSpriteIndex = 0;
            }
            else
            {
                currentSpriteIndex = 1;
            }
        }
        else
        {
            if (movement.x > 0)
            {
                currentSpriteIndex = 3;
            }
            else
            {
                currentSpriteIndex = 2;
            }
        }
        GetComponent<SpriteRenderer>().sprite = EntitySpriteSheet[currentSpriteIndex];
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
