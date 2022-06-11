using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BasicProjectileSpell : Spell
{
    public float Speed = 20f;
    public float RandomMoveRadius = 0f;
    public float RandomMoveSpeedScale = 0f;
    public float CollisionOffset = 0;
    public float CollisionDestroyTimeDelay = 5;
    public float MaxDistance = 0f, AirDrag = 0f, MinSpeed = 0f;
    public bool ExplodeOnReachMaxDistance = false;
    public GameObject[] EffectsOnCollision;
    public bool LocalCollisionEffectsUseHitNormal = true;
    public GameObject[] DeactivateObjectsOnCollision;

    public bool ParticleCollisions = false;
    public float DamagePerParticle = 1f;
    public ParticleSystem collisionParticles;
    public AudioClip particleCollisionSound;
    public float clipVolume;
    private List<ParticleCollisionEvent> collisionEvents;

    private Vector3 startPosition;
    private Vector3 travelDistance = Vector3.zero;
    private Quaternion startRotation;
    private bool isCollided = false;
    private Vector3 randomTimeOffset, playerOffset = new Vector3(0f, 1f, 0f);

    void Start() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        transform.parent = null;
        randomTimeOffset = Random.insideUnitSphere * 10;
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    public void Update() {
        UpdateWorldPosition();
    }

    void OnCollisionEnter(Collision collision) {
        if ( isCollided ) return;

        // Debug.Log(""+gameObject+"  Collided with  "+collision.gameObject);
        ContactPoint hit = collision.GetContact(0);
        isCollided = true;

        // Call local collision response to generate collision VFX
        LocalCollisionBehaviour(hit.point, hit.normal);
    }

    void OnParticleCollision(GameObject other) {
        if (!ParticleCollisions) return;
        int numCollisionEvents = collisionParticles.GetCollisionEvents(other, collisionEvents);
        if (particleCollisionSound) {
            foreach(var collision in collisionEvents) {
                AudioSource.PlayClipAtPoint(particleCollisionSound, collision.intersection, clipVolume);
            }
        }
    }

    void LocalCollisionBehaviour(Vector3 hitpoint, Vector3 hitNormal) {
        Vector3 normal = LocalCollisionEffectsUseHitNormal ? hitNormal : Vector3.up;
        foreach (var effect in DeactivateObjectsOnCollision) {
            if (effect != null) effect.SetActive(false);
        }
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var effect in particles) {
            if (effect != null) effect.Stop();
        }
        foreach (var effect in EffectsOnCollision) {
            GameObject instance = Instantiate(effect, hitpoint + normal * CollisionOffset, new Quaternion());
            instance.transform.LookAt(hitpoint + normal + normal * CollisionOffset);
            Destroy(instance, CollisionDestroyTimeDelay);
        }
        foreach(Collider col in GetComponents<Collider>()) col.enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
    }


    void DestroySelf() {
        Destroy(gameObject);
    }

    void UpdateWorldPosition() {
        if (isCollided) return;
        if ((MaxDistance > 0f && travelDistance.magnitude > MaxDistance) || Speed < 0.1f) {
            if (ExplodeOnReachMaxDistance) {
                LocalCollisionBehaviour(transform.position, transform.forward);
            } else {
                // Similar to LocalCollision Behaviour, but do not spawn any effects
                foreach (var effect in DeactivateObjectsOnCollision) {
                    if (effect != null) effect.SetActive(false);
                }
                foreach(Collider col in GetComponents<Collider>()) col.enabled = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().isKinematic = true;
                Invoke("DestroySelf", CollisionDestroyTimeDelay+1f);
            }
            isCollided = true;
            return;
        }

        Vector3 randomOffset = Vector3.zero;
        if (RandomMoveRadius > 0) {
            randomOffset = GetRadiusRandomVector() * RandomMoveRadius;
        }

        var frameMoveOffsetWorld = Vector3.zero;
        var currentForwardVector = (Vector3.forward + randomOffset) * Speed * Time.deltaTime;
        frameMoveOffsetWorld = startRotation * currentForwardVector;
        
        if (AirDrag > 0f) Speed = Mathf.Lerp(Speed, MinSpeed, AirDrag/100f);
        transform.position += frameMoveOffsetWorld;
        travelDistance += frameMoveOffsetWorld;
    }

    Vector3 GetRadiusRandomVector() {
        var x = Time.time * RandomMoveSpeedScale + randomTimeOffset.x;
        var vecX = Mathf.Sin(x / 7 + Mathf.Cos(x / 2)) * Mathf.Cos(x / 5 + Mathf.Sin(x));

        x = Time.time * RandomMoveSpeedScale + randomTimeOffset.y;
        var vecY = Mathf.Cos(x / 8 + Mathf.Sin(x / 2)) * Mathf.Sin(Mathf.Sin(x / 1.2f) + x * 1.2f);

        x = Time.time * RandomMoveSpeedScale + randomTimeOffset.z;
        var vecZ = Mathf.Cos(x * 0.7f + Mathf.Cos(x * 0.5f)) * Mathf.Cos(Mathf.Sin(x * 0.8f) + x * 0.3f);

        return new Vector3(vecX, vecY, vecZ);
    }
}
