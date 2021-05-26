using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redCubeController : MonoBehaviour
{
    public float MoveSpeed;
    public float right = 0;
    float left = 0;
    public float top = 0;
    float bottom = 0;
    Vector3 targetposplus;
    Vector3 targetposminus;
    Vector3 temp;
    Vector3 Xtargetposplus;
    Vector3 Xtargetposminus;
    Vector3 Xtemp;
    float Xright;
    float Xleft;
    float Ytop;
    float Ybottom;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        if (right > 0) top = 0;
        else if (top > 0) right = 0;



        Xright = transform.position.x + right;
        Xleft = transform.position.x - left;
        Ytop = transform.position.y + top;
        Ybottom = transform.position.y - bottom;
        targetposplus = new Vector3(transform.position.x, Ytop, 0);
        targetposminus = new Vector3(transform.position.x, Ybottom, 0);
        temp = targetposplus;
        Xtargetposplus = new Vector3(Xright, transform.position.y, 0);
        Xtargetposminus = new Vector3(Xleft, transform.position.y, 0);
        Xtemp = Xtargetposplus;
    }

    void FixedUpdate()
    {

        if (right == 0 && left == 0)
        {
            //top to bottom
            if (transform.position == temp)
            {
                if (temp == targetposplus) temp = targetposminus;
                else temp = targetposplus;
            }
            transform.position = Vector2.MoveTowards(transform.position, temp, MoveSpeed / 10);

        }
        else
        {

            if (transform.position == Xtemp)
            {
                if (Xtemp == Xtargetposplus) Xtemp = Xtargetposminus;
                else Xtemp = Xtargetposplus;
            }
            transform.position = Vector2.MoveTowards(transform.position, Xtemp, MoveSpeed / 10);
        }

    }
    private void OnTriggerStay(Collider target)
    {
        if (target.tag == "Trail")
        {
            Debug.Log("Destroying .. ");
            Destroy(gameObject, 0.2f);
        }

        if (target.tag == "deadly")
        {
            player.GetComponent<gameController>().gameOver = true;
        }
    }
   
}
