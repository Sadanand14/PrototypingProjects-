using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {

    private Slider timer;
    private float maxTime;

	// Use this for initialization
	void Start () {
        timer = this.GetComponent<Slider>();
        timer.maxValue = maxTime;
        timer.minValue = 0f;
        timer.value = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        timer.value+= Time.deltaTime;
        if (timer.value >= timer.maxValue)
        {
            GameObject.FindObjectOfType<LevelManagerScript>().GameComplete();
        }
	}

    public void SetMaxtTime(float x)
    {
        maxTime = x;
    }
}
