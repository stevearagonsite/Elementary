using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{
    public string[] scenesToLoadAtStart;
    void Awake()
    {
        for (int i = 0; i <scenesToLoadAtStart.Length; i++)
        {
            SceneLoadManager.instance.LoadSceneAsync(scenesToLoadAtStart[i], LoadSceneMode.Additive);
        }
    }
    
#if UNITY_EDITOR
    private void Start() {
        StartCoroutine("LazyAddComponent");
    }

    private IEnumerator LazyAddComponent() {
        while (true) {
            yield return new WaitForSeconds(3);
            if (!this.gameObject.GetComponent<DevelopInitPlayer>()) {
                this.gameObject.AddComponent<DevelopInitPlayer>();
            }
        }
    }
#endif
}

