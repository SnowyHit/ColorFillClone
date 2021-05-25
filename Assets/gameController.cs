using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public string Direction ;
    Rigidbody rb ;
    private bool isMoving ;
    private Vector3 orgPos , targetPos;
    private float timeToMove = 0.2f ;
    public float speed =2f;
    public float rayLenght = 1.0f ;
    public Transform gridPrefab;
    public Transform trailPrefab;
    public static int row = 13;
    public static int col = 21;
    public bool trigger ;
    public bool gameOver = false ;
    public bool isCreated = false  ;
    List<Vector2> coordinates = new List<Vector2>();
    public int[,] fillArea = new int[row,col];

    int LM = 1 << 8 ;
    // some function that builds the grid
    // Start is called before the first frame update
    private void Awake()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }
    private void Start()
    {
      for (int r = 0; r < row; r++)
      {
          for (int c = 0; c < col; c++)
          {
              Instantiate(gridPrefab, new Vector3(r, c, 1f), Quaternion.identity);
              fillArea[r,c] = 0  ;
          }
      }

      rb=gameObject.GetComponent<Rigidbody>();
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
      Direction = data.Direction.ToString() ;
      Debug.Log(Direction);

    }

    private void FixedUpdate()
    {
      if((rb.velocity.x == 0 && rb.velocity.y == 0) && !isCreated)
      {
        Debug.Log("init");
        Instantiate(trailPrefab, transform.position + new Vector3(0,0,0.4f) , Quaternion.identity);
        isCreated = true ;
        coordinates.Add(new Vector2(transform.position.x , transform.position.y));
        foreach (var coordinate in coordinates)
         {
            fillArea[(int)coordinate.x , (int)coordinate.y] = 3  ;
         }
      }

      if(!gameOver)
      {
          if(Direction == "Up" && !isMoving)
          {
            if(!Physics.Raycast(transform.position, Vector3.up, rayLenght, LM))
            StartCoroutine(MovePlayer(Vector3.up));
          }
          if(Direction == "Down" && !isMoving)
          {
            if(!Physics.Raycast(transform.position, Vector3.down, rayLenght,LM))
            StartCoroutine(MovePlayer(Vector3.down)) ;
          }
          if(Direction == "Right" && !isMoving)
          {
            if(!Physics.Raycast(transform.position, Vector3.right, rayLenght,LM))
            StartCoroutine(MovePlayer(Vector3.right)) ;
          }
          if(Direction == "Left" && !isMoving)
          {
            if(!Physics.Raycast(transform.position, Vector3.left, rayLenght,LM))
            StartCoroutine(MovePlayer(Vector3.left)) ;
          }
      }
    }

    void floodfill()
    {
        int area1 = 0 ;
        int area2 = 0 ;
        Vector2 fillStart = new Vector2(transform.position.x , transform.position.y) ;
        if(!Physics.Raycast(transform.position, Vector3.up, rayLenght)) fillStart += new Vector2(0,1);
        else if(!Physics.Raycast(transform.position, Vector3.down, rayLenght)) fillStart += new Vector2(0,-1);
        else if(!Physics.Raycast(transform.position, Vector3.right, rayLenght)) fillStart += new Vector2(1,0);
        else if(!Physics.Raycast(transform.position, Vector3.left, rayLenght)) fillStart += new Vector2(-1,0);
        Debug.Log(fillStart);

        fillingAlgorithm((int)fillStart.x ,(int)fillStart.y);
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                if(fillArea[r,c] == 1) area1 += 1;
                if(fillArea[r,c] == 0) area2 += 1;
            }
        }

          for (int r = 0; r < row; r++)
          {
              for (int c = 0; c < col; c++)
              {
                if(area1 < 1 && area2 < 1)
                {
                  if(area1 > area2) if(fillArea[r,c] == 0) fillArea[r,c] = 3;
                  else if(fillArea[r,c] == 1) fillArea[r,c] = 3;
                }

              }
          }


        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                if(fillArea[r,c] == 1) fillArea[r,c] = 0;
                if(fillArea[r,c] == 3){
                  Instantiate(trailPrefab, new Vector3(r,c,0.4f) , Quaternion.identity);
                  }
            }
        }


    }

    void fillingAlgorithm(int x , int y)
    {
      Debug.Log("Got in");
      if(x == row) return ;
      if(x < 0)return ;
      if(y == col) return ;
      if(y < 0)return ;
      if(fillArea[x,y] == 3) return;
      if(fillArea[x,y] == 1) return;
      fillArea[x,y] = 1 ;
      Debug.Log(new Vector2(x,y));
      fillingAlgorithm(x+1 , y) ;
      fillingAlgorithm(x-1 , y) ;
      fillingAlgorithm(x , y+1) ;
      fillingAlgorithm(x , y-1) ;
      return;
    }
    private void OnTriggerEnter(Collider target)
    {
        if(target.tag == "Trail")
       {
          trigger = true ;
       }

      if(target.tag == "deadly")
      {
        Debug.Log("GameOver");
        gameOver = true ;
      }

    }
    private void OnTriggerExit(Collider target)
    {
        if(target.tag == "Trail")
       {
          trigger = false ;
          isCreated = false ;
       }
       if(target.tag == "lowTrail")
      {
         target.tag = "deadly";
      }
    }
    private IEnumerator MovePlayer(Vector3 direction)
    {
      isMoving = true ;

      float elapsedTime = 0 ;

      orgPos = transform.position ;
      targetPos = orgPos + direction ;
      Instantiate(trailPrefab, orgPos+ new Vector3(0,0,0.4f) , Quaternion.identity);
      coordinates.Add(new Vector2(orgPos.x , orgPos.y));


      while(elapsedTime < timeToMove)
      {
        rb.velocity = direction * speed ;
        elapsedTime += Time.deltaTime ;
        yield return null ;
      }

      transform.position = targetPos ;
      rb.velocity = direction * 0 ;

      isMoving = false ;
    }
}
