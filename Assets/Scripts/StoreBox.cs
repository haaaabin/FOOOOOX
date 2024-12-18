using UnityEngine;
using UnityEngine.UI;

public class StoreBox : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;

    public GameObject storePanel = null;
    public Button closeBtn = null;
    public Text coinText = null;

    public static int[] skill = new int[2];

    public Button lifeBuyBtn = null;
    public Button shieldBuyBtn = null;

    public GameObject messagePanel = null;
    public Text messageText = null;
    public Button messageCloseBtn = null;

    private const int ITEM_COST = 100;
    private const string MESSSAGE = "보유하신 골드가 부족합니다.";

    // Start is called before the first frame update
    private void Start()
    {
        if (lifeBuyBtn != null)
            lifeBuyBtn.onClick.AddListener(() => BuyItem(SkillType.Skill_0));

        if (shieldBuyBtn != null)
            shieldBuyBtn.onClick.AddListener(() => BuyItem(SkillType.Skill_1));

        if (messageCloseBtn != null)
            messageCloseBtn.onClick.AddListener(() =>
            {
                messagePanel.SetActive(false);
            });
    }

    private void Update()
    {
        if (coinText != null)
            coinText.text = GlobalValue.g_UserGold.ToString();
    }

    private void BuyItem(SkillType skillType)
    {
        if (GlobalValue.g_UserGold >= ITEM_COST)
        {
            GlobalValue.g_UserGold -= ITEM_COST;

            m_SkType = skillType;
            int skillIndex = (int)m_SkType;
            GlobalValue.g_skillCount[skillIndex]++;
            skill[skillIndex]++;

            string skillKey = "SKItem_" + skillIndex.ToString();
            PlayerPrefs.SetInt(skillKey, GlobalValue.g_skillCount[skillIndex]);
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        }
        else
        {
            messagePanel.SetActive(true);
            messageText.text = MESSSAGE;
        }
    }
}

