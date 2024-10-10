using UnityEngine;
using UnityEngine.SceneManagement;

public class CamCtrl : MonoBehaviour
{
    public Transform target;
    public Vector2 minCameraBoundary;
    public Vector2 maxCameraBoundary;
    public float speed;

    private void LateUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x + 3f, target.position.y + 1.5f, this.transform.position.z);

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
