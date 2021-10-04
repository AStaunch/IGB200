using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySpaceScript : MonoBehaviour
{
    [System.Serializable]
    public enum TypeOfTile
    {
        Water, Void
    }
    public TypeOfTile emptySpaceType;
    public Sprite Filled;
    SpriteRenderer sr;
    BoxCollider2D bc;
    private Rigidbody2D rb;

    public GameObject objectIntoWaterSound;
    public GameObject objectIntoVoidSound;

    private void Awake() {
        gameObject.layer = 2;
        if (!TryGetComponent(out rb)) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;

        if (!TryGetComponent(out sr)) {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }
        sr.sprite = null;

        if (!TryGetComponent(out bc)) {
            bc = gameObject.AddComponent<BoxCollider2D>();
        }
        bc.size = new Vector2( 1.0f, 1.0f );
        bc.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out BlockScript _)){
            if (emptySpaceType == TypeOfTile.Water) {
                sr.sprite = Filled;
                Instantiate(objectIntoWaterSound);
                Destroy(collision.gameObject);
                Destroy(bc);
                Destroy(rb);
            } else if (emptySpaceType == TypeOfTile.Void) {
                Instantiate(objectIntoVoidSound);
                Destroy(collision.gameObject);
            }
        }
    }

    
}