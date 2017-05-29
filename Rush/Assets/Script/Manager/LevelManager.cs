using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    private static LevelManager _instance;
    public static LevelManager instance
    {
        get
        {
            return _instance;
        }
    }

    private static GameObject activeLevel;
    #region Lifecycle
    void Start ()
    {
    }

    void Awake()
    {
        if (_instance != null)
            throw new Exception("Tentative de création d'une autre instance de MonoBehaviorSingleton1 alors que c'est un singleton.");
        _instance = this;
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
    public void openLevel(string levelName)
    {
        if (activeLevel)
            Destroy(activeLevel);
        activeLevel = (GameObject)Instantiate(Resources.Load("Level/"+ levelName));
        activeLevel.transform.parent = GameObject.Find("LevelContain").transform;
    }

    public void destroyLevel()
    {
        GameManager.instance.restart();
        if (activeLevel)
            Destroy(activeLevel);
    }
    #endregion
}
