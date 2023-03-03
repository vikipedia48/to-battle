using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Order
{
    public int target; // 399*x+y
    public OrderType type;


    public Order(int target, OrderType type)
    {
        this.target = target;
        this.type = type;
    }
}

public enum OrderType
{
    No,
    Move,
    Fortify,
    Fire
}
