using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    //Variables
    public float horizSpeed = 40f;
    public float vertSpeed = 40f;
    public float camRotSpeed = 80f;
    public float cameraDistance = 10f;

    //Used to track if the camera will collide with anything
    float currentDistance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //Floats for camera movement
        float horizontal = Input.GetAxis("Horizontal") * horizSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * vertSpeed * Time.deltaTime;
        float rotation = Input.GetAxis("Rotation");

        //Translate the camera based on the floats specified above
        transform.Translate(Vector3.forward * vertical);
        transform.Translate(Vector3.right * horizontal);

        //If there is movement
        if (rotation != 0)
        {
            //Rotate the camera based on the above float
            transform.Rotate(Vector3.up, rotation * camRotSpeed * Time.deltaTime, Space.World);
        }

        //Raycast to determine if the camera will move through an object
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 100))
        {
            //Calculates the distance between the camera and object below it
            currentDistance = Vector3.Distance(transform.position, hit.point);
        }
        //If the camera is colliding with an object
        if (currentDistance != cameraDistance)
        {
            //Calculate a float to adjust the camera's height
            float difference = cameraDistance - currentDistance;
            //And adjust the camera's height
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3 (0, difference, 0), Time.deltaTime);
        }
    }
}
