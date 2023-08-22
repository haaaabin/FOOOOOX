using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreBox : MonoBehaviour
{
    public Button ExitBtn = null;
    public Button BuyBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if (BuyBtn != null)
            BuyBtn.onClick.AddListener(BuyBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuyBtnClick()
    {

    }
}
