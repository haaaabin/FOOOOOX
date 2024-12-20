using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [HideInInspector] public SkillType skillType = SkillType.SkCount;

    public enum GameState { Level1, Boss, GameOver, Ending }
    [HideInInspector] public GameState gameState;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
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

        gameState = GameState.Level1;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            InGameUI.instance.SetUIActive(false);
            PlayerCtrl.Instance.SetPlayerActive(false);
        }
        else
        {
            InGameUI.instance.SetUIActive(true);
            PlayerCtrl.Instance.SetPlayerActive(true);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}