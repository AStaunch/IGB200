using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRay : MonoBehaviour
{
    private Vector2[] facings;
    // Start is called before the first frame update
    void Awake()
    {
        facings = new Vector2[4];
        facings[0] = Vector2.up;
        facings[1] = Vector2.left;
        facings[2] = Vector2.down;
        facings[3] = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            int directionIndex = GetComponent<EntityManager>().EntityFacing;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, facings[directionIndex]);

            if(hit.collider != null)
            {
                float distance = Vector2.Distance(hit.point, new Vector2(transform.position.x, transform.position.y));
                int step_amnt = 10;
                for (float i = 0; i < distance; i += step_amnt)
                {
                    // TODO
                }
            }
        }
    }
}
