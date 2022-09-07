using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    private bool _inProgress;
    private DateTime _timerStart;
    private DateTime _timerEnd;

    private Coroutine _lastTimer;
    private Coroutine _lastDisplay;
    
    [Header("Production Time")] 
    public int days;
    public int hours;
    public int minutes;
    public int seconds;

    [Header("UI")] 
    [SerializeField] private GameObject window;
    [SerializeField] private GameObject timeLeftObj;
    
    [SerializeField] private TextMeshProUGUI startTimeText;
    [SerializeField] private TextMeshProUGUI endTimeText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    
    [SerializeField] private Slider timeLeftSlider;
    
    [SerializeField] private Button skipButton;
    [SerializeField] private Button startButton;

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartTimer);
        skipButton.onClick.AddListener(Skip);
    }
    
    #endregion
    
    #region UI Methods

    private void InitializeWindow()
    {
        if (_inProgress)
        {
            startTimeText.text = "Start Time : \n" + _timerStart;
            endTimeText.text = "End Time : \n" + _timerEnd;
        
            timeLeftObj.SetActive(true);

            _lastDisplay = StartCoroutine(DisplayTime());
        
            startButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(true);
        }
        else
        {
            startTimeText.text = "Start Time:";
            endTimeText.text = "End Time:";
            
            timeLeftObj.SetActive(false);
        }
        
    }

    private IEnumerator DisplayTime()
    {
        DateTime start = WorldTimeAPI.Instance.GetCurrentDateTime();
        TimeSpan timeLeft = _timerEnd - start;
        double totalSecondsLeft = timeLeft.TotalSeconds;
        double totalSeconds = (_timerEnd - _timerStart).TotalSeconds;
        string text;

        while (window.activeSelf && timeLeftObj.activeSelf)
        {
            text = "";
            timeLeftSlider.value = 1 - Convert.ToSingle((_timerEnd - WorldTimeAPI.Instance.GetCurrentDateTime()).TotalSeconds / totalSeconds);

            if (totalSecondsLeft > 0)
            {
                if (timeLeft.Days != 0)
                {
                    text += timeLeft.Days + "d ";
                    text += timeLeft.Hours + "h";
                    yield return new WaitForSeconds(timeLeft.Minutes * 60);
                }
                else if (timeLeft.Hours != 0)
                {
                    text += timeLeft.Hours + "h ";
                    text += timeLeft.Minutes + "m";
                    yield return new WaitForSeconds(timeLeft.Seconds);
                }
                else if (timeLeft.Minutes != 0)
                {
                    TimeSpan ts = TimeSpan.FromSeconds(totalSecondsLeft);
                    text += ts.Minutes + "m ";
                    text += ts.Seconds + "s";
                }
                else
                {
                    text += Mathf.FloorToInt((float) totalSecondsLeft) + "s";
                }

                timeLeftText.text = text;

                totalSecondsLeft -= Time.deltaTime;
                yield return null;
            }
            else
            {
                timeLeftText.text = "Finished";
                skipButton.gameObject.SetActive(false);
                _inProgress = false;
                break;
            }
        }

        yield return null;
    }

    public void OpenWindow()
    {
        window.SetActive(true);
        
        InitializeWindow();
    }
    public void CloseWindow()
    {
        window.SetActive(false);
    }
    
    #endregion
    
    #region Timed event

    private void StartTimer()
    {
        _timerStart = WorldTimeAPI.Instance.GetCurrentDateTime();
        TimeSpan time = new TimeSpan(days, hours, minutes, seconds);
        _timerEnd = _timerStart.Add(time);
        _inProgress = true;

        _lastTimer = StartCoroutine(Timer());
        
        InitializeWindow();
    }

    private IEnumerator Timer()
    {
        DateTime start = WorldTimeAPI.Instance.GetCurrentDateTime();
        double secondsToFinished = (_timerEnd - start).TotalSeconds;
        yield return new WaitForSeconds(Convert.ToSingle(secondsToFinished));

        _inProgress = false;
        Debug.Log("Finished");
    }

    private void Skip()
    {
        _timerEnd = WorldTimeAPI.Instance.GetCurrentDateTime();
        _inProgress = false;
        StopCoroutine(_lastTimer);

        timeLeftText.text = "Finished";
        timeLeftSlider.value = timeLeftSlider.maxValue;
        
        StopCoroutine(_lastDisplay);
        skipButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
    }
    
    #endregion
    
    
   
}
