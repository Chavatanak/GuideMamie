using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpawnerManager : MonoBehaviour {

    private static SpawnerManager _instance;

    /// <summary>
    /// instance unique de la classe     
    /// </summary>
    public static SpawnerManager instance
    {
        get
        {
            return _instance;
        }
    }

    public List<Spawner> list = new List<Spawner>();
    public static uint generalSpawnCount;

    #region Lifecycle
    // Use this for initialization
    void Start ()
    {
        if (Metronome.instance)
            Metronome.instance.onTime.AddListener(checkAction);
        if (GameManager.instance)
        {
            GameManager.instance.onRestart.AddListener(restart);
            GameManager.instance.onClear.AddListener(restart);
            GameManager.instance.onResolve.AddListener(stopFeedBack);
            GameManager.instance.onLevelSelect.AddListener(clearList);
        }
    }

    void Awake()
    {
        if (_instance != null)
            throw new Exception("Tentative de création d'une autre instance de MonoBehaviorSingleton1 alors que c'est un singleton.");
        _instance = this;
    }

    // Update is called once per frame
    void Update ()
    {

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
    private void checkAction(int pTime)
    {
        generalSpawnCount = 0;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            generalSpawnCount += list[i].spawnCount;
            if ((pTime % list[i].spawnRate == 0 || pTime == 0) && list[i].spawnCount != 0)
            {
                list[i].generateCube();
                list[i].spawnCount--;
            }
        }
    }

    private void restart()
    {
        startFeedBack();
        for (int i = list.Count - 1; i >= 0; i--)
            list[i].spawnCount = list[i].spawnNumber;
    }

    private void clearList()
    {
        list.Clear();
    }


    private void stopFeedBack()
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            list[i].transform.GetChild(1).GetComponent<ParticleSystem>().emissionRate = 0f;
            list[i].transform.GetChild(1).GetComponent<ParticleSystem>().Clear();
        }
    }
    private void startFeedBack()
    {
        for (int i = list.Count - 1; i >= 0; i--)
            list[i].transform.GetChild(1).GetComponent<ParticleSystem>().emissionRate = 1.5f;
    }
    #endregion
}
