using UnityEngine;
using System.Collections;

public class AudioMaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayTravelSFX()
    {
        AkSoundEngine.PostEvent("Play_Travel", gameObject);
    }
}
