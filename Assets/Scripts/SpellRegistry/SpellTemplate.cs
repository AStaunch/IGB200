using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;

public class SpellEffector
{
    public string Name { get; set; }
    public long DesiredId { get; set; }
    public Color[] Colors { get; set; }
    public Action<GameObject> Effector;
}

public class SpellTemplate
{

    public Action<SpellEffector> RunFunction;
    public Sprite icon;
    public string Name;
    public long CastingId;

    public SpellTemplate(string Name, Sprite sprite, Action<SpellEffector> onRun)
    {
        this.Name = Name;
        RunFunction = onRun;
        icon = sprite;
    }
}

public sealed class SpellRegistrySing
{
    public static SpellRegistrySing Instance { get; private set; }
    private SpellRegistry _registry;
    public SpellRegistry Registry { get { return _registry; } }
    private SpellRegistrySing() { _registry = new SpellRegistry(); }
    static SpellRegistrySing() { Instance = new SpellRegistrySing(); }
}

public class SpellRegistry
{
    public delegate void onAdd_d();
    public event onAdd_d onAdd;
    public delegate void onRemove_d();
    public event onRemove_d onRemove;


    private Dictionary<string, SpellTemplate> S_Registry = new Dictionary<string, SpellTemplate>();

    private ObjectIDGenerator iDGenerator = new ObjectIDGenerator();

    public void AddItemToregistry(SpellTemplate template)
    {
        template.CastingId = iDGenerator.GetId(template, out _);
        S_Registry.Add(template.Name, template);
        if(onAdd != null) 
            onAdd.Invoke();
    }

    public void RemoveItemToregistry(SpellTemplate template)
    {
        if (S_Registry.ContainsKey(template.Name))
        {
            S_Registry.Remove(template.Name);
            onRemove.Invoke();
        }
    }

    public void RemoveItemToregistry(string Name)
    {
        if (S_Registry.ContainsKey(Name))
        {
            S_Registry.Remove(Name);
            onRemove.Invoke();
        }
    }

    public void RemoveItemToregistry(int index)
    {
        if(index >= 0 && index < S_Registry.Count)
        {
            S_Registry.Remove(S_Registry.Keys.ElementAt(index));
            onRemove.Invoke();
        }
    }

    public SpellTemplate QueryRegistry(string Name)
    {
        return S_Registry.ContainsKey(Name) ? S_Registry[Name] : null;
    }

    public SpellTemplate QueryRegistry(int Index)
    {
        return S_Registry.Count < Index && Index >= 0 ? S_Registry.Values.ToList()[Index] : null;
    }

    public long QueryForSid(string Name)
    {
        return S_Registry.ContainsKey(Name) ? S_Registry[Name].CastingId : -1;
    }

    public SpellTemplate QueryWithSid(long Sid)
    {
        return S_Registry.Values.First((val) => { return val.CastingId == Sid; });
    }

    public SpellTemplate[] QueryAll()
    {
        return S_Registry.Values.ToArray();
    }

    #region old
    //public int QuerySidRegistry(string Name)
    //{
    //    return Sid_Registry.ContainsKey(Name) ? Sid_Registry[Name] : -1;
    //}

    //public int QuerySidRegistry(int Index)
    //{
    //    return Sid_Registry.Count < Index && Index >= 0 ? Sid_Registry.Values.ToList()[Index] : -1 ;
    //}

    //public Dictionary<string, int> PresentSidRegistry()
    //{
    //    return Sid_Registry;
    //}
    #endregion
}

public interface ExternalSpell
{
    void Initialize();
}
