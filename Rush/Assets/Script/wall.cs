using UnityEngine;
using System.Collections;

public class wall : MonoBehaviour {
    
	void Start ()
    {
        // rotation random pour diversifié les assets
        float randomSeed = Random.Range(0f, 1f);
        transform.rotation = Quaternion.Euler(new Vector3(0, randomSeed > .5f ? 0 : 180, 0));
    }
}
