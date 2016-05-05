using UnityEngine;
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
                new Vector3[] { new Vector3(-0.4f, 1.4f, 0f), new Vector3(-0.1f, 1.4f, 0.3f), new Vector3(0.4f, 1.2f, 0.2f), new Vector3(0.5f, 1.1f, 0f)},
                new Vector3[] { new Vector3(337.4f, 135.7f, 306.2f), new Vector3(345.4f, 192.2f, 312.5f), new Vector3(323.6f, 279.6f, 283.3f), new Vector3(310.8f, 347f, 271.8f) }
            );
        private static AttackPattern rightLeft = new AttackPattern(
                new Vector3[] { new Vector3(0.8f, 1.6f, 0f), new Vector3(0.5f, 1.4f, 0.4f), new Vector3(-0.1f, 1.3f, 0.3f), new Vector3(-0.3f, 1.3f, 0.1f)},
                new Vector3[] { new Vector3(353.2f, 241.4f, 55.9f), new Vector3(313.3f, 141.1f, 73.9f), new Vector3(298.4f, 65.9f, 76.2f), new Vector3(285f, 174.4f, 245.2f)}
            );
        private static AttackPattern topDown = new AttackPattern(
                new Vector3[] { new Vector3(0.1f, 1.8f, 0.1f), new Vector3(0.1f, 1.4f, 0.3f), new Vector3(-0.1f, 1.1f, 0.2f), new Vector3(-0.1f, 1.1f, -0.1f) },
                new Vector3[] { new Vector3(59.5f, 186.2f, 17.8f), new Vector3(325.5f, 171f, 6f), new Vector3(258.8f, 1.2f, 176.6f), new Vector3(305.6f, 194.8f, 202.3f) }
            );

        private AttackPattern(Vector3[] pos, Vector3[] rot)
        {
            this.positions = pos;
            this.rotations = rot;
        }

        public static AttackPattern GetPattern(Vector3 initPos, Vector3 initRot)
        {
            return GetPattern(UnityEngine.Random.Range(0, 3), initPos, initRot);     // Because of range of LEFT_TO_RIGHT to TOP_DOWN being 0 - 2
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