using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcecreamShelf : Shelf
{
    private void Start()
    {
        Price = 100000;
    }

    public override int GetShelfType()
    {
        return 2;
    }
}
