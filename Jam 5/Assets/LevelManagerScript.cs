using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour {

    private float Score1, Score2, Score3, Score4, Score5, Score6, Score7;
    public float Health1, Health2, Health3, Health4, Health5, Health6, Health7;

    // Use this for initialization
    void Start () {
        Score1 = 0;
        Score2 = 0;
        Score3 = 0;
        Score4 = 0;
        Score5 = 0;
        Score6 = 0;
        Score7 = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public float Get1(){return Score1;}
    public float Get2(){return Score2;}
    public float Get3(){return Score3;}
    public float Get4(){return Score4;}
    public float Get5(){return Score5;}
    public float Get6(){return Score6;}
    public float Get7(){return Score7;}
    
    public void Set1(int x) { Score1 += x; }
    public void Set2(int x) { Score2 += x; }
    public void Set3(int x) { Score3 += x; }
    public void Set4(int x) { Score4 += x; }
    public void Set5(int x) { Score5 += x; }
    public void Set6(int x) { Score6 += x; }
    public void Set7(int x) { Score7 += x; }
}
