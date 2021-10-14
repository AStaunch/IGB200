using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float Velocity;
    public Vector3 Direction;
    public float Damage;
    public EnumsAndDictionaries.Elements element;

    // Update is called once per frame
    void Start() {
        gameObject.layer = 6;
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f * (GetComponent<SpriteRenderer>().bounds.size.x + GetComponent<SpriteRenderer>().bounds.size.y) / 2f;
        circleCollider.isTrigger = true;
    }
    void Update()
    {
        transform.position += Velocity * Direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.isTrigger) {
            if(collision.transform.TryGetComponent(out iHealthInterface iHealth)) {
                iHealth.TakeDamage(Damage, element);
            }
        }
    }
}
