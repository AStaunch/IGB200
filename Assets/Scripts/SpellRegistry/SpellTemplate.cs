using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    public class SpellEffector
    {
        public string Name { get; set; }
        public long DesiredId { get; set; }
        public Action<GameObject> FireEffect;
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
        private Dictionary<string, SpellTemplate> S_Registry = new Dictionary<string, SpellTemplate>();

        private ObjectIDGenerator iDGenerator = new ObjectIDGenerator();

        public void AddItemToregistry(SpellTemplate template)
        {
            template.CastingId = iDGenerator.GetId(template, out _ );
            S_Registry.Add(template.Name, template);
        }

        public void RemoveItemToregistry(SpellTemplate template)
        {
            S_Registry.Remove(template.Name);
        }

        public void RemoveItemToregistry(string Name)
        {
            S_Registry.Remove(Name);
        }

        public void RemoveItemToregistry(int index)
        {
            S_Registry.Remove(S_Registry.Keys.ElementAt(index));
        }

        public SpellTemplate QueryRegistry(string Name)
        {
            return S_Registry.ContainsKey(Name) ? S_Registry[Name] : null;
        }

        public SpellTemplate QueryRegistry(int Index)
        {
            return S_Registry.Count < Index && Index >=0 ? S_Registry.Values.ToList()[Index] : null;
        }

        public long QueryForSid(string Name)
        {
            return S_Registry.ContainsKey(Name) ? S_Registry[Name].CastingId : -1;
        }

        public SpellTemplate QueryWithSid(long Sid)
        {
            return S_Registry.Values.First((val) => { return val.CastingId == Sid; });
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
