using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkCtrl : MonoBehaviour
{
    [Header("--- Trunk Monster --- ")]
    public GameObject m_TMonBullet = null;
    public GameObject m_TshootPos = null;

    public int turn;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        Invoke("Think", 3);
        
    }

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = new Vector2(turn * 2f, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + turn * 0.8f, rigid.position.y - 1f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            turn *= -1;
            sprite.flipX = turn == 1;
            CancelInvoke();
            Invoke("Think", 2);
        }
    }

    void Think()
    {
        turn = Random.Range(-1, 2);

        anim.SetInteger("State", turn);
        if (turn != 0)
            sprite.flipX = turn == 1;

    }
}
