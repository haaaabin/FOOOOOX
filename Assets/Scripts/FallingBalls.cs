using UnityEngine;

public class FallingBalls : MonoBehaviour
{
    [HideInInspector] public float m_DownSpeed = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0.0f, -m_DownSpeed * Time.deltaTime, 0.0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("RemoveBallZone"))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player != null)
        {
            player.TakeDamage(1, transform.position);
            Destroy(gameObject);
        }
    }
}
