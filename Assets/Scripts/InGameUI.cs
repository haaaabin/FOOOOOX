using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("---- Hp -----")]
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject bossHpBar;
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
        }
    }

    void Start()
    {
        InitUI();

        if (gameExitBtn != null)
        {
            gameExitBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });
        }

        if (replayBtn != null)
        {
            replayBtn.onClick.AddListener(() =>
            {
                GameManager.Instance().ReplayGame();
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
    public void InitUI()
    {
        settingPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        topBarPanel.SetActive(true);
        controlPanel.SetActive(true);
        bossHpBar.SetActive(false);

        if (scoreText != null)
            scoreText.text = PlayerCtrl.Instance.score.ToString();

        UpdateHeart();
        InitSoundUI();
    }

    public void UpdateHeart()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < PlayerCtrl.Instance.currentHp)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
        }
    }

    public void InitSoundUI()
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
        if (damageCanvas == null)
        {
            damageCanvas = GameObject.Find("Damage_Canvas").transform;
        }

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

    public void GameEndPanelUI()
    {
        gameOverPanel.SetActive(true);
        topBarPanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    private IEnumerator FadeScreen(float targetAlpha)
    {
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