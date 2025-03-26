using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VeryAnimation
{
    public class AnimatorParameterToPositionY : MonoBehaviour
    {
        public GameObject sourceObject;
        private Animator anime;
        public string parameterName;
        private void Start()
        {
            anime = sourceObject.GetComponent<Animator>();
        }
        private void Update()
        {
            if (sourceObject == null) return;
            var animator = anime;
            if (animator == null || !animator.isInitialized) return;
            var value = animator.GetFloat(parameterName);
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, value, pos.z);
        }
    }
}
