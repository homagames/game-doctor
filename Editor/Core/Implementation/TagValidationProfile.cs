using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Simple Scriptable Object Validation Profile. Populated with checks from chosen tags.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(TagValidationProfile),
        menuName = GameDoctorConstants.ASSET_MENU_NAME + nameof(TagValidationProfile))]
    public class TagValidationProfile : ScriptableObject, IValidationProfile
    {
        public string Name => name;
        [SerializeField, TextArea] private string description;
        public string Description => description;
        public HashSet<ICheck> CheckList { get; } = new HashSet<ICheck>();
        [SerializeField] private List<string> tags = new List<string>();

        public void AddTag(string tag)
        {
            if(!tags.Contains(tag))
                tags.Add(tag);
            this.PopulateChecks(tag);
        }
        
        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
            this.RemoveChecks(tag);
        }
        
        public HashSet<string> GetAllTags()
        {
            return new HashSet<string>(tags);
        }

        public async Task Check()
        {
            foreach (var check in CheckList)
                await check.Execute();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CheckList.Clear();
            this.PopulateChecks(tags.ToArray());
        }
#endif
    }
}