using UnityEngine;

namespace Mewlist
{
    public class MassiveCloudsMaterial
    {
        public Material CloudMaterial { get; private set; }
        public Material ShadowMaterial { get; private set; }
        public Material HeightFogMaterial { get; private set; }
        public Material MixMaterial { get; private set; }
        public Material VolumetricShadowMaterial { get; private set; }
        public Material VolumetricShadowMixMaterial { get; private set; }
        private MassiveCloudsProfile Profile { get; set; }

        public MassiveCloudsMaterial()
        {
            if (CloudMaterial == null)
            {
                CloudMaterial = new Material(Shader.Find("MassiveClouds"));
            }
            if (ShadowMaterial == null)
            {
                ShadowMaterial =  new Material(Shader.Find("MassiveCloudsScreenSpaceShadow"));
            }
            if (HeightFogMaterial == null)
            {
                HeightFogMaterial = new Material(Shader.Find("MassiveCloudsHeightFog"));
            }
            if (VolumetricShadowMaterial == null)
            {
                VolumetricShadowMaterial = new Material(Shader.Find("MassiveCloudsVolumetricShadow"));
            }
            if (MixMaterial == null)
            {
                MixMaterial = new Material(Shader.Find("MassiveCloudsMix"));
            }
            if (VolumetricShadowMixMaterial == null)
            {
                VolumetricShadowMixMaterial = new Material(Shader.Find("MassiveCloudsVolumetricShadowMix"));
            }
        }

        private bool Initialized
        {
            get
            {
                return CloudMaterial != null &&
                       ShadowMaterial != null &&
                       HeightFogMaterial != null &&
                       VolumetricShadowMaterial != null &&
                       MixMaterial != null &&
                       VolumetricShadowMixMaterial != null;
            }
        }

        public void SetProfile(MassiveCloudsProfile profile)
        {
            Profile = profile;
            if (Profile == null) return;

            profile.SetMaterial(CloudMaterial, this.Profile.Parameter);
            CloudMaterial.DisableKeyword("_SHADOW_ON");
            CloudMaterial.DisableKeyword("_HEIGHTFOG_ON");
            profile.SetMaterial(ShadowMaterial, this.Profile.Parameter);
            profile.SetMaterial(HeightFogMaterial, this.Profile.Parameter);
            profile.SetMaterial(VolumetricShadowMaterial, this.Profile.Parameter);
            profile.SetMaterial(MixMaterial, this.Profile.Parameter);
            profile.SetMaterial(VolumetricShadowMixMaterial, this.Profile.Parameter);
        }

        public void SetFogColor(Color color, Color topColor)
        {
            CloudMaterial.SetColor("_FogColor", color);
            HeightFogMaterial.SetColor("_FogColor", color);
            HeightFogMaterial.SetColor("_FogColorTop", topColor);
        }

        public void SetDirectionalLight(MassiveCloudsLight light, float scale)
        {
            CloudMaterial.SetVector("_MassiveCloudsSunLightDirection", light.LightDirection);
            ShadowMaterial.SetVector("_MassiveCloudsSunLightDirection", light.LightDirection);
            VolumetricShadowMaterial.SetVector("_MassiveCloudsSunLightDirection", light.LightDirection);
            CloudMaterial.SetVector("_MassiveCloudsSunLightColor", light.Color * light.Intensity * scale);
        }

        public void SetNightLight(MassiveCloudsLight light, float scale)
        {
            CloudMaterial.SetVector("_MassiveCloudsMoonLightDirection", light.LightDirection);
            ShadowMaterial.SetVector("_MassiveCloudsMoonLightDirection", light.LightDirection);
            VolumetricShadowMaterial.SetVector("_MassiveCloudsMoonLightDirection", light.LightDirection);
            CloudMaterial.SetVector("_MassiveCloudsMoonLightColor", light.Color * light.Intensity * scale);
        }

        public void SetScrollOffset(Vector3 offset)
        {
            CloudMaterial.SetVector("_ScrollOffset", offset);
            ShadowMaterial.SetVector("_ScrollOffset", offset);
            VolumetricShadowMaterial.SetVector("_ScrollOffset", offset);
        }

        public void SetBaseColor(Color color)
        {
            CloudMaterial.SetColor("_BaseColor", color);
        }

        public void SetFade(float v)
        {
            if (Profile == null) return;
            CloudMaterial.SetFloat("_Density", Profile.Parameter.Density * v);
            ShadowMaterial.SetFloat("_Density", Profile.Parameter.Density * v);
            VolumetricShadowMaterial.SetFloat("_Density", Profile.Parameter.Density * v);
            HeightFogMaterial.SetFloat("_GroundHeight", Mathf.Lerp(-1000, Profile.Parameter.GroundHeight, v));
            HeightFogMaterial.SetFloat("_HeightFogRange", Profile.Parameter.HeightFogRange * v);
        }

        public void SetShaodwColor(Color color)
        {
            if (Profile == null) return;
            ShadowMaterial.SetColor("_ShadowColor", color);
            VolumetricShadowMixMaterial.SetColor("_ShadowColor", color);
        }

        public void SetParameter(MassiveCloudsParameter parameter)
        {
            if (Profile == null) return;
            Profile.SetMaterial(CloudMaterial, parameter);
            Profile.SetMaterial(ShadowMaterial, parameter);
            Profile.SetMaterial(HeightFogMaterial, parameter);
            Profile.SetMaterial(VolumetricShadowMaterial, parameter);
            Profile.SetMaterial(MixMaterial, parameter);
            Profile.SetMaterial(VolumetricShadowMaterial, parameter);
            Profile.SetMaterial(VolumetricShadowMixMaterial, parameter);
        }
    }

    public class MassiveCloudsMixer
    {
        private MassiveCloudsMaterial material = new MassiveCloudsMaterial();
        private MassiveCloudsProfile from;
        private MassiveCloudsProfile target;

        private bool lerp = false;
        private float mix = 1;
        private float duration = 1;

        public MassiveCloudsMaterial Material
        {
            get { return material; }
        }

        public MassiveCloudsProfile CurrentProfile
        {
            get
            {
                if (lerp)
                    return target;
                return mix < 0.5f ? from : target;
            }
        }

        public MassiveCloudsParameter CurrentParameter
        {
            get
            {
                if (lerp)
                    return target.Lerp(@from, mix);
                return CurrentProfile.Parameter;
            }
        }

        private float Density
        {
            get
            {
                if (lerp)
                    return 1f;
                return Mathf.Pow(2f * Mathf.Abs(mix - 0.5f), 0.3f);
            }
        }

        public void ChangeTo(MassiveCloudsProfile profile, bool lerp)
        {
            var firstTime = from == null && target == null;

            if (target != profile)
            {
                from = target;
                target = profile;
                mix = 0f;
                this.lerp = lerp;
            }
            else
            {
                return;
            }

            if (!Application.isPlaying || firstTime)
            {
                mix = 1f;
            }

            material.SetProfile(CurrentProfile);
            material.SetFade(Density);
        }

        public void SetParameter(MassiveCloudsParameter parameter)
        {
            material.SetParameter(parameter);
        }

        public void SetDuration(float t)
        {
            duration = t;
        }
        
        public void Update()
        {
            if (mix >= 1f) return;
            mix = Mathf.Min(1f, mix + Time.deltaTime / duration);
            material.SetProfile(CurrentProfile);
            if (CurrentProfile != null)
                material.SetParameter(CurrentParameter);
            material.SetFade(Density);
        }
    }
}