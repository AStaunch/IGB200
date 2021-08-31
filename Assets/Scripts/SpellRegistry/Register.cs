using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using UnityEngine;

class Register : MonoBehaviour
{
    private Dictionary<EntityManager.Directions, Vector2> Converter_ = new Dictionary<EntityManager.Directions, Vector2>() 
    {
        { EntityManager.Directions.Up,      new Vector2(0, 1) },
        { EntityManager.Directions.Down,    new Vector2(0,-1) },
        { EntityManager.Directions.Left,    new Vector2(-1,0) },
        { EntityManager.Directions.Right,   new Vector2(1, 0) },
    };

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        #region Register game spells and effect first
        //fuck valve - Agreed - Doubled
        //Ray Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Ray", null, new Action<SpellEffector>((effector) =>
        {
            Debug.Log("This would be a Raycast");
            RaycastHit2D hit = Physics2D.Raycast(GameObject.FindGameObjectWithTag("Player").transform.position, Converter_[(EntityManager.Directions)GameObject.FindGameObjectWithTag("Player").GetComponent<EntityManager>().GetEntityFacing()]);
            //There is no point using a facing variable, when this debug function will be removed soon
            Debug.DrawRay(GameObject.FindGameObjectWithTag("Player").transform.position, Converter_[(EntityManager.Directions)GameObject.FindGameObjectWithTag("Player").GetComponent<EntityManager>().GetEntityFacing()]);
            
            if (hit.collider != null)
            {
                //Passes Hit data to the Effector
                RayData ryd = new RayData() { Data = hit };
                effector.Effector.Invoke(ryd);

                //Create the Sprites for the Ray Spell 
                SpellRenderer rayDrawer = FindObjectOfType<SpellRenderer>();
                rayDrawer.CreateRaySprites(GameObject.FindGameObjectWithTag("Player").transform, hit, effector.Colors);
            }
        })));

        //Orb Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Orb", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Orb");
            effector.Effector.Invoke(null);//The null in this function would be the game object required
            })));

        //Arc Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Arc", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Arc");
            effector.Effector.Invoke(null);//The null in this function would be the game object required
            })));

        //Cone Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Cone", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Cone");
            effector.Effector.Invoke(null);//The null in this function would be the game object required
            })));

        //Shield Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Shield", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Shield");
            effector.Effector.Invoke(null);//The null in this function would be the game object required
            })));

        //Runner Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Runner", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Runner");
            effector.Effector.Invoke(null);//The null in this function would be the game object required
            })));




        //This should become its own singleton for global access, or be wrapped in a static class
        List<SpellEffector> Effectors = new List<SpellEffector>
        {
            #region Fire
            new SpellEffector()
            {
                Name = "Fire",
                DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
                Effector = new Action<EffectorData>((EffectorData) =>
                {
                    RayData data = (RayData)EffectorData;
                    
                })
            },
            new SpellEffector()
            {
                Name = "Fire",
                DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Orb"),
                Effector = new Action<EffectorData>((EffectorData) =>
                {
                    
                })
            }
            #endregion
        };

        #endregion

        #region Import any addon templates and effects

        string cdir = Environment.CurrentDirectory;
        string adir = Directory.Exists($"{cdir}\\Addon") ? $"{cdir}\\Addon" : Directory.CreateDirectory($"{cdir}\\Addon").FullName;
        Type inter = typeof(ExternalSpell);
        foreach (string file in Directory.GetFiles(adir))
        {
            if (Path.GetExtension(file) == ".dll")
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    foreach (Type atype in assembly.GetTypes().Where((type) => { return inter.IsAssignableFrom(type); }))
                    {
                        MethodInfo info = atype.GetMethod("Initialize");
                        info.Invoke(Activator.CreateInstance(atype), null);
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Source} - {e.Message}");
                }
            }
        }

        #endregion
    }
}