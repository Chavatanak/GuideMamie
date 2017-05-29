using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class CustomTimer : MonoBehaviour {

    private static CustomTimer _instance;

    /// <summary>
    /// instance unique de la classe     
    /// </summary>
    public static CustomTimer instance
    {
        get
        {
            return _instance;
        }
    }

    public static int elapsedTime { get; set; }
    protected static bool timeCanRun = false;

    #region Lifecycle
    // Use this for initialization
    void Start()
    {
        elapsedTime = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if(timeCanRun)
            elapsedTime++;
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
    /**
     *  Démarre le Timer
     */
    public static void StartTimer()
    {
        timeCanRun = true;
    }

    /**
     *  Arrete le Timer
     */
    public static void StopTimer()
    {
        timeCanRun = false;
    }

    /**
     *  Remet le Timer à zero et le démarre
     */
    public static void ResetAndStartTimer()
    {
        Metronome.instance.timeCount = 0;
        timeCanRun = true;
    }

    /**
     *  Remet le Timer à zero et le démarre
     */
    public static void ResetAndStopTimer()
    {
        Metronome.instance.timeCount = 0;
        timeCanRun = false;
    }
    # endregion

}
