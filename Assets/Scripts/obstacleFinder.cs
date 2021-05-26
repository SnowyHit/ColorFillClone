using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleFinder : MonoBehaviour
{
    bool justOnce = true ;
    bool obstacle =false ;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update()
    {
       if(justOnce)
        {
            if (obstacle) player.GetComponent<gameController>().fillArea[(int)transform.position.x, (int)transform.position.y] = 4;
            else player.GetComponent<gameController>().fillArea[(int)transform.position.x, (int)transform.position.y] = 0;
            justOnce = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            obstacle = true; 
        }
       
    }
}
