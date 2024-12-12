using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkShelf : Shelf
{
    void Start()
    {
        Price = 70000;
    }

    public override int GetShelfType()
    {
        return 0;
    }
}
