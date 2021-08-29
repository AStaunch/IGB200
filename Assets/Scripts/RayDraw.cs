using UnityEngine;
using System.Collections;
using static EntityManager;
using System;

public class RayDraw : MonoBehaviour
{
    [Header("Laser pieces")]
    public GameObject[] laserPieces;
    public Material palette;

    //TODO: Make RayDraw Static
    public void CreateRaySprites(Transform origin, RaycastHit2D other, Color[] colors) {
        GameObject start = null;
        GameObject middle = null;
        GameObject end = null;


        GameObject rayMaster = Instantiate(new GameObject("Ray Master"), origin.position, origin.rotation);
        rayMaster.transform.parent = origin;

        Vector3 Direction = origin.transform.GetComponent<EntityManager>().GetEntityDirection();
        int DirectionIndex = origin.transform.GetComponent<EntityManager>().GetEntityFacing();
        float rotationAmount = 90 * DirectionIndex * -1^(DirectionIndex);
        ChangeColours(colors);

        // Create the laser start from the prefab
        if (start == null) {
            start = Instantiate(laserPieces[0]) as GameObject;
            start.transform.parent = rayMaster.transform;
            start.transform.localPosition = Vector2.zero;
            start.transform.Rotate(Vector3.forward * rotationAmount);            
        }

        // Laser middle
        if (middle == null) {
            middle = Instantiate(laserPieces[1]) as GameObject;
            middle.transform.parent = rayMaster.transform;
            middle.transform.localPosition = Vector2.zero;
            middle.transform.Rotate(Vector3.forward * rotationAmount);            
        }

        // Define an "infinite" size, not too big but enough to go off screen
        float maxLaserSize = 20f;
        float currentLaserSize = maxLaserSize;

        // Raycast at the right as our sprite has been design for that
        if (other.collider != null) {
            // We touched something!

            // -- Get the laser length
            currentLaserSize = Vector2.Distance(other.point, origin.position);

            // -- Create the end sprite
            if (end == null) {
                end = Instantiate(laserPieces[2]) as GameObject;
                end.transform.parent = rayMaster.transform;
                end.transform.localPosition = Vector2.zero;
                end.transform.Rotate(Vector3.forward * rotationAmount);
            }
        } else {
            // Nothing hit
            // -- No more end
            if (end != null) Destroy(end);
        }

        // Place things
        // -- Gather some data
        Renderer renderer1 = start.GetComponent<Renderer>();
        float startSpriteWidth = renderer1.bounds.size.x;
        float endSpriteWidth = 0f;
        if (end != null) endSpriteWidth = end.GetComponent<Renderer>().bounds.size.x;

        // -- the middle is after start and, as it has a center pivot, have a size of half the laser (minus start and end)
        middle.transform.localScale = new Vector3(middle.transform.localScale.x, (currentLaserSize - startSpriteWidth), middle.transform.localScale.z);
        middle.transform.localPosition = Direction * (currentLaserSize / 2f);

        // End?
        if (end != null) {
            end.transform.localPosition = Direction * currentLaserSize;
        }


        rayMaster.AddComponent<DestroyThis>();
        start.AddComponent<DestroyThis>();
        middle.AddComponent<DestroyThis>();
        end.AddComponent<DestroyThis>();
    }

    private void ChangeColours(Color[] colors) {
        foreach (GameObject piece in laserPieces) {
            Material material = palette;
            material.SetColor("_PrimaryColour", colors[0]);
            material.SetColor("_SecondaryColour", colors[1]);
            material.SetColor("_AccentColour1", colors[2]);
            material.SetColor("_AccentColour2", colors[3]);
            piece.GetComponent<Renderer>().material = material;
        }
    }
}