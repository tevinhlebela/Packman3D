using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public static GameController instance;
    public bool gameOver;
    public bool spawnGhost = false, deletedot = false;

    public GameObject ghostPrefab;
    public int ghostCount;

    public GameObject[] dots, walls, ghosts;

    private AudioSource background, endSound;

    private float volume, timer;
    private float xOffset = 0f, zOffset = 0f;

    public Maze mazePrefab;
    private Maze mazeInstance;
    private MeshRenderer mr;

    public Text txtCenter, txtHelp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        gameOver = false;
    }

    void Start () {
        StartCoroutine(StartBox());
        Time.timeScale = 0;
        txtCenter.text = "Press any Key to Begin";
        background = GetComponents<AudioSource>()[0];
        BeginGame();
    }

    private void BeginGame()
    {
        mazeInstance = Instantiate(mazePrefab) as Maze;
        mazeInstance.Generate();
        mazeInstance.name = "mazeInstance";


        dots = GameObject.FindGameObjectsWithTag("dot"); //reduce number of dots
        foreach (GameObject dot in dots)
        {
            if (deletedot == true || (System.Math.Abs(dot.transform.position.x) < 2.5f && System.Math.Abs(dot.transform.position.z) < 2.5f))
            {
                Destroy(dot);
            }
            deletedot = deletedot == true ? false : true;
        }

        walls = GameObject.FindGameObjectsWithTag("mazewallObs");
        foreach (GameObject wall in walls)
        {
            if ((System.Math.Abs(wall.transform.position.x) < 3f && System.Math.Abs(wall.transform.position.z) < 3f) || 
                (System.Math.Abs(wall.transform.position.x) > 18.0f && System.Math.Abs(wall.transform.position.z) > 18.0f))
            {
                Destroy(wall);
            }
        }

        mazeInstance.ToggleMaze();
    }

    public void MakeGhosts()
    {
        Debug.Log("Making ghosts");
        var ghost1 = (GameObject) Instantiate(ghostPrefab, new Vector3(18f, 0, 18f), transform.rotation);
        var ghost2 = (GameObject) Instantiate(ghostPrefab, new Vector3(18f, 0, -18f), transform.rotation);
        var ghost3 = (GameObject) Instantiate(ghostPrefab, new Vector3(-18f, 0, 18f), transform.rotation);
        var ghost4 = (GameObject) Instantiate(ghostPrefab, new Vector3(-18f, 0, -18f), transform.rotation);
    }

    public IEnumerator StartBox()
    {
        while (!Input.anyKey)
        {
            yield return null;
        }
        txtHelp.text = "";
        txtCenter.text = "";
        Time.timeScale = 1;
        MakeGhosts();
        mazeInstance.ToggleMaze();
        GameObject.Find("Pacman").transform.position = new Vector3(0f, 0f, 0f);
    }

    void Update () {
        
        if (GameController.instance.gameOver)
        {
            PlayerDead();
        }

        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameObject.Find("OverheadLight").gameObject.GetComponent<Light>().enabled = false;
            GameObject.Find("Spotlight").gameObject.GetComponent<Light>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerDead();
            //UnityEditor.EditorApplication.isPlaying = false;  //hide for build
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Pause))
        {
            volume = Time.timeScale == 1 ? AudioListener.volume : volume;
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            spawnGhost = spawnGhost == true ? false : true;
            AudioListener.volume = Time.timeScale == 0 ? 0f : volume; 
        }

        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.U))
        {
            GameObject.Find("Pacman").GetComponent<CapsuleCollider>().enabled = false;
        }

        if (timer < Time.time)
        {
            timer = Time.time + 5f;
            spawnGhost = true;
        }

        if (spawnGhost && Time.timeScale == 1)
        {
            ghostCount = GameObject.FindGameObjectsWithTag("ghost").Length;
            if (ghostCount < 10) {
                xOffset = Random.Range(-20f, 20f);
                zOffset = Random.Range(-20f, 20f);
                Vector3 spot = new Vector3(xOffset, 0, zOffset);

                Transform target = GameObject.Find("Pacman").gameObject.transform;
                transform.LookAt(target);

                float distance = Vector3.Distance(spot, target.position);

                if (distance > 9f)
                {
                    var ghostX = (GameObject) Instantiate(ghostPrefab, spot, transform.rotation);
                }
            }
            spawnGhost = false;
        }

    }

    public void MuteBG()
    {
        background.mute = true;
    }

    public void PlayerDead()
    {
        StopAllCoroutines();
        MuteBG();
        spawnGhost = false;
        gameOver = true;
    }

}
