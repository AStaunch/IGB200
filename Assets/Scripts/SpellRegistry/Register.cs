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
    public Sprite RaySprite;
    private Sprite RaySprite_ { get { return RaySprite != null ? RaySprite : null; } }

    public Sprite ArcSprite;
    private Sprite ArcSprite_ { get { return ArcSprite != null ? ArcSprite : null; } }

    public Sprite OrbSprite;
    private Sprite OrbSprite_ { get { return OrbSprite != null ? OrbSprite : null; } }

    public Sprite ConeSprite;
    private Sprite ConeSprite_ { get { return ConeSprite != null ? ConeSprite : null; } }

    public Sprite ShieldSprite;
    private Sprite ShieldSprite_ { get { return ShieldSprite != null ? ShieldSprite : null; } }

    public Sprite RunnerSprite;
    private Sprite RunnerSprite_ { get { return RunnerSprite != null ? RunnerSprite : null; } }

    //public Sprite RaySprite;
    //private Sprite RaySprite_ { get { return RaySprite != null ? RaySprite : null; } }

    void Start()
    {  
        if(GameObject.FindGameObjectWithTag("SpellReg") == null)
        {
            #region Register game spells and effect first
            //fuck valve - Agreed - Doubled - Thirded - Fourthd - Fifth'd
            //Ray Template
            SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Ray", RaySprite_, new Action<SpellEffector>((effector) =>
            {
                GameObject CasterObject = GameObject.FindGameObjectWithTag("Player");
                Vector2 RayDirection = CasterObject.GetComponent<iFacingInterface>().GetEntityDirection();
                Vector2 RayOrigin = CasterObject.transform.position;
                RayOrigin += 0.5f * CasterObject.GetComponent<SpriteRenderer>().bounds.size * RayDirection;
                float maxDistance = 10f;

                RaycastHit2D hit = Physics2D.Raycast(RayOrigin , RayDirection, maxDistance);
                ////There is no point using a facing variable, when this debug function will be removed soon
                //Debug.DrawRay(GameObject.FindGameObjectWithTag("Player").transform.position, RayDirection);
                RayData ryd = new RayData() { CasterObject = CasterObject, Data = hit, Calling_template = SpellRegistrySing.Instance.Registry.QueryRegistry("Ray") };
                if (hit.collider != null)
                {
                    Debug.Log("Hit: " + hit.transform.name);
                    effector.Effector.Invoke(ryd);

                }
                //Create the Sprites for the Ray Spell 
                SpellRenderer rayDrawer = FindObjectOfType<SpellRenderer>();
                rayDrawer.DrawRaySprite(ryd, effector.Colors);
            }),1));

            //Orb Template
            SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Orb", OrbSprite_, new Action<SpellEffector>((effector) =>
            {
                Console.WriteLine("This would be a Orb");
                effector.Effector.Invoke(null);//The null in this function would be the game object required
            }),1));

            //Arc Template
            SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Arc", ArcSprite_, new Action<SpellEffector>((effector) =>
            {
                Console.WriteLine("This would be a Arc");

                SpellRenderer arcDrawer = FindObjectOfType<SpellRenderer>();
                GameObject CasterObject = GameObject.FindGameObjectWithTag("Player");
                ArcData acd = new ArcData() {
                    CasterObject = CasterObject,
                    ArcDirection = EnumsAndDictionaries.ArcDirections.Left,
                    Data = arcDrawer.CreateArcProjectile(GameObject.FindGameObjectWithTag("Player").transform, effector.Colors, EnumsAndDictionaries.ArcDirections.Left),
                    Calling_template = SpellRegistrySing.Instance.Registry.QueryRegistry("Arc")
                };
                effector.Effector.Invoke(acd);//The null in this function would be the game object required
            }),1));

            //Cone Template
            SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Cone", ConeSprite_, new Action<SpellEffector>((effector) =>
            {


                Console.WriteLine("This would be a Cone");
                effector.Effector.Invoke(null);//The null in this function would be the game object required
            }),1));

            //Shield Template
            SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Shield", ShieldSprite_, new Action<SpellEffector>((effector) =>
            {
                Console.WriteLine("This would be a Shield");
                effector.Effector.Invoke(null);//The null in this function would be the game object required
            }),1));

            //Runner Template
            SpellRegistrySing.Instance.Registry.AddItemToregistry(new SpellTemplate("Runner", RunnerSprite_, new Action<SpellEffector>((effector) =>
            {
                Console.WriteLine("This would be a Runner");
                effector.Effector.Invoke(null);//The null in this function would be the game object required
            }),1));


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
        Destroy(this.transform.gameObject);
    }
}