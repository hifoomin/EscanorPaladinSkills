using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EscanorPaladinSkills.Components
{
    public class SwordSizeController : MonoBehaviour
    {
        public Vector3 idealSwordSize;
        public Transform sword;

        public void Start()
        {
            idealSwordSize = Vector3.one * Main.swordScale.Value;
        }

        public void LateUpdate()
        {
            if (!sword)
            {
                return;
            }

            sword.localScale = idealSwordSize;
        }
    }
}