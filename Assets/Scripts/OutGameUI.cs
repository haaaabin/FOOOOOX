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
                PlayerCtrl player = PlayerCtrl.Instance;
                if (player != null)
                    player.InIt();
                InGameUI inGameUI = InGameUI.instance;
                if (inGameUI != null)
                {
                    inGameUI.InitRefreshUI();
                    inGameUI.InitSoundUI();
                }
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
