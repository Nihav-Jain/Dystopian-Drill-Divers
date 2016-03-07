using UnityEngine;
using System.Collections;
using enumerations;

namespace menu
{
    public class LoadOnTap : MonoBehaviour
    {
        public ScenesEnum sceneName;

        public void LoadScene()
        {

            Application.LoadLevel((int)sceneName);
        }



    }
}