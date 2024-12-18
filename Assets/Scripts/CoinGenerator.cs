using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public GameObject coinObject;
    private Transform[] coinPositions;

    private void Start()
    {
        coinPositions = GameObject.Find("CoinPos").GetComponentsInChildren<Transform>();
        for (int i = 1; i < coinPositions.Length; i++)
        {
            GameObject coin = Instantiate(coinObject);
            coin.transform.position = coinPositions[i].position;
        }
    }
}
