using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    [CreateAssetMenu(fileName = nameof(ValidationProfileConfiguration),
        menuName = "Homa Games/Game Doctor/" + nameof(ValidationProfileConfiguration))]
    public class ValidationProfileConfiguration : ScriptableObject
    {
        [SerializeReference]
        public List<Check> Checks = new List<Check>();
    }
}