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

    public void DrawRaySprite(Transform origin, RaycastHit2D other, Color[] colors) {
        GameObject start = null;
        GameObject middle = null;
        GameObject end = null;
        spellMaster = new GameObject("Ray Master");
        spellMaster.transform.position = origin.position;

        Directions Direction = origin.GetComponent<iCreatureInterface>().GetEntityDirectionEnum();
        Vector2 DirectionVect = VectorDict[Direction];
        float rotationAmount = RotationDict[Direction];
        offset = Vector3.Scale(origin.GetComponent<SpriteRenderer>().bounds.size, DirectionVect) * 0.5f;

        //Set Sprite Colours
        Material material = CreateMaterial(colors);

        // Create the laser start from the prefab
        Vector3 startOffset = 0.5f * origin.GetComponent<SpriteRenderer>().bounds.size * DirectionVect;
        if (start == null) {
            start = CreateObject(rayPieces[0], material);  
            start.transform.position += startOffset;
            start.transform.Rotate(Vector3.forward * rotationAmount);
            start.GetComponent<SpriteRenderer>().sortingOrder = origin.GetComponent<SpriteRenderer>().sortingOrder;
        }

        // Laser middle
        if (middle == null) {
            middle = CreateObject(rayPieces[1], material);
            middle.transform.Rotate(Vector3.forward * rotationAmount);
            middle.GetComponent<SpriteRenderer>().sortingOrder = start.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }

        // Define an "infinite" size, not too big but enough to go off screen
        float maxLaserSize = Vector2.Distance(offset, other.transform.position);
        float currentLaserSize = maxLaserSize;

        // Raycast at the right as our sprite has been design for that
        if (other.collider != null) {
            // We touched something!

            // -- Get the laser length
            currentLaserSize = Vector2.Distance(other.point, origin.position + offset);

            // -- Create the end sprite
            if (end == null) {
                end = CreateObject(rayPieces[2], material);
                end.transform.Rotate(Vector3.forward * rotationAmount);
                end.GetComponent<SpriteRenderer>().sortingOrder = start.GetComponent<SpriteRenderer>().sortingOrder - 2;
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
        float endSpriteWidth = 0;
        if (end != null) {
            endSpriteWidth = end.GetComponent<Renderer>().bounds.size.x; 
        }

        // -- the middle is after start and, as it has a center pivot, have a size of half the laser (minus start and end)
        middle.transform.localScale = new Vector3(middle.transform.localScale.x, (currentLaserSize - (endSpriteWidth + startSpriteWidth)), middle.transform.localScale.z);
        middle.transform.localPosition = DirectionVect * (currentLaserSize / 2f);
        middle.transform.localPosition += 0.5f * startOffset;

        // End?
        if (end != null) {
            end.transform.localPosition = DirectionVect * currentLaserSize;

            end.AddComponent<DestroyThis>();
        }
        spellMaster.AddComponent<DestroyThis>();
        start.AddComponent<DestroyThis>();
        middle.AddComponent<DestroyThis>();
    }
    #endregion
    public AnimationCurve arcCurve;
    #region Arc Drawer
    public GameObject CreateArcBall(Transform origin, Color[] colors) {
        Material material = CreateMaterial(colors);
        GameObject arcBall = CreateObject(rayPieces[0], material);
        Directions direction = origin.GetComponent<iCreatureInterface>().GetEntityDirectionEnum();
        
        arcBall.AddComponent<ArcBehaviour>().StartArc(direction);
        arcBall.transform.position = origin.position;
        return arcBall;
    }
    #endregion

    #region Orb Drawer

    #endregion

    #region Cone Drawer
    public void DrawConeSprite(Transform origin, RaycastHit2D[] points, Color[] colors) {
        throw new NotImplementedException();
    }

    #endregion

    #region Shield Drawer

    #endregion

    #region Runner Drawer

    #endregion

    #region Recycled Code
    private GameObject CreateObject(Sprite sprite, Material material) {
        GameObject obj = new GameObject();
        obj.AddComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "VFX";
        obj.GetComponent<Renderer>().material = material;
        obj.transform.parent = spellMaster.transform;
        obj.transform.localPosition = Vector2.zero + new Vector2(offset.x, offset.y);
        return obj;
    }

    private Material CreateMaterial(Color[] colors) {
        return CreateMaterial(colors, shader);
    }

    public static Material CreateMaterial(Color[] colors, Shader shader) {
        Material material = new Material(shader);
        material.SetColor("_PrimaryColour", colors[0]);
        material.SetColor("_SecondaryColour", colors[1]);
        material.SetColor("_AccentColour1", colors[2]);
        material.SetColor("_AccentColour2", colors[3]);
        return material;
    }
    #endregion
}