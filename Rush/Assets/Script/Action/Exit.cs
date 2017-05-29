using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Exit : ActionObject
{
    public enum SelectColor { cyan, magenta, yellow, white };
    public SelectColor color = SelectColor.cyan;
    public Color currentColor { get; private set; }
    
    #region Lifecycle
    void Start ()
    {
        base.Start();
        ActionManager.instance.listExit.Add(this);

        // initialisation couleur
        currentColor = TeamColor.list[(int)color];
        GetComponentInChildren<Renderer>().material.color = currentColor;
        transform.position = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z);
    }
    void FixedUpdate()
    {
    }
    #endregion
}
