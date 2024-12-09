using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTower : Tower
{
    private void Start()
    {
        ElementType = Element.POISON;
    }

    public override Debuff GetDebuff()
    {
        return new PoisonDebuff(Target);
    }
}
