using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {
    
    // CONTROL VARIABLES
    public float Damage = 10f;
    public AuricaSpell auricaSpell;
    public float ManaChannelCost = 40f;
    public float Duration = 1f;
    public bool TurnToAimPoint = true;
    public bool IsChannel = false, IsSelfTargeted = false, IsOpponentTargeted = false;

    // MODIFIER VARIABLES
    private float modSize;
    private float modDamage;
    private float modSpellStrength;
    private float modSpeed;
    private float modManaCost;

    // Scripting variables
    private float spellStrength = 1f;
    private GameObject owner;
    private ManaDistribution damageModifier;
    private bool canHitOwner = true;

    public virtual void SetSpellStrength(float newStrength) {
        // Debug.Log("New Spell Strength: "+newStrength);
        spellStrength = newStrength;
    }

    public virtual void SetSpellDamageModifier(ManaDistribution newMod) {
        damageModifier = newMod;
        // Debug.Log("New modifier set: "+auricaSpell.GetSpellDamageModifier(GetSpellDamageModifier()));
    }

    public float GetSpellStrength() {
        return spellStrength;
    }

    public ManaDistribution GetSpellDamageModifier() {
        return damageModifier;
    }

    public virtual void SetOwner(GameObject ownerGO, bool _canHitOwner = true) {
        owner = ownerGO;
        canHitOwner = _canHitOwner;
    }

    public bool GetCanHitOwner() {
        return canHitOwner;
    }

    public GameObject GetOwner() {
        return owner;
    }

    public void DestroySpell() {
        Destroy(gameObject);
    }
}
