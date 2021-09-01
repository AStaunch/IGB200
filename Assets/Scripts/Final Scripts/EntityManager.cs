using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class EntityManager : MonoBehaviour
{
    #region Define Enums And Dictionaries
    public enum Directions
    {
        Up,
        Right,
        Down,
        Left
    }

    [System.Serializable]
    public enum Properties
    {
        Heavy, Light, Flamable, Fireproof, Metal, Insulated, Door, Indestructable, Immovable
    }
    public Properties[] entityProperties;

    [System.Serializable]
    public enum EntityType
    {
        Object,
        Creature
    }
    public EntityType entityType;

    public static Dictionary<EntityManager.Directions, Vector2> VectorDict = new Dictionary<EntityManager.Directions, Vector2>()
    {
        { EntityManager.Directions.Up,      new Vector2(0, 1) },
        { EntityManager.Directions.Down,    new Vector2(0,-1) },
        { EntityManager.Directions.Left,    new Vector2(-1,0) },
        { EntityManager.Directions.Right,   new Vector2(1, 0) },
    };

    public static Dictionary<EntityManager.Directions, int> IntDict = new Dictionary<EntityManager.Directions, int>()
    {
        { EntityManager.Directions.Up,      0},
        { EntityManager.Directions.Down,    2},
        { EntityManager.Directions.Left,    3 },
        { EntityManager.Directions.Right,   1 },
    };
    Directions CurrentDirection = Directions.Down;
    #endregion

    private Rigidbody2D rb;
    [SerializeField]
    private float health = 10;
    private float maxHealth;

    //[SerializeField]
    public float entitySpeed;
    [SerializeField]
    private Sprite[] EntitySpriteSheet;

    private Animator anim;
    // Start is called before the first frame update
    void Awake() {
        if (!gameObject.TryGetComponent(out rb) && entityType.Equals(EntityType.Creature)) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        if(rb != null){
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            if(rb.drag == 0)
                rb.drag = 1f;
        }
        maxHealth = health;
        if(gameObject.TryGetComponent(out anim)){
            
        }
    }

    public void TakeDamage(float damage) {
        if (entityProperties.Contains(Properties.Indestructable)){
            damage = 0f; 
        }
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (0 >= health) {
            EntityDeath();
        }
    }
    public void UpdatePosition(Vector3 change) {
        if (change != Vector3.zero) {
            UpdateDirection(change);
        }
        change = entitySpeed * change.normalized;
        rb.velocity = (change);
    }

    public void UpdateAnimation(Vector3 change) {
        if (change != Vector3.zero) {
            anim.SetFloat("moveX", change.x);
            anim.SetFloat("moveY", change.y);
            anim.SetBool("moving", true);
        } else {
            anim.SetBool("moving", false);
        }
    }

    public void UpdateDirection(Vector3 change) {   //Up ; Right; Down; Left
        if (change != Vector3.zero) {

            if (Mathf.Abs(change.x) < Mathf.Abs(change.y)) {
                if (change.y > 0) {
                    CurrentDirection = Directions.Up;
                } else {
                    CurrentDirection = Directions.Down;
                }
            } else {
                if (change.x > 0) {
                    CurrentDirection = Directions.Right;
                } else {
                    CurrentDirection = Directions.Left;
                }
            }
        }
    }

    private void EntityDeath() {

        if (entityType == EntityType.Object) {
            if (entityProperties.Contains(Properties.Door) && TryGetComponent(out DoorScript portal)) {
                portal.SetDoor(true);
            }
        } else {
            Destroy(this.gameObject, Time.deltaTime);
        }
        // TODO: Impliment Colour change
        // SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public Directions GetEntityDirectionEnum() {
        return CurrentDirection;
    }
    public int GetEntityFacing() {
        return IntDict[CurrentDirection];
    }

    public Vector2 GetEntityDirection() {
        return VectorDict[CurrentDirection];
    }
}
