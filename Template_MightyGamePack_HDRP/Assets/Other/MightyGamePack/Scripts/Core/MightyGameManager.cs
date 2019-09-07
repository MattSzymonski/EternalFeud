/* Mighty Game Jam Pack
 * Copyright (C) Mateusz Szymonski 2019 - All Rights Reserved
 * Written by Mateusz Szymonski <matt.szymonski@gmail.com>
 * 
 * 
 * 
 * Contents:
 * - MightyGameManager
 * - MightyUIManager (Cusor use 
 * 
 * 
 * 
 * 
 * - MightyAudioManager
 * - MightyAnimatorFunctions
 * - CameraShaker (https://github.com/andersonaddo/EZ-Camera-Shake-Unity, Copyright (c) 2019 Road Turtle Games, MIT License)
 * - Wait there is more!   TODO
 * 
 * SETUP:
 * All these elements need to be properly set.
 * There is a lot of dependencies in Unity project (UI Menus, Sound mixers, Animations, Input)
 * All these need to be set in only one way. If you don't want to get cancer while setting this up just use template project (modify it of course by adding your own game mechanics)
 * Modifications to the UI structure are of course allowed but can break things so make them wisely.
 * 
 * Warning! Mighty Game Jam Pack systems use special input settings.
 * Please close Unity and replace data in InputManager.asset in projectSettings directory with data from MightyInputManager.txt file provided with the pack or use template project.
 * If you want to use your own input settings you need to replace keys/buttons/axes names in several places in code but better don't do that.
 * To add another gamepad support just duplicate input settings for gamepad, change joystick index and rename 1 to 2, 3, 4, etc (use notepad to do this)
 * Changes made via notepad will be visible after refresing player settings view or restarting Unity
 * 
 * 
 * HOW TO USE IT:
 * When developing game just use GAMECONTROLLING FUNCTIONS in MightyGameManager. 
 * You don't need to care about UI.
 * GAMECONTROLLING FUNCTIONS are called when pressing buttons in UI
 * 
 * There is GameState variable that makes the game loop
 * In typical cases the main gameplay mechanics should work only in "Playing" state
 * 
 * To hide whole UI on start for faster development use debugHideUI variable
 * 
 * Options allows for setting the sound volume with sliders. 
 * Again, this works only when sound mixers are created and properly set.
 * 
 * Refer to all other managers (Audio, Particle effects, etc) via GameManager
 * The easiest way to do this is by "MightyGamePack.MightyGameManager.gameManager.particleEffectsManager"
 * 
 * If you don't know what some weird named parameters do, just hover cursor, there are tooltips!
 * 
 * When developing a game MightyGameManager is only file that should be modified (mostly add game-specific code) other files/classes should remain untouched
 * 
 * 
 * MODIFICATIONS:
 * If you don't want pause menu for example, just delete it in the scene UI and remove some dependencies in code like "Pause game when click escape button".
 * When changing fonts remember that they have different sizes and text can disappear, simply scale text in each text component to fix it.
 * 
 * 
 * 
 * Dependencies: https://github.com/dbrizov/NaughtyAttributes
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using UnityEngine.UI;

namespace MightyGamePack
{
    public enum GameState
    {
        Playing,
        MainMenu,
        OptionsMenu,
        PauseMenu,
        GameOverMenu
    };

    public class MightyGameManager : MonoBehaviour
    {
        [HideInInspector]
        public static MightyGameManager gameManager;


        [Header("Info")]
        public GameState startGameState = GameState.MainMenu; //Game state set at the start of the game
        [ReadOnly]
        public GameState gameState;
        [ReadOnly]
        public GameState lastGameState;

        [Header("Settings")]
        [Tooltip("Hides whole UI on start for faster development. Sets game state to playing on start")]
        public bool debugHideUI;

        public bool displayInGameScore;

        [Tooltip("Trigger restart game function during translation between main menu and game (works only when transitionMMToG in MightyUIManager is true)")] public bool restartGameMMToG;
        [Tooltip("Trigger restart game function during translation between game over menu or pause menu and game (works only when transitionRestart in MightyUIManager is true)")] public bool restartGameGOMOrPMToG;
        [ShowIf("restartGameMMToG")]
        [Tooltip("Restart game additional time to wait")] public float restartGameDelay;

        

        [Header("References to set")]
        public GameObject UIGameobject;
        public MightyUIManager UIManager;
        public MightyAudioManager audioManager;
        public MightyParticleEffectsManager particleEffectsManager;


        [Header("----Game----")]
        //public float score;

        public GameObject spawnerPlayer1;
        public GameObject spawnerPlayer2;

        //Add here more project related stuff xxD
        public bool spawnSheeps = true;

        public int startHealthPlayer1 = 1000;
        public int startHealthPlayer2 = 1000;

        [ReadOnly] public int healthPlayer1 = 1000;
        [ReadOnly] public int healthPlayer2 = 1000;

        public Image healthPlayer1Slider;
        public Image healthPlayer2Slider;

        public List<Sheep> sheeps;

        [Header("Sheeps spawn")]
        public List<GameObject> sheepSpawnerPlayer1;
        public List<GameObject> sheepSpawnerPlayer2;

        [ReadOnly] public float sheepSpawnTimer = 0;
        [Range(0.0001f, 4.9f)] public float sheepSpawnRate = 1.0f;

        public GameObject sheepPrefabPlayer1;
        public GameObject sheepPrefabPlayer2;

        [ReadOnly] public int sheepDrownToSpawn = 0;

        [Header("Sheeps update")]


        [ReadOnly] public float allSheepsUpdateTimer = 0; //Counting to one (eg.) second, in this time all sheeps will be updated
        public float allSheepsUpdateTime = 1.0f; //One second
        [ReadOnly] public int sheepsToUpdateInCurrentCycle = 0;

        float sheepUpdateTimer = 0;
        [ReadOnly] public float sheepUpdatePeriod = 0;
        [ReadOnly] public int sheepToUpdate = 0;

        [HideInInspector] public bool cleanupSheepList;



        bool cycleStarted = false;
        int currentSheep = 0;


        void UpdateSheep()
        {
            if (sheeps.Count > 0)
            {
              
                if (allSheepsUpdateTimer < 1 / sheeps.Count) //One second
                {
                    allSheepsUpdateTimer += 1 * Time.deltaTime;
                }
                else
                {
                    Sheep currentlyUpdatedSheep = sheeps[currentSheep];
                    currentlyUpdatedSheep.UpdateSheep();
                    Debug.Log("Updating sheep " + currentlyUpdatedSheep);

                    if (currentSheep < sheeps.Count - 1)
                    {
                        currentSheep++;
                    }
                    else
                    {
                        currentSheep = 0;
                    }
                }
             }

               





            /*


            if (sheeps.Count > 0)
            {
                if (allSheepsUpdateTimer < allSheepsUpdateTime) //One second
                {
                    allSheepsUpdateTimer += 1 * Time.deltaTime;
                }
                else
                {
                    if (cleanupSheepList) //Remove empty elements that remains after sheep death
                    {
                        for (int i = 0; i < sheeps.Count; i++)
                        {
                            if (sheeps[i] == null)
                            {
                                sheeps.RemoveAt(i);
                            }
                        }
                        cleanupSheepList = false;
                    }

                    allSheepsUpdateTimer = 0;
                    sheepsToUpdateInCurrentCycle = sheeps.Count;
                    sheepUpdatePeriod = allSheepsUpdateTime / sheepsToUpdateInCurrentCycle;
                    sheepToUpdate = sheeps.Count - 1;
                    Debug.Log("NEXT CYCLE");
                }


                if (sheepUpdateTimer < sheepUpdatePeriod)
                {
                    sheepUpdateTimer += 1 * Time.deltaTime;
                }
                else
                {
                    if (sheepToUpdate > -1)
                    {
                      
                   
                    Sheep currentlyUpdatedSheep = sheeps[sheepToUpdate];
                    currentlyUpdatedSheep.UpdateSheep();

                    Debug.Log("Updating sheep " + currentlyUpdatedSheep);

                    if (currentlyUpdatedSheep.owner == 1 && currentlyUpdatedSheep.territory == 2)
                    {
                        healthPlayer2--;
                        return;
                    }

                    if (currentlyUpdatedSheep.owner == 2 && currentlyUpdatedSheep.territory == 1)
                    {
                        healthPlayer1--;
                        return;
                    }

                    sheepToUpdate = sheepToUpdate - 1;
                    sheepUpdateTimer = 0;
                    }
                    else
                    {
                       // allSheepsUpdateTimer = 0;
                    }
                }
            }
            */
        }












        void Awake()
        {
            gameManager = this;

            gameState = startGameState;
            lastGameState = startGameState;

            if (debugHideUI)
            {
                gameState = GameState.Playing;
                lastGameState = GameState.Playing;
                UIManager.enabled = false;
                UIGameobject.SetActive(false);
            }





        }

        void Start()
        {
            if (gameManager != this)
            {
                Debug.LogError("There can be only one MightyGameManager at a time");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }



        void Update()
        {
            if (gameState == GameState.Playing)
            {
                //score += Time.unscaledDeltaTime;
                //UIManager.SetInGameScore(Mathf.Round(score)); //In seconds

                SpawnSheeps();
                UpdateSheep();

                healthPlayer1Slider.fillAmount = (healthPlayer1 / startHealthPlayer1) * 100;
                healthPlayer2Slider.fillAmount = (healthPlayer2 / startHealthPlayer2) * 100;

                if (healthPlayer1 <= 0)
                {
                    GameOver(2);
                }

                if (healthPlayer2 <= 0)
                {
                    GameOver(1);
                }

            }

            if (UIManager.spriteCustomCursor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    UIManager.SpriteCustomCursorClickPlayAnimation("Click");
                    UIManager.SpriteCustomCursorClickPlayParticleSystem();
                }
            }

          


        }




        //---------------------------------------------GAME MECHANICS FUNCTIONS---------------------------------------------


        


        void SpawnSheeps()
        {
            if (sheepSpawnTimer < 5 - sheepSpawnRate)
            {
                sheepSpawnTimer += 1 * Time.fixedDeltaTime;
            }
            else
            {
                SpawnSheep();
                if (sheepDrownToSpawn > 15)
                {
                    SpawnSheep();
                    SpawnSheep();
                    sheepDrownToSpawn -= 2;
                }
                if(sheepDrownToSpawn > 0)
                {
                    SpawnSheep();
                    sheepDrownToSpawn -= 1;
                }
                //Invoke("SpawnSheep", Random.Range(0.0f, 0.75f));
                sheepSpawnTimer = 0;
            }




        }

        void SpawnSheep()
        {
            if(Random.Range(0,2) == 0) //Spawn for player 1
            {
                if (sheepSpawnerPlayer1.Count > 0 && spawnSheeps)
                {
                    Vector2 point = Random.insideUnitCircle * 3;
                    Vector3 position = sheepSpawnerPlayer1[Random.Range(0, sheepSpawnerPlayer1.Count)].transform.position;
                    GameObject newSheep = Instantiate(sheepPrefabPlayer1, new Vector3(position.x + point.x, position.y, position.z + point.y), Quaternion.identity) as GameObject;
                    newSheep.name = "Sheep_" + sheeps.Count.ToString();
                    newSheep.GetComponent<Sheep>().owner = 1;
                    newSheep.transform.parent = GameObject.Find("Sheeps").transform;
                    sheeps.Add(newSheep.GetComponent<Sheep>());
                }
            }
            else //Spawn for player 2
            {
                if(sheepSpawnerPlayer2.Count > 0 && spawnSheeps)
                {
                    Vector2 point = Random.insideUnitCircle * 3;
                    Vector3 position = sheepSpawnerPlayer2[Random.Range(0, sheepSpawnerPlayer2.Count)].transform.position;
                    GameObject newSheep = Instantiate(sheepPrefabPlayer2, new Vector3(position.x + point.x, position.y, position.z + point.y), Quaternion.identity) as GameObject;
                    newSheep.GetComponent<Sheep>().owner = 2;
                    newSheep.transform.parent = GameObject.Find("Sheeps").transform;
                    newSheep.name = "Sheep_" + sheeps.Count.ToString();
                    sheeps.Add(newSheep.GetComponent<Sheep>());
                }
            }
        }



        //---------------------------------------------GAMECONTROLLING FUNCTIONS---------------------------------------------

        public void PlayGame()
        {
            /*
            Vector2 point = Random.insideUnitCircle * 3;
            Vector3 position = spawnerPlayer1.transform.position;
            GameObject player1 = Instantiate(player1, new Vector3(position.x + point.x, position.y, position.z + point.y), Quaternion.identity) as GameObject;
            newSheep.GetComponent<Sheep>().owner = 2;
            */
            //spawnerPlayer1
            // spawnerPlayer2

        }

        [Button]
        public void GameOver(int winner)
        {
            if (!debugHideUI)
            {
                UIManager.GameOver();
            }

            if(winner == 1)
            {
                UIManager.SetInGameScore("White player wins!");
            }
            if (winner == 2)
            {
                UIManager.SetInGameScore("Black player wins!");
            }

        }

        public void PauseGame()
        {

        }

        public void UnpauseGame()
        {

        }

        public void OpenOptions()
        {

        }

        public void RestartGame() //Clearing the scene, removing enemies, respawning player, zeroing score, etc
        {
            for (int i = sheeps.Count; i-- > 0;)
            {
                GameObject sheepToDestroy = sheeps[i].gameObject;
                sheeps.RemoveAt(i);
                Destroy(sheepToDestroy);
            }
            healthPlayer1 = startHealthPlayer1;
            healthPlayer2 = startHealthPlayer2;

            UIManager.SetInGameScore("");
        }

        public void BackToMainMenu()
        {

        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Cannot quit game in editor");
#else
        Application.Quit();      
#endif
        }

        //-----------------------------------------------OTHER FUNCTIONS-------------------------------------------------

        public void SetGameState(GameState value)
        {
            lastGameState = gameState;
            gameState = value;
        }




        void OnDrawGizmos()
        {
            for (int i = 0; i < sheepSpawnerPlayer1.Count; i++)
            {
                DebugExtension.DrawPoint(sheepSpawnerPlayer1[i].transform.position, Color.red, 1);
                DebugExtension.DrawCircle(sheepSpawnerPlayer1[i].transform.position, Vector3.up, Color.red, 3);
            }
            for (int i = 0; i < sheepSpawnerPlayer2.Count; i++)
            {
                DebugExtension.DrawPoint(sheepSpawnerPlayer2[i].transform.position, Color.blue, 1);
                DebugExtension.DrawCircle(sheepSpawnerPlayer2[i].transform.position, Vector3.up, Color.blue, 3);
            }
        }



        }






}
