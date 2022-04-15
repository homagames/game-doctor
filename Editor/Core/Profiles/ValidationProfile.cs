using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    public abstract class ValidationProfile
    {
        public abstract List<Check> Checks { get; }
    }
}
