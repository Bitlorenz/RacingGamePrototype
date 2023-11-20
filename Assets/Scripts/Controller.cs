using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //questo enum serve per distinguere tra i tre tipi di guida
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }

    [SerializeField] private driveType drive;
    
    private InputManager IM;
    private GameObject wheelColliders, wheelMeshes;
    private GameObject[] wheelMesh = new GameObject[4];
    private WheelCollider[] wheels = new WheelCollider[4];
    public float KPH; //Km per Hour, misura della velocità
    public float DownForceValue = 50f, radius = 6;
    public int motorTorque = 100;
    private GameObject centerOfMass;
    private Rigidbody rigidbody;
    [Header("Attributi ruote")]
    
    //public float motorTorque; splittiamo la potenza per avere un miglior feeling
    public float steeringMax = 4;
    public float brakePower;

    void Start()
    {
        //motorTorque = 500;
        getObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        addDownForce();
        animateWheels();
        moveVehicle();
        steerVehicle();
    }

    private void moveVehicle()
    {

        //usando l'input manager non c'è bisogno di distinguere tra i vari casi, basta recuperare la variabile corrispondente
        //che se premuta restituisce un valore positivo da moltiplicare con la potenza del motore
        if (drive == driveType.allWheelDrive)
        {
            //la potenza deve essere redistribuita su tutte le ruote
            for (int i = 0; i < wheels.Length; i++)
                wheels[i].motorTorque = IM.vertical * (motorTorque / 4);//procedere avanti
        }
        else if (drive == driveType.rearWheelDrive)
        {//distribuisco la potenza solo sulle ultime due ruote (i=2,3)
            for (int i = 2; i < wheels.Length; i++)
                wheels[i].motorTorque = IM.vertical * (motorTorque / 2);
        }
        else if (drive == driveType.frontWheelDrive)
        {//distribuisco la potenza solo sulle prime due ruote (i=0,1)
            for (int i = 0; i < wheels.Length-2; i++)
                wheels[i].motorTorque = IM.vertical * (motorTorque / 2);
        }

        //formula per calcolare la velocità
        KPH = rigidbody.velocity.magnitude * 3.6f;

        //uso del freno premendo spazio
        if (IM.handbrake)
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = brakePower;
        }
        else //si sbloccano se non lo premiamo
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = 0;
        }
    }

    //metodo per sterzare il veicolo
    private void steerVehicle()
    {
        /*Questa è una formula avanzata e complicata per migliorare l'angolo di sterzata, per aggiungerla
         * è meglio rimuovere le due linee di codice in fondo e aggiungere la var radius
         * acerman steering formula*/
        //steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;

        if (IM.horizontal > 0)
        {
            //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
        }
        else if (IM.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            //transform.Rotate(Vector3.up * steerHelping);

        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }

        //for (int i = 2; i < wheels.Length - 2; i++)
        //    wheels[i].steerAngle = IM.horizontal * steeringMax;
    }


    //metodo per animare le ruote facendo girare le mesh delle ruote, modificando rotazione e posizione
    private void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for(int i = 0;i < wheels.Length;i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }


    //metodo per recuperare gli oggetti programmaticamente anzichè da editor
    //miglioriamo la visibilità delle variabili, non le tocchiamo dall'editor
    private void getObjects()
    {
        //recupero lo script InputManager attaccato al kart
        IM = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
        centerOfMass = GameObject.Find("centromassa");
        rigidbody.centerOfMass = centerOfMass.transform.localPosition;
        //recuperiamo i due gameObjects padri
        wheelColliders = gameObject.transform.Find("wheelColliders").gameObject;
        wheelMeshes = gameObject.transform.Find("wheelMeshes").gameObject;
        //recuperiamo i gameObject figli di wheelCollider
        wheels[0] = wheelColliders.transform.Find("0").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("1").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("2").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("3").gameObject.GetComponent<WheelCollider>();
        //recuperiamo i gameObject figli di wheelMeshes
        wheelMesh[0] = wheelMeshes.transform.Find("0").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("1").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("2").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("3").gameObject;
    }

    //metodo per imprimere alla macchina una forza dall' alto verso il basso per mantenere la macchina
    //piantata a terra quando curva
    private void addDownForce()
    {
        rigidbody.AddForce(-transform.up * DownForceValue * rigidbody.velocity.magnitude);
    }
}
