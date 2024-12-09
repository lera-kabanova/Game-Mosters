using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormTower : Tower
{
    private void Start()
    {
        ElementType = Element.STORM;
    }

    public override Debuff GetDebuff()
    {
        return new StormDebuff(Target);
    }
}
