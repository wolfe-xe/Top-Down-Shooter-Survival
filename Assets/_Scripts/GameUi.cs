using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameUi : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public RectTransform healthBar;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text gameOverScoreUI;
    Spawn spawner;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawn>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = Score.score.ToString("D6");
        float healthPercent = 0;
        if(player != null)
        {
            healthPercent = player.health / player.startingHP;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber)
    {
        string[] number = { "ONE", "TWO", "THREE", "FOUR", "FIVE" };
        newWaveTitle.text = "- WAVE " + number[waveNumber - 1] + " -";
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StartCoroutine(AnimateNewWaveBanner());

        /*StartCoroutine("AnimateNewWaveBanner");
        StopCoroutine("AnimateNewWaveBanner");*/
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.black, new Color(0,0,0,1), 1));
        gameOverScoreUI.text = scoreUI.text;
        //scoreUI.gameObject.SetActive(false);
        //healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }


    IEnumerator AnimateNewWaveBanner()
    {
        float animatePercent = 0;
        float speed = 2.5f;
        float delayTime = 1.5f;
        int dir = 1;

        float endDelayTime = Time.time + 1/speed + delayTime;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if(animatePercent >=1)
            {
                animatePercent = 1;
                if(Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-150, 0, animatePercent);
            yield return null;
        }

    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) 
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);

            yield return null;
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
