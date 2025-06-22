using UnityEngine;
using UnityEngine.SceneManagement;

public class EndlessGameManager : GameManager
{
    public override void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);

        if (audioSource != null && pelletEatClip != null)
        {
            audioSource.PlayOneShot(pelletEatClip);
        }

        if (!HasRemainingPellets())
        {
            if (cat != null)
            {
                cat.WinSequence();

                if (audioSource != null && catWinClip != null)
                    audioSource.PlayOneShot(catWinClip);
            }

            Invoke(nameof(NewRound), 3f);
        }
    }

    private void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    protected override void LoadNextLevel()
    {
        ReloadCurrentLevel();
    }
}
