using Cinemachine;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;

    // Start is called before the first frame update
    void Start()
    {
        vCam.Follow = PlayerCtrl.Instance.transform;
    }
}
