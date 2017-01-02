using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Check to see if the any objects are within sight of the agent.")]
    [TaskCategory("Movement")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=11")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class CanSeeObject : Conditional
    {
        [Tooltip("Should the 2D version be used?")]
        public bool usePhysics2D;
        [Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;
        [Tooltip("The tag of the object that we are searching for")]
        public SharedString targetTag;
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [Tooltip("The raycast offset relative to the pivot position")]
        public SharedVector3 offset;
        [Tooltip("The target raycast offset relative to the pivot position")]
        public SharedVector3 targetOffset;
        [Tooltip("The offset to apply to 2D angles")]
        public SharedFloat angleOffset2D;
        [Tooltip("The object that is within sight")]
        public SharedGameObject returnedObject;
        public SharedGameObject eye;
        public SharedFloat fieldOfViewAngleY = 60.0f;

        public bool IsViewSight = false;
        public bool IsViewLine = false;

        public override void OnStart()
        {
            if (!string.IsNullOrEmpty(targetTag.Value))
                targetObject.SetValue(GameObject.FindGameObjectWithTag(targetTag.Value));

            if (eye.Value == null)
            {
                eye.SetValue(gameObject);
            }
        }

        // Returns success if an object was found otherwise failure
        public override TaskStatus OnUpdate()
        {
            if (usePhysics2D)
            {
                if (!string.IsNullOrEmpty(targetTag.Value))
                { // If the target tag is not null then determine if there are any objects within sight based on the tag
                    returnedObject.Value = MovementUtility.WithinSight2D(eye.Value.transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, GameObject.FindGameObjectWithTag(targetTag.Value), targetOffset.Value, angleOffset2D.Value, ignoreLayerMask);
                }
                else if (targetObject.Value == null)
                { // If the target object is null then determine if there are any objects within sight based on the layer mask
                    returnedObject.Value = MovementUtility.WithinSight2D(eye.Value.transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, targetOffset.Value, angleOffset2D.Value, ignoreLayerMask);
                }
                else
                { // If the target is not null then determine if that object is within sight
                    returnedObject.Value = MovementUtility.WithinSight2D(eye.Value.transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObject.Value, targetOffset.Value, angleOffset2D.Value, ignoreLayerMask);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(targetTag.Value))
                { // If the target tag is not null then determine if there are any objects within sight based on the tag
                    returnedObject.Value = MovementUtility.WithinSight(eye.Value.transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, GameObject.FindGameObjectWithTag(targetTag.Value), targetOffset.Value, ignoreLayerMask);
                }
                else if (targetObject.Value == null)
                { // If the target object is null then determine if there are any objects within sight based on the layer mask
                    returnedObject.Value = MovementUtility.WithinSight(eye.Value.transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, targetOffset.Value, ignoreLayerMask);
                }
                else
                { // If the target is not null then determine if that object is within sight
                    returnedObject.Value = MovementUtility.WithinSight(eye.Value.transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObject.Value, targetOffset.Value, ignoreLayerMask, fieldOfViewAngleY.Value);
                }
            }
            if (returnedObject.Value != null)
            {
                // Return success if an object was found
                return TaskStatus.Success;
            }
            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }

        // Reset the public variables
        public override void OnReset()
        {
            fieldOfViewAngle = 90;
            viewDistance = 1000;
            offset = Vector3.zero;
            targetOffset = Vector3.zero;
            angleOffset2D = 0;
            targetTag = "";
        }

        // Draw the line of sight representation within the scene window
        public override void OnDrawGizmos()
        {
            //if (eye.Value == null) return;
            //if (IsViewSight)
            //    MovementUtility.DrawLineOfSight(eye.Value.transform, offset.Value, fieldOfViewAngle.Value, angleOffset2D.Value, viewDistance.Value, usePhysics2D);

            //if (IsViewLine)
            //{
            //    if (targetObject.Value == null) return;

            //    Gizmos.DrawLine(eye.Value.transform.position + offset.Value, targetObject.Value.transform.position + targetOffset.Value);
            //}
        }
    }
}