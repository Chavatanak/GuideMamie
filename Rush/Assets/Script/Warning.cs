using UnityEngine;
using System.Collections;

public class Warning : MonoBehaviour {
    
	void Start () {
        WarningManager.instance.list.Add(this);
	}

    public void destroy()
    {
        Destroy(this.gameObject);
        WarningManager.instance.list.Remove(this);
    }
}
