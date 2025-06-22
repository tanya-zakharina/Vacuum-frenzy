using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    public int selectedSkin = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSkin(int skinIndex)
    {
        selectedSkin = skinIndex;
        PlayerPrefs.SetInt("SelectedSkin", skinIndex);
    }

    public void ResetSkin()
    {
        selectedSkin = -1;
    }
}
