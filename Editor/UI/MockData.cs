using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;

public class ValidationProfile : IValidationProfile
{
    public string Name => "Profile";
    public string Description => "Desc";
    public List<ICheck> CheckList { get; set; } = new List<ICheck>();
    public async Task Check()
    {
        foreach (var check in CheckList)
        {
            await check.Execute();
        }
    }
}
