#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-06 오후 3:38:16
 *	기능 : 
 *	
 *	Mouse Drag로 원하는 방향에 쏘는 공 컨트롤러.
 *	출처 : https://www.youtube.com/watch?v=2PZHOEAXrdY
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CShootingBallController : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EAddForceType
    {
        VelocityChange,
        AddForce_Force,
        AddForce_Impulse,
    }

    /* public - Field declaration            */

    public EAddForceType eAddForceType = EAddForceType.VelocityChange;

    public float idleTime = 5;                      //How long the player has to be inactive for the Help Gesture to begin animating
    public float shootingPowerX = 10;                //The amount of power which can be applied in the X direction
    public float shootingPowerY = 10;                //The amount of power which can be applied in the Y direction
    public bool explodeEnabled;                 //If you want to do something when the projectile reaches the last point of the trajectory
    public bool grabWhileMoving;                //Off means the player won't be able to shoot until the "ball" is still. On means they can stop the "ball" by clicking on it and shoot
    public bool usingHelpGesture;               //If you want to use the Help Gesture

    /* protected & private - Field declaration         */

    CTrajectoryDrawer _pTrajectoryCalculator = null;

    private Rigidbody2D ballRB;                 //The Rigidbody2D attached to the projectile the player will be shooting
    private GameObject ball;                    //The projectile the player will be shooting
    private GameObject ballClick;               //The area which the player needs to click in to activate a shot
    private GameObject helpGesture;             //The Help Gesture which will become active after a period of inactivity

    private Vector3 vecBallPos;                    //Position of the ball
    private Vector3 fingerPos;                  //Position of the pressed down finger/cursor on the screen 
    private Vector2 vecShotForce;                  //How much velocity will be applied to the ball

    private float idleTimer = 7f;               //How long the initial inactivity period will need to be before the Help Gesture shows up
    private bool ballIsClicked = false;         //If the cursor is hovering over the "Ball Click Area"
    private bool ballIsClicked2 = false;        //If the finger/cursor is pressing down in the "Ball Click Area" to activate the shot

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    // ========================================================================== //

    /* protected - Override & Unity API         */

    void Start()
    {
        _pTrajectoryCalculator = GetComponent<CTrajectoryDrawer>();

        ball = gameObject;                                          //Script has to be applied to the "ball"
        ballClick = GameObject.Find("Ball Click Area");         //BALL CLICK AREA MUST HAVE THE SAME NAME IN HIERARCHY AS IT DOES HERE OTHERWISE SHOOTING WON'T BE POSSIBLE AND OTHER ERRORS MAY OCCUR

        if (usingHelpGesture == true)
        {                               //If you're using the Help Gesture
            helpGesture = GameObject.Find("Help Gesture");          //HELP GESTURE MUST HAVE THE SAME NAME IN HIERARCHY AS IT DOES HERE IF usingHelpGesture is true
        }
        ballRB = GetComponent<Rigidbody2D>();                       //"Ball"'s Rigidbody2D is applied to ballRB
    }

    void Update()
    {
        if (usingHelpGesture == true)
        {                               //If you're using the Help Gesture...
            helpGesture.transform.position = new Vector3(vecBallPos.x, vecBallPos.y, vecBallPos.z);  //It will have the same position as the "ball"
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);    //Used to determine if the finger/cursor is on the Ball Click Area

        if (hit.collider != null && ballIsClicked2 == false)
        {                   //If the the location of the cursor is over anything and the player hasn't activated the shot already... (This has to be done first since something has to be applied to the hit.collider before asking what the name is)
            if (hit.collider.gameObject.name == ballClick.gameObject.name)
            {   //and If the name of what the cursor is overlapping is the same as the "ball"'s name...
                ballIsClicked = true;                                           //First step of activating the shot is done
            }
            else
            {                                                           //If the name of what the cursor is overlapping is something other than the "ball"...
                ballIsClicked = false;                                          //Don't start activating the shot
            }
        }
        else
        {                                                               //If the cursor isn't overlapping anything or the shot is already activated...
            ballIsClicked = false;                                              //Don't activate/reactivate the shot
        }

        if (ballIsClicked2)
        {                                           //If shot is already activated...
            ballIsClicked = true;                                               //Keep ballIsClicked true for later
        }


        if ((ballRB.velocity.x * ballRB.velocity.x) + (ballRB.velocity.y * ballRB.velocity.y) <= 0.0085f)
        { //if the "ball" is moving really slow...
            ballRB.velocity = new Vector2(0f, 0f);                              //Make the "ball" stop moving
            idleTimer -= Time.deltaTime;                                        //Begin the timer for the Help Gesture

        }
        else
        {                                                               //If the "ball" is still moving fast enough...
            _pTrajectoryCalculator.DoActive_TrajectoryDots(false);                                    //Don't allow the trajectory to be shown. (This is for if you're in the process of aiming and something causes the ball to move)
        }


        if (usingHelpGesture == true && idleTimer <= 0f)
        {                       //If you're using the Help Gesture and the amount of time the player has been idle for has expired...
            helpGesture.GetComponent<Animator>().SetBool("Inactive", true); //Begin the Help animation
        }
        vecBallPos = ball.transform.position;                                      //ballPos is updated to the position of the "ball"

        if ((Input.GetKey(KeyCode.Mouse0) && ballIsClicked == true) && ((ballRB.velocity.x == 0f && ballRB.velocity.y == 0f) || (grabWhileMoving == true)))
        {   //If player has activated a shot										//when you press down
            ballIsClicked2 = true;                                              //Final step of activation is complete

            if (usingHelpGesture == true)
            {                                       //If you're using the Help Gesture...
                idleTimer = idleTime;                                           //It is reset
                helpGesture.GetComponent<Animator>().SetBool("Inactive", false);    //If the animation is playing, it will stop
            }

            fingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //The position of your finger/cursor is found
            fingerPos.z = 0;                                                    //The z position is set to 0

            if (grabWhileMoving == true)
            {                                       //If you've enabled shooting while the ball is moving
                ballRB.velocity = new Vector2(0f, 0f);                          //The "ball" stops moving
                ballRB.isKinematic = true;                                      //The "ball" isn't affected by other forces (it stays in the same spot)
            }

            //The distance between where the finger/cursor is and where the "ball" is when screen is being pressed
            Vector3 ballFingerDiff = vecBallPos - fingerPos;                               //The distance between the finger/cursor and the "ball" is found

            vecShotForce = new Vector2(ballFingerDiff.x * shootingPowerX, ballFingerDiff.y * shootingPowerY);  //The velocity of the shot is found

            if ((Mathf.Sqrt((ballFingerDiff.x * ballFingerDiff.x) + (ballFingerDiff.y * ballFingerDiff.y)) > (0.4f)))
            { //If the distance between the finger/cursor and the "ball" is big enough...
                _pTrajectoryCalculator.DoActive_TrajectoryDots(true);                             //Display the trajectory
            }
            else
            {
                _pTrajectoryCalculator.DoActive_TrajectoryDots(false);                              //Otherwise... Cancel the shot
                if (ballRB.isKinematic == true)
                {
                    ballRB.isKinematic = false;
                }
            }

            switch (eAddForceType)
            {
                case EAddForceType.VelocityChange:
                    _pTrajectoryCalculator.DoDrawTrajectory_VelocityChange(ballRB.gravityScale, vecBallPos, vecShotForce);
                    break;

                case EAddForceType.AddForce_Force:
                    _pTrajectoryCalculator.DoDrawTrajectory_Force(ballRB, vecBallPos, vecShotForce);
                    break;

                case EAddForceType.AddForce_Impulse:
                    _pTrajectoryCalculator.DoDrawTrajectory_Impulse(ballRB, vecBallPos, vecShotForce);
                    break;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {                               //If the player lets go...
            ballIsClicked2 = false;                                         //Aiming is no longer happening

            if (_pTrajectoryCalculator.Check_IsDrawDots())
            {                           //If the player was aiming...
                if (explodeEnabled == true)
                {
                    //If the player was shooting and explodeEnabled is true...
                }

                _pTrajectoryCalculator.DoActive_TrajectoryDots(false);                                //The trajectory will hide

                switch (eAddForceType)
                {
                    case EAddForceType.VelocityChange:
                        ballRB.velocity = vecShotForce; //The "ball" will have its new velocity
                        break;

                    case EAddForceType.AddForce_Force:
                        ballRB.AddForce(vecShotForce, ForceMode2D.Force);
                        break;

                    case EAddForceType.AddForce_Impulse:
                        ballRB.AddForce(vecShotForce, ForceMode2D.Impulse);
                        break;
                }

                if (ballRB.isKinematic == true)
                    ballRB.isKinematic = false;                             //It's no longer kinematic
            }
        }

    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test