using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct ItemtoPlace
{

    public enum ActionType
    {
        aArrow,
        aConveyor,
        aStop,
        aSwitch
    };
    public ActionType item;

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
    public Direction direction;

    public int number;
}
