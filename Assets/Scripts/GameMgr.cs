using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Button m_ExitBtn = null;

    // ---- 코인-----
    [Header(" ---- Coin ----- ")]
    [HideInInspector] public GameObject m_CoinItem = null;
    [HideInInspector] public Transform[] Coin_points;
    public Text m_GoldText = null;
    int m_CurGold = 0;

    [Header("---- Monster Spawn ----")]
    //몬스터가 출현할 위치를 담을 배열
    [HideInInspector] public Transform[] MR_points;
    [HideInInspector] public Transform[] S_points;
    [HideInInspector] public Transform[] P_points;

    //몬스터 프리팹을 할당할 변수
    public GameObject[] monsterPrefab;
    public Transform mon_Root = null;

    public static GameMgr Inst = null;

    void Awake()
    {
        Inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;

        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(StorePanel);

        //--- Monster Spawn
        MR_points = GameObject.Find("MushRoomSpawnPos").GetComponentsInChildren<Transform>();
        S_points = GameObject.Find("SnailSpawnPos").GetComponentsInChildren<Transform>();
        P_points = GameObject.Find("PlantSpawnPos").GetComponentsInChildren<Transform>();
        if (MR_points.Length > 0 || S_points.Length > 0 || P_points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
        }

        //---Coin Spawn
        m_CoinItem = Resources.Load("Coin") as GameObject;

        Coin_points = GameObject.Find("CoinPos").GetComponentsInChildren<Transform>();
        for(int i= 1; i < Coin_points.Length; i++)
        {
            GameObject coin = Instantiate(m_CoinItem) as GameObject;
            coin.transform.position = Coin_points[i].position;
        }

    }
    IEnumerator CreateMonster()
    {
        for (int i = 1; i < MR_points.Length; i++)
        {
            GameObject mon = Instantiate(monsterPrefab[0]) as GameObject;
            mon.transform.SetParent(mon_Root);
            mon.transform.position = MR_points[i].position;
        }
        for (int i = 1; i < S_points.Length; i++)
        {
            GameObject mon = Instantiate(monsterPrefab[1]) as GameObject;
            mon.transform.SetParent(mon_Root);
            mon.transform.position = S_points[i].position;
        }
        for (int i = 1; i < P_points.Length; i++)
        {
            GameObject mon = Instantiate(monsterPrefab[2]) as GameObject;
            mon.transform.SetParent(mon_Root);
            mon.transform.position = P_points[i].position;
        }
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {

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

    public void SpawnCoin(Vector3 a_Pos)
    {
        if (m_CoinItem == null)
            return;

        GameObject a_CoinObj = Instantiate(m_CoinItem) as GameObject;
        a_CoinObj.transform.position = a_Pos;
        Destroy(a_CoinObj, 10.0f);
    }

    public void AddGold()
    {
        m_CurGold += 10;
        if (m_CurGold < 0)
            m_CurGold = 0;

        m_GoldText.text = m_CurGold.ToString();
    }
}
