using UnityEngine.SceneManagement;

namespace BattleCity.Assistance.Static
{
    public enum SelectedGameMode : byte
    {
        None = 0,
        Mode_1Player = 1,
        Mode_2Player = 2,
    }

    public static class GameStaticVariables
    {
        public static SelectedGameMode GameMode { get; set; }

        public static int Player1Score { get; set; }
        public static int Player2Score { get; set; }
    }

    public class SceneHelper
    {
        public static void SwitchScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
