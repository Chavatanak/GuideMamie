using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

    [SerializeField]
    private float _speed = 0.025f;
    
	void Start () {
	    
	}
	
	void Update () {
        
        // animation de courant d'eau
        for (int i = transform.GetChildCount() - 1; i >= 0; i--)
        {
            transform.GetChild(i).transform.Translate(new Vector3(0f, 0f, _speed), Space.World);

            if (transform.GetChild(i).transform.localPosition.z > 42.6f)
                transform.GetChild(i).transform.localPosition = new Vector3(transform.GetChild(i).transform.localPosition.x, transform.GetChild(i).transform.localPosition.y, -42.6f);
        }

	}
}
