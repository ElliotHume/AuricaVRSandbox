using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdVd.GlyphRecognition;
using UnityEngine.SceneManagement;

public class DrawingPlane : MonoBehaviour {
    public GlyphDrawInput glyphDrawInput;
    public GlyphRecognition glyphRecognition;

    ContactPoint lastContactPoint;
    Vector3 lastContactPosition;

    // Start is called before the first frame update
    void Start() {

    }

    void OnCollisionEnter(Collision collision) {
        // Debug.Log("ENTER: "+collision.gameObject);
        if (collision.gameObject.tag == "DrawingPoint") {
            ContactPoint contact = collision.contacts[0];
            Vector3 localSpacePoint = transform.InverseTransformPoint(contact.point);
            // Debug.Log("Contact point: "+contact.point.ToString()+"    LocalSpacePoint: "+localSpacePoint.ToString());
            Vector2 twospace = new Vector2(-localSpacePoint.x, localSpacePoint.y);
            try {
                glyphDrawInput.BeginCustomDrag(twospace);
            } catch {
                print("failure to start drag");
            }
        }
    }

    void OnCollisionStay(Collision collision) {
        // Debug.Log("STAY:  "+collision.gameObject);
        if (collision.gameObject.tag == "DrawingPoint") {
            ContactPoint contact = collision.contacts[0];
            lastContactPoint = contact;
            Vector3 localSpacePoint = transform.InverseTransformPoint(contact.point);
            Vector2 twospace = new Vector2(-localSpacePoint.x, localSpacePoint.y);
            try {
                glyphDrawInput.CustomDrag(twospace);
            } catch {
                print("failure to drag");
            }
        }  
    }

    void OnCollisionExit(Collision collision) {
        // Debug.Log("END: " +collision.gameObject);
        if (collision.gameObject.tag == "DrawingPoint") {
            ContactPoint contact = lastContactPoint;
            Vector3 localSpacePoint = transform.InverseTransformPoint(contact.point);
            Vector2 twospace = new Vector2(-localSpacePoint.x, localSpacePoint.y);
            try {
                glyphDrawInput.EndCustomDrag(twospace);
            } catch {
                print("failure to end drag");
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
        // Debug.Log("ENTER: "+collider.gameObject);
        if (collider.gameObject.tag == "DrawingPoint") {
            Vector3 contact = collider.ClosestPoint(transform.position);
            lastContactPosition = contact;
            Vector3 localSpacePoint = transform.InverseTransformPoint(contact);
            // Debug.Log("Contact point: "+contact.point.ToString()+"    LocalSpacePoint: "+localSpacePoint.ToString());
            Vector2 twospace = new Vector2(-localSpacePoint.x, localSpacePoint.y);
            try {
                glyphDrawInput.BeginCustomDrag(twospace);
            } catch {
                print("failure to start drag");
            }
        }
    }

    void OnTriggerStay(Collider collider) {
        // Debug.Log("STAY:  "+collider.gameObject);
        if (collider.gameObject.tag == "DrawingPoint") {
            Vector3 contact = collider.ClosestPoint(transform.position);
            lastContactPosition = contact;
            Vector3 localSpacePoint = transform.InverseTransformPoint(contact);
            Vector2 twospace = new Vector2(-localSpacePoint.x, localSpacePoint.y);
            try {
                glyphDrawInput.CustomDrag(twospace);
            } catch {
                print("failure to drag");
            }
        }  
    }

    void OnTriggerExit(Collider collider) {
        // Debug.Log("END: " +collision.gameObject);
        if (collider.gameObject.tag == "DrawingPoint") {
            Vector3 contact = collider.ClosestPoint(transform.position);
            lastContactPosition = contact;
            Vector3 localSpacePoint = transform.InverseTransformPoint(contact);
            Vector2 twospace = new Vector2(-localSpacePoint.x, localSpacePoint.y);
            try {
                glyphDrawInput.EndCustomDrag(twospace);
            } catch {
                print("failure to end drag");
            }
        }
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}