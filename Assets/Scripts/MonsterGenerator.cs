using System.Collections;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    [Header("---- Monster Spawn ----")]
    [HideInInspector] public Transform[] mushroomPoints;
    [HideInInspector] public Transform[] snailPoints;
    [HideInInspector] public Transform[] plantPoints;
    [HideInInspector] public Transform[] rockPoints;
    [HideInInspector] public Transform[] miniRockPoints;
    [HideInInspector] public Transform[] trunkPoints;

    public GameObject[] monsterPrefab;
    public Transform monsterRoot = null;
    private Transform[][] spawnPoints;  // 모든 스폰 포인트를 저장할 배열

    private void Start()
    {
        // 각 스폰  포인트 그룹을 찾아서 배열에 저장
        mushroomPoints = GameObject.Find("MushRoomSpawnPos").GetComponentsInChildren<Transform>();
        snailPoints = GameObject.Find("SnailSpawnPos").GetComponentsInChildren<Transform>();
        plantPoints = GameObject.Find("PlantSpawnPos").GetComponentsInChildren<Transform>();
        rockPoints = GameObject.Find("RockSpawnPos").GetComponentsInChildren<Transform>();
        miniRockPoints = GameObject.Find("MiniRockSpawnPos").GetComponentsInChildren<Transform>();
        trunkPoints = GameObject.Find("TrunkSpawnPos").GetComponentsInChildren<Transform>();

        // 스폰 포인트들을 배열로 저장
        spawnPoints = new Transform[][] { mushroomPoints, snailPoints, plantPoints, rockPoints, miniRockPoints, trunkPoints };

        // 스폰 포인트가 있는지 확인하고, 있다면 몬스터 생성 코루틴 시작
        if (AnyPointsAvailable())
        {
            StartCoroutine(CreateMonster());
        }
    }

    // 스폰 포인트가 있는지 확인
    private bool AnyPointsAvailable()
    {
        foreach (var points in spawnPoints)
        {
            if (points != null && points.Length > 0)
                return true;
        }
        return false;
    }

    // 몬스터 생성 코루틴
    private IEnumerator CreateMonster()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform[] points = spawnPoints[i];
            if (points == null || points.Length <= 1) continue; // 유효한 스폰 포인트가 없으면 건너뜀

            for (int j = 1; j < points.Length; j++)
            {
                GameObject monster = Instantiate(monsterPrefab[i]);
                monster.transform.SetParent(monsterRoot);
                monster.transform.position = points[j].position;
                yield return null;
            }
        }
    }
}
