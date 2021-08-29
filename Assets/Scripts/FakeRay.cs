using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;

public class FakeRay : MonoBehaviour
{
    [SerializeField]
    private Color[] paletteColors;
    private Vector2[] facings;
    private float baseDmg = 10f;

    public LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        facings = new Vector2[4];
        facings[0] = Vector2.up;
        facings[1] = Vector2.right;
        facings[2] = Vector2.down;
        facings[3] = Vector2.left;
    }

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
        int directionIndex = GetComponent<EntityManager>().GetEntityFacing();
        Transform origin = GameObject.FindGameObjectWithTag("Player").transform;
        RaycastHit2D hit = Physics2D.Raycast(origin.position, facings[directionIndex]);
        if (hit.collider != null)
        {
            DrawRay(transform.position,hit.point);
            Debug.Log("Hit at " + hit.point + "!");
            GetComponent<RayDraw>().CreateRaySprites(origin.transform, hit, directionIndex);

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

    void DrawRay(Vector2 start, Vector2 end)
    {
        float tileWidth = GetComponent<Collider2D>().bounds.size.x;
        float distance = Vector2.Distance(start, end);
        float tileDistance = distance / tileWidth;
        float remainder = (tileDistance % 1);
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        for(float i = 0; i < tileDistance; i += 1) {
            if(i < remainder) {

                i = remainder;
            }
        }
    }
}