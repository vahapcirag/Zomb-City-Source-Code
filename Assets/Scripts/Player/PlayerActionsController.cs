using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerActionsController : NetworkBehaviour
{
    List<GameObject> guns;

    [SerializeField] public GameObject activeGun;
    [SerializeField] GameObject bulletGO;

    [SerializeField] public Transform bulletRefTransform;

    [SerializeField] public GunValues values;

    [SerializeField] Bullet bullet;

    [SerializeField] Rigidbody2D rb;

    [SyncVar] public float x, y;

    bool whichBaretta = false;

    float timeBetweenBullets = 0;


    bool check = false;
    private void Start()
    {
        Invoke("FindActiveGun", 0.1f);
    }

    void Update()
    {
        timeBetweenBullets += Time.deltaTime;

        if (!hasAuthority)
            return;
        float xx, yy;

        xx = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        yy = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        CmdMousePos(xx, yy);


        ChangeGun();

        if (activeGun != null && (timeBetweenBullets >= values.specs.gunTimeBetweenBullets) && (values.specs.gunBulletCapacity >= 0) && !values.specs.gunIsReloading)
        {
            if (values.specs.gunIsFullAutomatic && Input.GetButton("Fire1"))
            {
                CmdSpawnBullet();
                timeBetweenBullets = 0;
            }
            else if (!values.specs.gunIsFullAutomatic && Input.GetButtonDown("Fire1"))
            {

                CmdSpawnBullet();
                timeBetweenBullets = 0;
            }

        }

    }

    [Command]
    void CmdMousePos(float x_, float y_)
    {
        x = x_;
        y = y_;
    }

    [ClientRpc]
    void RpcMousePos()
    {

    }

    [Command]
    void CmdSpawnGun()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Guns/Guitar"), gameObject.transform, false);
        NetworkServer.Spawn(go);
        RpcSetGunTransform(go);

    }
    [ClientRpc]
    void RpcSetGunTransform(GameObject go)
    {
        int removeIndex=0;

        for (int i = 0; i < go.name.Length; i++)
        {
            if (go.name[i] == '(')
            {
                removeIndex = i;
                break;

            }
        }

        go.name = go.name.Remove(removeIndex);

        go.transform.parent = gameObject.transform;
        go.transform.localPosition = Resources.Load<GameObject>("Guns/" + go.name).transform.position;
        go.transform.localScale = Resources.Load<GameObject>("Guns/" + go.name).transform.lossyScale;
    }

    [Command]
    public void CmdSpawnBullet()
    {
        if (values.specs.gunName != "Baretta")
            bulletGO = Instantiate(Resources.Load<GameObject>("Bullet"), bulletRefTransform.position, Quaternion.identity);
        else
        {
            if (whichBaretta)
            {
                bulletRefTransform = activeGun.transform.GetChild(1).transform;
                whichBaretta = false;
            }
            else
            {
                bulletRefTransform = activeGun.transform.GetChild(2).transform;
                whichBaretta = true;
            }
            bulletGO = Instantiate(Resources.Load<GameObject>("Bullet"), bulletRefTransform.position, Quaternion.identity);

        }

        NetworkServer.Spawn(bulletGO);
        CmdGiveShapeToBullet(bulletGO);
    }


    [Command]
    public void CmdGiveShapeToBullet(GameObject b)
    {

        RpcGiveShapeToBullet(b);
    }

    [ClientRpc]
    public void RpcGiveShapeToBullet(GameObject b)
    {

        Vector2 direction;
        Vector2 mousePos = new Vector2(x, y);
        bulletGO = b;
        rb = bulletGO.GetComponent<Rigidbody2D>();
        direction = mousePos - (Vector2)bulletRefTransform.position;

        if (mousePos.x > bulletRefTransform.position.x)
        {
            bulletGO.transform.Rotate(Vector3.forward, (Mathf.Atan(direction.y / direction.x) * 57.2958f));
        }
        else
            bulletGO.transform.Rotate(Vector3.forward, (Mathf.Atan(direction.y / direction.x) * 57.2958f) + 180);
        rb.velocity = direction.normalized * values.specs.bulletSpeed;

        if (activeGun.name == "Guitar")
            bulletGO.transform.localScale = new Vector3(.2f, .2f, .2f);

        bullet = bulletGO.GetComponent<Bullet>();
        bullet.player = gameObject;
        bullet.damage = values.specs.gunDamage;

        bulletGO.GetComponent<SpriteRenderer>().sprite = values.specs.gunBulletSprite;
    }



    void FindActiveGun()
    {
        if (!hasAuthority)
            return;

        CmdFindActiveGun(4);
    }
    [Command]
    void CmdFindActiveGun(int a)
    {

        RpcFindActiveGun(a);
    }

    [ClientRpc]
    void RpcFindActiveGun(int a)
    {
        activeGun = gameObject.transform.GetChild(4).gameObject;

        values = activeGun.GetComponent<GunValues>();
        bulletRefTransform = activeGun.transform.GetChild(1).transform;
    }

    void ChangeGun()
    {
        if (Input.GetButtonDown("FirstGun"))
        {
            if (!transform.GetChild(4).gameObject.activeSelf)
                CmdChangeGun(1);
        }
        else if (Input.GetButtonDown("SecondGun"))
        {
            if (!transform.GetChild(5).gameObject.activeSelf)
                CmdChangeGun(2);
        }

        else if (Input.GetButtonDown("ThirdGun"))
        {
            if (!transform.GetChild(6).gameObject.activeSelf)
                CmdChangeGun(3);
        }
    }

    [Command]
    void CmdChangeGun(int childIndex)
    {
        RpcChangeGun(childIndex);
    }

    [ClientRpc]
    void RpcChangeGun(int childIndex)
    {
        switch (childIndex)
        {
            case 1:

                transform.GetChild(4).gameObject.SetActive(true);

                activeGun = transform.GetChild(4).gameObject;
                values = activeGun.GetComponent<GunValues>();
                bulletRefTransform = activeGun.transform.GetChild(1).transform;

                if (transform.childCount >= 6)
                    transform.GetChild(5).gameObject.SetActive(false);
                if (transform.childCount >= 7)
                    transform.GetChild(6).gameObject.SetActive(false);

                break;

            case 2:
                if (transform.childCount < 6)
                    return;

                transform.GetChild(5).gameObject.SetActive(true);

                activeGun = transform.GetChild(5).gameObject;
                values = activeGun.GetComponent<GunValues>();
                bulletRefTransform = activeGun.transform.GetChild(1).transform;

                transform.GetChild(4).gameObject.SetActive(false);

                if (transform.childCount >= 7)
                    transform.GetChild(6).gameObject.SetActive(false);
                break;

            case 3:
                if (transform.childCount < 7)
                    return;

                transform.GetChild(6).gameObject.SetActive(true);

                activeGun = transform.GetChild(6).gameObject;
                values = activeGun.GetComponent<GunValues>();
                bulletRefTransform = activeGun.transform.GetChild(1).transform;

                transform.GetChild(4).gameObject.SetActive(false);

                if (transform.childCount >= 6)
                    transform.GetChild(5).gameObject.SetActive(false);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasAuthority)
            return;
        int gunCount = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == "Gun")
                gunCount++;
        }
        Debug.Log(gunCount);
        if (collision.gameObject.tag == "Gun" && (gunCount<3))
        {
            CmdSetParent(collision.gameObject);
            
        }
    }

    [Command]
    void CmdSetParent(GameObject go)
    {
        RpcSetParent(go);
        RpcSetGunTransform(go );
    }
    [ClientRpc]
    void RpcSetParent(GameObject go)
    {
        activeGun.SetActive(false);
        go.transform.parent = transform;
        activeGun = go;
        values = go.GetComponent<GunValues>();
        go.transform.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        bulletRefTransform = activeGun.transform.GetChild(1).transform;
    }
}
