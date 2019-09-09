using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

//Animated parameter parameters should not be changed in playing mode
namespace MightyGamePack
{
    public enum AnimatedElement
    {
        Position,
        Rotation,
        Scale
    };

    [ExecuteInEditMode]
    public class TransformJuicer : MonoBehaviour
    {
        public AnimatedElement animatedElement;
        public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Tooltip("Animation curve value will be multiplied by this value (Eg. (0,1,0) will animate only in Y axis")] public Vector3 animationDirectionMultiplier;
        public float evaluationSpeed;
        public bool looping;

        bool active;
        float evaluateState;
        float curveValue;

        [Header("Collisions")]
        public bool detectCollisions;
        [ShowIf("detectCollisions")] [Tooltip("Object will detect collisions with objects with these tags")] public string[] tagsToCollide;

        Vector3 startPosition;
        Vector3 startRotation;
        Vector3 startScale;

        void Start()
        {
            if (animatedElement == AnimatedElement.Position)
            {
                startPosition = transform.localPosition;
            }

            if (animatedElement == AnimatedElement.Rotation)
            {
                startRotation = transform.localEulerAngles;
            }

            if (animatedElement == AnimatedElement.Scale)
            {
                startScale = transform.localScale;
            }
        }

        void Update()
        {
            Evaluate();
        }

        void Evaluate()
        {
            if (active)
            {
                if (evaluateState < 1)
                {
                    evaluateState += evaluationSpeed * Time.deltaTime;
                    curveValue = animationCurve.Evaluate(evaluateState);
                }
                else
                {
                    if (looping)
                    {
                        evaluateState = 0;
                    }
                    else
                    {
                        active = false;
                    }
                    curveValue = animationCurve.Evaluate(evaluateState);
                }

                switch (animatedElement)
                {
                    case AnimatedElement.Position:
                        transform.localPosition = startPosition + animationDirectionMultiplier * curveValue;
                        break;
                    case AnimatedElement.Rotation:
                        transform.eulerAngles = startRotation + animationDirectionMultiplier * curveValue;
                        break;
                    case AnimatedElement.Scale:
                        transform.localScale = startScale + animationDirectionMultiplier * curveValue;
                        break;
                }
            }
        }

        public void StartJuicing()
        {
            active = true;
            evaluateState = 0;
        }

        public void StopJuicing()
        {
            active = false;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (detectCollisions)
            {
                for (int i = 0; i < tagsToCollide.Length; i++)
                {
                    if (collision.transform.tag == tagsToCollide[i])
                    {
                        StartJuicing();
                        break;
                    }
                }
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (detectCollisions)
            {
                for (int i = 0; i < tagsToCollide.Length; i++)
                {
                    if (collision.transform.tag == tagsToCollide[i])
                    {
                        StartJuicing();
                        break;
                    }
                }
            }
        }

    }
}