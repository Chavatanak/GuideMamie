using UnityEngine;
using System.Collections;

public class Arrow : ActionObject
{
	void Start ()
    {
        base.Start();
        ActionManager.instance.listArrow.Add(this);
    }
    public void destroy()
    {
        ActionManager.instance.listArrow.Remove(this);
        Destroy(this.gameObject);
    }
}
