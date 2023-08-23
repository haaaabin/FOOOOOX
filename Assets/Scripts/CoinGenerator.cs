using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    // ---- ÄÚÀÎ-----
    [Header(" ---- Coin ----- ")]
    [HideInInspector] public GameObject m_CoinItem = null;
    [HideInInspector] public Transform[] Coin_points;

    // Start is called before the first frame update
    void Start()
    {
        //---Coin Spawn
        m_CoinItem = Resources.Load("Coin") as GameObject;

        Coin_points = GameObject.Find("CoinPos").GetComponentsInChildren<Transform>();
        for (int i = 1; i < Coin_points.Length; i++)
        {
            GameObject coin = Instantiate(m_CoinItem) as GameObject;
            coin.transform.position = Coin_points[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
