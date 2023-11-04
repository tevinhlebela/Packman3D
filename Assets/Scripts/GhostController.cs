using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostController : MonoBehaviour
{
    private Rigidbody rb;

    private float speed, timer, updateTimer = 1f;

    public Transform target;
    private NavMeshAgent agent, targetAgent;
    private Renderer tr;
    private Tree tree;

    //public Material[] mats = new Material[10];

    public AudioSource ghostDie, ghostSpawn;
    public bool playOnce = true;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.Warp(gameObject.transform.position);  //make sure it's on the mesh
        agent.speed = Random.Range(5f, 20f);

        target = GameObject.Find("Pacman").transform;
        targetAgent = GameObject.Find("Pacman").GetComponent<NavMeshAgent>();

        timer = Time.time + 8f;

        ghostDie = GameObject.Find("GhostSound").gameObject.GetComponent<AudioSource>();
        ghostSpawn = GameObject.Find("GhostSpawn").gameObject.GetComponent<AudioSource>();    //clones won't play attached audio
        ghostSpawn.Play();

        tree = gameObject.GetComponentInChildren<Tree>();
        tr = tree.GetComponent<Renderer>();

        //Debug.Log(gameObject.name + " tr.material: " + tr.material);
        //Debug.Log("mats: " + mats.Length);
        //nothing works to change material
        //tr.material = mats[0];
        //tr.materials = mats;
        //tr.material = tr.materials[9];

        tr.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    void FixedUpdate()
    {
        if (timer < Time.time)
        {
            rb.transform.localScale *= Random.Range(.9f, .95f);
            agent.speed *= rb.transform.localScale.y;

            if (agent.speed < 2f && playOnce)
            {
                gameObject.GetComponent<SphereCollider>().enabled = false;
                playOnce = agent.enabled = false;
                ghostDie.Play();
                rb.isKinematic = false;
                rb.angularVelocity = new Vector3(Random.Range(60f, 120f), 0f, Random.Range(60f, 120f));
                Destroy(gameObject, 4f);
            }
            else if (playOnce)
            {
                timer = Time.time + Random.Range(5f, 8f);
                //Debug.Log(this.name + " " + transform.position + " " + transform.localScale + " " + transform);
            }
        }

        if (updateTimer < Time.time && targetAgent.isActiveAndEnabled && agent.isActiveAndEnabled)
        {
            agent.destination = target.position;
            updateTimer = Time.time + Random.Range(.1f,.2f);
        }
    }

}
