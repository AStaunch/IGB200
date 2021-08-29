using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;

public class FakeRay : MonoBehaviour
{
    [SerializeField]
    private Color[] paletteColors;
    private float baseDmg = 10f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        Vector2 facing = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EntityManager>().GetEntityDirection();
        Transform origin = GameObject.FindGameObjectWithTag("Player").transform;
        RaycastHit2D hit = Physics2D.Raycast(origin.position, facing);
        if (hit.collider != null)
        {
            Debug.Log("Hit at " + hit.point + "!");
            GetComponent<RayDraw>().CreateRaySprites(origin.transform, hit, paletteColors);

            if (hit.transform.gameObject.TryGetComponent<EntityManager>(out EntityManager otherEntity)) {
                if (otherEntity.entityProperties.Contains(Properties.Flamable)) {
                    otherEntity.TakeDamage(baseDmg * 2f);
                } else if (otherEntity.entityProperties.Contains(Properties.Fireproof)) {
                    otherEntity.TakeDamage(0f);
                } else {
                    otherEntity.TakeDamage(baseDmg);
                }
            }
        }
    }
}