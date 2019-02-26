using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteroShower : MonoBehaviour {

    public GameObject meteorprefab;
    public List<GameObject> meteors;

    private int maxMeteorCount;
    public int meteorsperround;//Defines the number of meteors for each wave of meteors
    private int currentWindow;
    public Camera camera;
    private Vector2 viewMax, viewMin;
    public float speed;

  
	// Use this for initialization
	void Start () {
        Vector2 bounds = Vector2.zero;
        Vector2 Cameraposition = new Vector2(camera.transform.position.x, camera.transform.position.y);
        if (camera.orthographic)
           bounds = new Vector2(camera.orthographicSize * Screen.width / Screen.height, camera.orthographicSize);
        else
        {
            Debug.LogError("Camera is not orthographic!", camera);
        }
        viewMax = Cameraposition + bounds;
        viewMin = Cameraposition - bounds;
        //Debug.Log(viewMax + "   " + viewMin);
        Vector2 pos = Vector2.zero;
        maxMeteorCount = meteorsperround;
        currentWindow = 0;
        for(int i =0;i<maxMeteorCount;i++)
        {
            pos.x = Random.Range(viewMin.x, viewMax.x);
            pos.y = Random.Range(viewMax.y+2, viewMax.y+5);
            meteors.Add(Instantiate(meteorprefab, pos, Quaternion.identity));
            meteors[i].SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        Vector2 pos = Vector2.zero;
        Vector3 target = Vector3.zero;
        for (int i = currentWindow;i<currentWindow+meteorsperround;i++)
        {
            target.x = meteors[i].transform.position.x-(5*((i%2==0)?1:-1));
            target.y = viewMin.y-5;
            //Debug.Log(meteors[i].activeSelf == false);
            if (meteors[i].activeSelf == false)
            {
                pos.x = Random.Range(viewMin.x+2, viewMax.x+2);
                pos.y = Random.Range(viewMax.y + 2, viewMax.y + 5);
                meteors[i].transform.position = pos;
                meteors[i].SetActive(true);
                meteors[i].transform.position = Vector3.MoveTowards(meteors[i].transform.position, target, speed * Time.deltaTime);
            }
            else
            {
                meteors[i].transform.position = Vector3.MoveTowards(meteors[i].transform.position, target, speed * Time.deltaTime);
            }
        }
        for(int i=0;i<maxMeteorCount;i++)
        {
            if(meteors[i].transform.position.y < viewMin.y)
            {
                meteors[i].SetActive(false);
            }
        }
        currentWindow += meteorsperround;
        currentWindow %= maxMeteorCount;
	}
}
