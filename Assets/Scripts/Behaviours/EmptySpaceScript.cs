using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;
using static SoundManager;
public class EmptySpaceScript : MonoBehaviour, iHealthInterface
{
    public VoidType VoidType_;

    private string VoidFills = "VoidFills"; // 0 - Block Fill, 1 - Frozen Water
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private bool isFrozen_ { 
        get { return isFrozen; }
        set { isFrozen = value; ToggleFrozen(value); }
    }
    private bool isFrozen;

    private void Awake() {
        gameObject.layer = 2;
        if (!TryGetComponent(out rb)) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Static;
        rb.gravityScale = 0;
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
            if (VoidType_ == VoidType.Water && !bc.isTrigger) 
            {
                sr.sprite = SpriteDict[VoidFills][0];
                Instantiate(SoundDict[VoidType_ + "DropSound"]);
                Destroy(collision.gameObject);
                Destroy(bc); Destroy(rb);
            } else if (VoidType_ == VoidType.Void && !bc.isTrigger) 
            {
                Instantiate(SoundDict[VoidType_ + "DropSound"]);
                Destroy(collision.gameObject);
            }
        }
    }

    private void ToggleFrozen(bool boo){
        if (boo) {
            sr.sprite = SpriteDict[VoidFills][1];
            bc.isTrigger = true;
        } else {
            sr.sprite = null;
            bc.isTrigger = false;
        }
    }
    private IEnumerator FreezeForTime(float duration) {
        float time = 0;
        isFrozen_ = true;
        while (time < duration && isFrozen_ == true) {
            time += Time.deltaTime;
            yield return null;
        }
        isFrozen_ = false;
    }

    public void TakeDamage(float damage, Elements damageType) {
        if (VoidType_ == VoidType.Water && damageType == Elements.Ice) {
            float Duration = 7f;
            StartCoroutine(FreezeForTime(Duration));
        } else if (VoidType_ == VoidType.Water && damageType == Elements.Fire) {
            isFrozen_ = false;
        }
    }

    public void EntityDeath() {
        throw new NotImplementedException();
    }
    public int Health_ { get => -1; set => _ = value; }
    public int MaxHealth_ { get => -1; set => _ = value; }
    public Elements[] DamageImmunities_ { get => new Elements[0]; set => _ = value; }
    public Properties[] EntityProperties_ { get => new Properties[0]; set => _ = value; }
    public EntityTypes EntityType_ => EntityTypes.Object;
}