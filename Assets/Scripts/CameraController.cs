using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player, cameraLookAt, cameraPosition;
    public float speed = 0;
    public float defaultFOV = 0, desiredFOV = 0;
    [Range(0,5)] public float smothTime = 0; //dato che è in un intervallo 0-5 si può anche allontanare nell'accelerazione velocemente (non succederebbe se range=0-1)
    private Controller controller; //recuperiamo riferimento al controllo


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        cameraLookAt = Player.transform.Find ("cameraLookAt").gameObject;
        cameraPosition = Player.transform.Find ("cameraConstraint").gameObject;
        controller = Player.GetComponent<Controller>();
        defaultFOV = Camera.main.fieldOfView;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        follow();
        boostFOV();

        //calcoliamo la velocità della camera in modo tale che possa seguire meglio il player
        //speed = (float)((controller.KPH >= 50) ? 20 : controller.KPH / 4);
    }

    //leghiamo la posizione del player a quella della camera
    //Nella scene noi controlliamo la posizione della camera tramite cameraConstraint, la sua posizione è quella della camera in game,
    //mentre cameraLookAt è l'offset cui punta la camera, se vogliamo la visuale dal volante lo posizioniamo sul volante, se invece
    //la vogliamo fuori dalla macchina lo posizioniamo posteriormente
    private void follow()
    {
        if (speed <= 23) //valore std per far variare il Field of View della camera quando accelera
            //con la variabile speed possiamo far variare la posizione della camera con la velocità del player
            speed = Mathf.Lerp(speed, controller.KPH / 4, Time.deltaTime);
        else
            speed = 23;
        gameObject.transform.position = Vector3.Lerp(cameraPosition.transform.position, transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt (cameraLookAt.gameObject.transform.position);
    }

    private void boostFOV()
    {
        if(Input.GetKey(KeyCode.LeftShift)) 
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, desiredFOV, Time.deltaTime * smothTime);
        else
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFOV, Time.deltaTime * smothTime);


    }
}
