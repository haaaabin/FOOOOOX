using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
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
        m_CoinItem = Resources.Load("Coin") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddGold()
    {
        m_CurGold += 10;
        if (m_CurGold < 0)
            m_CurGold = 0;

        m_GoldText.text = m_CurGold.ToString();
    }
}
