﻿using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
#if !UNITY_5_0
using HelpURL = BehaviorDesigner.Runtime.Tasks.HelpURLAttribute;
#endif

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("Tactical")]
    [TaskDescription("Move towards the target. The agents will start attacking when they are within distance")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Tactical/documentation.php?id=3")]
    [TaskIcon("Assets/Behavior Designer Tactical/Editor/Icons/{SkinColor}MarchingFireIcon.png")]
    public class MarchingFire : NavMeshTacticalGroup
    {
        [Tooltip("The number of agents that should be in a row")]
        public SharedInt agentsPerRow = 2;
        [Tooltip("The separation between agents")]
        public SharedVector2 separation = new Vector2(2, 2);
        [Tooltip("The distance to stop marching fire and continue attacking")]
        public SharedFloat attackDistance = 2;

        private Vector3 offset;
        private bool inPosition;

        protected override void FormationUpdated(int index)
        {
            base.FormationUpdated(index);

            if (leader.Value != null) {
                var row = formationIndex / agentsPerRow.Value;
                var column = formationIndex % agentsPerRow.Value;

                // Arrange the agents in charging position.
                if (column == 0) {
                    offset.Set(0, 0, -separation.Value.y * row);
                } else {
                    offset.Set(separation.Value.x * (column % 2 == 0 ? -1 : 1) * (((column - 1) / 2) + 1), 0, -separation.Value.y * row);
                }
            }

            inPosition = false;
        }

        public override TaskStatus OnUpdate()
        {
            var baseStatus = base.OnUpdate();
            if (baseStatus != TaskStatus.Running || !started) {
                return baseStatus;
            }
            var attackCenter = CenterAttackPosition();
            var attackRotation = ReverseCenterAttackRotation(attackCenter);
            // Move the agents into their starting position if they haven't been there already.
            if (!inPosition) {
                var leaderTransform = leader.Value != null ? leader.Value.transform : transform;
                var destination = TransformPoint(leaderTransform.position, offset, attackRotation);
                if (tacticalAgent.HasArrived()) {
                    // The agent is in position but it may not be facing the target.
                    if (tacticalAgent.RotateTowardsPosition(TransformPoint(attackCenter, offset, attackRotation))) {
                        // Notify the leader when the agent is in position.
                        if (leaderTree != null) {
                            leaderTree.SendEvent("UpdateInPosition", formationIndex, true);
                        } else {
                            UpdateInPosition(0, true);
                        }
                        inPosition = true;
                    }
                } else {
                    tacticalAgent.SetDestination(destination);
                }
            } else if (canAttack) {
                // All of the agents are in position. Start moving towards the attack point until the agents get within attack distance. Once they are
                // within attack distance they should start attacking and stop marching fire. The agents can attack while they are moving into position.
                var destination = TransformPoint(attackCenter, offset, attackRotation);
                if (tacticalAgent.AttackPosition || (destination - transform.position).magnitude <= attackDistance.Value) {
                    if (!tacticalAgent.AttackPosition) {
                        tacticalAgent.AttackPosition = true;
                    }
                    if (MoveToAttackPosition()) {
                        tacticalAgent.TryAttack();
                    }
                } else {
                    // The agent isn't in position yet. Set the destination and try to attack if the agent is within the attack distance.
                    tacticalAgent.SetDestination(destination);
                    FindAttackTarget();
                    if ((transform.position - tacticalAgent.TargetTransform.position).magnitude <= tacticalAgent.AttackAgent.AttackDistance()) {
                        if (tacticalAgent.RotateTowardsPosition(tacticalAgent.TargetTransform.position)) {
                            tacticalAgent.TryAttack();
                        }
                    }
                }
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            base.OnReset();

            agentsPerRow = 2;
            separation = new Vector2(2, 2);
            attackDistance = 2;
        }
    }
}