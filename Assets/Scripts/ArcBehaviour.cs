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
    public void StartArc(Directions direction, GameObject spellMaster) {
        //initialise Variables
        float distance = 10f;
        float time = 3f;
        float maxTime = 3f * time;
        AnimationCurve arcCurve = FindObjectOfType<SpellRenderer>().arcCurve;

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<CircleCollider2D>();
        startPosition = transform.position;

        Vector2 VectorDirection = VectorDict[direction];
        Vector2 SwappedDirection = new Vector2(VectorDirection.y, VectorDirection.x);
        float i = 0;
        while (i < maxTime) {
            if(i <= time){
                    float iNormalised = i / time;
                    Vector2 arcOffsetX = distance * iNormalised * VectorDirection;
                    Vector2 arcOffsetY = distance * arcCurve.Evaluate(iNormalised) * SwappedDirection;
                    Vector2 arcOffsetXY = arcOffsetX + arcOffsetY;
                    rb.MovePosition(startPosition + arcOffsetXY);
                    
                    //DebugCode - TODO Get Rid of
                    float intervalDebug = i / 0.5f;
                    intervalDebug = intervalDebug % 1;
                    if (intervalDebug < 0.01f) {
                        Debug.Log(arcOffsetXY);
                    }
            } else {

            }
            i += Time.deltaTime;
        }

        spellMaster.AddComponent<DestroyThis>();
        gameObject.AddComponent<DestroyThis>();
    }
}
