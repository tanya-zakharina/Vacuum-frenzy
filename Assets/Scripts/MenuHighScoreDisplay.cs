using UnityEngine;
using TMPro;

public class MenuHighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString().PadLeft(4, '0');
        }
        else
        {
            Debug.LogWarning("HighScoreText не назначен в инспекторе!");
        }
    }

    public void UpdateHighScoreDisplay()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString().PadLeft(4, '0');
        }
    }
}
