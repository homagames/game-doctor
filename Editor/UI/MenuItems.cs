using UnityEditor;

namespace HomaGames.GameDoctor.Ui
{
    internal static class MenuItems
    {
        [MenuItem("Window/Homa Games/Game Doctor/Open Profile...", false, 0)]
        public static void OpenProfileSelector()
            => ProfileSelectorPopup.Open();

        [MenuItem("Window/Homa Games/Game Doctor/Show Opened Profile", true, 1)]
        public static bool IsProfileOpened()
            => GameDoctorWindow.IsProfileOpened;

        [MenuItem("Window/Homa Games/Game Doctor/Show Opened Profile", false, 1)]
        public static void OpenMainWindow() 
            => EditorWindow.GetWindow<GameDoctorWindow>().Show();

    }
}