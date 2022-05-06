using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public interface IIssue
    {
        ICheck Check { get; }
        AutomatableType AutomatableType { get; }
        Priority Priority { get; }
        Task Fix();
    }
}