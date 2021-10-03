using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Navmesh_instantiate_experimental : MonoBehaviour
{
    public bool Debug_visuals;

    [Header("Required variables")]
    public Tilemap Tile_map;
    public int PerUnitFreq = 2;
    public float ObstacleOffset;
    public List<AStar_experimental.Tiles> Tile_info;


    private void Start()
    {
        //DontDestroyOnLoad(this);
        AStar_experimental.PerUnitFreq = PerUnitFreq;
        AStar_experimental.ObstacleOffset = ObstacleOffset;
        AStar_experimental.TileInfo = Tile_info;
        AStar_experimental.Ground = Tile_map;
        AStar_experimental.Setup();
        if (!Debug_visuals)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (Debug_visuals)
        {
            for(int i = 0; i < AStar_experimental.NodeMap.Count;i++)
            {
                AStar_experimental.Node n = AStar_experimental.NodeMap[i];
                //Gizmos.DrawCube(n.Position, new Vector3(AStar.DisplayDist, AStar.DisplayDist, 0.0001f));
                Gizmos.DrawSphere(n.Position, 0.025f);
            }
        }
    }
}
