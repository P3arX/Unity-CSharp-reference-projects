using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject pipePrefab;

    private int score;

    private bool canRestart = true;

    [SerializeField]
    private GameObject menuUI, retryUI;

    [SerializeField]
    private Text scoreText, highscoreText;

    [SerializeField]
    private BirdController bird;

    private void Awake()
    {
        pipePrefab = Resources.Load(GameData.PIPE_PREFAB_PATH) as GameObject;
    }

    private void Start()
    {
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt(GameData.HIGHSCORE_SAVE_KEY);
    }

    private void Update()
    {
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && (menuUI.activeSelf || retryUI.activeSelf) && canRestart)
            StartGame();

        scoreText.text = "Score: " + score;
    }

    private void SpawnPipe()
    {
        Vector2 spawnPoint = new Vector2(10, Random.Range(-2f, 2f));
        Instantiate(pipePrefab, spawnPoint, Quaternion.identity);
    }

    private void StopPipes()
    {
        GameObject[] pipes = GameObject.FindGameObjectsWithTag(GameData.PIPE_TAG);
        foreach (var pipe in pipes)
            pipe.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void DestroyPipes()
    {
        GameObject[] pipes = GameObject.FindGameObjectsWithTag(GameData.PIPE_TAG);

        if(pipes.Length > 0)
            foreach (var pipe in pipes)
                Destroy(pipe);
    }

    private void SaveHighscore(int potentiallyNewHighscore)
    {
        if(PlayerPrefs.GetInt(GameData.HIGHSCORE_SAVE_KEY) < potentiallyNewHighscore)
        {
            PlayerPrefs.SetInt(GameData.HIGHSCORE_SAVE_KEY, potentiallyNewHighscore);
            highscoreText.text = "Highscore: " + potentiallyNewHighscore;
        }
    }

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(1);
        canRestart = true;
    }

    private void StartGame()
    {
        canRestart = false;
        DestroyPipes();
        menuUI.SetActive(false);
        retryUI.SetActive(false);
        scoreText.gameObject.SetActive(true);
        score = 0;
        InvokeRepeating("SpawnPipe", 0f, 2f - (score / 50));
        bird.Init();
    }

    public void StopGame()
    {
        CancelInvoke();
        StopPipes();
        SaveHighscore(score);
        retryUI.SetActive(true);
        StartCoroutine(DeathTimer());
    }

    public void IncreaseScore()
    {
        score++;
        Debug.Log("Increased score");
    }
}
