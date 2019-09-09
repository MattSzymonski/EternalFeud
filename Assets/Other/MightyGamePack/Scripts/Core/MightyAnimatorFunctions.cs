/* 
 * Makes it super easy to trigger various functions, effects, sounds, etc from animations.
 * When this script will be added as a component to animated objects its functions will be availabe for animation events to set.
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MightyGamePack
{
    public class MightyAnimatorFunctions : MonoBehaviour
    {
        MightyGameManager gameManager;
        int disableCounter;

        void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
        }

        public void Disable(int times) //This is used when you dont want sound to be played several times (hack to bypass sound generating on button highlight and selection after click)
        {
            disableCounter = times;
        }

        public void UIPlaySound(string soundName)
        {
            if (disableCounter == 0)
            {

            }
            else
            {
                disableCounter--;
            }

        }
    }
}
