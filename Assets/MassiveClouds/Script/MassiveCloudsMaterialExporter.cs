#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Mewlist
{
    [RequireComponent(typeof(MassiveClouds))]
    public class MassiveCloudsMaterialExporter : MonoBehaviour
    {
        private MassiveClouds MassiveClouds
        {
            get { return GetComponent<MassiveClouds>(); }
        }

        public void SaveToMaterial(string path)
        {
#if UNITY_EDITOR
            var mat = MassiveClouds.Profiles[0].CreateMaterialForExport();
            mat.SetColor("_BaseColor", MassiveClouds.CloudColor);
            mat.SetColor("_FogColor", RenderSettings.fogColor);
            mat.SetColor("_BaseColor", RenderSettings.fogColor);
            mat.SetColor("_MassiveCloudsSunLightColor", MassiveClouds.Sun.Color);
            mat.SetColor("_MassiveCloudsMoonLightColor", MassiveClouds.Moon.Color);
            mat.SetVector("_MassiveCloudsSunLightDirection", MassiveClouds.Sun.LightDirection);
            mat.SetVector("_MassiveCloudsMoonLightDirection", MassiveClouds.Moon.LightDirection);
            Color[] colors = new Color[3];
            RenderSettings.ambientProbe.Evaluate(
                new []{Vector3.up, Vector3.back, Vector3.down},
                colors);
            mat.SetColor("_AmbientTopColor", colors[0]);
            mat.SetColor("_AmbientMidColor", colors[1]);
            mat.SetColor("_AmbientBottomColor", colors[2]);
            
            AssetDatabase.CreateAsset(mat, path.Replace(Application.dataPath, "Assets"));
#endif
        }
    }
}