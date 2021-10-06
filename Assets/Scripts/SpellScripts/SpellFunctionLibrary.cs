using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;

public static class SpellFunctionLibrary
{
    private static Dictionary<Elements, Properties[]> ElementPropertyPairs = new Dictionary<Elements, Properties[]> {
        {Elements.Fire,             new Properties[] {Properties.Flamable,  Properties.Fireproof} },
        {Elements.Ice,              new Properties[] {Properties.Freezable, Properties.Unfreezable} },
        {Elements.Electricity,      new Properties[] {Properties.Metal,     Properties.Insulated} },
        {Elements.Life,             new Properties[] {Properties.Undead,    Properties.Undead} },
        {Elements.Pull,             new Properties[] {Properties.Light,     Properties.Heavy} },
        {Elements.Push,             new Properties[] {Properties.Light,     Properties.Heavy} },
        {Elements.Death,            new Properties[] {Properties.Undead,    Properties.Undead } },
        {Elements.Earth,            new Properties[] {Properties.Frozen,    Properties.Metal} },
    };

    private static Dictionary<Elements, float[]> ElementValuePairs = new Dictionary<Elements, float[]> {
        {Elements.Fire,         new float[] {2f, 0f} },
        {Elements.Ice,          new float[] {2f, 0f} },
        {Elements.Electricity,  new float[] {2f, 0f} },
        {Elements.Life,         new float[] {-1f, -1f} },
        {Elements.Pull,         new float[] {-1.5f,-0.5f} },
        {Elements.Push,         new float[] {1.5f, 0.5f} },
        {Elements.Death,        new float[] {0f, 0f} },
        {Elements.Earth,        new float[] {5f, .5f} },
    };

    public static float ComputeOutPutValue(Elements element, Properties[] properties, float inputValue) {
        if(properties.Length == 0) {
            return inputValue;
        }
        if (properties.Contains(ElementPropertyPairs[element][0])) {
            inputValue *= ElementValuePairs[element][0];
        } else if (properties.Contains(ElementPropertyPairs[element][1])) {
            inputValue *= ElementValuePairs[element][1];
        }
        if (properties.Contains(Properties.Immovable) && (element == Elements.Pull || element == Elements.Push)) {
            inputValue = 0f;
        }
        return inputValue;
    }

    public static IEnumerator CheckVelocityCanBridgeGaps(GameObject gameObject) {
        if(gameObject.transform.TryGetComponent(out Rigidbody2D rb)) {
            while (rb.velocity.magnitude > 1f) {
                gameObject.layer = 6;
                yield return null;
            }
            gameObject.layer = 7;
        } else {
            yield return null;
        }
    }

    public static IEnumerator LerpSelf(GameObject targetObject, Vector2 targetPosition, float duration) {
        if (targetObject.TryGetComponent(out iPhysicsInterface iPhysics)) {
            float time = 0;
            Vector2 startPosition = targetObject.transform.position;
            targetPosition -= 0.6f * targetObject.GetComponent<SpriteRenderer>().bounds.size * targetObject.GetComponent<iFacingInterface>().GetEntityDirection();
            while (time < duration) {
                float t = time / duration;
                t = t * t * (3f - 2f * t);
                iPhysics.RB_.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
                time += Time.deltaTime;
                targetObject.layer = 6;
                yield return null;
            }
            iPhysics.RB_.MovePosition(targetPosition);
            targetObject.layer = 7;
        }        
    }

    public static IEnumerator ArcHitDetection(ArcBehaviour ac, ArcData Arc_, Elements element) {
        float Strength = Arc_.baseStrength;
        while (ac.HitCollider == null && ac.HitCollision == null) {
            yield return null;
        }
        Collider2D ColHit = null;
        if (ac.HitCollider) {
            ColHit = ac.HitCollider;
        } else if (ac.HitCollision != null) {
            ColHit = ac.HitCollision.collider;
        }

        

        if (ColHit.transform.TryGetComponent(out iPropertyInterface IPro)) {
            Strength = ComputeOutPutValue(element, IPro.EntityProperties_, Strength);
        }

        if (element == Elements.Pull) {
            Strength = -Strength;
        }

        if (element == Elements.Push || element == Elements.Pull) {
            if (ColHit.transform.TryGetComponent(out iPhysicsInterface PI)) {
                Vector2 direction = ColHit.transform.position - ac.transform.position;
                Debug.Log(ColHit.transform.position + " : " + direction);
                PI.UpdateVelocity(Strength, direction.normalized);
            }
        } else {
            if (ColHit.transform.TryGetComponent(out iHealthInterface HI)) {
                HI.TakeDamage(Strength, element);
            }
        }
    }

    public static GameObject[] ConeCast(float Distance, GameObject Origin, Directions Direction) {
        const float angle = 60;
        const int noRays = 15;
        Vector2 direction = VectorDict[Direction];
        List<GameObject> hitPoints = new List<GameObject>();
        float startAngle = CalculateStartAngle(direction) - (angle / 2);
        int[] StopMask = new int[] { 8 };
        for (int i = 0; i < noRays; i++) {
            float SendAngle = startAngle + i * (angle / noRays);
            SendAngle = SendAngle * Mathf.Deg2Rad;
            Vector2 targetPosition = Distance * new Vector3(Mathf.Sin(SendAngle), Mathf.Cos(SendAngle));
            RaycastHit2D hit = Physics2D.CircleCast(Origin.transform.position, 0.1f, targetPosition);
            if (hit.collider != null) {
                if (!hitPoints.Contains(hit.transform.gameObject)) {
                    hitPoints.Add(hit.transform.gameObject);
                }
            }
            //Debug.DrawRay(Origin.transform.position, targetPosition, Color.blue, 1f);
        }
        return hitPoints.ToArray();
    }

    private static float CalculateStartAngle(Vector2 direction) {
        float value = (float)((Mathf.Atan2(direction.x, direction.y) / System.Math.PI) * 180f);
        return value;
    }
}
