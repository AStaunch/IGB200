using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Properties
{
    Heavy, Light, Flamable, Fireproof, Metal, Insulated,
}
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
        // TODO: Impliment Colour change
        GetComponent<SpriteRenderer>().color.g = 0;
        GetComponent<SpriteRenderer>().color.b = 0;
        Destroy(this.gameObject, Time.deltaTime);
    }

    private void RaycastFireMock(ParameterClass parameter)
    {
        float baseDmg = 10f;
        //Get Raycast
        /////   Create the Beam

        //Get Distance (d)
        //Length of Sections (B, M, E)
        //Get No of Tiles in Distance
        /* Palette Swap Colours of Ray.
         * Get First Tile; set to start
         * Get Last Tile; set to end
         * For everyother tile; set to middle
         * 
         */
        

        /////   Effect What It Hits
        //Get The Game object it hits
        //Check EntityType Enum
        //Check EntityProp Enum for Flamable/Fireproof
        GameObject other = hit.gameObject;


        //Property.Behave(GameObject other, Float baseDmg)
       
        if (other.TryGetComponent<EntityManager>(out EntityManager otherEntity))
        {
            if (otherEntity.entityProperties.Contains(Properties.Flamable))
            {
                otherEntity.TakeDamage(baseDmg*2f);
            }
            else if (otherEntity.entityProperties.Contains(Properties.Fireproof))
            {
                otherEntity.TakeDamage(0f);
            }
            else
            {
                otherEntity.TakeDamage(baseDmg);
            }
        }
    }
}
