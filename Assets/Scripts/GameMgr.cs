using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    // ---- 상점 -----
    [Header(" ---- Store ----- ")]
    public Button m_StoreBtn = null;
    public GameObject m_StorePanel = null;
    public Button m_ExitBtn = null;

    // ---- 코인-----
    [Header(" ---- Coin ----- ")]
    [HideInInspector] public GameObject m_CoinItem = null;
    public Text m_GoldText = null;
    int m_CurGold = 0;

    public static GameMgr Inst = null;


    void Awake()
    {
        Inst = this;    
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;

        m_CoinItem = Resources.Load("Prefab/Coin") as GameObject;

        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(StorePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StorePanel()
    {
        if (m_StorePanel != null)
            m_StorePanel.SetActive(true);

        Time.timeScale = 0.0f;

        if(m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(()=>
            {
                m_StorePanel.SetActive(false);
                Time.timeScale = 1.0f;
            });
    }

    public void AddGold()
    {
        m_CurGold += 10;
        if (m_CurGold < 0)
            m_CurGold = 0;

        m_GoldText.text = m_CurGold.ToString();
    }
}
