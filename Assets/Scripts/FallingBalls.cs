using UnityEngine;

public class FallingBalls : MonoBehaviour
{
    private float m_DownSpeed = 20f;

    void Update()
    {
        transform.Translate(0.0f, -m_DownSpeed * Time.deltaTime, 0.0f);

        if (transform.position.y < -6.5f)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
