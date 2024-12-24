using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CamCtrl : MonoBehaviour
{
    private Transform target;
    private Vector2 minCameraBoundary;
    private Vector2 maxCameraBoundary;
    private Vector3 targetPos;
    public float speed;
    public CinemachineVirtualCamera vCam;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BossScene")
        {
            vCam.gameObject.SetActive(false);
        }
        target = PlayerCtrl.Instance.transform;
    }

    private void LateUpdate()
    {
        targetPos = new Vector3(target.position.x, 0, this.transform.position.z);
        minCameraBoundary = new Vector2(0.5f, 0);
        maxCameraBoundary = new Vector2(32.5f, 0);

        if (target.position.x > 22)
        {
            minCameraBoundary = new Vector2(32.5f, 0);
        }

        targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }
}
