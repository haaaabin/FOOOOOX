using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;

    public Button m_BuyBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if(m_BuyBtn != null)
        {
            m_BuyBtn.onClick.AddListener(() =>
            {
                GameMgr a_StoreBox = null;
                GameObject a_StoreObj = GameObject.Find("GameMgr");
                if (a_StoreObj != null)
                    a_StoreBox = a_StoreObj.GetComponent<GameMgr>();
                if (a_StoreBox != null)
                    a_StoreBox.BuySkillItem(m_SkType);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
