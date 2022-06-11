using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using TMPro;

[RequireComponent(typeof(HVRGrabbable))]
public class SpellCastingObject : MonoBehaviour {

    public AuricaSpell Spell;
    public TMP_Text spellText;
    public GameObject castingLine;

    [Header("Settings")]
    public float TriggerPullThreshold = .7f;
    public float TriggerResetThreshold = .6f;
    private bool IsTriggerReset, IsTriggerPulled;


    public HVRGrabbable Grabbable { get; private set; }

    private HapticData triggerHaptics = new HapticData(.20f, .50f, 150f);

    // Start is called before the first frame update
    void Start() {
        Grabbable = GetComponent<HVRGrabbable>();
        Grabbable.Grabbed.AddListener(OnGrabbed);

        if (Spell != null) spellText.text = Spell.c_name;
    }

    private void OnGrabbed(HVRGrabberBase grabber, HVRGrabbable grabbable) {

    }

    // Update is called once per frame
    void Update() {
        CheckTriggerPull();
    }

    void FixedUpdate() {
        if (IsTriggerPulled) PlayHaptics(Grabbable, triggerHaptics);
    }

    protected virtual void CheckTriggerPull() {
        if (!Grabbable.IsHandGrabbed)
            return;

        var controller = Grabbable.HandGrabbers[0].Controller;

        if (controller.Trigger <= TriggerResetThreshold) {
            IsTriggerReset = true;
        }

        if (controller.Trigger > TriggerPullThreshold && IsTriggerReset) {
            TriggerPulled();
            IsTriggerReset = false;
            IsTriggerPulled = true;
        }
        else if (controller.Trigger < TriggerPullThreshold && IsTriggerPulled) {
            IsTriggerPulled = false;
            TriggerReleased();
        }
    }
    
    public void TriggerPulled() {
        ShowCastingLine();
    }

    public void TriggerReleased() {
        HideCastingLine();
    }

    public void SetSpell(AuricaSpell targetSpell) {
        Spell = targetSpell;
        spellText.text = Spell.c_name;
    }

    private void PlayHaptics(HVRGrabbable grabbable, HapticData data) {
        if (grabbable.HandGrabbers.Count == 0 || !grabbable.HandGrabbers[0].IsMine || data == null) return;

        var controller = grabbable.HandGrabbers[0].Controller;
        controller.Vibrate(data);
    }

    private void ShowCastingLine() {
        castingLine.SetActive(true);
    }

    private void HideCastingLine() {  
        castingLine.SetActive(false);
    }
}
