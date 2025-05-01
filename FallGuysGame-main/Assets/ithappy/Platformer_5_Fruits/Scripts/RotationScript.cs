using System.Collections.Generic;
using UnityEngine;

namespace ithappy
{

    public class RotationScript : MonoBehaviour
    {

        public enum RotationAxis
        {

            X,
            Y,
            Z, 
            YA
        }


        public List<Transform> points; //이동 경로 리스트
        public float moveSpeed = 2.0f; //이동 속도
        private int currenPointIndex = 0; //현재 목표 지점 인덱스
        private Renderer objectRenderer;
        private Vector3 initialScale; //초기 크기

        public RotationAxis rotationAxis = RotationAxis.Y;
        public float rotationSpeed = 50.0f;




        void Update()
        {


            float rotationValue = rotationSpeed * Time.deltaTime;

            Vector3 axis = Vector3.zero;
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    axis = Vector3.right;
                    break;
                case RotationAxis.Y:
                    axis = Vector3.up;
                    break;
                case RotationAxis.YA:
                    axis = Vector3.down;
                    break;
                case RotationAxis.Z:
                    axis = Vector3.forward;
                    break;
            }

            transform.Rotate(axis, rotationValue);
        }


    }
}

