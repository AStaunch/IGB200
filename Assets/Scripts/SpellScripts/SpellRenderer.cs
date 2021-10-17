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

    #region Ray Drawer
    public Sprite[] rayPieces;
    GameObject spellMaster;
    public void DrawRaySprite(RayData Ray_, Color[] colors) {
        Transform origin = Ray_.CasterObject.transform; 
        RaycastHit2D other = Ray_.Data;
        spellMaster = new GameObject("Ray Master");

        Directions Direction = origin.GetComponent<iFacingInterface>().GetEntityDirectionEnum();
        Vector2 DirectionVect = VectorDict[Direction];
        float rotationAmount = RotationDict[Direction];
        Vector3 offset = GenerateOffset(origin, DirectionVect);
        spellMaster.transform.position = origin.position + offset;

        //Set Sprite Colours
        Material material = CreateMaterial(colors);

        // Create the laser start
        GameObject start = CreateObject(rayPieces[0], material, offset);  
        start.transform.position += offset;
        start.transform.Rotate(Vector3.forward * rotationAmount);
        start.GetComponent<SpriteRenderer>().sortingOrder = origin.GetComponent<SpriteRenderer>().sortingOrder;

        // Laser middle
        GameObject middle = CreateObject(rayPieces[1], material, offset);
        middle.transform.Rotate(Vector3.forward * rotationAmount);
        middle.GetComponent<SpriteRenderer>().sortingOrder = start.GetComponent<SpriteRenderer>().sortingOrder - 1;

        // -- Create the end sprite
        GameObject end = CreateObject(rayPieces[2], material, offset);
        end.transform.Rotate(Vector3.forward * rotationAmount);
        end.GetComponent<SpriteRenderer>().sortingOrder = start.GetComponent<SpriteRenderer>().sortingOrder + 1;

        // Define an the maximum size, not too big but enough to go off screen
        float maxLaserSize = Vector2.Distance(offset, other.point);
        float currentLaserSize = Vector2.Distance(other.point, origin.position + offset);

        // Place things
        // -- Gather some data
        Renderer renderer1 = start.GetComponent<Renderer>();
        float startSpriteWidth = renderer1.bounds.size.x;
        float endSpriteWidth = end.GetComponent<Renderer>().bounds.size.x;

        // -- the middle is after start and, as it has a center pivot, have a size of half the laser (minus start and end)
        middle.transform.localScale = new Vector3(middle.transform.localScale.x, (currentLaserSize - (endSpriteWidth + startSpriteWidth)), middle.transform.localScale.z);
        middle.transform.localPosition = DirectionVect * (currentLaserSize / 2f);
        middle.transform.localPosition += 0.5f * offset;

        end.transform.localPosition = DirectionVect * currentLaserSize;
        
        spellMaster.AddComponent<DestroyThis>();
        start.AddComponent<DestroyThis>();
        middle.AddComponent<DestroyThis>();
        end.AddComponent<DestroyThis>();
    }
    #endregion
    public AnimationCurve arcCurve;
    public GameObject ArcSprite;
    #region Arc Drawer
    public GameObject CreateArcProjectile(Transform origin, Color[] colors) {
        spellMaster = new GameObject("Arc Master");
        spellMaster.transform.position = origin.position;
        Vector3 offset = GenerateOffset(origin, origin.GetComponent<iFacingInterface>().GetEntityDirection());
        GameObject coneObject = Instantiate(ArcSprite);
        coneObject.transform.position = origin.position + offset;
        coneObject.GetComponent<SpriteRenderer>().material = CreateMaterial(colors);
        coneObject.GetComponent<SpriteRenderer>().sortingLayerName = "VFX";
        coneObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
        TrailRenderer tr = coneObject.AddComponent<TrailRenderer>();
        tr.startColor = colors[0];
        tr.endColor = colors[2];
        tr.sortingLayerName = "Objects";
        tr.time = 0.8f;
        tr.startWidth = 0.4f;
        tr.endWidth = 0.2f;
        tr.material = origin.GetComponent<Renderer>().material;
        tr.sortingLayerName = "VFX";
        tr.sortingOrder = 5;
        Destroy(spellMaster, 1f);
        return coneObject;
    }
    #endregion

    #region Orb Drawer
    public Sprite OrbSprite;
    #endregion
    public GameObject ConeSprite;
    #region Cone Drawer
    public void DrawConeSprite(iEffectorData Data, Color[] colors) {
        ConeData coneData = (ConeData)Data;
        spellMaster = new GameObject("Cone Master");
        spellMaster.transform.position = coneData.CasterObject.transform.position;
        GameObject coneObject = Instantiate(ConeSprite);
        coneObject.transform.parent = spellMaster.transform;
        Vector2 offset = GenerateOffset(coneData.CasterObject.transform, coneData.CasterObject.transform.GetComponent<iFacingInterface>().GetEntityDirection());
        coneObject.transform.localPosition = Vector2.zero + offset;
        coneObject.GetComponent<Renderer>().material = CreateMaterial(colors);
        coneObject.transform.Rotate(Vector3.forward * RotationDict[Data.CasterObject.GetComponent<iFacingInterface>().GetEntityDirectionEnum()]);
        Destroy(spellMaster, 1f);
    }

    #endregion
    public Sprite ShieldSprite;
    #region Shield Drawer

    #endregion

    #region Runner Drawer

    #endregion

    #region Effects and Particles
    public GameObject PulseFXObject;
    public void CreateBurstFX(Vector3 position, Color[] colors) {
        Material material = CreateMaterial(colors);
        GameObject PFXO =  Instantiate(PulseFXObject);
        PFXO.GetComponent<Renderer>().material = material;
        PFXO.transform.position = position;
        Destroy(PFXO, 0.5f);
    }
    #endregion
    #region Recycled Code
    private GameObject CreateObject(Sprite sprite, Material material, Vector2 offset) {
        GameObject obj = new GameObject();
        obj.AddComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "VFX";
        obj.GetComponent<Renderer>().material = material;
        obj.transform.parent = spellMaster.transform;
        obj.transform.localPosition = Vector2.zero + offset;
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

    private Vector2 GenerateOffset(Transform Origin, Vector2 Direction) {
        return 0.5f * Origin.GetComponent<SpriteRenderer>().bounds.size * Direction;
    }
    #endregion
}