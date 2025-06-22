using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
    public GameObject skin1; 
    public GameObject skin2;
    public Image Kot;
    public Image Tuchka;

    private void Start()
    {
        if (SkinManager.Instance != null)
        {
            SkinManager.Instance.selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
        }

        int selected = SkinManager.Instance != null ? SkinManager.Instance.selectedSkin : 0;

        if (skin1 != null) skin1.SetActive(selected == 0);
        if (skin2 != null) skin2.SetActive(selected == 1);

        if (Kot != null) Kot.gameObject.SetActive(selected == 0);
        if (Tuchka != null) Tuchka.gameObject.SetActive(selected == 1);
    }

    public void SelectSkin(int index)
    {
        SkinManager.Instance.SetSkin(index);
    }
}
