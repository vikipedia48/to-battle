using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectAddableUnit : MonoBehaviour
{
    public short id;
    public TeamSelectScript teamSelectLogic;

    private void Start()
    {
        teamSelectLogic = GameObject.Find("TeamSelection").GetComponent<TeamSelectScript>();
    }

    public void RemoveThisUnit()
    {
        teamSelectLogic.RemoveUnit(id);
        Destroy(this.gameObject);
    }
}
