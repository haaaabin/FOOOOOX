using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{ 
    //스크린의 월드 좌표
    public static Vector3 m_ScreenWMin = new Vector3(-20.0f, -6.0f, 0.0f);
    public static Vector3 m_ScreenWMax = new Vector3(20f, 6.0f, 0.0f);
    //스크린의 월드 좌표

    public Transform target;
    public float speed;

    //카메라 영역 설정
    public Vector2 center;
    public Vector2 size;

    public static Vector3 CamVec;

    float height;   //카메라의 월드 세로
    float width;    //월드 가로

    //Start is called before the first frame update
    void Start()
    {
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;  //월드세로 * 스크린 가로 / 스크린 세로
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }

    //Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed); //카메라와 플레이어 사이의 벡터 값 반환
        transform.position = new Vector3(transform.position.x, 0, -10f);

        float clampX = Mathf.Clamp(transform.position.x, 0, width * 1000);    //Mathf.Clamp(value, min, max)
        float ly = size.y * 0.5f - height;  //size.y의 절반값 - 세로
        float clampY = Mathf.Clamp(transform.position.y, -ly + center.y, ly + center.y);
        transform.position = new Vector3(clampX, clampY, -10f);
        CamVec = transform.position;
    }
}
