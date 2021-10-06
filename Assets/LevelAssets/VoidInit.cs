using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static SpriteManager;

public class VoidInit : MonoBehaviour
{
    public List<TileAssetArrays> TileAssets;

    public Tilemap Tile_map;
    // Start is called before the first frame update
    void Awake()
    {        
        for (int x = Tile_map.cellBounds.xMin; x < Tile_map.cellBounds.xMax; x++) {
            for (int y = Tile_map.cellBounds.yMin; y < Tile_map.cellBounds.yMax; y++) {
                Vector3Int TilePosRaw = new Vector3Int(x, y, (int)Tile_map.transform.position.z);
                Vector3 TilePos = Tile_map.CellToWorld(TilePosRaw);
                TileAssetArrays tmp = TileAssets.Find((tile) => { return System.Array.Find(tile.Tiles, (spt) => { return spt == ((Tile)Tile_map.GetTile(TilePosRaw)).sprite; }); });

                if (Tile_map.HasTile(TilePosRaw) && tmp != null) {
                    GameObject VoidTile = CreateVoidObject(tmp.voidType);
                    VoidTile.transform.position = TilePos + 0.5f * tmp.Object.GetComponent<Collider2D>().bounds.size;
                    VoidTile.transform.parent = this.transform;
                }
            }
        }
    }

    private GameObject CreateVoidObject(VoidType voidType) {
        GameObject GO = new GameObject();
        GO.AddComponent<EmptySpaceScript>().VoidType_ = voidType;
        GO.GetComponent<EmptySpaceScript>().Filled = SpriteDict["FilledTile"][0];
        return GO;
    }
}

[System.Serializable]
public class TileAssetArrays {
    public Sprite[] Tiles;
    public GameObject Object;
    public VoidType voidType;

}
//[System.Serializable]
//public class TileAssetPair {
//    public Sprite Tiles;
//    public GameObject Object;
//    public VoidType voidType;
//}
[System.Serializable]
public enum VoidType {
    Water, Void
}
