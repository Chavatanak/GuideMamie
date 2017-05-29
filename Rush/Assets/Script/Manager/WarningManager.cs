using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WarningManager : MonoBehaviour {

    private static WarningManager _instance;
    public static WarningManager instance
    {
        get
        {
            return _instance;
        }
    }

    public List<Warning> list = new List<Warning>();

    #region Lifecycle
    // Use this for initialization
    void Start () {

        if (GameManager.instance)
        {
            GameManager.instance.onRestart.AddListener(clearList);
            GameManager.instance.onClear.AddListener(clearList);
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
    void Update () {

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
    private void clearList()
    {
        for (int i = list.Count - 1; i >= 0; i--)
            list[i].destroy();
    }
    #endregion
}
