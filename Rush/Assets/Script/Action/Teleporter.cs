using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    public enum SelectColor { red, green, blue, Black};
    public SelectColor color = SelectColor.Black;
    public Color currentColor { get; private set; }

    protected Transform doorA;
    protected Transform doorB;

    protected LayerMask mask;
    protected RaycastHit hit;

    protected List<Transform> bothPosition = new List<Transform>();

    #region Lifecycle
    void Start ()
    {
        ActionManager.instance.listTeleporter.Add(this);
        mask = 1 << LayerMask.NameToLayer("cube");
        doorA = gameObject.transform.GetChild(0).transform;
        doorB = gameObject.transform.GetChild(1).transform;

        bothPosition.Add(doorA);
        bothPosition.Add(doorB);

        // initialisation couleur
        currentColor = TeamColor.listTeleporter[(int)color];
        gameObject.transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<Renderer>().material.color = currentColor;
        gameObject.transform.GetChild(1).transform.GetChild(0).GetComponentInChildren<Renderer>().material.color = currentColor;
    }
    #endregion

    #region Methods
    // check s'il y a un cube sur l'un des deux teleporteurs
    public List<Transform> checkBoth()
    {
        if (Physics.Raycast(doorA.position - Vector3.up * .6f, Vector3.up, out hit, 1f, mask) 
         || Physics.Raycast(doorB.position - Vector3.up * .6f, Vector3.up, out hit, 1f, mask))
            return bothPosition;
        else
            return null;
    }

    public void destroy()
    {
        ActionManager.instance.listTeleporter.Remove(this);
        Destroy(this.gameObject);
    }
    #endregion
}
