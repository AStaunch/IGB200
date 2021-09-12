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
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        #region Register game spells and effect first
        //fuck valve - Agreed - Doubled - Thirded
        //Ray Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Ray", null, new Action<SpellEffector>((effector) =>
        {
            Debug.Log("This would be a Raycast");
            RaycastHit2D hit = Physics2D.Raycast(GameObject.FindGameObjectWithTag("Player").transform.position, GameObject.FindGameObjectWithTag("Player").GetComponent<EntityManager>().GetEntityDirection());
            //There is no point using a facing variable, when this debug function will be removed soon
            Debug.DrawRay(GameObject.FindGameObjectWithTag("Player").transform.position, GameObject.FindGameObjectWithTag("Player").GetComponent<EntityManager>().GetEntityDirection());

            if (hit.collider != null)
            {
                //Passes Hit data to the Effector
                RayData ryd = new RayData() { Data = hit, Calling_template = SpellRegistrySing.Instance.Registry.QueryRegistry("Ray") };
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

            SpellRenderer arcDrawer = FindObjectOfType<SpellRenderer>();

            ArcData acd = new ArcData() { Data = arcDrawer.CreateArcBall(GameObject.FindGameObjectWithTag("Player").transform, effector.Colors),
                                            Calling_template = SpellRegistrySing.Instance.Registry.QueryRegistry("Arc") };
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