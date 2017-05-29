using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;

public class RemoveActionEvent : UnityEvent<Transform> { }

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    /// <summary>
    /// instance unique de la classe     
    /// </summary>
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }
    
    [System.NonSerialized] public UnityEvent onRestart;
    [System.NonSerialized] public UnityEvent onClear;
    [System.NonSerialized] public UnityEvent onHome;
    [System.NonSerialized] public UnityEvent onPlay;
    [System.NonSerialized] public UnityEvent onResolve;
    [System.NonSerialized] public UnityEvent onMenu;
    [System.NonSerialized] public UnityEvent onLevelSelect;
    [System.NonSerialized] public UnityEvent onWin;
    [System.NonSerialized] public UnityEvent onGameOver;

    public delegate void State();
    public State gameState;

    public RemoveActionEvent onRemoveAction;

    // Mouse
    Ray ray;
    LayerMask groundMask;
    LayerMask actionMask;
    LayerMask staticActionMask;
    RaycastHit hit;

    // Item in Hand
    private static GameObject itemInHand;
    public static float rotationInHand;

    #region Lifecycle
    void Start()
    {
        groundMask = 1 << LayerMask.NameToLayer("ground");
        actionMask = 1 << LayerMask.NameToLayer("action");
        staticActionMask = 1 << LayerMask.NameToLayer("staticaction");
        rotationInHand = 0f;

        MusicLoopsManager.manager.StartMusic(MusicType.menuMusic);

        if (Metronome.instance)
            Metronome.instance.onTime.AddListener(checkWin);

        if (CubeManager.instance)
        {
            CubeManager.instance.onGameOver.AddListener(setModeGameOver);
            CubeManager.instance.onWarning.AddListener(setWarning);
        }
    }
    void Awake () {
        if (_instance != null)
            throw new Exception("Tentative de création d'une autre instance de MonoBehaviorSingleton1 alors que c'est un singleton.");
        _instance = this;

        gameState = stateHome;

        onRestart = new UnityEvent();
        onClear = new UnityEvent();

        onHome = new UnityEvent();
        onPlay = new UnityEvent();
        onResolve = new UnityEvent();
        onMenu = new UnityEvent();
        onLevelSelect = new UnityEvent();
        onWin = new UnityEvent();
        onGameOver = new UnityEvent();

        onRemoveAction = new RemoveActionEvent();
    }
    
    void Update ()
    {
        gameState();
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

    #region State Machine

        #region Menu
        public void setModeHome()
        {
            gameState = stateHome;
            CustomTimer.StopTimer();
            onHome.Invoke();
        }
        protected void stateHome()
        {

        }
        #endregion

        #region LevelSelect
        public void setLevelSelect()
        {
            gameState = stateLevelSelect;
            CustomTimer.StopTimer();
            MusicLoopsManager.manager.PlayMusic(MusicType.menuMusic);
            LevelManager.instance.destroyLevel();
            onLevelSelect.Invoke();
        }
        protected void stateLevelSelect()
        {

        }
        #endregion

        #region Play
        public void setModePlay()
        {
            gameState = statePlay;
            clear();
            CustomTimer.ResetAndStopTimer();

            MusicLoopsManager.manager.PlayMusic(MusicType.thinkMusic);
            onPlay.Invoke();
        }
        protected void statePlay()
        {
            checkGroundToPlace();
            checkActionToRemove();
            if (Input.GetKeyDown("space")) ActionManager.instance.restart();
        }
        #endregion

        #region Resolve
        public void setModeResolve()
        {
            gameState = stateResolve;

            if (itemInHand != null)
                Destroy(itemInHand);

            CustomTimer.StartTimer();

            MusicLoopsManager.manager.PlayMusic(MusicType.solveMusic);
            onResolve.Invoke();
        }
        protected void stateResolve()
        {

        }
        #endregion

        #region Menu
        public void setMenu()
        {
            gameState = stateMenu;
            CustomTimer.StopTimer();
            onMenu.Invoke();
        }
        protected void stateMenu()
        {

        }
        #endregion
    
        #region Pause
        public void setModePause()
            {
                gameState = statePause;
                CustomTimer.StopTimer();
            }
        protected void statePause()
        {

        }
        #endregion

        #region Victory
        public void setModeVictory()
        {
            gameState = stateVictory;
            CustomTimer.StopTimer();
            SfxManager.manager.PlaySfx("SFX_JingleVictoire");
            onWin.Invoke();
        }
        protected void stateVictory()
        {
        }
        #endregion

        #region GameOver
        public void setModeGameOver()
        {
            gameState = stateGameOver;
            CustomTimer.StopTimer();
            SfxManager.manager.PlaySfx("SFX_JingleDefaite");
            onGameOver.Invoke();
        }
        protected void stateGameOver()
        {

        }
        #endregion

    #endregion

    #region Methods
    private void checkWin(int pTime)
    {
        if (ActionManager.canWin && SpawnerManager.generalSpawnCount == 0 && CubeManager.instance.list.Count == 0)
            setModeVictory();
    }

    // supprime cube, warning, actions et reinitialise spawner
    public void restart()
    {
        if (itemInHand != null)
            Destroy(itemInHand);

        CustomTimer.ResetAndStartTimer();
        onRestart.Invoke();
    }

    // supprime cube, warning et reinitialise spawner
    public void clear()
    {
        if (itemInHand != null)
            Destroy(itemInHand);

        CustomTimer.ResetAndStartTimer();
        onClear.Invoke();
    }

    public void createLevel(string levelName)
    {
        CustomTimer.ResetAndStartTimer();
        LevelManager.instance.openLevel(levelName);
        setModePlay();
    }

    public void setWarning(List<Transform> pList)
    {
        for (int i = pList.Count - 1; i >= 0; i--)
        {
            GameObject GoWarning = (GameObject)Instantiate(Resources.Load("Prefab/Warning"));
            GoWarning.transform.position = pList[i].position;
            GoWarning.transform.parent = GameObject.Find("Action").transform;
        }
    }

    private static GameObject activeBtn;
    public void grabItemToPlace(GameObject pBouton)
    {
        SfxManager.manager.PlaySfx("SFX_SelecFleche");

        if (itemInHand != null)
        {
            UIManager.instance.checkBtnToIncremente(itemInHand.transform.eulerAngles.y, itemInHand.name.Remove(itemInHand.name.Length - 7));
            cleanItemInHandClass(); 
        }
        if (int.Parse(pBouton.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text) > 0)
        {
            if (itemInHand == null)
            {

                UIManager.instance.decrementBouton(pBouton);
                activeBtn = pBouton;
                itemInHand = (GameObject)Instantiate(Resources.Load("Prefab/Action/" + pBouton.name.Remove(pBouton.name.Length - 7).Substring(2)));
                itemInHand.transform.position = Vector3.zero;
                itemInHand.transform.rotation = Quaternion.Euler(new Vector3(0, rotationInHand, 0));
                itemInHand.layer = 0;
                itemInHand.transform.GetChild(0).gameObject.layer = 0;
            }
        }
    }
    
    private void checkGroundToPlace()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, groundMask) && itemInHand != null)
        {
            itemInHand.transform.position = hit.transform.position + Vector3.up*1.5f;
            itemInHand.transform.SetParent(GameObject.Find("Action").transform);

            if (Input.GetMouseButtonDown(0) 
                && !Physics.Raycast(hit.transform.position, Vector3.up, 1f, groundMask) 
                && !Physics.Raycast(hit.transform.position, Vector3.up, 0.51f, actionMask)
                && !Physics.Raycast(hit.transform.position, Vector3.up, 1.01f, actionMask)
                && !Physics.Raycast(hit.transform.position, Vector3.up, 0.51f, staticActionMask))
            {
                GameObject GoAction = (GameObject)Instantiate(Resources.Load("Prefab/Action/" + itemInHand.name.Remove(itemInHand.name.Length - 7))); ;
                GoAction.transform.position = hit.transform.position + Vector3.up;
                GoAction.transform.rotation = Quaternion.Euler(new Vector3(0, rotationInHand, 0));
                GoAction.transform.parent = GameObject.Find("Action").transform;

                if(int.Parse(activeBtn.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text) > 0)
                {
                    SfxManager.manager.PlaySfx("SFX_SelecFleche");
                    UIManager.instance.decrementBouton(activeBtn);
                }
                else
                    cleanItemInHandClass();
            }
        }
    }

    private void checkActionToRemove()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, actionMask) && Input.GetMouseButtonDown(0) && itemInHand == null)
            onRemoveAction.Invoke(hit.transform);
    }

    private void cleanItemInHandClass()
    {
        Destroy(itemInHand);
        switch (itemInHand.name.Remove(itemInHand.name.Length - 7))
        {
            case "Arrow":
                ActionManager.instance.listArrow.Remove(itemInHand.GetComponent<Arrow>());
                break;
            case "Conveyor":
                ActionManager.instance.listConveyor.Remove(itemInHand.GetComponent<Conveyor>());
                break;
            case "Stop":
                ActionManager.instance.listStop.Remove(itemInHand.GetComponent<Stop>());
                break;
            case "Switch":
                ActionManager.instance.listSwitch.Remove(itemInHand.GetComponent<Switch>());
                break;
            default:
                break;
        }
    }
    #endregion
}
