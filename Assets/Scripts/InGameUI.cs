using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    [Header("--- Damage Text---")]
    private Vector3 stCacPos;
    public Transform damageCanvas;
    public GameObject damageRoot;
    private DamageText dmgText;

    [Header("--- Score, Coin ---")]
    public TextMeshProUGUI scoreText;
    public GameObject coin;
    public GameObject diamond;
    private int curScore = 0;

    [Header("---- Hp -----")]
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject bossHpObj;
    public Image bossHpImg;

    [Header("---- Setting -----")]
    public Button settingBtn;
    public GameObject settingPanel;
    public Button gameExitBtn;
    public Toggle sound_Toggle;
    public Slider sound_Slider;

    [Header("---- GameOver -----")]
    public GameObject gameOverPanel;
    public GameObject topBarPanel;
    public GameObject controlPanel;
    public Button replayBtn;
    public Button goTitleBtn;
    public Text titleText;

    public Image fadePanel;
    private float fadeDuration = 2f;
    void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        settingPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        topBarPanel.SetActive(true);
        controlPanel.SetActive(true);

        InitRefreshUI();
        InitSoundUI();
    }

    private void InitRefreshUI()
    {
        if (GlobalValue.g_UserGold <= 0)
            GlobalValue.g_UserGold = 0;

        if (scoreText != null)
            scoreText.text = GlobalValue.g_UserGold.ToString();

        UpdateHeart();
    }

    public void UpdateHeart()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < PlayerCtrl.instance.currentHp)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
        }
    }

    private void InitSoundUI()
    {
        if (sound_Toggle != null)
        {
            sound_Toggle.onValueChanged.AddListener(SoundToggleOnOff);
            int soundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
            sound_Toggle.isOn = (soundOnOff == 1);
        }
        if (sound_Slider != null)
        {
            sound_Slider.onValueChanged.AddListener(SoundSliderChange);
            sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1);
        }
    }

    private void SoundToggleOnOff(bool value)
    {
        if (value == true)
            PlayerPrefs.SetInt("SoundOnOff", 1);
        else
            PlayerPrefs.SetInt("SoundOnOff", 0);

        SoundManager.Instance.SoundOnOff(value);
    }

    private void SoundSliderChange(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        SoundManager.Instance.SoundVolume(value);
    }

    public void OnOffSettingPanel()
    {
        settingPanel.SetActive(!settingPanel.activeSelf);
        Time.timeScale = settingPanel.activeSelf ? 0.0f : 1.0f;
    }

    public void DamageText(float value, Vector3 position, Color color)
    {
        if (damageCanvas == null || damageRoot == null)
            return;

        GameObject dmgObject = Instantiate(damageRoot);
        dmgObject.transform.SetParent(damageCanvas);
        dmgText = dmgObject.GetComponent<DamageText>();
        if (dmgText != null)
            dmgText.InitDamage(value, color);
        stCacPos = new Vector3(position.x, position.y + 1.4f, 0.0f);
        dmgObject.transform.position = stCacPos;
    }

    public void SpawnCoin(Vector3 position)
    {
        if (coin == null)
            return;

        GameObject coinObject = Instantiate(coin);
        coinObject.transform.position = position;
        Destroy(coinObject, 10);
    }

    public void SpawnDiamond(Vector3 position)
    {
        if (diamond == null)
            return;

        GameObject diamondObject = Instantiate(diamond);
        diamondObject.transform.position = position;
        Destroy(diamondObject, 10);
    }

    public void AddScore(int value = 50)
    {
        if (curScore < 0)
            curScore = 0;

        curScore += value;
        scoreText.text = curScore.ToString();

        GlobalValue.g_UserGold += curScore;
        PlayerPrefs.SetInt("UserScore", GlobalValue.g_UserGold);
    }

    public void GameOver()
    {
        GameManager.Instance().gameState = GameManager.GameState.GameOver;
        Time.timeScale = 0.0f;
        PlayerPrefs.DeleteAll();
        GameEndPanelUI();
    }

    private void GameEndPanelUI()
    {
        gameOverPanel.SetActive(true);
        topBarPanel.SetActive(false);
        controlPanel.SetActive(false);

        if (replayBtn != null)
        {
            replayBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Level1");
                SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
            });
        }

        if (goTitleBtn != null)
        {
            goTitleBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });
        }
    }

    public void GameEnding()
    {
        GameManager.Instance().gameState = GameManager.GameState.Ending;

        Time.timeScale = 0.0f;

        PlayerPrefs.DeleteAll();

        titleText.text = "GAME ENDING !";
        GameEndPanelUI();
    }
    private IEnumerator FadeScreen(float targetAlpha)
    {
        Debug.Log($"FadeScreen started: TargetAlpha = {targetAlpha}");
        float startAlpha = fadePanel.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, targetAlpha);
    }

    public void ChangeScene()
    {
        StartCoroutine(ChangeBossScene());
    }
    public IEnumerator ChangeBossScene()
    {
        yield return FadeScreen(1f);
        AsyncOperation bossSceneLoad = SceneManager.LoadSceneAsync("BossScene");
        bossSceneLoad.allowSceneActivation = false;
        AsyncOperation gameUISceneLoad = SceneManager.LoadSceneAsync("GameUIScene", LoadSceneMode.Additive);
        gameUISceneLoad.allowSceneActivation = false;
        while (!bossSceneLoad.isDone || !gameUISceneLoad.isDone)
        {
            if (bossSceneLoad.progress >= 0.9f || gameUISceneLoad.progress >= 0.9f)
            {
                bossSceneLoad.allowSceneActivation = true;
                gameUISceneLoad.allowSceneActivation = true;
                PlayerCtrl.Instance.transform.position = new Vector2(-3, -3);
                yield return FadeScreen(0f);
            }
            yield return null;
        }
    }
}