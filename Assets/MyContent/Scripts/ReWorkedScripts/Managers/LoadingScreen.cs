using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

    public Image progressBar;
    public LoadScreenManager lsm;


    void Update ()
    {
        progressBar.fillAmount = lsm.progress;
    }
}
