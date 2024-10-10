using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviour
{
    public Button startGameBtn;
    public Button gameDescriptionBtn;
    public Button gameEndBtn;

    public GameObject descriptionPanel;
    public Button exitBtn;

    private void Start()
    {
        if (startGameBtn != null)
        {
            startGameBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Level1");
                SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
            });
        }

        if (gameDescriptionBtn != null)
        {
            gameDescriptionBtn.onClick.AddListener(() =>
            {
                if (descriptionPanel != null)
                    descriptionPanel.SetActive(true);

                if (exitBtn != null)
                    exitBtn.onClick.AddListener(() =>
                    {
                        descriptionPanel.SetActive(false);
                    });
            });
        }

        if (gameEndBtn != null)
        {
            gameEndBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
        SoundManager.Instance.PlayBGM("under the rainbow", 1.0f);
    }
}
