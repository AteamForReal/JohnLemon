using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public float gameTimerDuration = 60f; 
    public GameObject player;
    public CanvasGroup exitBackgroundImageCanvasGroup;
    public AudioSource exitAudio;
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    public AudioSource caughtAudio;
    public TextMeshProUGUI timerText;

    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    float m_Timer;
    bool m_HasAudioPlayed;
    float m_GameTimer;

    void Start()
    {
        m_GameTimer = gameTimerDuration; 
        UpdateTimerDisplay(); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
        }
    }

    public void CaughtPlayer()
    {
        m_IsPlayerCaught = true;
    }

    void Update()
    {
        
        if (!m_IsPlayerAtExit && !m_IsPlayerCaught)
        {
            m_GameTimer -= Time.deltaTime;

            if (m_GameTimer <= 0f)
            {
                m_GameTimer = 0f; 
                m_IsPlayerCaught = true; 
            }

            UpdateTimerDisplay(); 
        }

        if (m_IsPlayerAtExit)
        {
            EndLevel(exitBackgroundImageCanvasGroup, false, exitAudio);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel(caughtBackgroundImageCanvasGroup, true, caughtAudio);
        }
    }

    void EndLevel(CanvasGroup imageCanvasGroup, bool doRestart, AudioSource audioSource)
    {
        if (!m_HasAudioPlayed)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }

        m_Timer += Time.deltaTime;
        imageCanvasGroup.alpha = m_Timer / fadeDuration;

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            if (doRestart)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                Application.Quit();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        
        int minutes = Mathf.FloorToInt(m_GameTimer / 60);
        int seconds = Mathf.FloorToInt(m_GameTimer % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}