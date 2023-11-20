using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float vertical, horizontal;
    public bool handbrake;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        //L'asse jump restituisce un valore tra 0 e 1
        handbrake = (Input.GetAxis("Jump") != 0)? true : false;

    }
}
