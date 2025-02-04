using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    SphereCollider damageCollider;

    private float explosionSpeed = 20f;
    private float damageRadius = 5f;

    void Start()
    {
        damageCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacles") || collider.gameObject.layer == LayerMask.NameToLayer("Inner") || collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        while (damageCollider.radius < damageRadius)
        {
            GetComponent<MeshRenderer>().enabled = false;

            damageCollider.radius += explosionSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }
}
