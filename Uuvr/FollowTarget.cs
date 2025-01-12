using System;
using UnityEngine;

namespace Uuvr
{
    public class FollowTarget : UuvrBehaviour
    {
        public Transform? Target; // The transform to follow
        public Vector3 LocalPosition = Vector3.zero; // Local position offset
        public Quaternion LocalRotation = Quaternion.identity; // Local rotation offset

        public FollowTarget(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Updates the position and rotation of the object to follow the target.
        /// </summary>
        protected override void OnBeforeRender()
        {
            base.OnBeforeRender();

            // Ensure the target is valid
            if (Target == null) return;

            // Update position and rotation based on the target's transform
            transform.position = Target.TransformPoint(LocalPosition);
            transform.rotation = Target.rotation * LocalRotation;
        }
    }
}
