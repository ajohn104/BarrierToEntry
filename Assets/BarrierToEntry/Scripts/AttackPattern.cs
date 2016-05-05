using UnityEngine;
using System.Collections;
using System;

namespace BarrierToEntry
{
    public class AttackPattern
    {
        public const int LEFT_TO_RIGHT = 0;
        public const int RIGHT_TO_LEFT = 1;
        public const int TOP_DOWN = 2;

        protected Vector3[] positions;
        protected Vector3[] rotations;

        private const float attackTime = 1f;
        private float lastTime = 0f;

        private static AttackPattern leftRight = new AttackPattern(
                new Vector3[] { new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3() },
                new Vector3[] { new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3() }
            );
        private static AttackPattern rightLeft = new AttackPattern(
                new Vector3[] { new Vector3(0.8f, 1.6f, 0f), new Vector3(0.5f, 1.4f, 0.4f), new Vector3(-0.1f, 1.3f, 0.3f), new Vector3(-0.3f, 1.3f, 0.1f)},
                new Vector3[] { new Vector3(353.2f, 241.4f, 55.9f), new Vector3(313.3f, 141.1f, 73.9f), new Vector3(298.4f, 65.9f, 76.2f), new Vector3(285f, 174.4f, 245.2f)}
            );
        private static AttackPattern topDown = new AttackPattern(
                new Vector3[] { new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3() },
                new Vector3[] { new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3() }
            );

        private AttackPattern(Vector3[] pos, Vector3[] rot)
        {
            this.positions = pos;
            this.rotations = rot;
        }

        public static AttackPattern GetPattern(int direction, Vector3 initPos, Vector3 initRot)
        {
            switch(direction)
            {
                case LEFT_TO_RIGHT: return GetPattern(leftRight, initPos, initRot);
                case RIGHT_TO_LEFT: return GetPattern(rightLeft, initPos, initRot);
                case TOP_DOWN: return GetPattern(topDown, initPos, initRot);
            }
            return null;
        }

        private static AttackPattern GetPattern(AttackPattern pattern, Vector3 initPos, Vector3 initRot)
        {
            Vector3[] pos = new Vector3[pattern.positions.Length + 1];
            Vector3[] rot = new Vector3[pattern.rotations.Length + 1];

            pos[0] = initPos;
            rot[0] = initRot;

            Array.Copy(pattern.positions, 0, pos, 1, pattern.positions.Length);
            Array.Copy(pattern.rotations, 0, rot, 1, pattern.positions.Length);

            return new AttackPattern(pos, rot);
        }

        public Vector3 GetExpectedPosition(float time)
        {
            lastTime = time;
            float startI = Mathf.Clamp(Mathf.Floor(((positions.Length - 1) * time) / attackTime), 0f, positions.Length - 1);
            float endI = Mathf.Clamp(startI + 1f, 0f, positions.Length-1);
            float startT = ((attackTime * startI) / (positions.Length - 1));
            float endT = ((attackTime * endI) / (positions.Length - 1));
            float dt = ((time - startT) / (endT - startT));
            return Vector3.Lerp(positions[(int)startI], positions[(int)endI], dt);
        }

        public Vector3 GetExpectedRotation(float time)
        {
            lastTime = time;
            float startI = Mathf.Clamp(Mathf.Floor(((rotations.Length - 1) * time) / attackTime), 0f, rotations.Length - 1);
            float endI = Mathf.Clamp(startI + 1f, 0f, rotations.Length - 1);
            float startT = ((attackTime * startI) / (positions.Length - 1));
            float endT = ((attackTime * endI) / (positions.Length - 1));
            float dt = ((time - startT) / (endT - startT));
            return Quaternion.Lerp(Quaternion.Euler(rotations[(int)startI]), Quaternion.Euler(rotations[(int)endI]), dt).eulerAngles;
        }

        public bool isDone()
        {
            return lastTime > attackTime;
        }

    }
}