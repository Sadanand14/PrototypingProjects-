// Jared White
// February 24th, 2019
// TowerCube.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]

/// <summary>
/// Represents the moving Player cubes of the tower, and enabled them to move left and right at set speeds
/// </summary>

public class TowerCube : MonoBehaviour {

    private Tower tower;
    private int ID;
    [Serializable]

    public class InputKeys
    {
        public KeyCode leftKey = KeyCode.LeftArrow;
        public KeyCode rightKey = KeyCode.RightArrow;
    }

    public void SetID(int x)
    {
        ID = x;
    }

    public int GetID()
    {
        return ID;
    }
    // Instances
    #region Members
    private float cubeSpeed = 1f;           // The speed at which this cube can navigate
    private bool invincible = false;         // Can this tower piece be damaged

    public static float KnockSpeed = 10f;   // The global speed at which all cubes will be knocked downwards 
    public static float FallSpeed = 5f;     // The global speed at which all cubes will fall

    public InputKeys movementKeys;          // The keys with which to move

    public bool testDamage = false;         // For Debugging only!

    private Vector2 targetDifference = Vector2.zero;
    private Vector2 currentDifference = Vector2.zero;
    private bool falling = false;           // Is this block currently falling?
    private bool knocking = false;          // Is this block currently being knocked down?

    private float invincibleTimer = 0;
    private const float INVINCIBLE_TIME = 1.2f;

    private Color startColor;
    #endregion


    // Properties
    #region Properties
    /// <summary>
    /// Get or set the speed this block can move at.
    /// </summary>
    public float Speed
    {
        get { return cubeSpeed; }
        set
        {
            // Ensure the speed is not negative
            if (value < 0) { value *= -1; }
            cubeSpeed = value;
        }
    }

    /// <summary>
    /// Get/set whether this cube is currently being knocked to the ground if it's not currently falling.
    /// If being set to true, this cube will become invincible.
    /// </summary>
    public bool Knocking
    {
        get { return knocking; }
        set
        {
            if (!falling)
            {
                if (value)
                {
                    Invincible = true;
                    invincibleTimer = Time.time + INVINCIBLE_TIME;
                }
                knocking = value;
            }
        }
    }

    /// <summary>
    /// Get/set whether this cube is currently falling to the ground if it's not currently being knocked
    /// </summary>
    public bool Falling
    {
        get { return falling; }
        set
        {
            if (!knocking)
            {
                falling = value;
            }
        }
    }

    /// <summary>
    /// Get/set whether this cube is invincible.
    /// Cannot be set from invincible if the cube is currently being knocked down.
    /// </summary>
    public bool Invincible
    {
        get { return invincible; }
        set
        {
            if (!Knocking)
            {
                invincible = value;
            }

            if (value)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = startColor;
            }
        }
    }
    #endregion


    // Use this for initialization
    void Start () {
        tower = GameObject.FindObjectOfType<Tower>();
        startColor = GetComponent<SpriteRenderer>().color;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Move the cube if it's currently being knocked down
        MoveKnocking();

        // Move the cube if it's currently falling
        MoveFalling();

        if (Input.GetKey(movementKeys.leftKey))
        {
            MoveLeft();
        }
        if (Input.GetKey(movementKeys.rightKey))
        {
            MoveRight();
        }

        if (invincible && Time.time >= invincibleTimer)
        {
            Invincible = false;
        }
	}

   

    // METHODS
    #region Methods

    /// <summary>
    /// Move the player cube left if not falling
    /// </summary>
    public void MoveLeft()
    {
        if (!falling && !knocking)
        {
            transform.position -= new Vector3(cubeSpeed * Time.deltaTime, 0);
            SnapMoveInputs();
        }
    }


    /// <summary>
    /// Move the player cube right if not falling
    /// </summary>
    public void MoveRight()
    {
        if (!falling && !knocking)
        {
            transform.position += new Vector3(cubeSpeed * Time.deltaTime, 0);
            SnapMoveInputs();
        }
    }

    
    /// <summary>
    /// Make sure the blocks stay in their lane.
    /// </summary>
    private void SnapMoveInputs()
    {
        // If it's not the bottom block
        if (transform.parent.GetComponent<Tower>() == null)
        {
            if (transform.position.x + GetComponent<SpriteRenderer>().bounds.extents.x
                < transform.parent.position.x - transform.parent.GetComponent<SpriteRenderer>().bounds.extents.x)
            {
                transform.position -= new Vector3(
                    (transform.position.x + GetComponent<SpriteRenderer>().bounds.extents.x) - (transform.parent.position.x - transform.parent.GetComponent<SpriteRenderer>().bounds.extents.x),
                    0);
            }
            else if (transform.position.x - GetComponent<SpriteRenderer>().bounds.extents.x
                > transform.parent.position.x + transform.parent.GetComponent<SpriteRenderer>().bounds.extents.x)
            {
                transform.position += new Vector3(
                    (transform.parent.position.x + transform.parent.GetComponent<SpriteRenderer>().bounds.extents.x) - (transform.position.x - GetComponent<SpriteRenderer>().bounds.extents.x),
                    0);
            }
        }

        // If it's the bottom block
        else
        {
            if (transform.position.x < -6)
            {
                transform.position = new Vector3(-6, transform.position.y, transform.position.z);
            }
            else if (transform.position.x > 6)
            {
                transform.position = new Vector3(6, transform.position.y, transform.position.z);
            }
        }
    }

    
    /// <summary>
    /// Move the cube downwards at global knockdown speed
    /// </summary>
    private void MoveKnocking()
    {
        if (knocking)
        {
            transform.position += new Vector3(0, -KnockSpeed * Time.deltaTime);
        }
    }


    /// <summary>
    /// Move the cube downwards at global fall speed
    /// </summary>
    private void MoveFalling()
    {
        if (falling)
        {
            // Fall a speed towards the target
            Vector3 increment = new Vector3(
                targetDifference.normalized.x * FallSpeed * Time.deltaTime,
                targetDifference.normalized.y * FallSpeed * Time.deltaTime);

            currentDifference += (Vector2)increment;
            transform.position += increment;


            // If it's gone past the target y, put it back and stop falling
            if (targetDifference.y - currentDifference.y > 0)
            {
                Vector2 fixDif = targetDifference - currentDifference;
                transform.position += (Vector3)fixDif;
                
                StopFalling();
                return;
            }
        }
    }

    
    /// <summary>
    /// Start falling, and set the difference of location to land in
    /// </summary>
    /// <param name="difference">The difference to fall in</param>
    public void StartFalling(Vector2 difference)
    {
        Falling = true;
        targetDifference += difference;
    }


    /// <summary>
    /// Set falling members back to default/neutral state
    /// </summary>
    public void StopFalling()
    {
        Falling = false;
        targetDifference = Vector2.zero;
        currentDifference = Vector2.zero;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Respawn")
        {
            Debug.Log("I got hit");
            if (tower) { tower.HitCube(this.gameObject); }
            else { Debug.Log("tower not found"); }
            collision.gameObject.SetActive(false);
            GameObject.FindObjectOfType<LevelManagerScript>().LogScore(ID, -500);
        }
    }

    public int CheckPoistion()
    {
        for (int i = 0; i < tower.playerCubes.Count;i++)
        {
            if (this.name == tower.playerCubes[i].name)
            {
                return i;
            }
        }
        return 10;
    }
}