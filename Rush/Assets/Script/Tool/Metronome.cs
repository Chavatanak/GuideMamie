using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class TimerEvent : UnityEvent<int>
{

}
public class Metronome : MonoBehaviour
{

    private static Metronome _instance;
    /// <summary>
    /// instance unique de la classe     
    /// </summary>
    public static Metronome instance
    {
        get
        {
            return _instance;
        }
    }
    
    public Slider sliderTempo;

    public UnityEvent onTimeStart;
    public TimerEvent onTime;
    public static float progressTime { get; private set; }
    public int timeCount;
    public int tempo = 40;
    private int newTempo;

    #region Lifecycle
    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        if (_instance != null)
            throw new Exception("Tentative de création d'une autre instance de MonoBehaviorSingleton1 alors que c'est un singleton.");
        _instance = this;
        timeCount = 0;
        onTime = new TimerEvent();
        onTimeStart = new UnityEvent();

        newTempo = tempo;
    }

    // Update is called once per frame
    void Update()
    {
        if (CustomTimer.elapsedTime == tempo)
        {
            tempo = newTempo;
            CustomTimer.elapsedTime = 0;
            onTimeStart.Invoke();
            onTime.Invoke(timeCount);
            timeCount++;
        }
        progressTime = (float)CustomTimer.elapsedTime / tempo;
    }

    public void Dispose()
    {
        _instance = null;
    }

    protected void OnDestroy()
    {
        _instance = null;
    }
    #endregion

    #region Methods
    public void setTempo()
    {
        newTempo = (int)sliderTempo.value;
    }
    #endregion
}