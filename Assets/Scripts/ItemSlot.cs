using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;

    public Button[] ItemSlotBtn;

    // Start is called before the first frame update
    void Start()
    {
        //StoreBox a_StoreBox = null;
        //GameObject a_StoreObj = GameObject.Find("StoreBox");
        //if (a_StoreObj != null)
        //    a_StoreBox = a_StoreObj.GetComponent<StoreBox>();

        //if (ItemSlotBtn[0] != null)
        //{
        //    ItemSlotBtn[0].onClick.AddListener(() =>
        //    {
        //        if (a_StoreBox != null)
        //            a_StoreBox.BuySkillItem(m_SkType);
        //    });
        //}
        //else if (ItemSlotBtn[1] != null)
        //{
        //    ItemSlotBtn[1].onClick.AddListener(() =>
        //    {
        //        if (a_StoreBox != null)
        //            a_StoreBox.BuySkillItem(m_SkType);
        //    });
        //}
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}


