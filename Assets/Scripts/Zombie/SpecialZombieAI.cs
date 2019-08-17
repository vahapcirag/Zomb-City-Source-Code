using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Pathfinding;

public class SpecialZombieAI : NetworkBehaviour
{
    public Transform targetTransform;
    Path path;
    Seeker seeker;
    GameObject target;
    Rigidbody2D rb;

    public Vector2 path_;

    public Vector2 unNormalizedDirection;

    public bool IsPrepairing = true;

    public float speed = 100f;
    public float nextWayPointDistance = 3f;
    float timeBetweenDamage;

    int currentWayPoint;


    bool reachedEndOfPath;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();


        if (!isServer)
            return;

        InvokeRepeating("FindTarget", 0f, 0.5f);
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }


    void FixedUpdate()
    {
        timeBetweenDamage += Time.deltaTime;

        if (!isServer)
            return;

        Vector2 vel = rb.velocity;

        if (path == null)
            return;

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
            reachedEndOfPath = false;

        path_ = (Vector2)path.vectorPath[currentWayPoint];
        unNormalizedDirection = ((Vector2)path.vectorPath[currentWayPoint] - rb.position);
        Vector2 direction = unNormalizedDirection.normalized;
        Vector2 velocity = direction * speed * Time.deltaTime;

        Debug.Log(((Vector2)targetTransform.position - rb.position).magnitude);

        if (((Vector2)targetTransform.position-rb.position).magnitude>=5)
        rb.velocity = velocity;

        else
        {
            if(!IsPrepairing)
            rb.velocity = velocity * 10;
        }


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance)
            currentWayPoint++;
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    void UpdatePath()
    {
        if (GameObject.FindGameObjectsWithTag("Player") == null)
            return;

        if (seeker.IsDone() && targetTransform != null)
            seeker.StartPath(rb.position, (Vector2)targetTransform.position, OnPathComplete);
    }

    void FindTarget()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
            return;


        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        for (int i = 0; i < players.Length; i++)
        {
            if (i == 0)
            {
                target = players[i];
            }
            else
            {
                float distance_1 = Vector2.Distance(rb.position, (Vector2)players[i].transform.position);
                float distance_2 = Vector2.Distance(rb.position, (Vector2)target.transform.position);

                if (distance_1 < distance_2)
                    target = players[i];
            }
        }

        targetTransform = target.transform;
    }
}
