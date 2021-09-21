using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Navmesh_instantiate : MonoBehaviour
{
    public bool Debug_visuals;

    [Header("Required variables")]
    public Tilemap Tile_map;
    public int PerUnitFreq = 2;
    public float ObstacleOffset;
    public List<AStar.Tiles> Tile_info;


    private void Start()
    {
        //DontDestroyOnLoad(this);
        AStar.PerUnitFreq = PerUnitFreq;
        AStar.ObstacleOffset = ObstacleOffset;
        AStar.TileInfo = Tile_info;
        AStar.Ground = Tile_map;
        AStar.Setup();
        if (!Debug_visuals)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (Debug_visuals)
        {
            for(int i = 0; i < AStar.NodeMap.Count;i++)
            {
                AStar.Node n = AStar.NodeMap[i];
                //Gizmos.DrawCube(n.Position, new Vector3(AStar.DisplayDist, AStar.DisplayDist, 0.0001f));
                Gizmos.DrawSphere(n.Position, 0.025f);
            }
        }
    }
}
