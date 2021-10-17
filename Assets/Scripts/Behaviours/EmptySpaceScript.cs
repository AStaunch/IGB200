using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpriteManager;

public class EmptySpaceScript : MonoBehaviour
{
    public VoidType VoidType_;
    public string VoidFills = "VoidFills";
    // 0 - Block Fill, 1 - Frozen Water
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private Rigidbody2D rb;

    public GameObject objectIntoWaterSound;
    public GameObject objectIntoVoidSound;
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
            if (VoidType_ == VoidType.Water && !bc.isTrigger) {
                sr.sprite = SpriteDict[VoidFills][0];
                Instantiate(objectIntoWaterSound);
                Destroy(collision.gameObject);
                Destroy(bc);
                Destroy(rb);
            } else if (VoidType_ == VoidType.Void && !bc.isTrigger) {
                Instantiate(objectIntoVoidSound);
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

    public void EffectTile(Elements element) {
        //If it isnt water or the trigger isnt ice, dont worry for now
        if(VoidType_ == VoidType.Water && element == Elements.Ice) {
            float Duration = 7f;
            StartCoroutine(FreezeForTime(Duration));
        } else if (VoidType_ == VoidType.Water && element == Elements.Fire) {
            isFrozen_ = false;
        }
        //Define Vars

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
}