using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BlockBreaker3D.View.InGame
{
    [RequireComponent(typeof(PlayerInput))]
    public class TutorInput : MonoBehaviour
    {
        private Subject<Unit> onSpaceSubject = new Subject<Unit>();

        public IObservable<Unit> OnSpaceAsObservable() => onSpaceSubject;
        public void OnSpace(InputValue value)
        {
            if (value.isPressed)
            {
                onSpaceSubject.OnNext(Unit.Default);
            }
        }
    }
}