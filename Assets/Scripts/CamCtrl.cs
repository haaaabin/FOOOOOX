using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CamCtrl : MonoBehaviour
{
    public Transform target;
    public float speed;

    //카메라 영역 설정
    public Vector2 minCameraBoundary;
    public Vector2 maxCameraBoundary;

    //Start is called before the first frame update
    void Start()
    {
        
    }

    //Update is called once per frame
    void LateUpdate()
    {

        Vector3 targetPos = new Vector3(target.position.x + 3f, target.position.y + 2f, this.transform.position.z) ;

        if (SceneManager.GetActiveScene().name == "BossScene")
        {
            targetPos = new Vector3(target.position.x, 0, this.transform.position.z);
            maxCameraBoundary = new Vector2(31.3f, 0);

            if (target.position.x > 22)
            {
                minCameraBoundary = new Vector2(31.0f, 0);
            }
        }


        targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

    }
}
