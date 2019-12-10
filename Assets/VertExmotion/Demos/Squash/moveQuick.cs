using UnityEngine;
using System.Collections;

namespace Kalagaan
{

    public class moveQuick : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            transform.position = Random.insideUnitSphere * 5f;

        }
    }
}