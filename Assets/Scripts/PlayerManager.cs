using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    [Tooltip("The current Health of our player")]
    public float Health = 100f;

    [Tooltip("The current Mana pool of our player")]
    public float Mana = 500f;

    [Tooltip("The rate at which Mana will regenerate (Mana/second)")]
    public float ManaRegen = 2.5f;

    [Tooltip("The rate at which Mana regen will increase if a spell hasnt been cast recently")]
    public float ManaRegenGrowthRate = 12f;

    [Tooltip("Where spells will spawn from the right hand")]
    public Transform rightHandCastingAnchor;
    [Tooltip("Where spells will spawn from the left hand")]
    public Transform leftHandCastingAnchor;
    [Tooltip("Where spells will spawn from when cast on yourself")]
    public Transform PlayerTransformAnchor;

    public static GameObject PlayerGameObject;
    public static PlayerManager Instance;

    private float maxMana, maxHealth;


    // Start is called before the first frame update
    void Start() {
        PlayerManager.PlayerGameObject = gameObject;
        PlayerManager.Instance = this;

        maxMana = Mana;
        maxHealth = Health;
    }

    // Update is called once per frame
    void Update() {
        // Regen mana if below maxMana
        if (Mana < maxMana) {
            Mana += ManaRegen * Time.deltaTime * ((1.1f - Mana / maxMana) * ManaRegenGrowthRate);
            if (Mana > maxMana) Mana = maxMana;
        }

        if (Mana < 0f) Mana = 0f;
    }
}
