using UnityEngine;
using UnityEngine.UI;

public enum BossState
{
    Boss_Move,
    Boss_Attack,
    Fall_Bull
}

public enum MonType
{
    Walk_Monster,
    Run_Monster,
    Plant_Monster,
    Attack_Monster,
    Snail_Monster,
    Boss
}

public class MonsterController : MonoBehaviour
{
    public MonType monType = MonType.Walk_Monster;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator anim;
    private float moveSpeed = 3;
    private float runSpeed = 5;

    //hpBar
    public GameObject hpBarObj = null;
    public Image hpBarImg = null;
    private float hp = 100;
    [HideInInspector] public float curHp = 100;

    //--- Monster
    public int turn = 1;
    [Header("--- Plant Monster --- ")]
    public GameObject plantMonBullet = null;
    public GameObject plantMonShootPos = null;
    private float bulletSpeed = 10.0f;

    [Header("--- Trunk Monster --- ")]
    public GameObject trunkMonBullet = null;
    public GameObject trunkMonShootPos = null;

    [Header("--- Boss --- ")]
    private BossState bossState = BossState.Boss_Move;
    private int shootCount = 0;

    public GameObject bossHpBarObj = null;
    public Image bossHpBarImg = null;
    public GameObject bossBullet = null;
    public float bossBulletSpeed = 15.0f;
    public GameObject bossShootPos = null;

    [Header("--- Bullet --- ")]
    public GameObject bulletObj = null;
    private int fallCount = 0;
    private float shootTime = 0;
    private float shootDelay = 0.9f;

    private float delayTime = 10;
    private bool isChange;
    private bool isDie = false;

    private void Start()
    {
        isDie = false;
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        switch (monType)
        {
            case MonType.Walk_Monster:
                SetInitialHp(50.0f);
                break;
            case MonType.Attack_Monster:
                SetInitialHp(200.0f);
                break;
            case MonType.Boss:
                SetInitialHp(5000.0f);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!isDie)
        {
            switch (monType)
            {
                case MonType.Walk_Monster:
                    MoveMonster(turn, moveSpeed);
                    break;
                case MonType.Run_Monster:
                    MoveMonster(turn, runSpeed);
                    break;
                case MonType.Plant_Monster:
                    HandleAttackMonster(plantMonBullet, plantMonShootPos, Vector2.left, Vector2.right, 14, 10, shootDelay);
                    break;
                case MonType.Attack_Monster:
                    MoveMonster(turn, moveSpeed);
                    HandleAttackMonster(trunkMonBullet, trunkMonShootPos, Vector2.left, Vector2.right, 20, 6, 0.55f);
                    break;
                case MonType.Snail_Monster:
                    MoveMonster(turn, isChange ? 7 : moveSpeed);
                    break;
                case MonType.Boss:
                    Boss_AI();
                    break;
            }
        }
    }
    private void SetInitialHp(float hp)
    {
        this.hp = hp;
        curHp = this.hp;
    }

    private void MoveMonster(int direction, float speed)
    {
        rigid.velocity = new Vector3(direction * speed, rigid.velocity.y);
        CheckPlatform();
    }

    private void CheckPlatform()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + turn, rigid.position.y - 0.8f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            turn *= -1;
        }
        if (turn != 0)
            sprite.flipX = turn == 1;
    }

    private void HandleAttackMonster(GameObject bulletPrefab, GameObject shootPos, Vector2 leftDirection, Vector2 rightDirection, float boxWidth, float boxHeight, float attackDelay)
    {
        Collider2D coll = Physics2D.OverlapBox(transform.position, new Vector2(boxWidth, boxHeight), 0, LayerMask.GetMask("Player"));
        if (coll != null)
        {
            anim.SetBool("Attack", true);
            shootTime += Time.deltaTime;

            if (shootTime >= attackDelay)
            {
                Vector3 direction = coll.transform.position.x < transform.position.x ? leftDirection : rightDirection;
                ShootBullet(bulletPrefab, shootPos.transform, direction, bulletSpeed);
                shootTime = 0f;
            }
            sprite.flipX = coll.transform.position.x >= transform.position.x;
        }
        else
        {
            anim.SetBool("Attack", false);
        }
    }

    private void ShootBullet(GameObject bulletPrefab, Transform shootPos, Vector3 direction, float speed)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        BulletController bulletCtrl = bullet.GetComponent<BulletController>();
        bulletCtrl.InitializeBullet(shootPos.position, direction, speed, 1.0f);
    }

    private void Boss_AI()
    {
        Collider2D a_Coll = Physics2D.OverlapBox(transform.position, new Vector2(20, 20), 0, LayerMask.GetMask("Player"));
        if (a_Coll != null && bossHpBarObj != null)
        {
            bossHpBarObj.SetActive(true);
        }

        switch (bossState)
        {
            case BossState.Boss_Move:
                BossMove();
                break;
            case BossState.Fall_Bull:
                BossFallBull();
                break;
            case BossState.Boss_Attack:
                BossAttack();
                break;
        }
    }

    private void BossMove()
    {
        MoveBoss(-turn * 4);
        delayTime -= Time.deltaTime;
        if (delayTime <= 0.0f)
        {
            delayTime = 1f;
            shootTime = 0.5f;
            bossState = BossState.Fall_Bull;
        }
    }

    private void BossFallBull()
    {
        delayTime -= Time.deltaTime;
        if (delayTime <= 0.0f)
        {
            MoveBoss(-turn * 5);

            shootTime -= Time.deltaTime;
            if (shootTime <= 0.0f)
            {
                DropFallingBalls();
            }
        }
    }

    private void DropFallingBalls()
    {
        GameObject ballPrefabObj = Instantiate(bulletObj);
        int dropPosX = Random.Range(23, 38);
        ballPrefabObj.transform.position = new Vector3(dropPosX, 6.2f, 0.0f);

        fallCount++;
        if (fallCount < 14)
        {
            shootTime = 0.5f;
        }
        else
        {
            fallCount = 0;
            shootTime = 0.5f;
            delayTime = 1;
            bossState = BossState.Boss_Attack;
        }
        SoundManager.Instance.PlayGUISound("Fall", 1.2f);
    }

    private void BossAttack()
    {
        delayTime -= Time.deltaTime;
        if (delayTime <= 0.0f)
        {
            sprite.color = new Color(1, 0, 0, 1);
            anim.SetBool("Attack", true);

            MoveBoss(-turn * 9);

            shootTime -= Time.deltaTime;
            if (shootTime <= 0.0f)
                ShootBossBullets();
        }
    }

    private void ShootBossBullets()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject bossBulletObj = Instantiate(bossBullet);
            Vector3 bulletPos = bossBullet.transform.position;
            bulletPos.y -= 0.4f - (i * 1.2f);
            bossBulletObj.transform.position = bulletPos;

            BulletController monBulletCtrl = bossBulletObj.GetComponent<BulletController>();
            monBulletCtrl.InitializeBullet(bossBulletObj.transform.position, Vector3.left * turn, bossBulletSpeed, 1.0f);
        }

        shootCount++;
        if (shootCount < 19)
        {
            shootTime = 0.7f;
        }
        else
        {
            sprite.color = new Color(1, 1, 1, 1);
            anim.SetBool("Attack", false);
            shootCount = 0;
            delayTime = 10.0f;
            bossState = BossState.Boss_Move;
        }
    }

    private void MoveBoss(float direction)
    {
        rigid.velocity = new Vector2(direction, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + (-turn * 2.8f), rigid.position.y);
        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Wall"));
        if (rayHit.collider != null)
        {
            turn *= -1;
        }
        sprite.flipX = turn == -1;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "P_Bullet")
        {
            if (monType == MonType.Snail_Monster)
            {
                Destroy(coll.gameObject);
            }
            else
            {
                Destroy(coll.gameObject);
                TakeDemaged();
            }
            if (monType == MonType.Boss)
            {
                BossTakeDemaged();
                OnDamaged();
            }
        }

        if (coll.gameObject.tag == "Shield")
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                MonsterDie();
            }
        }
    }

    public void TakeDemaged(float value = 50)
    {
        anim.SetTrigger("Hit");

        curHp -= value;

        if (hpBarObj != null)
        {
            hpBarObj.SetActive(true);
            if (hpBarImg != null)
            {
                hpBarImg.fillAmount = curHp / hp;
            }
        }

        if (monType == MonType.Snail_Monster)
        {
            if (curHp == 50)
            {
                anim.SetTrigger("Hit");
                anim.SetBool("ChangeShell", true);
                isChange = true;
            }
            else if (curHp <= 0.0f)
            {
                anim.SetTrigger("ShellHit");
                curHp = 0.0f;
                GameMgr.instance.SpawnCoin(transform.position);
                MonsterDie();
            }
        }
        else
        {
            if (curHp <= 0.0f)
            {
                curHp = 0.0f;
                if (hpBarObj != null) hpBarObj.SetActive(false);
                GameMgr.instance.SpawnCoin(transform.position);
                MonsterDie();
            }
        }
    }

    private void BossTakeDemaged(float a_Value = 50)
    {
        if (curHp <= 0.0f)
            return;

        curHp -= a_Value;
        if (bossHpBarImg != null)
            bossHpBarImg.fillAmount = curHp / hp;

        if (curHp <= 0.0f)
        {
            curHp = 0.0f;
            if (bossHpBarObj != null) bossHpBarObj.SetActive(false);
            MonsterDie();
            GameMgr.instance.SpawnDiamond(transform.position);
        }
    }

    private void MonsterDie()
    {
        isDie = true;
        rigid.velocity = Vector2.zero;
        gameObject.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Destroy(gameObject, 0.5f);
    }

    private void OnDamaged()
    {
        sprite.color = new Color(1, 0, 0, 0.5f);
        Invoke("OffDamaged", 1);
    }

    private void OffDamaged()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }

}
