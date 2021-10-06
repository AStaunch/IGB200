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
    public void DrawRaySprite(RayData Ray_, Color[] colors) {
        Transform origin = Ray_.CasterObject.transform; 
        RaycastHit2D other = Ray_.Data;
        GameObject start = null;
        GameObject middle = null;
        GameObject end = null;
        spellMaster = new GameObject("Ray Master");
        spellMaster.transform.position = origin.position;

        Directions Direction = origin.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
        Vector2 DirectionVect = VectorDict[Direction];
        float rotationAmount = RotationDict[Direction];
        GenerateOffset(origin, DirectionVect);

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
    public Sprite ArcSprite;
    #region Arc Drawer
    public GameObject CreateArcProjectile(Transform origin, Color[] colors) {
        spellMaster = new GameObject("Arc Master");
        spellMaster.transform.position = origin.position;
        GameObject arcBall = CreateObject(ArcSprite, CreateMaterial(colors));
        arcBall.transform.position = origin.position;
        TrailRenderer tr = arcBall.AddComponent<TrailRenderer>();
        tr.startColor = colors[0];
        tr.endColor = colors[2];
        tr.sortingLayerName = "Objects";
        tr.time = 0.8f;
        tr.startWidth = 0.4f;
        tr.endWidth = 0.2f;
        tr.material = origin.GetComponent<Renderer>().material;
        return arcBall;
    }
    #endregion

    #region Orb Drawer
    public Sprite OrbSprite;
    #endregion
    public Sprite ConeSprite;
    #region Cone Drawer
    public void DrawConeSprite(iEffectorData Data, Color[] colors) {
        ConeData coneData = (ConeData)Data;
        spellMaster = new GameObject("Arc Master");
        spellMaster.transform.position = coneData.CasterObject.transform.position;
        GameObject coneObject = CreateObject(ArcSprite, CreateMaterial(colors));
        coneObject.transform.Rotate(Vector3.forward * RotationDict[Data.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum()]);
    }

    #endregion
    public Sprite ShieldSprite;
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

    private void GenerateOffset(Transform Origin, Vector3 Direction) {
        offset = Vector3.Scale(Origin.GetComponent<SpriteRenderer>().bounds.size, Direction) * 0.5f;
    }
    #endregion
}