using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    [HideInInspector] public GameObject coinObject = null;
    [HideInInspector] public Transform[] coinPositions;

    private void Start()
    {
        coinObject = Resources.Load("Coin") as GameObject;
        coinPositions = GameObject.Find("CoinPos").GetComponentsInChildren<Transform>();
        for (int i = 1; i < coinPositions.Length; i++)
        {
            GameObject coin = Instantiate(coinObject);
            coin.transform.position = coinPositions[i].position;
        }
    }
}
