using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonBulletCtrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.right;
    float m_MoveSpeed = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
    }

    public void MBulletSpawn(Vector3 a_StPos, Vector3 a_DirVec, float a_Speed)
    {
        m_DirVec = a_DirVec;
        transform.position = new Vector3(a_StPos.x, a_StPos.y, 0.0f);
        m_MoveSpeed = a_Speed;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            Destroy(gameObject);
        }
    }

}
