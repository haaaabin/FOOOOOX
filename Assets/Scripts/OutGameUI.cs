using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutGameUI : MonoBehaviour
{
    public Button gameStartBtn;
    public Button gameQuitBtn;

    private void Start()
    {
        if (gameStartBtn != null)
        {
            gameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Level1");
                SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
            });
        }

        if (gameQuitBtn != null)
        {
            gameQuitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
        SoundManager.Instance.PlayBGM("under the rainbow", 1.0f);
    }
}
