/*
 * Coded By                             : ***** *****
 * Date                                 : 2018-06-03
 * Description                          : This class controlls food objects. It also instantiates scoreDisplay objects.
 *                                          Sorry it got chunky. This project evolved as I learned so it's design isn't
 *                                          always intuitive.
 */
using UnityEngine;


public class FoodController : MonoBehaviour {

    //public varibles
	public float ySpeed;
    public float xRotateRange, yRotateRange, zRotateRange;
	public int scoreValue;
    public bool isBonusItem;
    public int bonusPointMultiplier;
    public float bonusTimeDuration;
    public bool causesExplosion;
    public float explosionIntensity;
    public float showTextTime;
    public GameObject hasBeenEatten;
    public GameObject ScoreDisplay;
    public AudioSource audioSource;

    //private variables
    private float rotateX, rotateY, rotateZ;
    private float xSpeed;
	private float yMax;
	private float newX, newY;
    private float camWidth, camHeight;
    private Vector3 rotationRate;
	private Vector3 velocity;
    private AudioSource gameAudio;
    private SoundController soundController;
    private Vector3 pos;
    private Quaternion rot;
    private GameObject score;
    private GameObject myScoreDisplay;
    private GameObject foodSpawner;
    private Camera mainCamera;


    
    
    
    // Use this for initialization
	void Start () {

        //initialize the camera
        mainCamera = Camera.main;
        camHeight = mainCamera.orthographicSize;
        camWidth = camHeight * mainCamera.aspect;

        //just telling it where to send sound effects
        soundController = audioSource.GetComponent<SoundController>();

        //spawn at food spawner
        foodSpawner = GameObject.Find("Spawner");
        
        //set the score value for eatting the food
        score = GameObject.Find("Score");
    
        //randomly set the food to move left or right
        xSpeed = ySpeed / 50;
        if (Random.RandomRange(0.0f, 1.0f) > 0.5f)
        {
            xSpeed = -xSpeed;
        }

        //set the velocity of the food
        velocity.Set(xSpeed, ySpeed, 0);
        gameObject.GetComponent<Rigidbody>().velocity = velocity;

        //create a random rotation rate within a given range for this given food item
        rotateX = Random.RandomRange(0, xRotateRange);
        rotateY = Random.RandomRange(0, yRotateRange);
        rotateZ = Random.RandomRange(0, zRotateRange);
        rotationRate = gameObject.transform.eulerAngles = new Vector3(
            gameObject.transform.eulerAngles.x + rotateX,
            gameObject.transform.eulerAngles.y + rotateY,
            gameObject.transform.eulerAngles.z + rotateZ
            );
    }

	// Update is called once per frame
	void Update () {

        //rotate the object
        transform.Rotate(rotationRate * Time.deltaTime);

        //checks for new screen touch input
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            //Raycast to see if its a hit
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            //Ok I guess this is really where the checking happens
            if (Physics.Raycast(raycast, out raycastHit))
            {
                Eatten();
            }
        }

        //if the food has left the screen
        if ((transform.position.x > (camWidth + 5)) || (transform.position.x < -(camWidth + 5)) || (transform.position.y > (camWidth + 5))) {
            Destroy(gameObject);
        }
	}

    //everything needed to snack on some food.
    //this is activated when food is tapped on to android or clicked on the computer
    void Eatten () {

        //play a random purr
		soundController.NomPurr ();
        //change the text display to the name of the current eatten item
        hasBeenEatten.GetComponent<EattenController>().changeText(gameObject);
        //display the score gained
        score.GetComponent<ScoreController>().ChangeScore(scoreValue);
        instantiateScoreDisplay();

        //checks to see if this food item causes a "fruit explosion" bonus
        if (causesExplosion)
        {
            foodSpawner.GetComponent<FoodSpawner>().Explosion(explosionIntensity);
        }

        //checks to see if this food item gives a bonus multiplier to food point values.
        if (isBonusItem)
        {
            score.GetComponent<ScoreController>().initiateBonusPeriod(bonusPointMultiplier, bonusTimeDuration);
        }

        //destroy/eat this food
        Destroy (gameObject);
	}
    
    //if you click on food, cat eats.
	void OnMouseOver () {
		if (Input.GetMouseButtonDown (0)) {
			Eatten ();
		}
	}

    //create the pop up display of the score
    void instantiateScoreDisplay ()
    {
        pos.Set(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        rot.Set(0, 0, 0, 0);
        myScoreDisplay = Instantiate(ScoreDisplay, pos, rot);
        myScoreDisplay.transform.position = pos;
        myScoreDisplay.GetComponent<ScoreDisplayController>().setScore();
    }

}
