using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nation
{
    public string Name;
    public bool HorseArchersEnabled;
    public float[] UnitBuffs;
    public Color[] Colors;

    public Nation(string name, bool horseArchersEnabled, float[] unitBuffs, Color[] colors)
    {
        Name = name;
        HorseArchersEnabled = horseArchersEnabled;
        UnitBuffs = unitBuffs;
        Colors = colors;
    }
}
