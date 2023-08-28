using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreBox : MonoBehaviour
{
    public Button ExitBtn = null;
    public GameObject m_StorePanel = null;
    public Text m_StoreGold = null;

    public ItemSlot[] m_ItemSlot;

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();

        if (ExitBtn != null)
            ExitBtn.onClick.AddListener(() =>
            {
                m_StorePanel.SetActive(false);
                Time.timeScale = 1.0f;
            });

        if (m_StoreGold != null)
            m_StoreGold.text = GlobalValue.g_UserGold.ToString();




    }

    // Update is called once per frame
    void Update()
    {

    }

   
}  

