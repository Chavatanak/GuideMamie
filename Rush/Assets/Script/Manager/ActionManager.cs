using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class ArrowEvent : UnityEvent<List<Transform>>{ }
public class SwitchEvent : UnityEvent<Dictionary<Transform, bool>> { }
public class ConveyorEvent : UnityEvent<List<Transform>> { }
public class TeleporterEvent : UnityEvent<Dictionary<Transform, Transform>> { }
public class StopEvent : UnityEvent<List<Transform>> { }
public class ExitEvent : UnityEvent<Dictionary<Transform, Color>> { }

public class RemoveItemEvent : UnityEvent<float, string> { }

public class ActionManager : MonoBehaviour {

    private static ActionManager _instance;
    public static ActionManager instance
    {
        get
        {
            return _instance;
        }
    }

    public ArrowEvent onArrow;
    public SwitchEvent onSwitch;
    public ConveyorEvent onConveyor;
    public TeleporterEvent onTeleporter;
    public StopEvent onStop;
    public ExitEvent onExit;

    public RemoveItemEvent onRemoveItem;

    public List<Arrow> listArrow = new List<Arrow>();
    public List<Switch> listSwitch = new List<Switch>();
    public List<Conveyor> listConveyor = new List<Conveyor>();
    public List<Teleporter> listTeleporter = new List<Teleporter>();
    public List<Stop> listStop = new List<Stop>();
    public List<Exit> listExit = new List<Exit>();

    private List<Transform> listArrowToSend = new List<Transform>();
    private Dictionary<Transform, bool> listSwitchToSend = new Dictionary<Transform, bool>();
    private List<Transform> listConveyorToSend = new List<Transform>();
    private Dictionary<Transform, Transform> listTeleporterToSend = new Dictionary<Transform, Transform>();
    private List<Transform> listStopToSend = new List<Transform>();
    private Dictionary<Transform, Color> listExitToSend = new Dictionary<Transform, Color>();

    public static bool canWin = false;

    #region Lifecycle
    void Start()
    {
        if (Metronome.instance)
            Metronome.instance.onTime.AddListener(checkAction);
        if (GameManager.instance)
        {
            GameManager.instance.onRestart.AddListener(restart);
            GameManager.instance.onLevelSelect.AddListener(clearList);
            GameManager.instance.onRemoveAction.AddListener(removeItem);
        }
    }
    void Awake ()
    {
        if (_instance != null)
            throw new Exception("Tentative de création d'une autre instance de MonoBehaviorSingleton1 alors que c'est un singleton.");
        _instance = this;

        onArrow = new ArrowEvent();
        onSwitch = new SwitchEvent();
        onConveyor = new ConveyorEvent();
        onTeleporter = new TeleporterEvent();
        onStop = new StopEvent();
        onExit = new ExitEvent();

        onRemoveItem = new RemoveItemEvent();
    }
    #endregion


    #region Methods
        public void checkAction(int pTime)
        {
            checkArrow();
            checkSwitch();
            checkConveyor();
            checkTeleporter();
            checkStop();
            checkExit();
        }

        #region Methods
        public void checkArrow()
            {
                for (int i = listArrow.Count - 1; i >= 0; i--)
                    if (listArrow[i].checkIn() != null)
                        listArrowToSend.Add(listArrow[i].checkIn());

                if (listArrowToSend.Count > 0)
                {
                    onArrow.Invoke(listArrowToSend);
                    listArrowToSend.Clear();
                }
            }
        public void checkSwitch()
        {
            for (int i = listSwitch.Count - 1; i >= 0; i--)
                if (listSwitch[i].checkIn() != null)
                {
                    listSwitchToSend.Add(listSwitch[i].checkIn(), listSwitch[i].switched);
                    listSwitch[i].switched = !listSwitch[i].switched;
                }

            if (listSwitchToSend.Count > 0)
            {
                onSwitch.Invoke(listSwitchToSend);
                listSwitchToSend.Clear();
            }
        }
        public void checkConveyor()
        {
            for (int i = listConveyor.Count - 1; i >= 0; i--)
                if (listConveyor[i].checkIn() != null)
                    listConveyorToSend.Add(listConveyor[i].checkIn());

            if (listConveyorToSend.Count > 0)
            {
                onConveyor.Invoke(listConveyorToSend);
                listConveyorToSend.Clear();
            }
        }
        public void checkTeleporter()
        {
            for (int i = listTeleporter.Count - 1; i >= 0; i--)
                if (listTeleporter[i].checkBoth() != null)
                    listTeleporterToSend.Add(listTeleporter[i].checkBoth()[0], listTeleporter[i].checkBoth()[1]);

            if (listTeleporterToSend.Count > 0)
            {
                onTeleporter.Invoke(listTeleporterToSend);
                listTeleporterToSend.Clear();
            }
        }
        public void checkStop()
        {
            for (int i = listStop.Count - 1; i >= 0; i--)
                if (listStop[i].checkIn() != null)
                    listStopToSend.Add(listStop[i].checkIn());

            if (listStopToSend.Count > 0)
            {
                onStop.Invoke(listStopToSend);
                listStopToSend.Clear();
            }
        }
        public void checkExit()
        {
            for (int i = listExit.Count - 1; i >= 0; i--)
                if (listExit[i].checkIn() != null)
                    listExitToSend.Add(listExit[i].checkIn(), listExit[i].currentColor);

            if (listExitToSend.Count > 0)
            {
                onExit.Invoke(listExitToSend);
                listExitToSend.Clear();
                canWin = true;
            }
        }
        #endregion

        #region Clean
        public void removeItem(Transform pTarget)
                {
                    for (int i = listArrow.Count - 1; i >= 0; i--)
                        if (listArrow[i].transform.position == pTarget.transform.position)
                        {
                            listArrow[i].destroy();
                            onRemoveItem.Invoke(pTarget.transform.eulerAngles.y, "Arrow");
                            return;
                        }

                    for (int i = listConveyor.Count - 1; i >= 0; i--)
                        if (listConveyor[i].transform.position == pTarget.transform.position)
                        {
                            listConveyor[i].destroy();
                            onRemoveItem.Invoke(pTarget.transform.eulerAngles.y, "Conveyor");
                            return;
                        }

                    for (int i = listStop.Count - 1; i >= 0; i--)
                        if (listStop[i].transform.position == pTarget.transform.position)
                        {
                            listStop[i].destroy();
                            onRemoveItem.Invoke(pTarget.transform.eulerAngles.y, "Stop");
                            return;
                        }

                    for (int i = listTeleporter.Count - 1; i >= 0; i--)
                        if(listTeleporter[i].transform.position == pTarget.transform.position)
                        {
                            listTeleporter[i].destroy();
                            onRemoveItem.Invoke(pTarget.transform.eulerAngles.y, "Teleporter");
                            return;
                        }

                    for (int i = listSwitch.Count - 1; i >= 0; i--)
                        if (listSwitch[i].transform.position == pTarget.transform.position)
                        {
                            listSwitch[i].destroy();
                            onRemoveItem.Invoke(pTarget.transform.eulerAngles.y, "Switch");
                        }
                }
        public void restart()
            {
                int i = 0;
                for (i = listArrow.Count - 1; i >= 0; i--)
                    listArrow[i].destroy();

                for (i = listSwitch.Count - 1; i >= 0; i--)
                    listSwitch[i].destroy();

                for (i = listConveyor.Count - 1; i >= 0; i--)
                    listConveyor[i].destroy();

                for (i = listStop.Count - 1; i >= 0; i--)
                    listStop[i].destroy();
        
                canWin = false;

                UIManager.instance.resetActionBouton();
            }
        private void clearList()
            {
                listTeleporter.Clear();
                listExit.Clear();
                restart();
        }
        #endregion
    #endregion
}
