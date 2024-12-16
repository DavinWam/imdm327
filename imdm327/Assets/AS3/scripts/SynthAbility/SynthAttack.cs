using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthAttack : MonoBehaviour
{
    public SynthAbility synthAbility;
    private Transform target;
    public float trackingSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * trackingSpeed * Time.deltaTime;
        }
    }

    public void TrackTarget(GameObject targetObject)
    {
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
    }
}
