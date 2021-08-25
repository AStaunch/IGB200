using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteProcessing : MonoBehaviour
{
    private Texture SwapColours(Texture baseSprite, Color[] colors) {
        Material material = GetComponent<SpriteRenderer>().material;
        material.mainTexture = baseSprite;
        material.SetColor("_PrimaryColour", colors[0]);
        material.SetColor("_SecondaryColour", colors[1]);
        material.SetColor("_AccentColour1", colors[2]);
        material.SetColor("_AccentColour2", colors[3]);
        return material.mainTexture;
    }

    public Texture[] Colourize(Texture[] baseSprites, Color[] colors) {
       Texture[] returnedSprites = new Texture[baseSprites.Length];
        for(int i = 0; i < baseSprites.Length; i++) {
            returnedSprites[i] = SwapColours(baseSprites[i], colors);
        }
        return returnedSprites;
    }
}
