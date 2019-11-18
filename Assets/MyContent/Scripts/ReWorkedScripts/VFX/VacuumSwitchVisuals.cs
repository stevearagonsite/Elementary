using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VacuumSwitchVisuals : MonoBehaviour {

    VacuumSwitch sw;
    public Image progressBar;
    public Text text;

	void Start () {
        sw = GetComponent<VacuumSwitch>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	
	void Execute ()
    {
        var prog = sw.GetCurrentProgressPercent();
        progressBar.fillAmount = prog;
        text.text = (int)(prog *100) + "%";

    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
