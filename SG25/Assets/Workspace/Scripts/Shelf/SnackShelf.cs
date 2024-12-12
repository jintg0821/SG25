using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackShelf : Shelf
{
    void Start()
    {
        Price = 50000;
    }

    public override int GetShelfType()
    {
        return 1;
    }
}