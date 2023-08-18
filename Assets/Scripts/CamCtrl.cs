using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    public Transform target;
    public float speed;

    //카메라 영역 설정
    public static Vector2 minCameraBoundary = new Vector2(-0.3f, -1.85f);
    public static Vector2 maxCameraBoundary = new Vector2(189f, 0.7f);

    //Start is called before the first frame update
    void Start()
    {
    
    }

    //Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + 3f, this.transform.position.z) ;

        targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

    }
}
