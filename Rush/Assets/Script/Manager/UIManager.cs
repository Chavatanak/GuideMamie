using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class btnOrientationEvent : UnityEvent<float> { }


public struct ItemData
{
    public GameObject bouton;
    public float orientation;
    public int number;
}

public class UIManager : MonoBehaviour {

    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            return _instance;
        }
    }

    private static RectTransform activePanel;
    public RectTransform Home;
    public RectTransform Hud;
    public RectTransform HudReflexion;
    public RectTransform Menu;
    public RectTransform LevelSelect;
    public RectTransform Win;
    public RectTransform GameOver;
    
    public List<ItemData> list = new List<ItemData>();
    private List<int> listCounter = new List<int>();

    public Camera guiCamera;

    #region Lifecycle
    // Use this for initialization
    void Start()
    {
        activePanel = Home; // 1er ecran au lancement du jeu
        activePanel.gameObject.SetActive(true);
        if (ActionManager.instance)
            ActionManager.instance.onRemoveItem.AddListener(checkBtnToIncremente);

        if (GameManager.instance)
        {
            GameManager.instance.onHome.AddListener(delegate {      openPanel(Home);            });
            GameManager.instance.onPlay.AddListener(delegate {      openPanel(HudReflexion);    });
            GameManager.instance.onResolve.AddListener(delegate {   openPanel(Hud);             });
            GameManager.instance.onMenu.AddListener(delegate {      openPanel(Menu);            });
            GameManager.instance.onWin.AddListener(delegate {       openPanel(Win);             });
            GameManager.instance.onGameOver.AddListener(delegate {  openPanel(GameOver);        });
            GameManager.instance.onRestart.AddListener(delegate {
                resetActionBouton();
                openPanel(Hud);
            });
            GameManager.instance.onLevelSelect.AddListener(delegate {
                clearActionBouton();
                openPanel(LevelSelect);
            });
        }
    }
    void Awake()
    {
        if (_instance != null)
            throw new Exception("Tentative de création d'une autre instance de MonoBehaviorSingleton1 alors que c'est un singleton.");
        _instance = this;
    }
    void Update ()
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            list[i].bouton.transform.localRotation = Quaternion.Euler(-50f, 0f, 0f);
            list[i].bouton.transform.GetChild(1).transform.localRotation = Quaternion.Euler(new Vector3(0, -list[i].bouton.transform.parent.parent.rotation.eulerAngles.y + list[i].orientation,0));

            list[i].bouton.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = listCounter[i].ToString();
        }
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
    private void openPanel(RectTransform panel)
    {
        activePanel.gameObject.SetActive(false);
        activePanel = panel;
        activePanel.gameObject.SetActive(true);
    }

    public void setActionBouton(List<ItemtoPlace> pList)
    {
        float marge = 120f;
        float totalWidth = pList.Count * marge;

        for (int i = pList.Count - 1; i >= 0; i--)
        {
            float btnPosition = totalWidth - (totalWidth * 1.5f) + i * marge + marge / 2;

            string itemName;

            switch (pList[i].item)
            {
                case ItemtoPlace.ActionType.aArrow:
                    itemName = "Arrow";
                    break;
                case ItemtoPlace.ActionType.aConveyor:
                    itemName = "Conveyor";
                    break;
                case ItemtoPlace.ActionType.aStop:
                    itemName = "Stop";
                    break;
                case ItemtoPlace.ActionType.aSwitch:
                    itemName = "Switch";
                    break;
                default:
                    itemName = " ";
                    break;
            }

            GameObject GoBouton = (GameObject)Instantiate(Resources.Load("UI/BT" + itemName));

            GoBouton.transform.SetParent(GameObject.Find("HudReflexion").transform);
            GoBouton.transform.localPosition = new Vector3( 650f, btnPosition, 0);
            GoBouton.transform.localScale = new Vector3(1f, 1f, 1f);
            GoBouton.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            GoBouton.transform.GetChild(1).name = pList[i].direction.ToString();

            ItemData SData = new ItemData();
            SData.bouton = GoBouton;
            SData.orientation = cardinalToRotation(pList[i].direction.ToString());
            SData.number = pList[i].number;

            listCounter.Add(pList[i].number);
            list.Add(SData);

            GoBouton.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = pList[i].number.ToString();
        }
    }
    
    public void clearActionBouton()
    {
        for (int i = list.Count - 1; i >= 0; i--)
            Destroy(list[i].bouton);
        list.Clear();
        listCounter.Clear();
    }

    public void resetActionBouton()
    {
        for (int i = list.Count - 1; i >= 0; i--)
            listCounter[i] = list[i].number ;
    }

    public void checkBtnToIncremente(float pOrientation, string pItemName)
    {
        float newOrientation;
        if (pOrientation == 270f)
            newOrientation = -90f;
        else
            newOrientation = pOrientation;
        
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].bouton.name.Remove(list[i].bouton.name.Length - 7).Substring(2) == pItemName && list[i].orientation == newOrientation)
                incrementBouton(list[i].bouton);
        }
    }

    public void decrementBouton(GameObject pBouton)
    {
        for (int i = list.Count - 1; i >= 0; i--)
            if (pBouton == list[i].bouton)
                listCounter[i]--;
    }
    public void incrementBouton(GameObject pBouton)
    {
        for (int i = list.Count - 1; i >= 0; i--)
            if (pBouton == list[i].bouton)
                listCounter[i]++;
    }
    
    public void sendOrientation(GameObject pItem)
    {
        GameManager.rotationInHand = cardinalToRotation(pItem.name);
    }

    private float cardinalToRotation(string pCardinal)
    {
        switch (pCardinal)
        {
            case "North":
                return 0f;
            case "South":
                return 180f;
            case "East":
                return 90f;
            case "West":
                return -90f;
            default:
                return 0f;
        }
    }
    private string rotationToCardinal(int pInt)
    {
        switch (pInt)
        {
            case 0:
                return "North";
            case 180:
                return "South";
            case 90:
                return "East";
            case -90:
                return "West";
            default:
                return "";
        }
    }
    #endregion
}
