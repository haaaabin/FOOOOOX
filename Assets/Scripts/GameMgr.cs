using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Level1,
    Boss,
    GameOver,
    Ending
}

public class GameMgr : MonoBehaviour
{
    [HideInInspector] public SkillType skillType = SkillType.SkCount;
    public static GameState gameState = GameState.Level1;

    private DmgTextCtrl dmgText;
    private Vector3 stCacPos;
    [Header("--- Damage Text ---")]
    public Transform damageCanvas;
    public GameObject damageRoot;

    public Text goldText;

    [Header(" ---- Coin, Dia ----- ")]
    public GameObject coin;
    public GameObject diamond;
    int curGold = 0;

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
    public Button replayBtn;
    // public Button gameExitBtn;
    public Text titleText;

    [Header("---- Skill Cool Timer -----")]
    public GameObject skCoolPrefab;
    public Transform skCoolRoot;
    public SkInvenNode[] skInvenNode;

    public Image hpBarImg;
    public static GameMgr Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;
    }

    private void Start()
    {
        GlobalValue.LoadGameData();
        Time.timeScale = 1.0f;

        settingPanel.SetActive(false);
        storePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        InitRefreshUI();
        RefreshSkill();
        InitSoundUI();

        if (SceneManager.GetActiveScene().name == "Level1")
        {
            SoundManager.Instance.PlayBGM("hurry_up_and_run", 1.0f);

        }
        else if (SceneManager.GetActiveScene().name == "BossScene")
        {
            SoundManager.Instance.PlayBGM("Maniac", 1.0f);
        }
    }


    private void Update()
    {
        if (!(gameState == GameState.GameOver))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) ||
                Input.GetKeyDown(KeyCode.Keypad1))
            {
                UseSkill_Key(SkillType.Skill_0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) ||
                Input.GetKeyDown(KeyCode.Keypad2))
            {
                UseSkill_Key(SkillType.Skill_1);
            }
        }
    }

    private void InitRefreshUI()
    {
        if (GlobalValue.g_UserGold <= 0)
            GlobalValue.g_UserGold = 0;

        if (goldText != null)
            goldText.text = GlobalValue.g_UserGold.ToString();

        if (hpBarImg != null)
            hpBarImg.fillAmount = PlayerCtrl.hp / PlayerCtrl.initHp;

        for (int i = 0; i < StoreBox.skill.Length; i++)
        {
            if (skInvenNode.Length <= i)
                return;

            skInvenNode[i].skType = (SkillType)i;
            skInvenNode[i].skCountText.text = StoreBox.skill[i].ToString();
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

    public void OnOffStorePanel()
    {
        storePanel.SetActive(!storePanel.activeSelf);
        Time.timeScale = storePanel.activeSelf ? 0.0f : 1.0f;
        if (storePanel.activeSelf)
            storePanel.GetComponent<StoreBox>();
    }

    public void GameExit()
    {
        PlayerPrefs.DeleteAll();
        PlayerCtrl.initHp = 500;
        PlayerCtrl.hp = 500;
        SceneManager.LoadScene("TitleScene");
    }

    public void UseSkill_Key(SkillType skType)
    {
        if (GlobalValue.g_skillCount[(int)skType] <= 0)
            return;

        PlayerCtrl.instance.UseSkill_Item(skType);

        if ((int)skType < skInvenNode.Length)
            skInvenNode[(int)skType].skCountText.text =
                        GlobalValue.g_skillCount[(int)skType].ToString();

    }

    public void SkillTimeMethod(float time, float duration)
    {
        GameObject gameObject = Instantiate(skCoolPrefab);
        gameObject.transform.SetParent(skCoolRoot, false);
        SkillCoolCtrl skNode = gameObject.GetComponent<SkillCoolCtrl>();
        skNode.InitState(time, duration);
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

    public void AddGold(int value = 10)
    {
        if (curGold < 0)
            curGold = 0;

        if (GlobalValue.g_UserGold <= 0)
            GlobalValue.g_UserGold = 0;

        curGold += value;
        GlobalValue.g_UserGold += value;

        goldText.text = GlobalValue.g_UserGold.ToString();
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
    }

    public void RefreshSkill()
    {
        for (int i = 0; i < GlobalValue.g_skillCount.Length; i++)
        {
            if (skInvenNode.Length <= i)
                return;

            skInvenNode[i].skType = (SkillType)i;
            skInvenNode[i].skCountText.text = GlobalValue.g_skillCount[i].ToString();
        }
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        Time.timeScale = 0.0f;

        PlayerPrefs.DeleteAll();
        PlayerCtrl.initHp = 500;
        PlayerCtrl.hp = 500;

        GameEndPanelUI();
    }

    private void GameEndPanelUI()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

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
        gameState = GameState.Ending;
        Time.timeScale = 0.0f;

        PlayerPrefs.DeleteAll();

        titleText.text = "GAME ENDING !";
        GameEndPanelUI();
    }
}

