using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public class ArcBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 startPosition;

    private void Start() {
       
    }
    public void StartArc(Directions direction) {
        //initialise Variables
        float distance = 10f;
        float time = 3f;

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;
        rb.freezeRotation = true;
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<CircleCollider2D>();
        startPosition = transform.position;
        StartCoroutine(moveArc(VectorDict[direction], time, distance));

    }

    IEnumerator moveArc(Vector2 VectorDirection, float arcTime, float distance) {
        Vector2 SwappedDirection = new Vector2(VectorDirection.y, VectorDirection.x);
        float i = 0;

        AnimationCurve arcCurve = FindObjectOfType<SpellRenderer>().arcCurve;
        float maxTime = 3f * arcTime;
        while (i < maxTime) {
            if (i <= arcTime) {
                float iNormalised = i / arcTime;
                Vector2 arcOffsetX = distance * iNormalised * VectorDirection;
                Vector2 arcOffsetY = distance * arcCurve.Evaluate(iNormalised) * SwappedDirection;
                Vector2 arcOffsetXY = arcOffsetX + arcOffsetY;
                rb.MovePosition(startPosition + arcOffsetXY);

                //DebugCode - TODO Get Rid of
                float intervalDebug = i / 0.5f;
                intervalDebug %= 1;
                if (intervalDebug < 0.01f) {
                    Debug.Log(arcOffsetXY);
                }
            } else {
            }
            i += Time.deltaTime;
            yield return null;
        }
        KillThis();
    }

    public void KillThis() {

    }
    public iPropertyInterface em = null;
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.transform.TryGetComponent(out  em)) {
            
        }
    }
}
