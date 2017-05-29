using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class WarningEvent : UnityEvent<List<Transform>> { }

public class CubeManager : MonoBehaviour {

    private static CubeManager _instance;
    public static CubeManager instance
    {
        get
        {
            return _instance;
        }
    }

    public WarningEvent onWarning;
    public UnityEvent onGameOver;

    public List<Cube> list = new List<Cube>();
    public List<Transform> listWarningPos = new List<Transform>();

    private Vector3 positionToCheck;

    #region Lifecycle
    void Start ()
    {
        if (Metronome.instance)
            Metronome.instance.onTimeStart.AddListener(startAction);
        if (GameManager.instance)
        {
            GameManager.instance.onRestart.AddListener(restart);
            GameManager.instance.onClear.AddListener(restart);
            GameManager.instance.onLevelSelect.AddListener(clearList);
        }
        if (ActionManager.instance)
        {
            ActionManager.instance.onArrow.AddListener(checkArrow);
            ActionManager.instance.onSwitch.AddListener(checkSwitch);
            ActionManager.instance.onConveyor.AddListener(checkConveyor);
            ActionManager.instance.onTeleporter.AddListener(checkTeleporter);
            ActionManager.instance.onStop.AddListener(checkStop);
            ActionManager.instance.onExit.AddListener(checkExit);
        }
    }
    void Awake()
    {
        if (_instance != null)
            throw new Exception("Tentative de création d'une autre instance de MonoBehaviorSingleton1 alors que c'est un singleton.");
        _instance = this;

        onGameOver = new UnityEvent();
        onWarning = new WarningEvent();
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
    private void startAction()
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].checkKillZone())
                addWarning(list[i].transform);

            list[i].move();
        }
        if(listWarningPos.Count>0)
        {
            onWarning.Invoke(listWarningPos);
            onGameOver.Invoke();
            listWarningPos.Clear();
        }
    }

    public void listenCube(Cube pCube)
    {
        pCube.onWarningCol.AddListener(addWarning);
    }

    private void addWarning(Transform pTransform)
    {
        listWarningPos.Add(pTransform);
    }

    private void checkExit(Dictionary<Transform, Color> listExit)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            positionToCheck = new Vector3(Mathf.Round(list[i].transform.position.x), Mathf.Round(list[i].transform.position.y), Mathf.Round(list[i].transform.position.z));
            foreach (KeyValuePair<Transform, Color> pair in listExit)
                if (pair.Key.position.ToString() == positionToCheck.ToString() && pair.Value == list[i].GetComponent<Renderer>().materials[1].color)
                    list[i].setModeExit();
        }
    }

    private void checkArrow(List<Transform> listArrow)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            positionToCheck = new Vector3(Mathf.Round(list[i].transform.position.x), Mathf.Round(list[i].transform.position.y), Mathf.Round(list[i].transform.position.z));
            for (int j = listArrow.Count - 1; j >= 0; j--)
            { 
                if (listArrow[j].position.ToString() == positionToCheck.ToString())
                {
                    float arrowRotation = listArrow[j].rotation.ToEulerAngles().y * (180 / Mathf.PI);
                    if (arrowRotation < -179f) arrowRotation = 180f; // Fix probleme de rotation
                    switch ((int)arrowRotation)
                    {
                        case 90:
                            list[i].moveDirection = Vector3.right;
                            break;
                        case -90:
                            list[i].moveDirection = Vector3.left;
                            break;
                        case 180:
                            list[i].moveDirection = Vector3.back;
                            break;
                        case -180:
                            list[i].moveDirection = Vector3.back;
                            break;
                        default:
                            list[i].moveDirection = Vector3.forward;
                            break;
                    }
                    list[i].setAxisAndPivot();
                }
            }
        }
    }

    private void checkSwitch(Dictionary<Transform,bool> listSwitch)
    {
        for (int i = list.Count - 1; i >= 0; i--){
            positionToCheck = new Vector3(Mathf.Round(list[i].transform.position.x), Mathf.Round(list[i].transform.position.y), Mathf.Round(list[i].transform.position.z));

            foreach (KeyValuePair<Transform, bool> pair in listSwitch)
            {
                if (pair.Key.position.ToString() == positionToCheck.ToString())
                {
                    if(pair.Value)
                        list[i].moveDirection = Vector3.Cross(list[i].moveDirection, Vector3.down);
                    else
                        list[i].moveDirection = Vector3.Cross(list[i].moveDirection, Vector3.up);

                    list[i].setAxisAndPivot();
                }


            }
        }
    }

    private void checkConveyor(List<Transform> listConveyor)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            positionToCheck = new Vector3(Mathf.Round(list[i].transform.position.x), Mathf.Round(list[i].transform.position.y), Mathf.Round(list[i].transform.position.z));
            for (int j = listConveyor.Count - 1; j >= 0; j--)
            {
                if (listConveyor[j].position.ToString() == positionToCheck.ToString())
                {
                    float conveyorRotation = listConveyor[j].rotation.ToEulerAngles().y * (180 / Mathf.PI);

                    list[i].transitionDirection = conveyorRotation;
                    list[i].setModeTransition();
                }
            }
        }
    }

    private void checkStop(List<Transform> listStop)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            positionToCheck = new Vector3(Mathf.Round(list[i].transform.position.x), Mathf.Round(list[i].transform.position.y), Mathf.Round(list[i].transform.position.z));
            for (int j = listStop.Count - 1; j >= 0; j--)
                if (listStop[j].position.ToString() == positionToCheck.ToString() && !list[i].wasWaiting)
                    list[i].setModeWait();
        }
    }

    private void checkTeleporter(Dictionary<Transform, Transform> listTeleporter)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            positionToCheck = new Vector3(Mathf.Round(list[i].transform.position.x), Mathf.Round(list[i].transform.position.y), Mathf.Round(list[i].transform.position.z));

            foreach (KeyValuePair<Transform, Transform> pair in listTeleporter)
            {
                if (pair.Key.position.ToString() == positionToCheck.ToString() && list[i].canTeleport)
                    list[i].setModeTeleport(pair.Value.position);
                if (pair.Value.position.ToString() == positionToCheck.ToString() && list[i].canTeleport)
                    list[i].setModeTeleport(pair.Key.position);

                list[i].setAxisAndPivot();
            }
        }
    }
    
    private void restart()
    {
        for (int i = list.Count - 1; i >= 0; i--)
            list[i].destroy();
    }

    private void clearList()
    {
        restart();
        list.Clear();
    }
    #endregion
}
