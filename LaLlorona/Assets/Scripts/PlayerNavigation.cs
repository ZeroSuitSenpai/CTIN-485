using UnityEngine;
using System.Collections;

public class PlayerNavigation : MonoBehaviour {

    NavMeshAgent navigationAgent;

	// Use this for initialization
	void Start () {
        navigationAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        ClickToMove();
	}

    //Pretty self explanatory 
    void ClickToMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseClick;

            if (Physics.Raycast(mouseRay, out mouseClick, 100))
            {
                navigationAgent.SetDestination(mouseClick.point);
            }
        }
    }
}  
