using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{


   

    [SyncVar] public float health;
    [SerializeField] public float controlHealth;
    [SerializeField] public float timeBetweenDamages;
    [SerializeField] private float speed;
    [SerializeField] private float vertical;
    [SerializeField] private float horizontal;

    Rigidbody2D playerRb;

    Vector3 mousePos;

    Vector2 position_;
    Vector2 mouse;
    Vector2 move;

    [SerializeField] Transform refTransform;

    [SerializeField] GameObject[] sprites = new GameObject[3];

    public Transform a;

    NetworkIdentity id;

    [SerializeField] Animator animator;

    [SerializeField] bool pose1 = false;
    [SerializeField] bool pose2 = false;
    [SerializeField] bool pose3 = false;
    [SerializeField] bool directionOfView = true;
    [SyncVar] bool check = true;
    [SerializeField] bool oneTime = false;

    [SerializeField] bool takedDamage = false;

    public bool isTakingDamage = false;

    void Start()
    {

        health = 100f;
        timeBetweenDamages = 0f;
        controlHealth = health;
        playerRb = gameObject.GetComponent<Rigidbody2D>();
        id = gameObject.GetComponent<NetworkIdentity>();
        animator = sprites[0].GetComponent<Animator>();

        
    }

    private void Update()
    {
        if (timeBetweenDamages < 3f)
            timeBetweenDamages += Time.deltaTime;

        mouse.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mouse.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        if (!hasAuthority)
            return;

       


        

        if (health <= 0 && hasAuthority)
            CmdTurnOff();


        if ((controlHealth != health))
        {
            isTakingDamage = true;
            if (!oneTime && check)
                StartCoroutine(ToggleBool());
            CmdFindAndChangeRenderers();
            oneTime = true;
        }

        if (timeBetweenDamages >= 1)
        {
            oneTime = false;
            controlHealth = health;
            isTakingDamage = false;
            check = true;
            CmdFindAndChangeRenderers();
        }
    }

    private void FixedUpdate()
    {

        if (!hasAuthority)
            return;

        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        move = new Vector2(horizontal * Time.deltaTime * speed, vertical * Time.deltaTime * speed);
        playerRb.velocity = move;
        CmdTranslate(move, Mathf.Abs(vertical + horizontal));
        ChangeSprite();
        ChangeDirectionOfView();
    }


    [Command]
    void CmdTranslate(Vector2 move, float animSpeedVaraible)
    {

        //playerRb.velocity = move;
        //Vector2 pos = playerRb.position;
        RpcTranslate(/*pos*/new Vector2(), animSpeedVaraible);

    }

    [ClientRpc]
    void RpcTranslate(Vector2 c, float animSpeedVaraible)
    {
        //if (!isServer)
        //    playerRb.position = c;

        animator.SetFloat("Speed", animSpeedVaraible);
    }

    void FindChildren()
    {
        for (int i = 0; i < 3; i++)
        {
            sprites[i] = this.gameObject.transform.GetChild(i).gameObject;
        }
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

        Vector2 vect = mouse - playerRb.position;

        float tan = vect.y / vect.x;
        float angle = Mathf.Atan(tan) * 57.2958f;

        bool angleCondition_1 = ((angle < 60f) || (angle < 0f && (angle > -60f)) && !pose3);
        bool angleCondition_2 = ((angle > 60f) || (angle < -60f && angle > -90f) && !pose2);
        bool yAxisCondition = (refTransform.position.y < mouse.y);

        if (!yAxisCondition && !pose3)
        {

            i = 3;
            pose1 = false;
            pose2 = false;
            pose3 = true;
            CmdChangeSprite(i, mouse);

        }

        else if (angleCondition_1 && yAxisCondition && !pose2)
        {

            i = 2;
            pose1 = false;
            pose2 = true;
            pose3 = false;
            CmdChangeSprite(i, mouse);

        }

        else if (angleCondition_2 && yAxisCondition && !pose1)
        {

            i = 1;
            pose1 = true;
            pose2 = false;
            pose3 = false;
            CmdChangeSprite(i, mouse);

        }
        else
            return;

    }

    void ChangeDirectionOfView()
    {
        if (((mouse - (Vector2)refTransform.position).x >= 0.1f) && !directionOfView)
        {
            CmdChangeDirectionOfView(1);
        }

        else if ((((Vector2)refTransform.position - mouse).x >= 0.1f) && directionOfView)
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


    public void TakeDamage(int damage)
    {
        CmdTakeDamage(damage);
    }

    [Command]
    void CmdTakeDamage(int damage)
    {
        RpcTakeDamage(damage);
    }

    [ClientRpc]
    void RpcTakeDamage(int damage)
    {
        if (timeBetweenDamages > 1f)
        {
            health -= damage;
            timeBetweenDamages = 0f;

        }

    }

    [Command]
    private void CmdTurnOff()
    {
        RpcTurnOff();
    }

    [ClientRpc]
    private void RpcTurnOff()
    {
        gameObject.SetActive(false);
    }

    void FindandAndChangeRendereres()
    {
        GameObject activeChild = null;

        for (int i = 0; i < 3; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                activeChild = transform.GetChild(i).gameObject;
                break;
            }
        }

        for (int i = 1; i < activeChild.transform.childCount; i++)
        {

            GameObject rendererGO;
            rendererGO = activeChild.transform.GetChild(i).gameObject;
            rendererGO.GetComponent<Renderer>().enabled = check;
        }
    }

    IEnumerator ToggleBool()
    {
        while (isTakingDamage)
        {

            check = !check;
            yield return new WaitForSeconds(.1f);
        }
    }

    public void BoolStaff()
    {

        CmdBoolStaff();
    }

    [Command]
    public void CmdBoolStaff()
    {
        RpcBoolStaff();
    }
    [ClientRpc]
    public void RpcBoolStaff()
    {
        isTakingDamage = false;
        check = true;
        Debug.Log("sd");
    }

    [Command]
    void CmdFindAndChangeRenderers()
    {
        RpcFindAndChangeRenderers();
    }

    [ClientRpc]
    void RpcFindAndChangeRenderers()
    {
        FindandAndChangeRendereres();
    }

   
}


