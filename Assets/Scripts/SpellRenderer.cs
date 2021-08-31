using UnityEngine;
using System.Collections;
using static EntityManager;
using System;

public class SpellRenderer : MonoBehaviour
{
    private static SpellRenderer _instance;

    public static SpellRenderer Instance { get { return _instance; } }


    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public Shader shader;
    #region Ray Drawer

    public Sprite[] rayPieces;


    GameObject spellMaster;
    public void CreateRaySprites(Transform origin, RaycastHit2D other, Color[] colors) {
        GameObject start = null;
        GameObject middle = null;
        GameObject end = null;


        spellMaster = new GameObject("Ray Master");
        spellMaster.transform.position = origin.position;
        spellMaster.transform.parent = origin;

        Vector3 Direction = origin.GetComponent<EntityManager>().GetEntityDirection();
        int rotationIndex = origin.GetComponent<EntityManager>().GetEntityFacing();
        float rotationAmount = 90 * rotationIndex * -1^(rotationIndex);

        //Set Sprite Colours
        Material material = createMaterial(colors);

        // Create the laser start from the prefab
        if (start == null) {
            start = createObject(rayPieces[0],material);
            
            start.transform.Rotate(Vector3.forward * rotationAmount);            
        }

        // Laser middle
        if (middle == null) {
            middle = createObject(rayPieces[1], material);
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
                end = createObject(rayPieces[2], material);
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


        spellMaster.AddComponent<DestroyThis>();
        start.AddComponent<DestroyThis>();
        middle.AddComponent<DestroyThis>();
        end.AddComponent<DestroyThis>();
    }

   
    #endregion

    #region Arc Drawer

    #endregion

    #region Orb Drawer

    #endregion

    #region Cone Drawer

    #endregion

    #region Shield Drawer

    #endregion

    #region Runner Drawer

    #endregion

    #region Recycled Code
    private GameObject createObject(Sprite sprite, Material material) {
        GameObject obj = new GameObject();
        obj.AddComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
        obj.GetComponent<Renderer>().material = material;
        obj.transform.parent = spellMaster.transform;
        obj.transform.localPosition = Vector2.zero;
        return obj;
    }

    private Material createMaterial(Color[] colors) {
        Material material = new Material(shader);
        material.SetColor("_PrimaryColour", colors[0]);
        material.SetColor("_SecondaryColour", colors[1]);
        material.SetColor("_AccentColour1", colors[2]);
        material.SetColor("_AccentColour2", colors[3]);
        return material;
    }
    #endregion
}