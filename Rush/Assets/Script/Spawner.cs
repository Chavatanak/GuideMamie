using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
    
    // Paramètres de spawn
    public int spawnRate = 3;
    public uint spawnNumber = 4;
    [System.NonSerialized]
    public uint spawnCount;

    // couleur
    public enum SelectColor { cyan, magenta, yellow, white };
    public SelectColor color = SelectColor.cyan;
    private Color currentColor;
    
    #region Lifecycle
    void Start ()
    {
        SpawnerManager.instance.list.Add(this);

        currentColor = TeamColor.list[(int)color];
        GetComponentInChildren<Renderer>().materials[1].color = currentColor;
        transform.GetChild(1).GetComponentInChildren<Renderer>().material.color = currentColor;
        spawnCount = spawnNumber;
    }
    #endregion

    #region Methods
    // créer le cube et lui defini sa couleur
    public void generateCube()
    {
        GameObject newCube = (GameObject)Instantiate(Resources.Load("Prefab/Mamie"));
        newCube.transform.position = transform.position;
        newCube.transform.rotation = transform.rotation;
        newCube.GetComponent<Renderer>().materials[1].color = currentColor;
        newCube.transform.parent = GameObject.Find("LevelContain").transform;
    }
    #endregion
}
