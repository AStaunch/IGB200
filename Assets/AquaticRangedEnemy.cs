using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquaticRangedEnemy : AbstractAquaticCreature
{
    private void Update() {
        if (!isInWater) {
            Anim_.SetTrigger("fall");
        }
    }
}
