using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameLevel
{
    Level1,
    Boss
}

public class GameMgr : MonoBehaviour
{
    //---- 캐릭터 머리 위에 데미지 띄우기용 변수 선언
    GameObject m_DmgClone;  //Damage Text 복사본을 받을 변수
    DmgTextCtrl m_DmgText;  //Damage Text 복사본에 있는 DmgText_Ctrl 컴포넌트를 받을 변수
    Vector3 m_StCacPos;     //시작 위치를 계산해 주기 위한 변수
    [Header("--- Damage Text ---")]
    public Transform m_Damage_Canvas = null;    //Damage_Canvas
    public GameObject m_DamageRoot = null;      //Damage프리팹

    // ---- 상점 -----
    [Header(" ---- Store ----- ")]
    public Button m_StoreBtn = null;
    public GameObject m_StorePanel = null;
    public Button ExitBtn = null;
    public Text m_StoreGold = null;

    public ItemSlot[] m_ItemSlot;

    public GameObject m_CoinItem = null;
    public Text m_GoldText = null;
    int m_CurGold = 0;

    PlayerCtrl m_Player = null;

    public SkInvenNode[] m_SkInvenNode;     //Skill 인벤토리 연결 변수

    //--- 설정 ----
    [Header("---- Config -----")]
    public Button m_ConfigBtn = null;
    public GameObject m_ConfigPanel = null;
    
    public static GameMgr Inst = null;

    public static GameLevel m_gameLevel = GameLevel.Level1;

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        GlobalValue.LoadGameData();

        RefreshGameUI();

        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(StoreBox);
         
        m_Player = GameObject.FindObjectOfType<PlayerCtrl>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //--단축키 이용으로 스킬 사용
        if(Input.GetKeyDown(KeyCode.Alpha1) ||
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

    public void UseSkill_Key(SkillType a_SkType)
    {
        if (GlobalValue.g_SkillCount[(int)a_SkType] <= 0)
            return;

        if (m_Player != null)
            m_Player.UseSkill_Item(a_SkType);

        if ((int)a_SkType < m_SkInvenNode.Length)
            m_SkInvenNode[(int)a_SkType].m_SkCountText.text =
                        GlobalValue.g_SkillCount[(int)a_SkType].ToString();

    }

    public void DamageText(float a_Value, Vector3 a_Pos, Color a_color)
    {
        if (m_Damage_Canvas == null || m_DamageRoot == null)
            return;

        m_DmgClone = (GameObject)Instantiate(m_DamageRoot); //DmgTextRoot 생성
        m_DmgClone.transform.SetParent(m_Damage_Canvas);    //m_Damage_Canvas에 붙여
        m_DmgText = m_DmgClone.GetComponent<DmgTextCtrl>();
        if (m_DmgText != null)
            m_DmgText.InitDamage(a_Value, a_color);
        m_StCacPos = new Vector3(a_Pos.x, a_Pos.y + 1.4f, 0.0f);
        m_DmgClone.transform.position = m_StCacPos;

    }


    public void SpawnCoin(Vector3 a_Pos)
    {
        if (m_CoinItem == null)
            return;

        GameObject a_CoinObj = Instantiate(m_CoinItem) as GameObject;
        a_CoinObj.transform.position = a_Pos;
        Destroy(a_CoinObj, 10);
    }

    public void AddGold(int value = 10)
    {        
        if (m_CurGold < 0)
            m_CurGold = 0;

        if (GlobalValue.g_UserGold <= 0)
            GlobalValue.g_UserGold = 0;

        m_CurGold += value;
        GlobalValue.g_UserGold += value;

        m_GoldText.text = m_CurGold.ToString();
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
    }

    public void StoreBox()
    {
        Time.timeScale = 0.0f;

        if (m_StorePanel != null)
            m_StorePanel.SetActive(true);

        if (ExitBtn != null)
            ExitBtn.onClick.AddListener(() =>
            {
                m_StorePanel.SetActive(false);
                Time.timeScale = 1.0f;
            });

        if (m_StoreGold != null)
            m_StoreGold.text = GlobalValue.g_UserGold.ToString();

    }

    public void BuySkillItem(SkillType a_SkType)
    {
        if (a_SkType < SkillType.Skill_0 || SkillType.SkCount < a_SkType)
            return;
        int a_Cost = 0;

        if (a_SkType == SkillType.Skill_0)
        {
            a_Cost = 500;
        }
        else if (a_SkType == SkillType.Skill_1)
        {
            a_Cost = 500;
        }

        GlobalValue.g_UserGold -= a_Cost;

        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        //if ((int)a_SkType < m_ItemSlot.Length)
        //    m_ItemSlot[(int)a_SkType].m_BuyBtn.onClick.AddListener(() =>
        //    {
        //        int a_SkIdx = (int)a_SkType;    //SkillType 인덱스로 변환
        //        GlobalValue.g_SkillCount[a_SkIdx]++;
        //        GlobalValue.g_UserGold -= a_Cost;

        //        //변동사항 로컬에 저장
        //        string a_Skill = "SkItem_" + (a_SkIdx).ToString();
        //        PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
        //        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        //    });
    }

    void RefreshGameUI()
    {
        for(int i = 0; i <GlobalValue.g_SkillCount.Length; i++)
        {
            if (m_SkInvenNode.Length <= i)
                return;

            m_SkInvenNode[i].m_SkType = (SkillType)i;
            m_SkInvenNode[i].m_SkCountText.text = GlobalValue.g_SkillCount[i].ToString();
        }
    }
}
