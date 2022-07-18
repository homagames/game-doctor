using HomaGames.GameDoctor.Core;
using UnityEditor;

namespace HomaGames.GameDoctor.Ui
{
    internal static class MenuItems
    {
        [MenuItem("Window/Homa Games/Game Doctor", false, 0)]
        public static void OpenOpenDefaultProfile()
            => GameDoctorWindow.Open();

    }
}