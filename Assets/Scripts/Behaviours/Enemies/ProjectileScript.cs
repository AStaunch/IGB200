using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float Velocity;
    public Vector3 Direction;
    public float Damage;
    public EnumsAndDictionaries.Elements element;
    public GameObject Shooter;

    // Update is called once per frame
    void Start() {
        gameObject.layer = 6;
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f * (GetComponent<SpriteRenderer>().bounds.size.x + GetComponent<SpriteRenderer>().bounds.size.y) / 2f;
        circleCollider.isTrigger = true;
    }
    void Update()
    {
        transform.position += Velocity * Time.deltaTime * Direction.normalized;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.isTrigger || collision.gameObject != Shooter) {
            if (collision.transform.TryGetComponent(out iHealthInterface iHealth)) {
                iHealth.TakeDamage(Damage, element);
                Debug.Log("Projectile Hit " + collision.transform.name);
            }
            Destroy(this.gameObject);
            Debug.Log("Trigger");
        }
        //if (collision.gameObject.layer == 8) {
        //    Destroy(this.gameObject);
        //    Debug.Log("TriggerWall");
        //}
    }
}
