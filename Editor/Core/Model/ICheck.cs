using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public interface ICheck
    {
        string Name { get; }
        string Description { get; }
        List<string> Tags { get; }
        ImportanceType Importance { get; }
        Priority Priority { get; }
        Task Execute();
        CheckResult CheckResult { get; }
    }
}