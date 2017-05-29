using UnityEngine;
using System.Collections;

public class Switch : ActionObject
{
    public bool switched;
    
    void Start ()
    {
        base.Start();
        ActionManager.instance.listSwitch.Add(this);
        switched = false;
    }

    public void destroy()
    {
        ActionManager.instance.listSwitch.Remove(this);
        Destroy(this.gameObject);
    }
}
