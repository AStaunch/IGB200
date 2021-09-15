using UnityEngine;
using System.Collections;
using static EnumsAndDictionaries;
using System;
using System.Collections.Generic;

public class SpellRenderer : MonoBehaviour
{
    #region Singleton Things

    private static SpellRenderer _instance;

    public static SpellRenderer Instance { get { return _instance; } }
    


    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion
    public Shader shader;
    private Vector3 offset;
    #region Ray Drawer
    public Sprite[] rayPieces;
    GameObject spellMaster;

    public void drawRaySprite(Transform origin, RaycastHit2D other, Color[] colors) {
        GameObject start = null;
        GameObject middle = null;
        GameObject end = null;
        spellMaster = new GameObject("Ray Master");
        spellMaster.transform.position = origin.position;
        spellMaster.transform.parent = origin;

        Directions Direction = origin.GetComponent<EntityManager>().GetEntityDirectionEnum();
        Vector3 DirectionVect = VectorDict[Direction];
        float rotationAmount = RotationDict[Direction];
        offset = Vector3.Scale(origin.GetComponent<SpriteRenderer>().bounds.size, DirectionVect) * 0.5f;

        //Set Sprite Colours
        Material material = createMaterial(colors);

        // Create the laser start from the prefab
        if (start == null) {
            start = createObject(rayPieces[0], material);
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
        middle.transform.localPosition = DirectionVect * (currentLaserSize / 2f);

        // End?
        if (end != null) {
            end.transform.localPosition = DirectionVect * currentLaserSize;
        }
        spellMaster.AddComponent<DestroyThis>();
        start.AddComponent<DestroyThis>();
        middle.AddComponent<DestroyThis>();
        end.AddComponent<DestroyThis>();
    }




    #endregion
    public AnimationCurve arcCurve;
    #region Arc Drawer
    public GameObject CreateArcBall(Transform origin, Color[] colors) {
        spellMaster = new GameObject("Arc Master");
        spellMaster.transform.position = origin.position;
        spellMaster.transform.parent = origin;

        Material material = createMaterial(colors);
        GameObject arcBall = createObject(rayPieces[0], material);
        Directions direction = origin.GetComponent<EntityManager>().GetEntityDirectionEnum();
        
        arcBall.AddComponent<ArcBehaviour>().StartArc(direction, spellMaster);
        arcBall.transform.position = origin.position;
        return arcBall;
    }
    #endregion

    #region Orb Drawer

    #endregion

    #region Cone Drawer
    public void drawConeSprite(Transform origin, RaycastHit2D[] points, Color[] colors) {
        int noRays = 10;
        float coneAngle = 60f;
        float maxDistance = 5f;
        Vector2 direction = maxDistance * origin.GetComponent<EntityManager>().GetEntityDirection();

        RaycastHit2D[] rays = new RaycastHit2D[noRays];
        Vector3[] positions = new Vector3[noRays + 1];
        
        for (int i = 0; i < rays.Length; i++) {
            float angle = coneAngle * (i/noRays);
            Vector2 direction2 = maxDistance * new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
            rays[i] = Physics2D.Raycast(origin.position, direction, maxDistance);
            positions[i] = rays[i].point;
        }
    }
    #endregion

    #region Shield Drawer

    #endregion

    #region Runner Drawer

    #endregion

    #region Recycled Code
    private GameObject createObject(Sprite sprite, Material material) {
        GameObject obj = new GameObject();
        obj.AddComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "VFX";
        obj.GetComponent<Renderer>().material = material;
        obj.transform.parent = spellMaster.transform;
        obj.transform.localPosition = Vector2.zero + new Vector2(offset.x, offset.y);
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