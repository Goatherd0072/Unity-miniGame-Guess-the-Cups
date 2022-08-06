using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager m_PlayerManager = null;
    public Transform FinishText = null;
    public Transform GameOverText = null;
    public Text FScoreText = null;
    public Text ScoreText = null;
    public Text HealthText = null;
    public int Health = 3;
    public int Score = 0;
    public bool IsOver = false;
    public bool IsStart = false;

    private void Awake()
    {
        m_PlayerManager = this;
    }
    void Start()
    {

    }

    void Update()
    {
        UI_Updater();

    }
    private void UI_Updater()
    {
        if (CupManager.m_CupManager.Rest_ball <= 0)
        {
            CupManager.m_CupManager.GameHard++;
            CupManager.m_CupManager.RestData();
            // Time.timeScale = 0;
            // IsOver = true;
            // FinishText.gameObject.SetActive(true);
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Time.timeScale = 1;
            //     IsOver = false;
            //     FinishText.gameObject.SetActive(false);
            //     CupManager.m_Main.Game_Hard++;
            //     CupManager.m_Main.RestData();
            // }

        }

        ScoreText.text = "" + Score;
        HealthText.text = "" + Health;

        if (Health <= 0)
        {
            Time.timeScale = 0;
            IsOver = true;
            GameOverText.gameObject.SetActive(true);
            FinishText.gameObject.SetActive(true);
            ScoreText.transform.gameObject.SetActive(false);
            HealthText.transform.gameObject.SetActive(false);
            FScoreText.text = ScoreText.text;
        }
    }
}
