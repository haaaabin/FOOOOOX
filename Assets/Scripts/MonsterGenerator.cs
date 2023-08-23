using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    [Header("---- Monster Spawn ----")]
    //몬스터가 출현할 위치를 담을 배열
    [HideInInspector] public Transform[] MR_points;
    [HideInInspector] public Transform[] S_points;
    [HideInInspector] public Transform[] P_points;


    //몬스터 프리팹을 할당할 변수
    public GameObject[] monsterPrefab;
    public Transform mon_Root = null;

    // Start is called before the first frame update
    void Start()
    {
        //--- Monster Spawn
        MR_points = GameObject.Find("MushRoomSpawnPos").GetComponentsInChildren<Transform>();
        S_points = GameObject.Find("SnailSpawnPos").GetComponentsInChildren<Transform>();
        P_points = GameObject.Find("PlantSpawnPos").GetComponentsInChildren<Transform>();
        if (MR_points.Length > 0 || S_points.Length > 0 || P_points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
