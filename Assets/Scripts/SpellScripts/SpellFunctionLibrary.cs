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
        {Elements.Pull,         new float[] {1.5f, 0.5f} },
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
        if (gameObject.transform.TryGetComponent(out Rigidbody2D rb)) {
            gameObject.layer = 6;
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
        Debug.Log($" Moving { targetObject.transform.name } to {targetPosition}");
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

    public static IEnumerator ArcHitDetection(ArcBehaviour ac, float Strength, Elements element) {
        Vector3 LastPosition = ac.transform.position;
        while ( ac.HitCollider == null) {
            LastPosition = ac.transform.position;
            yield return null;
        }

        
        if (ac.HitCollider.transform.TryGetComponent(out iPropertyInterface IPro)) {
            Strength = ComputeOutPutValue(element, IPro.EntityProperties_, Strength);
        }

        if (element == Elements.Push || element == Elements.Pull) {
            if (ac.HitCollider.transform.TryGetComponent(out iPhysicsInterface PI)) {
                Vector3 HitPosition = ac.CurrentPosition;
                Vector2 direction = HitPosition - LastPosition;
                PI.UpdateForce(Strength, direction.normalized);
            }
        } else {
            if (ac.HitCollider.transform.TryGetComponent(out iHealthInterface HI)) {
                HI.TakeDamage(Strength, element);
            }
        }
    }

    private static Vector3 NormaliseVector(Vector3 vector) {
        float x = Mathf.Abs(vector.x) / vector.x;
        float y = Mathf.Abs(vector.y) / vector.y;
        float z = Mathf.Abs(vector.z) / vector.z;
        return new Vector3(x, y, z);
    }

    public static IEnumerator ArcPlayerMove(ArcBehaviour ac, ArcData Arc_, Elements element) {
        float Strength = Arc_.baseStrength;
        Vector3 LastPosition = Arc_.CasterObject.transform.position;
        while (ac.HitCollider == null) {
            LastPosition = ac.transform.position;
            yield return null;
        }

        if (Arc_.CasterObject.TryGetComponent(out iPropertyInterface IP)) {
            Strength = ComputeOutPutValue(element, IP.EntityProperties_, Strength);
        }

        if (element == Elements.Pull) {
            float Distance = Vector2.Distance(Arc_.CasterObject.transform.position, LastPosition);
            Arc_.CasterObject.GetComponent<MonoBehaviour>().StartCoroutine(LerpSelf(Arc_.CasterObject, LastPosition, 1f));
        }else if (element == Elements.Push) {
            if (Arc_.CasterObject.TryGetComponent(out iPhysicsInterface PI)) {
                Vector2 direction = LastPosition - Arc_.CasterObject.transform.position;
                Debug.Log(LastPosition + " : " + direction);
                Arc_.CasterObject.GetComponent<iPhysicsInterface>().UpdateForce(-Strength, direction.normalized);
                Arc_.CasterObject.GetComponent<MonoBehaviour>().StartCoroutine(CheckVelocityCanBridgeGaps(Arc_.CasterObject));

            }
        }
    }

    public static void ConeProcess(ConeData Cone_, float baseStrength, Elements element) {
        string msg = "";
        foreach (GameObject gameObject in Cone_.Data) {
            msg += gameObject.name + " ";
            if (element == Elements.Pull || element == Elements.Push) {
                if (gameObject.TryGetComponent(out iPhysicsInterface HI)) {
                    float Strength = ComputeOutPutValue(element, HI.EntityProperties_, baseStrength);
                    Vector2 Direction = gameObject.transform.position - Cone_.CasterObject.transform.position;
                    HI.UpdateForce(Strength, Direction);
                    Debug.DrawLine(Cone_.CasterObject.transform.position, gameObject.transform.position, Color.magenta, 1f);
                }
            } else {
                if (gameObject.TryGetComponent(out iHealthInterface HI)) {
                    float Strength = ComputeOutPutValue(element, HI.EntityProperties_, baseStrength);
                    HI.TakeDamage(Strength, element);
                    Debug.DrawLine(Cone_.CasterObject.transform.position, gameObject.transform.position, Color.red, 1f);
                }
                if(gameObject.TryGetComponent(out EmptySpaceScript ESS)) {
                    ESS.EffectTile(element);
                    Debug.DrawLine(Cone_.CasterObject.transform.position, gameObject.transform.position, Color.cyan, 1f);
                }
            }
        }
        Debug.Log(msg);
    }

    public static GameObject[] ConeCast(float Distance, GameObject Origin, Directions Direction) {
        const float angle = 60;
        const int noRays = 15;
        Vector2 direction = VectorDict[Direction];
        List<GameObject> hitPoints = new List<GameObject>();
        float startAngle = CalculateStartAngle(direction) - (angle / 2);
        int[] StopMask = new int[] { 8 };

        Vector2 Offset = 0.5f * Origin.GetComponent<SpriteRenderer>().bounds.size * direction;
        Vector2 position = Origin.transform.position;
        Vector2 CastOrigin = position + Offset;


        for (int i = 0; i < noRays; i++) {
            float SendAngle = startAngle + i * (angle / noRays);
            SendAngle = SendAngle * Mathf.Deg2Rad;
            Vector2 targetPosition = Distance * new Vector3(Mathf.Sin(SendAngle), Mathf.Cos(SendAngle));

            // This would cast rays only against colliders in layer 5.
            int layerMask = 1 << 5;            
            // But instead we want to collide against everything except layer 5. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            RaycastHit2D[] Hits = Physics2D.RaycastAll(CastOrigin, targetPosition, Distance, layerMask);
            foreach(RaycastHit2D Hit in Hits) {
                if (Hit.collider != null) {
                    if (!hitPoints.Contains(Hit.transform.gameObject) && !StopMask.Contains(Hit.collider.gameObject.layer) && !Hit.collider.TryGetComponent(out PlayerEntity _)) {
                        hitPoints.Add(Hit.transform.gameObject);
                    }
                    if (StopMask.Contains(Hit.collider.gameObject.layer)) {
                        break;
                    }
                }
            }

            Debug.DrawRay(Origin.transform.position, targetPosition, Color.blue, 1f);
        }
        return hitPoints.ToArray();
    }

    private static float CalculateStartAngle(Vector2 direction) {
        float value = (float)((Mathf.Atan2(direction.x, direction.y) / System.Math.PI) * 180f);
        return value;
    }
}
