using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trailController : MonoBehaviour
{
    public GameObject player;
    public Rigidbody rb ;
    public bool isMoving = true ;


    // Start is called before the first frame update
    void Start()
    {
      player = GameObject.Find("player");
      rb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      if(rb.velocity.y == 0 && rb.velocity.x == 0)
      {
        isMoving = false ;
      }
      if(player.GetComponent<gameController>().trigger)
      {
        isMoving = false ;
      }

      if(!isMoving)
      {
        transform.localScale = new Vector3(0.95f,0.95f,1.6f) ;
        //TODO flood fill algorithm. do i have to get all the positions ? (i got them in coordinates on gameController)
        gameObject.tag = "Trail";

        var cubeRenderer = gameObject.GetComponent<Renderer>();

        cubeRenderer.material.SetColor("_Color", Color.blue);
      }
    }

}
