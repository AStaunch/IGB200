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
    public bool isFrozen_ { 
        get { return isFrozen; }
        set { isFrozen = value; ToggleFrozen(value); }
    }
    private bool isFrozen;
    float Duration = 7f;

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
        sr.sortingLayerID = -1;
        sr.material = SpellRenderer.Instance.defaultUnlit;

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
            StartCoroutine(FreezeForTime(Duration));
            ChainReactionIce();
        } else {
            sr.sprite = null;
            bc.isTrigger = false;
        }
    }

    private void ChainReactionIce() {
        int mask = 1 << 2;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 1f, mask)) {
            if(collider.TryGetComponent(out EmptySpaceScript ESS)) {
                if (!ESS.isFrozen_) {
                    ESS.isFrozen_ = true;
                }
            }
        }
    }

    private void ChainReactionElecticity() {
        int mask = 1 << 2 | 1 << 6 | 1 << 7 | 1 << 0;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 1f, mask)) {
            if (collider.TryGetComponent(out iHealthInterface iHealth)) {
                if (collider.TryGetComponent(out EmptySpaceScript ESS)) {
                    if (!ESS.isElectric_) {
                        ESS.TakeDamage(1f, Elements.Electricity);
                    }
                } else {
                    iHealth.TakeDamage(1f, Elements.Electricity);
                }
            }
        }
    }

    private IEnumerator FreezeForTime(float duration) {
        float time = 0;
        while (time < duration && isFrozen_ == true) {
            time += Time.deltaTime;
            yield return null;
        }
        isFrozen_ = false;
    }

    private IEnumerator ElectricForTime(float duration) {
        float time = 0;
        isElectric = true;
        while (time < duration && !isFrozen_) {
            time += Time.deltaTime;
            yield return null;
        }
        isElectric = false;
    }

    public void TakeDamage(float damage, Elements damageType) {
        if(VoidType_ == VoidType.Water) {
            if (damageType == Elements.Ice) {
                isFrozen_ = true;
            } else if (damageType == Elements.Fire) {
                isFrozen_ = false;
            } else if (damageType == Elements.Electricity) {
                if(!isFrozen_ && !isElectric_) {
                    ChainReactionElecticity();
                }
            }
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

    public bool isElectric_ { get => isElectric; set => StartCoroutine(ElectricForTime(0.1f)); }
    private bool isElectric;
}