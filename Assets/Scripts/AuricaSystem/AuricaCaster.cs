using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AuricaCaster : MonoBehaviour {

    public Aura aura;
    public static AuricaCaster Instance;

    [HideInInspector]
    public bool spellManasCached = false;

    // Lists of all components and spells
    private AuricaSpellComponent[] allComponents;
    private AuricaSpell[] allSpells;
    private AuricaPureSpell[] allPureSpells;

    // Runtime variables
    private List<AuricaSpell> discoveredSpells;
    private List<AuricaSpellComponent> currentComponents;
    private ManaDistribution currentDistribution;
    private float currentManaCost, spellStrength;
    private AuricaSpell currentSpellMatch;

    // Start is called before the first frame update
    void Start() {
        if (aura == null) aura = GetComponent<Aura>();

        // GAME SPECIFIC
    }

    void Awake() {
        AuricaCaster.Instance = this;
        allComponents = Resources.LoadAll<AuricaSpellComponent>("AuricaSpellComponents");
        allSpells = Resources.LoadAll<AuricaSpell>("AuricaSpells");
        allPureSpells = Resources.LoadAll<AuricaPureSpell>("AuricaPureSpells");
        currentComponents = new List<AuricaSpellComponent>();
        currentDistribution = new ManaDistribution();
    }

    public void AddComponent(string componentName) {
        foreach (AuricaSpellComponent c in allComponents) {
            if (c.c_name == componentName) {
                AddComponent(c);
                break;
            }
        }
    }

    public void AddComponent(AuricaSpellComponent newComponent) {
        currentComponents.Add(newComponent);
        currentManaCost += newComponent.GetManaCost(aura.GetAura());
        ManaDistribution oldMd = currentDistribution;
        currentDistribution = newComponent.CalculateDistributionChange(currentDistribution, aura.GetAura());

        Debug.Log("Added component: " + newComponent.c_name + "    Current Mana Cost: " + currentManaCost);
        // Debug.Log("Old Distribution: " + oldMd.ToString() + "    New Distribution: " + currentDistribution.ToString()+"         Change: "+(currentDistribution-oldMd).ToString());
    }

    public void RemoveLastComponent() {
        if (currentComponents.Count <= 0) {
            return;
        }
        AuricaSpellComponent component = currentComponents[currentComponents.Count - 1];
        if (component.hasFluxDistribution || component.hasSiphonDistribution) {
            return;
        }
        currentManaCost -= component.GetManaCost(aura.GetAura());
        currentDistribution = component.RemoveDistribution(currentDistribution, aura.GetAura());
        currentComponents.Remove(component);
        Debug.Log("REMOVED COMPONENT components left: "+currentComponents.Count);
    }

    public bool CanRemoveLastComponent() {
        AuricaSpellComponent component = currentComponents[currentComponents.Count - 1];
        return !(component.hasFluxDistribution || component.hasSiphonDistribution);
    }

    public AuricaSpell CastSpellByName(string componentsByName) {
        if (componentsByName == null || componentsByName == "") return null;
        ResetCast();
        string[] componentSeperator = new string[] { ", " };
        string[] splitComponents = componentsByName.Split(componentSeperator, System.StringSplitOptions.None);
        foreach (string item in splitComponents) {
            AddComponent(item);
        }
        return Cast();
    }

    public AuricaSpell CastSpellByObject(AuricaSpell spell) {
        ResetCast();
        if (spell.keyComponents.Count == 0) return null;
        foreach (AuricaSpellComponent component in spell.keyComponents) {
            AddComponent(component);
        }
        return Cast();
    }

    public float GetSpellStrengthForSpell(AuricaSpell spell) {
        if (spell.keyComponents.Count == 0) return 0f;
        ResetCast();
        foreach( AuricaSpellComponent component in spell.keyComponents) {
            AddComponent(component);
        }
        GetSpellMatch(currentComponents, currentDistribution);
        ResetCast();
        return spellStrength;
    }

    public AuricaSpell Cast() {
        AuricaPureSpell pureSpell = GetPureMagicSpellMatch(currentComponents, currentDistribution);
        AuricaSpell spell = pureSpell == null ? GetSpellMatch(currentComponents, currentDistribution) :  pureSpell.GetAuricaSpell(pureSpell.GetManaType(currentDistribution));
        if (spell != null) {
            if (pureSpell != null) currentManaCost += pureSpell.addedManaCost;
            return spell;
        }
        return null;
    }

    public AuricaSpell CastFinal() {
        AuricaPureSpell pureSpell = GetPureMagicSpellMatch(currentComponents, currentDistribution);
        AuricaSpell spell = pureSpell == null ? GetSpellMatch(currentComponents, currentDistribution) : pureSpell.GetAuricaSpell(pureSpell.GetManaType(currentDistribution));
        if (spell != null) {
            if (pureSpell != null) currentManaCost += pureSpell.addedManaCost;
            return spell;
        }
        return null;
    }

    public AuricaSpell GetSpellMatch(List<AuricaSpellComponent> components, ManaDistribution distribution) {
        int bestMatchCorrectComponents = 0;
        AuricaSpell spellMatch = null;
        foreach (AuricaSpell s in allSpells) {
            foreach (var item in components) {
                Debug.Log("Casted Component: "+item.c_name);
            }
            foreach (var item in s.keyComponents) {
                Debug.Log("Spell Key Component: "+item.c_name);
            }

            Debug.Log("Check Spell: " + s.c_name + "   IsMatch: " + s.CheckComponents(components) + "     Error:  " + s.GetError(distribution)+"  Num matching components: "+s.GetNumberOfMatchingComponents(components));
            if (s.CheckComponents(components) && s.GetNumberOfMatchingComponents(components) > bestMatchCorrectComponents) {
                spellMatch = s;
                bestMatchCorrectComponents = s.GetNumberOfMatchingComponents(components);
                spellStrength = (spellMatch.errorThreshold - s.GetError(distribution)) / spellMatch.errorThreshold + 0.5f;
                if (spellStrength < 0.5f) spellStrength = 0.5f;
            }
        }

        currentSpellMatch = spellMatch == null ? null : spellMatch;
        return currentSpellMatch;
    }

    public AuricaPureSpell GetPureMagicSpellMatch(List<AuricaSpellComponent> components, ManaDistribution distribution) {
        AuricaPureSpell spellMatch = null;
        foreach (AuricaPureSpell s in allPureSpells) {
            // Debug.Log("Check Pure Spell: " + s.c_name + "   IsMatch: " + s.CheckComponents(components) + "     Error:  " + s.GetError(s.GetManaType(distribution), distribution));
            if (s.CheckComponents(components, distribution)) {
                spellMatch = s;
                spellStrength = (spellMatch.errorThreshold - s.GetError(s.GetManaType(distribution), distribution)) / spellMatch.errorThreshold + 0.5f;
                if (spellStrength < 0.5f) spellStrength = 0.5f;
                // Debug.Log("Pure match: "+s.c_name+"   mana type:"+s.GetManaType(distribution)+"  error:"+s.GetError(s.GetManaType(distribution), distribution));
            }
        }
        currentSpellMatch = spellMatch == null ? null : spellMatch.GetAuricaSpell(spellMatch.GetManaType(currentDistribution));
        return spellMatch == null ? null : spellMatch;
    }

    public void ResetCast() {
        // Debug.Log("Resetting Aurica Cast");
        currentComponents.Clear();
        currentManaCost = 0f;
        currentDistribution = new ManaDistribution();
    }

    public float GetManaCost() {
        if (currentSpellMatch != null) {
            return (currentSpellMatch.baseManaCost + (currentSpellMatch.componentManaMultiplier * currentManaCost));
        }
        return currentManaCost;
    }

    public float GetSpellStrength() {
        return spellStrength;
    }

    public ManaDistribution GetCurrentDistribution() {
        return currentDistribution;
    }

    public List<AuricaSpellComponent> GetCurrentComponents() {
        return currentComponents;
    }

}
