using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreBox : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;

    public Button ExitBtn = null;
    public Text m_StoreGold = null;

    public Button[] m_ItemBtn;

    public Button m_LifeBuyBtn = null;
    public Button m_ShieldBuyBtn = null;

    public GameObject MessagePanel = null;
    public Text messageText = null;
    public Button CloseBtn = null;

    bool buying = true;
    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();

        if (ExitBtn != null)
            ExitBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1.0f;
                Destroy(gameObject);
            });

        if (m_StoreGold != null)
            m_StoreGold.text = GlobalValue.g_UserGold.ToString();

        if (m_LifeBuyBtn != null)
            m_LifeBuyBtn.onClick.AddListener(BuyLifeItem);

        if (m_ShieldBuyBtn != null)
            m_ShieldBuyBtn.onClick.AddListener(BuyShieldItem);

        if (CloseBtn != null)
            CloseBtn.onClick.AddListener(() =>
            {
                MessagePanel.SetActive(false);
            });
    }

    // Update is called once per frame
    void Update()
    {

    }


    void BuyLifeItem()
    {
        if(buying)
        {
            int a_Cost = 1000;
            m_SkType = SkillType.Skill_0;
            int a_SkIdx = (int)m_SkType;
            GlobalValue.g_SkillCount[a_SkIdx]++;
            GlobalValue.g_UserGold -= a_Cost;

            if (GlobalValue.g_UserGold <= a_Cost)
            {
                buying = false;
                MessagePanel.SetActive(true);
                messageText.text = "보유 금액 부족";

                GlobalValue.g_UserGold = 0;
            }

            string a_Skill = "SkItem_" + (a_SkIdx).ToString();
            PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        }

        RefreshSkItemUI();
    }

    void BuyShieldItem()
    {
        if (buying)
        {
            int a_Cost = 500;
            m_SkType = SkillType.Skill_1;
            int a_SkIdx = (int)m_SkType;
            GlobalValue.g_SkillCount[a_SkIdx]++;
            GlobalValue.g_UserGold -= a_Cost;

            string a_Skill = "SkItem_" + (a_SkIdx).ToString();
            PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);


            if (GlobalValue.g_UserGold < a_Cost)
            {
                MessagePanel.SetActive(true);
                messageText.text = "보유 금액 부족";

                buying = false;

                GlobalValue.g_UserGold = 0;
            }

            RefreshSkItemUI();
        }
    }
    //public void BuySkillItem(SkillType a_SkType)
    //{
    //    if (a_SkType < SkillType.Skill_0 || SkillType.SkCount <= a_SkType)
    //        return;

    //    if (m_ItemSlot[0])
    //        a_SkType = SkillType.Skill_0;

    //    else
    //        a_SkType = SkillType.Skill_1;

    //    int a_Cost = 500;

    //    int a_SkIdx = (int)m_SkType;
    //    GlobalValue.g_SkillCount[a_SkIdx]++;
    //    GlobalValue.g_UserGold -= a_Cost;

    //    string a_Skill = "SkItem_" + (a_SkIdx).ToString();
    //    PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
    //    PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

    //    if (GlobalValue.g_UserGold < a_Cost)
    //        Debug.Log("구매금액 부족");
    //    RefreshSkItemUI();
    //}

    void RefreshSkItemUI()
    {
        m_StoreGold.text = GlobalValue.g_UserGold.ToString();
        GameMgr.Inst.RefreshGameUI(); 
    }

}  

