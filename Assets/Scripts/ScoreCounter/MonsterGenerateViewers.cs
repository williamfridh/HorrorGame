/**
 * This script is responsible for generating viewers based on the type of generation.
 * It can generate viewers linearly or exponentially. It also adds settings to monsters
 * such as:
 * - viewerAddAmount is the amount of views added be iteration.
 * - viewerRemoveAmount is the amount of views removed be iteration.
 * - generatorSpeed is the speed of the generation.
 *
 * How to use:
 * - Attach this script to a monster object.
 * - Change inFieldOfView to true if the monster is in the field of view.
 * - Set the score object in the Unity Editor.
 *
 * Authors: William Fridh, Pontus Åhlin
 */

using UnityEngine;

public class MonsterGenerateViewers : MonoBehaviour
{

    private float addSpeed = 0f;                                    // Speed of the viewer adding (positive).
    private float removeSpeed = 0f;                                 // Speed of the viewer adding (positive).
    private float viewsGeneratedInRow = 0f;                         // Used for preventing errors in the player's viewer amount.
    
    [Tooltip("Point to player score script which holds viewer amount and likes.")]
    [SerializeField] PlayerScore PlayerScore;                       // Reference to the score script.
    [Tooltip("The speed of linear and exponetial (x^2) increase. Must be more than 0 and 1.1 if exponential.")]
    [SerializeField] public float viewerAddAmount = 1f;                    
    [Tooltip("The speed of linear and exponetial (x^2) decrease. Must be more than 0 if linear and 1 if exponential.")]
    [SerializeField] float viewerRemoveAmount = 0.5f;               // Amount of viewers removed per second.
    [Tooltip("The interval speed at which the viewers should be generated. Must be more than 0.")]
    [SerializeField] float generatorSpeed = 1.0f;                   // Speed of the viewer generation.
    enum IncreaseAndDecrease { Linear, Exponential };               // Type of viewer generation.
    [Tooltip("Mathmatical function for increasing and decreasing number of viewers added.")]
    [SerializeField] IncreaseAndDecrease increaseAndDecrease;
    [Tooltip("Update this value to change if viewers are added or removed.")]
    public bool inFieldOfView = false;                             
    [Tooltip("If the monster is requested by a viewer")]
    public bool viewerRequest = false;                             
     
    public float mult = 1.0f;
    [SerializeField] float multDec = 0.01f;



    // Start is called before the first frame update
    void Start()
    {
        //Sets the main camera to PlayerScore field.
        //Otherwise spawning of monsters doesn't work
        PlayerScore = Camera.main.GetComponent<PlayerScore>();

        // Perform all necessary checks to prevent errors.
        // Note that having the checks here means no checking while updating the
        // value of the components live.
        if (PlayerScore == null) {
            Debug.LogError("PlayerScore is not set. Removing MonsterGenerateViews script.");
            Destroy(this);
            return;
        }
        if (generatorSpeed == 0f)
            Debug.LogWarning("Generator speed is 0, which is invalid. It'll be set to 1 instead.");
            generatorSpeed = 1.0f;
        if (viewerAddAmount < 0f && increaseAndDecrease == IncreaseAndDecrease.Linear) {
            Debug.LogWarning("viewerAddAmount speed is less than 0, which is invalid. It'll be set to 1 instead.");
            viewerAddAmount = 1f;
        }
        if (viewerAddAmount < 1.1f && increaseAndDecrease == IncreaseAndDecrease.Exponential){
            Debug.LogWarning("viewerAddAmount speed is less than 1.1, which is invalid. It'll be set to 1.1 instead.");
            viewerAddAmount = 1.1f;
        }
        if (viewerRemoveAmount < 1.1f && increaseAndDecrease == IncreaseAndDecrease.Exponential){
            Debug.LogWarning("viewerRemoveAmount speed is less than 1.1, which is invalid. It'll be set to 1.1 instead.");
            viewerRemoveAmount = 1.1f;
        }
        
        // Add or remove viewers based on the current status.
        InvokeRepeating("AdjustViewers", generatorSpeed, generatorSpeed);
    }

    /**
      * Adjust viewers.
      *
      * This function the add and remove speeds based on the current state
      * of the view field. It also updates the player's viewer amount as well
      * as a local sum of viewers generated by this script used for preventing
      * error in the update of the players total amount of viewers.
      */
    void AdjustViewers() {

        // Adjust velocity.
        if (inFieldOfView) {
            AddViewers();                                       // Update addSpeed.
            mult -= multDec;
            print(mult);
            if(mult < 0){
                mult = 0;
            }
        } else if (viewsGeneratedInRow > 0f) {                  // Prevent removing viewers if there are none active generated by this script.
            RemoveViewers();                                    // Update removeSpeed.
        }
        float tmp = 0;                                          // Temporary variable for preventing removing too much from the real viewer amount.
        viewsGeneratedInRow += addSpeed - removeSpeed;          // Update local sum.
        if (viewsGeneratedInRow < 0f)                           // Generate tmp sum to fix faulty real viewer amount.
            tmp = -viewsGeneratedInRow;
        PlayerScore.viewers += addSpeed - removeSpeed + tmp;    // Update player viewer amount.
        PlayerScore.likes += (int) (addSpeed * (PlayerScore.viewers)/5);
        if (viewsGeneratedInRow <= 0f) {
            viewsGeneratedInRow = 0f;                           // Prevent faulty sum.
            addSpeed = 0f;                                      // Reset faulty add speed.
            removeSpeed = 0f;                                   // Reset faulty remove peed.
        }

    }

    /**
      * Add viewers.
      *
      * This function add viewers based on the type of viewer generation.
      * It adds in different ways depending on the selected type of
      * generation. Note how it also decreases the removeSpeed.
      */
    void AddViewers() {
        switch (increaseAndDecrease) {
            case IncreaseAndDecrease.Linear:
                addSpeed = viewerAddAmount;
                break;
            case IncreaseAndDecrease.Exponential:
                if (addSpeed == 0f)             
                    addSpeed = viewerAddAmount;
                else
                    addSpeed *= viewerAddAmount;
                break;
        }
        addSpeed *= mult;
        if (removeSpeed > 0) {                              // De crease the counterpart.
            removeSpeed -= addSpeed;
            if (removeSpeed < 0f || mult == 0)                           // Prevent faulty speed.
                removeSpeed = 0f;
        }
    }

    /**
      * Remove viewers.
      *
      * This function remove viewers based on the type of viewer generation.
      * It removes in different ways depending on the selected type of
      * generation. Note how it also decreases the Speed.
      */
    void RemoveViewers() {
        switch (increaseAndDecrease) {
            case IncreaseAndDecrease.Linear:
                removeSpeed = viewerRemoveAmount;
                break;
            case IncreaseAndDecrease.Exponential:
                if (removeSpeed == 0f)             
                    removeSpeed = viewerRemoveAmount;
                else
                    removeSpeed *= viewerRemoveAmount;
                break;
        }
        if (addSpeed > 0f) {                            // De crease the counterpart.
            addSpeed -= removeSpeed;
            if (addSpeed < 0f)                          // Prevent faulty speed.
                addSpeed = 0f;
        }
    }

}

