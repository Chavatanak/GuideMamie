using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Level : MonoBehaviour {

    [SerializeField]
    public List<ItemtoPlace> list = new List<ItemtoPlace>();
    
    void Start () {
        // Envoi le nombre d'item plaçable sur le niveau pendant la phase de reflexion
        UIManager.instance.setActionBouton(list);
    }
}
