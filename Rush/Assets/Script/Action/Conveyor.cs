using UnityEngine;
using System.Collections;

public class Conveyor : ActionObject {

	// Use this for initialization
	void Start ()
    {
        base.Start();
        ActionManager.instance.listConveyor.Add(this);
    }

    public void destroy()
    {
        ActionManager.instance.listConveyor.Remove(this);
        Destroy(this.gameObject);
    }
}
