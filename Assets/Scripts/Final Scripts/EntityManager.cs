using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityManager : MonoBehaviour
{
    #region Define Enums
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
        Heavy, Light, Flamable, Fireproof, Metal, Insulated, Door
    }
    public Properties[] entityProperties;

    [System.Serializable]
    public enum EntityType
    {
        Object,
        Creature
    }
    public EntityType entityType;
    #endregion
    private Rigidbody2D rb;
    [SerializeField]
    private float health = 10;
    private float maxHealth;

    //[SerializeField]
    protected float entitySpeed;
    [SerializeField]
    private Sprite[] EntitySpriteSheet;
    private int EntityFacing;

    // Start is called before the first frame update
    void Awake() {
        if (!TryGetComponent<Rigidbody2D>(out rb) && entityType.Equals(EntityType.Creature)) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        if(rb != null){
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            rb.drag = 1f;
        }
        maxHealth = health;

    }

    public void ChangeSprite(Vector3 movement) {   //Up ; left; Down; right
        if (Mathf.Abs(movement.x) < Mathf.Abs(movement.y)) {
            if (movement.y > 0) {
                EntityFacing = 0;
            } else {
                EntityFacing = 2;
            }
        } else {
            if (movement.x > 0) {
                EntityFacing = 1;
            } else {
                EntityFacing = 3;
            }
        }
        GetComponent<SpriteRenderer>().sprite = EntitySpriteSheet[EntityFacing];
    }

    public void TakeDamage(float damage) {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (0 >= health) {
            EntityDeath();
        }
    }

    public void MoveEntity(Vector3 difference) {
        Vector3 position = transform.position;
        rb.MovePosition(position + difference);
        ChangeSprite(difference);
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

    public int GetEntityFacing() {
        return EntityFacing;
    }

    public Vector2 GetEntityDirection() {
        Vector2[] facings = new Vector2[4];
        facings[0] = Vector2.up;
        facings[1] = Vector2.right;
        facings[2] = Vector2.down;
        facings[3] = Vector2.left;
        return facings[EntityFacing];
    }
}
