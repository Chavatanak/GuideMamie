using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamColor : MonoBehaviour
{

    public static List<Color> list = new List<Color>
    {
        new Color(0, 1, 1, 1),
        new Color(1, 0, 1, 1),
        new Color(1, 1, 0, 1),
        new Color(1, 1, 1, 1)
    };

    public static List<Color> listTeleporter = new List<Color>
    {
        new Color(1, 0, 0, .5f),
        new Color(0, 1, 0, .5f),
        new Color(0, 0, 1, .5f),
        new Color(0, 0, 0, .5f)
    };
}
