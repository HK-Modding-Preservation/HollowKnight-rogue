using UnityEngine.EventSystems;

namespace rogue;

internal static class DisplayManager
{
    static GameObject scorePanel = null;
    static TMPro.TextMeshPro scoreText;

    static void InitScorePanel()
    {
        if (scorePanel == null)
        {
            scorePanel = new GameObject("ScorePanel");
            GameObject.DontDestroyOnLoad(scorePanel);
            scorePanel.transform.SetPosition2D(0f, 8f);
            scoreText = scorePanel.AddComponent<TMPro.TextMeshPro>();
            scorePanel.layer = LayerMask.NameToLayer("UI");
            scoreText.fontSize = 12;
            scoreText.color = Color.white;
            scoreText.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }


    internal static void ShowScore(float score)
    {
        InitScorePanel();
        scoreText.text = $"Score: {score}";
    }



}