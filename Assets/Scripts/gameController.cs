using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool isCreated = false;
    public bool calculatable = false;
    public bool inTrail = false;
    public float Percentage = 100 ;
    public int[,] fillArea = new int[row,col];
    public Slider slider;
    public bool win = false;
    public GameObject sliderCanvas;
    public GameObject winScreen;
    public GameObject loseScreen;

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
          sliderCanvas = GameObject.Find("Slider");
          winScreen = GameObject.Find("WinScreen");
          winScreen.SetActive(false);
          loseScreen = GameObject.Find("LoseScreen");
        loseScreen.SetActive(false);
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
      Direction = data.Direction.ToString() ;
      Debug.Log(Direction);

    }

    private void FixedUpdate()
    {
        if(win)
        {
            gameOver = true;
            sliderCanvas.SetActive(false);
            winScreen.SetActive(true);
        }
        else
        {
            if(gameOver)
            {
                sliderCanvas.SetActive(false);
                loseScreen.SetActive(true); 
            }
        }


      slider.value = 100 - Percentage;
      if(fillArea[(int)transform.position.x , (int)transform.position.y] == 0)
        {
            trigger = false;
        }

        if (calculatable)
        {
            putin();
            floodfill(); 
        }
      
        //TODO flood fill floodfill() must be used very very less if i can . Must find good method to find if a player stopped moving. Or entered a Trail again.
        if (!gameOver)
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
    public void putin()
    {
        calculatable = false;
        if (fillArea[(int)transform.position.x, (int)transform.position.y] == 0)
        {
            Instantiate(trailPrefab, transform.position + new Vector3(0, 0, 0.4f), Quaternion.identity);
            fillArea[(int)transform.position.x, (int)transform.position.y] = 3;
        }

    }

    public void floodfill()
    {
        int area1 = 0 ;
        int area2 = 0 ;
        Vector2 fillStart = new Vector2(transform.position.x , transform.position.y) ;
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                if (area1 < 1 && area2 < 1)
                {
                    if (fillArea[r, c] == 0) fillStart = new Vector2(r,c); 
                }

            }
        }

        Debug.Log("Calculating the area .. "); 
        fillingAlgorithm((int)fillStart.x ,(int)fillStart.y);


        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                if(fillArea[r,c] == 1) area1 += 1;
                if(fillArea[r,c] == 0) area2 += 1;
            }
        }

        Debug.Log("Finding the small area");

        Debug.Log(new Vector2(area1, area2));

        Debug.Log("Found The Small Area ! ");

        for (int r = 0; r < row; r++)
          {
              for (int c = 0; c < col; c++)
              {
                if(area1 >= 1 && area2 >= 1)
                {
                    if (area1 > area2) { if (fillArea[r, c] == 0) fillArea[r, c] = 2; }
                    else { if (fillArea[r, c] == 1) fillArea[r, c] = 2; }
                }

              }
          }

        Debug.Log("Painted with 3"); 
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                if(fillArea[r,c] == 1) fillArea[r,c] = 0;
                if (fillArea[r, c] == 2)
                {
                    Instantiate(trailPrefab, new Vector3(r, c, 0.4f), Quaternion.identity);
                    fillArea[r, c] = 3;
                }
            }
        }
        Debug.Log("Instantiating Objects");
        if(area1> area2) Percentage = ((float)area1 / 273) * 100;
        else Percentage = ((float)area2 / 273) * 100;


        if((100-Percentage) > 80)
        {
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if (fillArea[r, c] == 0)
                    {
                        Instantiate(trailPrefab, new Vector3(r, c, 0.4f), Quaternion.identity);
                        fillArea[r, c] = 3;
                    }
                }
            }
            win = true; 
        }
        calculatable = false;
        
    }

    void fillingAlgorithm(int x , int y)
    {
      if(x == row) return ;
      if(x < 0)return ;
      if(y == col) return ;
      if(y < 0)return ;
      if(fillArea[x,y] > 0) return;
      fillArea[x,y] = 1 ;
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

      if (target.tag == "redCube")
       {
          Debug.Log("GameOver");
          gameOver = true;
       }

    }
    

    private void OnTriggerExit(Collider target)
    {

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
      if(fillArea[(int)orgPos.x, (int)orgPos.y] == 0) Instantiate(trailPrefab, orgPos + new Vector3(0,0,0.4f) , Quaternion.identity);
      fillArea[(int)orgPos.x, (int)orgPos.y] = 3 ;


       while (elapsedTime < timeToMove)
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
