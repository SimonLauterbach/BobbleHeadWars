using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject player;
    public GameObject[] spawnPoints;
    public GameObject alien;

    public int maxAliensOnScreen;
    public int totalAliens;
    public float minSpawnTime;
    public float maxSpawnTime;
    public int aliensPerSpawn;

    private int aliensOnScreen = 0;
    private float generatedSpawnTime = 0;
    private float currentSpawnTime = 0;

    public GameObject upgradePrefab;
    public Gun gun;
    public float upgradeMaxTimeSpawn = 7.5f;
    private bool spawnedUpgrade = false;
    private float actualUpgradeTime = 0;
    private float currentUpgradeTime = 0;
    public GameObject deathFloor;
    public Animator arenaAnimator;
    private void endGame()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.
        elevatorArrived);
        arenaAnimator.SetTrigger("PlayerWon");
    }

    // Start is called before the first frame update
    void Start()
    {
        actualUpgradeTime = Random.Range(upgradeMaxTimeSpawn - 3.0f,
        upgradeMaxTimeSpawn);
        actualUpgradeTime = Mathf.Abs(actualUpgradeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        currentUpgradeTime += Time.deltaTime;

        if (currentUpgradeTime > actualUpgradeTime)
        {
            // 1
            if (!spawnedUpgrade)
            {
                // 2
                int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                GameObject spawnLocation = spawnPoints[randomNumber];
                // 3
                GameObject upgrade = Instantiate(upgradePrefab) as GameObject;
                Upgrade upgradeScript = upgrade.GetComponent<Upgrade>();
                upgradeScript.gun = gun;
                upgrade.transform.position = spawnLocation.transform.position;
                // 4
                 spawnedUpgrade = true;

                SoundManager.Instance.PlayOneShot(SoundManager.Instance.powerUpAppear);
            }
        }

        //currentSpawnTime time passed since last update cell
        currentSpawnTime += Time.deltaTime;

        //Condition to gnereate a new wave of Aliens
        if (currentSpawnTime > generatedSpawnTime)
        {
            //Resets the timer after a spawn occurs
            currentSpawnTime = 0;
        }

        //Spawn-Time randomizer
        generatedSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);

        //Ensures number of Aliens is within limits
        if (aliensPerSpawn > 0 && aliensOnScreen < totalAliens)
        {
            List<int> previousSpawnLocations = new List<int>();
        }

        if (aliensPerSpawn > 0 && aliensOnScreen < totalAliens)
        {
            //This list keeps track of where you have already spawned Aliens
            List<int> previousSpawnLocations = new List<int>();


            //Limits number of Aliens to number of Spawnpoints
            if (aliensPerSpawn > spawnPoints.Length)
            {
                aliensPerSpawn = spawnPoints.Length - 1;
            }

            //Preventative code to make sure you do not spawn more Aliens than you've configured

            aliensPerSpawn = (aliensPerSpawn > totalAliens) ?
                              aliensPerSpawn - totalAliens : aliensPerSpawn;


            //This code loops for each Alien spawned
            for (int i = 0; i < aliensPerSpawn; i++)
            {
                if (aliensOnScreen < maxAliensOnScreen)
                {
                    //Keeps track of number of Aliens
                    aliensOnScreen += 1;

                    //Value of -1 means no index has been assigned or found for the spawnpoint
                    int spawnPoint = -1;

                    //While look keeps looking for a spawning point (index) that has not been used yet
                    while (spawnPoint == -1)
                    {
                        //create random index of List(array) between 0 and number of spawnpoints
                        int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                        //check to see if random spawnpoint has not already been used
                        if (!previousSpawnLocations.Contains(randomNumber))
                        {
                            //add this random number to the list
                            previousSpawnLocations.Add(randomNumber);
                            //use this random number for the spawn location
                            spawnPoint = randomNumber;
                        }
                    }

                    //Actual point(label) on arena to spawn next Alien
                    GameObject spawnLocation = spawnPoints[spawnPoint];

                    //Code to actually create a new Alien from a Prefab
                    GameObject newAlien = Instantiate(alien) as GameObject;

                    //Position the new Alien to the random unused spawn point
                    newAlien.transform.position = spawnLocation.transform.position;

                    //Get the Alien code from the new Alien spawned
                    Alien alienScript = newAlien.GetComponent<Alien>();

                    //Set the new Alien to target the player is currently
                    alienScript.target = player.transform;

                    Vector3 targetRotation = new Vector3(player.transform.position.x,
                    newAlien.transform.position.y, player.transform.position.z);
                    newAlien.transform.LookAt(targetRotation);
                    alienScript.OnDestroy.AddListener(AlienDestroyed);
                    alienScript.GetDeathParticles().SetDeathFloor(deathFloor);
                }
            }

        }

    }
    public void AlienDestroyed()
    {
        aliensOnScreen -= 1;
        totalAliens -= 1;
        if (totalAliens == 0)
        {
            Invoke("endGame", 2.0f);
        }
    }
}
