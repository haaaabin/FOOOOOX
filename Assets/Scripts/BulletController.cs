using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Vector3 direction = Vector3.right;
    private float moveSpeed = 10.0f;
    private float lifeTime = 1f; // 기본 수명

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    
    private void Update()
    {
        transform.position += direction * Time.deltaTime * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            Destroy(gameObject);
        }
    }
    
    public void InitializeBullet(Vector3 startPos, Vector3 dir, float speed, float bulletLifeTime = 0.7f)
    {
        direction = dir;
        transform.position = new Vector3(startPos.x, startPos.y, 0.0f);
        moveSpeed = speed;
        lifeTime = bulletLifeTime;
        Destroy(gameObject, lifeTime);
    }
}
