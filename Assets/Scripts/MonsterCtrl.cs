using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    bunny,
    dog,
    Mushroom
}
public class MonsterCtrl : MonoBehaviour
{

    public MonType m_MonType = MonType.bunny;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    public Transform PlayerTr;
    public Animator anim;

    public float speed = 7;
    private float m_Hp = 100;
    float m_CurHp = 100;

    public int nextMove;
    public float distance;
    Vector3 startPos;
    float curtime = 0;
    float maxtime = 2;

    bool isDie = false;

    public GameObject m_HpBarObj = null;
    public Image m_hpBarImg = null;
    public float m_HP = 100;
    public float m_curHp = 100;

    void Awake()
    {
        Invoke("Think", 5);
    }
    // Start is called before the first frame update
    void Start()
    {
        isDie = false;

        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        PlayerTr = GetComponent<Transform>();
        anim = GetComponent<Animator>();

       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDie)
        {
            if (m_MonType == MonType.bunny)
                bunny_AI();

            if (m_MonType == MonType.dog)
                dog_AI();

            if (m_MonType == MonType.Mushroom)
                mushroom_AI();

        }
    }

    void mushroom_AI()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.7f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null)
        {
            nextMove *= -1;
            sprite.flipX = nextMove == 1;
            CancelInvoke(); //���� �۵� ���� ��� Invoke �Լ��� ����
            Invoke("Think", 2);
        }
    }
    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);
       
        //Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
            sprite.flipX = nextMove == 1;   //���������� ���� flipx =true
        
        //Set Next Active
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }


    void bunny_AI()
    {
        rigid.velocity = new Vector2(nextMove * 3f, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.7f, rigid.position.y - 0.5f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            nextMove *= -1;
        }
        if (nextMove == 1)
            sprite.flipX = false;
        else if (nextMove == -1)
            sprite.flipX = true;
    }

    void dog_AI()
    {
       
        Debug.DrawRay(transform.position, Vector3.left, new Color(0, 1, 0));
        Debug.DrawRay(transform.position, Vector3.right, new Color(0, 1, 0));

        RaycastHit2D rayLeftHit = Physics2D.Raycast(transform.position, Vector3.left, distance, LayerMask.GetMask("Player"));
        RaycastHit2D rayRightHit = Physics2D.Raycast(transform.position, Vector3.right, distance, LayerMask.GetMask("Player"));

        if (rayLeftHit.collider != null)
        {
            sprite.flipX = false;
            anim.SetBool("DogRunning", true);
            transform.position = Vector3.MoveTowards(transform.position, rayLeftHit.collider.transform.position, Time.deltaTime * speed);
        }
        else if (rayRightHit.collider != null)
        {
            sprite.flipX = true;
            anim.SetBool("DogRunning", true);
            transform.position = Vector3.MoveTowards(transform.position, rayRightHit.collider.transform.position, Time.deltaTime * speed);
        }
        else
            anim.SetBool("DogRunning", false);     
    }

    

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag =="P_Bullet")
        {
            Destroy(coll.gameObject);
            TakeDemaged();
        }
    }
    public void TakeDemaged()
    {
        if (m_HpBarObj != null)
            m_HpBarObj.SetActive(true);

        m_curHp -= 20;

        if (m_hpBarImg != null)
            m_hpBarImg.fillAmount = m_curHp / m_Hp;

        if (m_curHp <= 0)
        {
            m_curHp = 0;
            m_HpBarObj.SetActive(false);
            MonsterDie();          
        }

        OnMonDamaged(PlayerTr.transform.position);

    }

    void MonsterDie()
    {
        isDie = true;
        
        anim.SetTrigger("Death");
        Destroy(gameObject, 0.3f);

        if(GameMgr.Inst.m_CoinItem != null)
        {
            GameObject a_CoinObj = Instantiate(GameMgr.Inst.m_CoinItem) as GameObject;
            a_CoinObj.transform.position = this.transform.position;
            
        }
    }

    void OnMonDamaged(Vector2 targetPos)
    {
        gameObject.layer = 9;
        sprite.color = new Color(0, 0, 0, 0.4f);
        Invoke("OffDamaged", 0.1f);
    }

    void OffDamaged()
    {
        gameObject.layer = 8;
        sprite.color = new Color(1, 1, 1, 1);
    }
}