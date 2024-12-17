using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    private DmgTextCtrl dmgText;
    private Vector3 stCacPos;
    [Header("--- Damage Text ---")]
    public Transform damageCanvas;
    public GameObject damageRoot;

    public TextMeshProUGUI scoreText;

    [Header(" ---- Coin, Dia ----- ")]
    public GameObject coin;
    public GameObject diamond;
    private int curScore = 0;

    [Header("---- Setting -----")]
    public Button settingBtn;
    public GameObject settingPanel;
    public Button gameExitBtn;
    public Toggle sound_Toggle;
    public Slider sound_Slider;

    [Header("---- Store -----")]
    public Button storeBtn;
    public GameObject storePanel;
    public Text coinText;


    [Header("---- GameOver -----")]
    public GameObject gameOverPanel;
    public GameObject tobBarPanel;
    public GameObject controlPanel;
    public Button replayBtn;
    // public Button gameExitBtn;
    public Text titleText;

    [Header("---- Skill Cool Timer -----")]
    public GameObject skCoolPrefab;
    public Transform skCoolRoot;
    public SkInvenNode[] skInvenNode;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        settingPanel.SetActive(false);
        storePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        tobBarPanel.SetActive(true);
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

        // if (hpBarImg != null)
        //     hpBarImg.fillAmount = PlayerCtrl.hp / PlayerCtrl.initHp;

        // for (int i = 0; i < StoreBox.skill.Length; i++)
        // {
        //     if (skInvenNode.Length <= i || skInvenNode[i] == null)
        //         return;

        //     skInvenNode[i].skType = (SkillType)i;
        //     skInvenNode[i].skCountText.text = StoreBox.skill[i].ToString();
        // }
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
        dmgText = dmgObject.GetComponent<DmgTextCtrl>();
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
        tobBarPanel.SetActive(false);
        controlPanel.SetActive(false);

        if (replayBtn != null)
        {
            replayBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Level1");
                SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
            });
        }

        if (gameExitBtn != null)
        {
            gameExitBtn.onClick.AddListener(() =>
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
}