using UnityEngine;
using System.Collections;

public class Stop : ActionObject
{
	void Start ()
    {
        base.Start();
        ActionManager.instance.listStop.Add(this);
    }
    public void destroy()
    {
        ActionManager.instance.listStop.Remove(this);
        Destroy(this.gameObject);
    }
}
