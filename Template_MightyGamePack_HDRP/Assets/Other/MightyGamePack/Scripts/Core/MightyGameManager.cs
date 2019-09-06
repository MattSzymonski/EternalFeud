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


        [Header("Game")]
        public float score;
        //Add here more project related stuff


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











        //---------------------------------------------GAMECONTROLLING FUNCTIONS---------------------------------------------

        public void PlayGame()
        {

        }

        [Button]
        public void GameOver()
        {
            if (!debugHideUI)
            {
                UIManager.GameOver();
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
            score = 0;
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

    }
}
