using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance()
    {
        if (instance == null)
        {
            Debug.LogWarning("GameManager 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1.0f;

        if (SceneManager.GetActiveScene().name == "Level1")
        {
            SoundManager.Instance.PlayBGM("hurry_up_and_run", 1.0f);

        }
        else if (SceneManager.GetActiveScene().name == "BossScene")
        {
            SoundManager.Instance.PlayBGM("Maniac", 1.0f);
        }
    }

    public void GameOver()
    {
        if(PlayerCtrl.Instance.currentHp > 0)
        {
            InGameUI.instance.titleText.text = "GAME END !";
        }
        Time.timeScale = 0.0f;
        InGameUI.instance.GameEndPanelUI();
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene("Level1");
        SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
    }

}