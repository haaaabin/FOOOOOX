using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowCam : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public CinemachineConfiner2D cinemachineConfiner2D;

    // Start is called before the first frame update
    void Start()
    {
        vCam.Follow = PlayerCtrl.Instance.transform;
        cinemachineConfiner2D.m_BoundingShape2D = GameObject.Find("Cam Border").GetComponent<PolygonCollider2D>();
    }
}
