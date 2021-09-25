using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;

public static class SpellFunctionLibrary
{
    private static Dictionary<Elements, Properties[]> ElementPropertyPairs = new Dictionary<Elements, Properties[]> {
        {Elements.Fire, new Properties[] {Properties.Flamable, Properties.Fireproof} },
        {Elements.Pull, new Properties[] {Properties.Light, Properties.Heavy} },
        {Elements.Push, new Properties[] {Properties.Light, Properties.Heavy} },
    };

    private static Dictionary<Elements, float[]> ElementValuePairs = new Dictionary<Elements, float[]> {
        {Elements.Fire, new float[] {2f, 0f} },
        {Elements.Ice, new float[]  {2f, 0f} },
        {Elements.Lightning, new float[] {2f, 0f} },
        {Elements.Life, new float[]  {2f, 0f} },

        {Elements.Pull, new float[] {1.5f, 0.5f} },
        {Elements.Push, new float[] {1.5f, 0.5f} },
    };

    public static float ComputeOutPutValue(Elements element, EntityManager otherEntity, float inputValue) {

        if (otherEntity.entityProperties.Contains(ElementPropertyPairs[element][0])) {
            inputValue *= ElementValuePairs[element][0];
        } else if (otherEntity.entityProperties.Contains(ElementPropertyPairs[element][1])) {
            inputValue *= ElementValuePairs[element][1];
        }
        return inputValue;
    }

    public static void ForceObject(GameObject obj, Vector2 direction, float baseStrength) {
        float commonFactor = 0.25f;
        if (obj.TryGetComponent(out EntityManager em)) {
            baseStrength = ComputeOutPutValue(Elements.Push, em, baseStrength);
            float force = baseStrength * commonFactor;
            Vector2 forceVector = force * direction;
            em.UpdateVelocity(forceVector);
        }
    }

    public static IEnumerator CheckVelocityCanBridgeGaps(GameObject gameObject) {
        Rigidbody2D rb = gameObject.transform.GetComponent<Rigidbody2D>();
        while (rb.velocity.magnitude > 1f) {
            gameObject.layer = 6;
            yield return null;
        }
        gameObject.layer = 7;
    }

    public static IEnumerator LerpObject(GameObject gameObject, Vector2 targetPosition, float duration) {
        EntityManager em = gameObject.GetComponent<EntityManager>();
        Rigidbody2D rb = gameObject.transform.GetComponent<Rigidbody2D>();

        float time = 0;
        Vector2 startPosition = gameObject.transform.position;
        targetPosition -= 0.6f * gameObject.GetComponent<SpriteRenderer>().bounds.size * em.GetEntityDirection();
        while (time < duration) {
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            time += Time.deltaTime;
            gameObject.layer = 6;
            yield return null;
        }
        rb.MovePosition(targetPosition);
        gameObject.layer = 7;
    }

    public static IEnumerator WaitForArcHit(float duration) {
        float time = 0;
        while (time > duration) {

            time += Time.deltaTime;
            yield return null;
        }
    }
}
