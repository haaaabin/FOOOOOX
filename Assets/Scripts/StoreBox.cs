using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreBox : MonoBehaviour
{
    public Button ExitBtn = null;
    public Text m_StoreGold = null;

    public ItemSlot[] m_ItemSlot;


    public Button m_LifeBuyBtn = null;
    public Button m_ShieldBuyBtn = null;

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




    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuySkillItem(SkillType a_SkType)
    {
        if (a_SkType < SkillType.Skill_0 || SkillType.SkCount < a_SkType)
            return;
        int a_Cost = 500;

        if (m_LifeBuyBtn != null)
            m_LifeBuyBtn.onClick.AddListener(() =>
            {
                a_SkType = SkillType.Skill_0;

            });

        else if (m_ShieldBuyBtn != null)
            m_ShieldBuyBtn.onClick.AddListener(() =>
            {
                a_SkType = SkillType.Skill_1;

            });

        int a_SkIdx = (int)a_SkType;
        //SkillType 인덱스로 변환
        GlobalValue.g_SkillCount[a_SkIdx]++;
        GlobalValue.g_UserGold -= a_Cost;

        string a_Skill = "SkItem_" + (a_SkIdx).ToString();
        PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

    }

}  

