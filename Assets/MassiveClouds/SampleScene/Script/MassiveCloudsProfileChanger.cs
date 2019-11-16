using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mewlist
{
    [RequireComponent(typeof(Toggle))]
    public class MassiveCloudsProfileChanger : MonoBehaviour
    {
        [SerializeField] private MassiveCloudsProfile profile;
        [SerializeField] private MassiveClouds massiveClouds;
        [SerializeField] private Text label;

        public void SetProfile(MassiveCloudsProfile profile)
        {
            this.profile = profile;
            label.text = profile.name;
        }
		
        private Toggle Toggle
        {
            get { return GetComponent<Toggle>(); }
        }
	
        private void OnEnable()
        {
            Toggle.onValueChanged.AddListener(Switch);
        }
	
        private void OnDisable()
        {
            Toggle.onValueChanged.RemoveListener(Switch);
        }
	
        // Use this for initialization
        void Switch (bool isOn)
        {
            if (!isOn) return;

            if (massiveClouds != null)
            {
                massiveClouds.SetProfiles(new List<MassiveCloudsProfile>() {profile});
            }
        }
    }
}