using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    [Serializable]
    public abstract class Check
    {
        public virtual string Name => GetType().FullName;
        public abstract string Description { get; }
        public abstract AutomatableType Automatable { get; }
        public abstract List<string> CompatibleProjectTypes { get; }
        public abstract CheckInstance Execute();
    }
    
    public abstract class FixableCheck : Check
    {
        public override AutomatableType Automatable => AutomatableType.Automatic;
        public abstract void Fix(CheckInstance checkInstance);
    }
    
    public abstract class InteractiveCheck : Check
    {
        public override AutomatableType Automatable => AutomatableType.Interactive;
        public abstract void LaunchInteractiveFix();
    }
}
