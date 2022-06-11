using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.HandPoser;
using HurricaneVR.Framework.Shared;

public class SpellCastingManager : MonoBehaviour {

    public static SpellCastingManager Instance;

    public HVRHandGrabber rightHandGrabber, leftHandGrabber;
    public HVRForceGrabber rightHandForceGrabber, leftHandForceGrabber;
    public GameObject leftHandGO, rightHandGO;
    public Transform SpellCastingObjectSpawnAnchor;
    public GameObject GenericSpellSpherePrefab, FizzlePrefab;

    // Start is called before the first frame update
    void Start() {
        SpellCastingManager.Instance = this;
    }

    public void CreateSpell(string side) {
        AuricaSpell newSpell = AuricaCaster.Instance.Cast();
        if (newSpell != null) {
            Debug.Log("Successful SpellCast: "+newSpell.c_name);
            GameObject spawnPrefab = GenericSpellSpherePrefab;
            bool generic = true;
            if (newSpell.spellSphereResource != null && newSpell.spellSphereResource != "") {
                spawnPrefab = Resources.Load<GameObject>(newSpell.spellSphereResource);
                generic = false;
            }
            GameObject newSpellSphere = Instantiate(spawnPrefab, SpellCastingObjectSpawnAnchor.position, SpellCastingObjectSpawnAnchor.rotation);
            if (generic) {
                SpellCastingObject sco = newSpellSphere.GetComponent<SpellCastingObject>();
                sco.SetSpell(newSpell);
            }
            AuricaCaster.Instance.ResetCast();
        } else {
            // No spell match found
            Debug.Log("Failed SpellCast");
            Vector3 position = side == "right" ? rightHandGO.transform.position : leftHandGO.transform.position;
            Quaternion rotation = side == "right" ? rightHandGO.transform.rotation : leftHandGO.transform.rotation;
            Instantiate(FizzlePrefab, position, rotation);
            AuricaCaster.Instance.ResetCast();
        }
    }

    public void SpellHandGrab(string side) {
        if (side == "right") {
        } else if (side == "left") {
        }
    }

    public void SpellHandUnGrab(string side) {

    }

    public void SpellHandTrigger(string side) {
        if (side == "right") {
        } else if (side == "left") {
        }
    }

    public void SpellHandReleaseTrigger(string side) {

    }
}
