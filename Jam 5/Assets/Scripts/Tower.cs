using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the whole tower of player cubes
/// </summary>
public class Tower : MonoBehaviour {

    // INSTANCE VARIABLES/MEMBERS
    #region Instances and Members

    public GameObject dirtSplashParticlePrefab;     // Prefab for crashing block particles
    public List<float> playerSpeeds;                // The various speeds for the players
    public List<float> damagePtsPerLevel;           // The tower damage taken depending on which cube height is hit (index 0 is bottom)


    public float baseDistanceFromBottom = .5f;              // How far the above this game object the bottom block should be
    [HideInInspector] public List<GameObject> playerCubes;  // The player cubes of this tower

    public float knockdownSpeed = 10f;  // The speed at which to knock blocks to the bottom of the speed
    public float fallSpeed = 5f;        // The speed at which blocks will fall
    public float raiseSpeed = 4f;       // The speed at which the overall tower will raise at

    public GameObject healthBar;        // Green HealthBar
    public float maxTowerHealth = 100f; // The maximum health the tower can be at
    private float currentHealth;        // Overall current health value of the tower
    
    private float targetYRise;          // helper member
    private float currentYRise;         // helper member
    #endregion


    // Properties
    #region Properties
    /// <summary>
    /// Get/set the current health state of the Tower. Cannot be raised above max health, or lowered below zero.
    /// </summary>
    public float Health
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            if (value != 0)
            {
                healthBar.transform.localScale = new Vector3(currentHealth / maxTowerHealth, 1f, 1f);
            }
            if (currentHealth > maxTowerHealth)
            {
                currentHealth = maxTowerHealth;
                healthBar.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (currentHealth < 0)
            {
                currentHealth = 0;
                TowerDie();
                healthBar.transform.localScale = new Vector3(0f, 1f, 1f);
            }
        }
    }

    /// <summary>
    /// Get whether the tower is currently rising
    /// </summary>
    public bool Rising
    {
        get { return (targetYRise > currentYRise); }
    }
    #endregion



    // Use this for initialization
    void Start () {
        // Setup this tower for gameplay
        SetupTower();
        currentHealth = maxTowerHealth;

        // Failsafe for knockdown speed
        if (knockdownSpeed <= 0)
        {
            knockdownSpeed = 10f;
        }

        // Failsafe for fall speed
        if (fallSpeed <= 0)
        {
            fallSpeed = 5f;
        }

        TowerCube.KnockSpeed = knockdownSpeed;
        TowerCube.FallSpeed = fallSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < playerCubes.Count; i++)
        {
            if (playerCubes[i].GetComponent<TowerCube>().testDamage)
            {
                playerCubes[i].GetComponent<TowerCube>().testDamage = false;
                HitCube(playerCubes[i]);
            }
        }

        UpdateKnockingCubes();
        SetBlockSpeeds();
        RaiseTower();
	}



    // Methods
    #region Methods
    /// <summary>
    /// Setup the tower for starting the game
    /// </summary>
    private void SetupTower()
    {
        // Add all of this object's TowerCube children to this list of playable cubes
        playerCubes.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<TowerCube>() != null)
            {
                playerCubes.Add(transform.GetChild(i).gameObject);

                //Debug.Log(new Vector3(transform.position.x, transform.position.y + baseDistanceFromBottom
                //    + (transform.GetChild(i).GetComponent<SpriteRenderer>().bounds.size.y * (i + 1))));

                transform.GetChild(i).transform.position = new Vector3(transform.position.x, transform.position.y + baseDistanceFromBottom
                    + (transform.GetChild(i).GetComponent<SpriteRenderer>().bounds.size.y * (i + 1)));
            }
        }

        // Setup the tower hierarchy
        for (int i = transform.childCount - 1; i > 0; i--)
        {
            //Debug.Log("Setup Backwards Loop: " + i);
            if (transform.GetChild(i).GetComponent<TowerCube>() != null)
            {
                //Debug.Log("Set Child " + i + " as child of " + (i - 1));
                MakeChildOf(transform.GetChild(i).gameObject, transform.GetChild(i - 1).gameObject);
            }
        }
    }

    
    /// <summary>
    /// A very lazy and inefficient method to set all the speeds of the players
    /// </summary>
    private void SetBlockSpeeds()
    {
        for (int i = 0; i < playerCubes.Count; i++)
        {
            if (i < playerSpeeds.Count)
            {
                playerCubes[i].GetComponent<TowerCube>().Speed = playerSpeeds[i];
            }

            else if (playerSpeeds.Count > 0)
            {
                playerCubes[i].GetComponent<TowerCube>().Speed = playerSpeeds[playerSpeeds.Count - 1];
            }

            else
            {
                playerCubes[i].GetComponent<TowerCube>().Speed = i * 1.4f;
            }
        }
    }


    /// <summary>
    /// Make something a child of something else
    /// </summary>
    /// <param name="child">The object to be made a child</param>
    /// <param name="parent">The object that will be the child's parent</param>
    private void MakeChildOf(GameObject child, GameObject parent)
    {
        child.transform.SetParent(parent.transform);
    }


    /// <summary>
    /// Update all the cubes that're being knocked to the bottom of the tower
    /// </summary>
    private void UpdateKnockingCubes()
    {
        for (int i = 0; i < playerCubes.Count; i++)
        {
            // If a cube has been knocked to the tower base height, stop its downwards motion
            if (playerCubes[i].GetComponent<TowerCube>().Knocking && playerCubes[i].transform.position.y <= transform.position.y)
            {
                // Splash dirt where it lands
                Instantiate(dirtSplashParticlePrefab, playerCubes[i].transform.position, Quaternion.identity);

                // Set it to be the ultimate parent of everyone, and place it beneath the tower
                playerCubes[i].transform.position
                    = new Vector3(
                        playerCubes[0].transform.position.x,
                        transform.position.y/* + playerCubes[i].GetComponent<SpriteRenderer>().bounds.size.y*/ + baseDistanceFromBottom);

                // Stop the knocking process for that cube
                playerCubes[i].GetComponent<TowerCube>().Knocking = false;

                // Reorder the cubes in the list - place this one in front
                GameObject placeholder = playerCubes[i];
                playerCubes.RemoveAt(i);
                playerCubes.Insert(0, placeholder);
                i--;

                // Fix the hierarchy
                playerCubes[0].transform.parent = transform;
                playerCubes[1].transform.parent = playerCubes[0].transform;

                // Raise the tower
                targetYRise += playerCubes[0].GetComponent<SpriteRenderer>().bounds.size.y;
                //RaiseTower();
            }
        }
    }

    
    /// <summary>
    /// Update any cubes that are in a falling state
    /// </summary>
    private void UpdateFallingCubes()
    {

    }


    /// <summary>
    /// Hit a cube, knock it down, and take tower damage
    /// </summary>
    /// <param name="cube">Cube to hit</param>
    public void HitCube(GameObject cube)
    {
        if (!cube.GetComponent<TowerCube>().Invincible && !cube.GetComponent<TowerCube>().Falling)
        {
            // Start the knocking process
            cube.GetComponent<TowerCube>().Knocking = true;

            // Take tower damage
            int index = playerCubes.IndexOf(cube);
            TakeDamage(index);

            //// Start the falling process for the cube above
            //if (index != playerCubes.Count - 1)
            //{
            //    playerCubes[index + 1].GetComponent<TowerCube>().StartFalling();
            //}
            
            // Set the children of this object (if any) to parent one up the hierarchy, and set them to fall
            if (cube.transform.childCount > 0 && /*playerCubes[0] != cube && */ index != playerCubes.Count - 1)
            {
                //Debug.Log((((Vector2)playerCubes[index - 1].transform.position
                //        + ((Vector2)cube.transform.GetChild(0).position - (Vector2)cube.transform.position))
                //        - (Vector2)playerCubes[index - 1].transform.position));
                playerCubes[index + 1].GetComponent<TowerCube>().StartFalling(
                    -(((Vector2)playerCubes[index - 1].transform.position
                        + ((Vector2)playerCubes[index + 1].transform.position - (Vector2)cube.transform.position))
                        - (Vector2)playerCubes[index - 1].transform.position));
                MakeChildOf(cube.transform.GetChild(0).gameObject, playerCubes[index - 1]);
            }

            cube.transform.parent = null;
        }
    }

    /// <summary>
    /// Take tower damage
    /// </summary>
    /// <param name="cube">Send in the cube</param>
    private void TakeDamage(GameObject cube)
    {
        int index = playerCubes.IndexOf(cube);
        if (index != -1)
        {
            TakeDamage(index);
        }
    }

    /// <summary>
    /// Take tower damage based on the height of the cube hit
    /// </summary>
    /// <param name="cubeHeight">The height index of the cube</param>
    private void TakeDamage(int cubeHeight)
    {
        if (damagePtsPerLevel.Count > 0)
        {
            if (cubeHeight <= damagePtsPerLevel.Count - 1)
            {
                //Debug.Log("Taking " + damagePtsPerLevel[cubeHeight] + " damage.");
                Health -= damagePtsPerLevel[cubeHeight];
            }
            else
            {
                //Debug.Log("Taking " + damagePtsPerLevel[damagePtsPerLevel.Count - 1] + " damage.");
                Health -= damagePtsPerLevel[damagePtsPerLevel.Count - 1];
            }
        }
        else
        {
            Health -= maxTowerHealth * .05f;
        }

        //Debug.Log("End Damage: " + currentHealth);
    }


    /// <summary>
    /// Move the whole tower upwards
    /// </summary>
    private void RaiseTower()
    {
        if (Rising)
        {
            playerCubes[0].transform.position += new Vector3(0, raiseSpeed * Time.deltaTime);
            currentYRise += raiseSpeed * Time.deltaTime;

            // If it's gone past the raise height, snap it back and stop raising.
            if (currentYRise > targetYRise)
            {
                playerCubes[0].transform.position += new Vector3(0, targetYRise - currentYRise);
                StopRaising();
            }
        }
    }

    
    /// <summary>
    /// Set raise components to default/zero state
    /// </summary>
    private void StopRaising()
    {
        targetYRise = 0;
        currentYRise = 0;
    }
   

    /// <summary>
    /// Kill the tower
    /// </summary>
    private void TowerDie()
    {
        GameObject.FindObjectOfType<LevelManagerScript>().GameOver();
    }
    #endregion
}
