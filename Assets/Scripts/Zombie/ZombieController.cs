using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ZombieController : NetworkBehaviour
{

    private ZombieManager zombieManager;

    GameManager gameManager;

    [SerializeField] AudioController audioController;

    [SerializeField] GameObject[] sprites = new GameObject[3];

    public Animator animator;

    Rigidbody2D rb;

    [SerializeField] Transform refTransform;

    [SerializeField] NormalZombieAI normalZombieAI;

    Vector2 distance;
    public int damage;
    public int health;

    float time = 0;

    [SerializeField] bool directionOfView = true;
    [SerializeField] bool pose1 = false;
    [SerializeField] bool pose2 = false;
    [SerializeField] bool pose3 = false;

    bool checkDead=false;

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer)
            return;

        rb = GetComponent<Rigidbody2D>();
        zombieManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZombieManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        health = zombieManager.zombieHealt;
        damage = zombieManager.zombieDamage;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (!isServer)
            return;

        if (normalZombieAI.targetTransform != null)
            distance = (Vector2)normalZombieAI.targetTransform.position;
        ChangeSprite();
        ChangeDirectionOfView();

        if (!audioController.audioSource.isPlaying && time>Random.Range(2,6))
        {
            audioController.RpcMakeSomeNoise(gameObject.name[0] + "N" + Random.Range(0, 4).ToString());
        }
    }

    public void TakeDamage(GameObject player, int gunDamage)
    {
        if (!isServer)
            return;
        Debug.Log("sa");
        CmdTakeDamage(player, gunDamage);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isServer || normalZombieAI.IsDead)
            return;

        if (GameObject.FindGameObjectsWithTag("Player") == null)
            return;

        if (collision.gameObject.tag == "Player")
        {
            if (!normalZombieAI.IsDead || !(gameObject.name[0] == 'F'))
                RpcChangeAnimation("IsAttacking", true);
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isServer)
            return;

        if (collision.gameObject.tag == "Player" && !(gameObject.name[0] == 'F'))
        {
            RpcChangeAnimation("IsAttacking", false);

        }
    }

    [ClientRpc]
    void RpcChangeAnimation(string name, bool a)
    {
        animator.SetBool(name, a);
    }

    [Command]
    void CmdTakeDamage(GameObject player, int gunDamage)
    {
        ZombieManager zombieManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZombieManager>();

        if (health <= 0 &&!checkDead)
        {
            if (player.GetComponent<PlayerController>().hasAuthority && !checkDead)
                gameManager.server_Score += gameManager.score;
            else
                gameManager.p2_Score += gameManager.score;

            normalZombieAI.IsDead = true;
            zombieManager.deadZombieCountOncurrentLevel++;
            zombieManager.deadZombieCount++;
            RpcChangeAnimation("IsDead", true);
            audioController.RpcMakeSomeNoise(gameObject.name[0] + "D" + Random.Range(1, 2).ToString());
            audioController.audioSource.Play();
            Invoke("CmdDestroy", 1.2f);

            checkDead = true;
        }
        else
        {
            health -= gunDamage;
            audioController.RpcMakeSomeNoise(gameObject.name[0] + "T" + Random.Range(1, 2).ToString());
            audioController.audioSource.Play();
        }

    }

    [Command]
    void CmdDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    void CmdChangeSprite(byte i, Vector2 mouse)
    {
        RpcChangeSprite(i, mouse);
    }

    [ClientRpc]
    void RpcChangeSprite(byte i, Vector2 mouse)
    {
        switch (i)
        {
            case 1:
                sprites[0].SetActive(false);
                sprites[1].SetActive(false);
                sprites[2].SetActive(true);
                animator = sprites[2].GetComponent<Animator>();
                break;

            case 2:
                sprites[0].SetActive(false);
                sprites[1].SetActive(true);
                sprites[2].SetActive(false);
                animator = sprites[1].GetComponent<Animator>();
                break;

            case 3:
                sprites[0].SetActive(true);
                sprites[1].SetActive(false);
                sprites[2].SetActive(false);
                animator = sprites[0].GetComponent<Animator>();
                break;
        }


    }

    void ChangeSprite()
    {


        byte i = 0;

        Vector2 vect = distance - rb.position;

        float tan = vect.y / vect.x;
        float angle = Mathf.Atan(tan) * 57.2958f;

        bool angleCondition_1 = ((angle < 60f) || (angle < 0f && (angle > -60f)) && !pose3);
        bool angleCondition_2 = ((angle > 60f) || (angle < -60f && angle > -90f) && !pose2);

        if (normalZombieAI.target == null)
        {
            return;
        }
        bool yAxisCondition = (refTransform.position.y < normalZombieAI.targetTransform.position.y);

        if (!yAxisCondition && !pose3)
        {

            i = 3;
            pose1 = false;
            pose2 = false;
            pose3 = true;
            CmdChangeSprite(i, normalZombieAI.unNormalizedDirection);

        }

        else if (angleCondition_1 && yAxisCondition && !pose2)
        {

            i = 2;
            pose1 = false;
            pose2 = true;
            pose3 = false;
            CmdChangeSprite(i, normalZombieAI.unNormalizedDirection);

        }

        else if (angleCondition_2 && yAxisCondition && !pose1)
        {

            i = 1;
            pose1 = true;
            pose2 = false;
            pose3 = false;
            CmdChangeSprite(i, normalZombieAI.unNormalizedDirection);

        }
        else
            return;

    }

    void ChangeDirectionOfView()
    {
        if (((distance - rb.position).x >= 0.1f) && !directionOfView)
        {
            CmdChangeDirectionOfView(1);
        }

        else if ((-(distance - rb.position).x >= 0.1f) && directionOfView)
        {
            CmdChangeDirectionOfView(2);
        }
    }

    [Command]
    void CmdChangeDirectionOfView(byte i)
    {
        RpcChangeDirectionOfView(i);
    }

    [ClientRpc]
    void RpcChangeDirectionOfView(byte i)
    {
        switch (i)
        {
            case 1:
                transform.localScale = new Vector3(.2f, .2f, .2f);
                directionOfView = true;
                break;
            case 2:
                transform.localScale = new Vector3(-.2f, .2f, .2f);
                directionOfView = false;
                break;
        }
    }


}
