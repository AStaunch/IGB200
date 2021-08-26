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
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        #region Register game spells and effect first
        //fuck valve - Agreed
        //Ray Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Ray", null, new Action<SpellEffector>((effector) =>
        {
            Debug.Log("This would be a Raycast");

            RaycastHit2D hit = Physics2D.Raycast(GameObject.FindGameObjectWithTag("Player").transform.position, GameObject.FindGameObjectWithTag("Player").transform.right);
            Debug.DrawRay(GameObject.FindGameObjectWithTag("Player").transform.position, GameObject.FindGameObjectWithTag("Player").transform.right);
            if (hit.collider != null)
            {
                GameObject gmeobj = new GameObject();
                gmeobj.transform.position = hit.point;

                effector.FireEffect.Invoke(gmeobj);//This gmeobj is destroyed by the effector 
            }
        })));

        //Orb Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Orb", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Orb");
            effector.FireEffect.Invoke(null);//The null in this function would be the game object required
            })));

        //Arc Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Arc", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Arc");
            effector.FireEffect.Invoke(null);//The null in this function would be the game object required
            })));

        //Cone Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Cone", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Cone");
            effector.FireEffect.Invoke(null);//The null in this function would be the game object required
            })));

        //Shield Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Shield", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Shield");
            effector.FireEffect.Invoke(null);//The null in this function would be the game object required
            })));

        //Runner Template
        SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Runner", null, new Action<SpellEffector>((effector) =>
        {
            Console.WriteLine("This would be a Runner");
            effector.FireEffect.Invoke(null);//The null in this function would be the game object required
            })));




        //This should become its own singleton for global access, or be wrapped in a static class
        List<SpellEffector> Effectors = new List<SpellEffector>
        {
            #region Fire
            new SpellEffector()
            {
                Name = "Fire",
                DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Ray"),
                FireEffect = new Action<GameObject>((gmeobj) =>
                {
                        
                })
            },
            new SpellEffector()
            {
                Name = "Fire",
                DesiredId = SpellRegistrySing.Instance.Registry.QueryForSid("Orb"),
                FireEffect = new Action<GameObject>((gmeobj) =>
                {
                    Console.WriteLine("This is the effector + Orb spell");
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

                //type of interface
                //select all of the types of the loaded assembly
                //where the type = the interface we want
            }
        }

        #endregion
    }
}