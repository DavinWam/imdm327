using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageWidget : MonoBehaviour
{
     private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        
    }
    public void PlayHit(){
        ps.Play();
    }
}
