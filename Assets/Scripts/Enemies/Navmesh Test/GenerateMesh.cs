using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GenerateMesh : MonoBehaviour
{
    public bool Generate_Points;
    public Vector2[] Bounds = new Vector2[2];
    public bool DrawMode = false;
    public float Displacement;

    private float Width = 0;
    private float Height = 0;

    private List<NavPoint> NavPoints = new List<NavPoint>();

    void Start()
    {
        Width = Bounds[1].x - Bounds[0].x;
        Height = Bounds[1].y - Bounds[0].y;
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Displacement = Displacement > 0.05f ? Displacement : 0.5f;
        if (Generate_Points)
        {
            Width = Bounds[1].x - Bounds[0].x;
            Height = Bounds[1].y - Bounds[0].y >= 0 ? Bounds[1].y - Bounds[0].y : (Bounds[1].y - Bounds[0].y) * -1;
            for (int Row = 0; Row < Mathf.Floor(Height / Displacement); Row++)
            {
                for (int Col = 0; Col < Mathf.Floor(Width / Displacement); Col++)
                {
                    Vector2 pos = new Vector2(Bounds[0].x + (Displacement * Col), Bounds[0].y - (Displacement * Row));

                    if (Physics2D.OverlapCircle(pos, 0.3f) != null)
                        //NavPoints.Add(new NavPoint() { Down_Validity = false, Left_Validity = false, Right_Validity = false, Up_Validity = false, position = pos });
                        continue;

                    RaycastHit2D up = Physics2D.Raycast(pos, new Vector2(0, 1));
                    bool Uphit = up.distance >= Displacement || up.collider == null;
                    RaycastHit2D down = Physics2D.Raycast(pos, new Vector2(0, -1));
                    bool Downhit = down.distance >= Displacement || down.collider == null;
                    RaycastHit2D left = Physics2D.Raycast(pos, new Vector2(-1, 0));
                    bool Lefthit = left.distance >= Displacement || left.collider == null;
                    RaycastHit2D right = Physics2D.Raycast(pos, new Vector2(1, 0));
                    bool Righthit = right.distance >= Displacement || right.collider == null;

                    NavPoints.Add(new NavPoint() { Down_Validity = Downhit, Left_Validity = Lefthit, Right_Validity = Righthit, Up_Validity = Uphit, position = pos });
                    Gizmos.DrawSphere(pos, 0.05f);

                    #region  Visualize Check line
                    Gizmos.color = Uphit? Color.green : Color.red;
                    Gizmos.DrawLine(pos, pos + new Vector2(0, Displacement));
                    Gizmos.color = Downhit ? Color.green : Color.red;
                    Gizmos.DrawLine(pos, pos + new Vector2(0, -Displacement));
                    Gizmos.color = Lefthit ? Color.green : Color.red;
                    Gizmos.DrawLine(pos, pos + new Vector2(-Displacement, 0));
                    Gizmos.color = Righthit ? Color.green : Color.red;
                    Gizmos.DrawLine(pos, pos + new Vector2(Displacement, 0));
                    Gizmos.color = Color.white;
                    #endregion
                }
            }
        }

        Gizmos.DrawLine(Bounds[0], new Vector2(Bounds[1].x, Bounds[0].y));
        Gizmos.DrawLine(new Vector2(Bounds[1].x, Bounds[0].y), Bounds[1]);
        Gizmos.DrawLine(Bounds[1], new Vector2(Bounds[0].x, Bounds[1].y));
        Gizmos.DrawLine(new Vector2(Bounds[0].x, Bounds[1].y), Bounds[0]);
    }


    public static NavPoint[] NavigatePath(GameObject gmeobj)
    {

        #region Return path of points

        List<NavPoint> result = new List<NavPoint>();
        float pos_x = Mathf.Round(gmeobj.transform.position.x * 4f)/4;
        float pos_y = Mathf.Round(gmeobj.transform.position.y * 4f)/4;



        #endregion


        return new NavPoint[0];
    }


    public class NavPoint
    {
        public Vector2 position { get;set; }
        public bool    Up_Validity;
        public bool  Down_Validity;
        public bool  Left_Validity;
        public bool Right_Validity;

        public NavPoint    Up_nav;
        public NavPoint  Down_nav;
        public NavPoint  Left_nav;
        public NavPoint Right_nav;
    }
}
