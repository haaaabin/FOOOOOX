using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreBox : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;

    public GameObject m_StoreBoxObj = null;
    public Button ExitBtn = null;
    public Text m_StoreGold = null;

    public static int[] skill = new int[2];

    public Button m_LifeBuyBtn = null;
    public Button m_ShieldBuyBtn = null;

    public GameObject MessagePanel = null;
    public Text messageText = null;
    public Button M_CloseBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();

        if (ExitBtn != null)
            ExitBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1.0f;
                m_StoreBoxObj.SetActive(false);
            });

        if (m_LifeBuyBtn != null)
            m_LifeBuyBtn.onClick.AddListener(BuyLifeItem);

        if (m_ShieldBuyBtn != null)
            m_ShieldBuyBtn.onClick.AddListener(BuyShieldItem);

        if (M_CloseBtn != null)
            M_CloseBtn.onClick.AddListener(() =>
            {
                MessagePanel.SetActive(false);
            });
    }

    // Update is called once per frame
    void Update()
    {
        if (m_StoreGold != null)
            m_StoreGold.text = GlobalValue.g_UserGold.ToString();

        GameMgr.Inst.RefreshSkill();
    }

    void BuyLifeItem()
    {
        int a_Cost = 100;
        if (GlobalValue.g_UserGold >= a_Cost)
        {
            GlobalValue.g_UserGold -= a_Cost;

            m_SkType = SkillType.Skill_0;
            int a_SkIdx = (int)m_SkType;    //인덱스로 변환
            GlobalValue.g_SkillCount[a_SkIdx]++;
            skill[a_SkIdx]++;

            string a_Skill = "SkItem_" + (a_SkIdx).ToString();
            PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

        }
        else
        {
            MessagePanel.SetActive(true);
            messageText.text = "보유 금액 부족";
        }
    }

    void BuyShieldItem()
    {
        int a_Cost = 100;
        if (GlobalValue.g_UserGold >= a_Cost)
        {
            GlobalValue.g_UserGold -= a_Cost;

            m_SkType = SkillType.Skill_1;
            int a_SkIdx = (int)m_SkType;    //인덱스로 변환
            GlobalValue.g_SkillCount[a_SkIdx]++;
            skill[a_SkIdx]++;

            string a_Skill = "SkItem_" + (a_SkIdx).ToString();
            PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

        }
        else
        {
            MessagePanel.SetActive(true);
            messageText.text = "보유 금액 부족";
        }
    }
}  

