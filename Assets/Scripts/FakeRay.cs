using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityManager;

public class FakeRay : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    private Color[] paletteColors;
    private Vector2[] facings;
    private float baseDmg = 10f;

    public LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        facings = new Vector2[4];
        facings[0] = Vector2.up;
        facings[1] = Vector2.left;
        facings[2] = Vector2.down;
        facings[3] = Vector2.right;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facings[directionIndex]);
        if (hit.collider != null)
        {
            DrawRay(transform.position,hit.point);
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

    void Scraps()
    {
        int directionIndex = GetComponent<EntityManager>().GetEntityFacing();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facings[directionIndex]);
        if (hit.collider != null)
        {
            GameObject other = hit.transform.gameObject;
            /////   Create the Beam
            //Get Distance (d)
            //Length of Sections (B, M, E)
            //Get No of Tiles in Distance
            /* Palette Swap Colours of Ray.
             * Get First Tile; set to start
             * Get Last Tile; set to end
             * For everyother tile; set to middle
             */

            float distance = Vector2.Distance(hit.point, new Vector2(transform.position.x, transform.position.y));


            Vector2 direction = hit.point - new Vector2(transform.position.x, transform.position.y);
            float angle = Vector2.Angle(new Vector2(transform.position.x, transform.position.y), hit.point);
            for (float i = 0; i < distance; i += 1)
            {
                if (i == 0)
                {

                }
                else if (i == distance - 1)
                {

                }
            }





            /////   Effect What It Hits
            //Get The Game object it hits
            //Check EntityType Enum
            //Check EntityProp Enum for Flamable/Fireproof



            //Property.Behave(GameObject other, Float baseDmg)

            if (other.TryGetComponent<EntityManager>(out EntityManager otherEntity))
            {
                if (otherEntity.entityProperties.Contains(Properties.Flamable))
                {
                    otherEntity.TakeDamage(baseDmg * 2f);
                }
                else if (otherEntity.entityProperties.Contains(Properties.Fireproof))
                {
                    otherEntity.TakeDamage(0f);
                }
                else
                {
                    otherEntity.TakeDamage(baseDmg);
                }
            }
        }
    }
}
