using System;
using UnityEngine;

namespace Mewlist
{
    [Serializable]
    public struct MassiveCloudsParameter
    {
        public enum RendererType
        {
            Authentic = 4,
            Surface = 0,
            Lucid = 2,
            Solid = 3
        }

        public Texture3D VolumeTexture;
        public Vector2 Tiling;
        [Range(1f, 32f)] public float Octave;
        [Range(0f, 1f)] public float Sculpture;
        [Range(-1f, 1f)] public float Phase;

        [Range(1f, 5000f)] public float DetailDistance;

        public RendererType Renderer;

        public bool Ramp;
        public Texture2D RampTexture;
        [Range(0.1f, 1f)] public float RampScale;
        [Range(-10f, 10f)] public float RampOffset;
        [Range(0f, 1f)] public float RampStrength;

        // Texture
        [Range(0f, 1f)] public float Softness;
        [Range(0f, 1f)] public float Density;
        [Range(0f, 1f)] public float Dissolve;
        [Range(0f, 1f)] public float Transparency;
        [Range(0.1f, 10f)] public float Scale;

        // Animation
        public Vector3 ScrollVelocity;

        // Lighting
        [Range(0f, 1f)] public float Lighting;
        [Range(0f, 1f)] public float DirectLight;
        [Range(0f, 1f)] public float Ambient;
        [Range(0f, 1f)] public float LightingQuality;
        [Range(0f, 1f)] public float LightSmoothness;
        [Range(0f, 1f)] public float LightScattering;
        [Range(0f, 1f)] public float Shading;

        [Range(0f, 1f)] public float EdgeLighting;
        [Range(-1f, 1f)] public float GlobalLighting;
        [Range(0f, 1f)] public float GlobalLightingRange;

        // Shadow
        public bool Shadow;
        [Range(0f, 1f)] public float ShadowSoftness;
        [Range(0f, 1f)] public float ShadowQuality;
        [Range(0f, 1f)] public float ShadowStrength;
        [Range(0f, 1f)] public float ShadowThreshold;
        public bool VolumetricShadow;
        [Range(0f, 1f)] public float VolumetricShadowDensity;
        [Range(0f, 1f)] public float VolumetricShadowStrength;

        // Finishing
        [Range(-1f, 1f)] public float Brightness;
        [Range(-1f, 1f)] public float Contrast;

        // Ray Marching
        public bool Horizontal;
        public bool RelativeHeight;
        [Range(0f, 1f)] public float HorizontalSoftnessTop;
        [Range(0f, 1f)] public float HorizontalSoftnessBottom;
        [Range(0f, 1f)] public float HorizontalSoftnessFigure;
        [Range(0f, 5000f)] public float FromHeight;
        [Range(0f, 10000f)] public float ToHeight;
        [Range(0f, 10000f)] public float Thickness;
        [Range(0f, 60000f)] public float FromDistance;
        [Range(0f, 60000f)] public float MaxDistance;
        [Range(1f, 400f)] public float Iteration;
        [Range(0.01f, 10f)] public float Fade;
        [Range(0f, 1f)] public float Optimize;

        // Height Fog
        public bool HeightFog;
        [Range(-10000f, 10000f)] public float GroundHeight;
        [Range(0f, 10000f)] public float HeightFogFromDistance;
        [Range(0.001f, 10000f)] public float HeightFogRange;

        public MassiveCloudsParameter(MassiveCloudsParameter other)
        {
            VolumeTexture = other.VolumeTexture;
            Tiling = other.Tiling;
            Octave = other.Octave;
            Sculpture = other.Sculpture;
            Phase = other.Phase;
            DetailDistance = other.DetailDistance;

            Renderer = other.Renderer;

            Ramp = other.Ramp;
            RampTexture = other.RampTexture;
            RampScale = other.RampScale;
            RampOffset = other.RampOffset;
            RampStrength = other.RampStrength;

            Softness = other.Softness;
            Density = other.Density;
            Dissolve = other.Dissolve;
            Transparency = other.Transparency;
            Scale = other.Scale;

            ScrollVelocity = other.ScrollVelocity;

            Lighting = other.Lighting;
            DirectLight = other.DirectLight;
            Ambient = other.Ambient;
            LightingQuality = other.LightingQuality;

            LightSmoothness = other.LightSmoothness;
            LightScattering = other.LightScattering;
            Shading = other.Shading;
            EdgeLighting = other.EdgeLighting;
            GlobalLighting = other.GlobalLighting;
            GlobalLightingRange = other.GlobalLightingRange;

            Shadow = other.Shadow;
            ShadowSoftness = other.ShadowSoftness;
            ShadowQuality = other.ShadowQuality;
            ShadowStrength = other.ShadowStrength;
            ShadowThreshold = other.ShadowThreshold;
            VolumetricShadow = other.VolumetricShadow;
            VolumetricShadowDensity = other.VolumetricShadowDensity;
            VolumetricShadowStrength = other.VolumetricShadowStrength;

            Brightness = other.Brightness;
            Contrast = other.Contrast;

            RelativeHeight = other.RelativeHeight;
            FromHeight = other.FromHeight;
            ToHeight = other.ToHeight;
            FromDistance = other.FromDistance;
            Horizontal = other.Horizontal;
            MaxDistance = other.MaxDistance;
            Thickness = other.Thickness;
            Iteration = other.Iteration;
            HorizontalSoftnessTop = other.HorizontalSoftnessTop;
            HorizontalSoftnessBottom = other.HorizontalSoftnessBottom;
            HorizontalSoftnessFigure = other.HorizontalSoftnessFigure;
            Optimize = other.Optimize;

            Fade = other.Fade;

            HeightFog = other.HeightFog;
            GroundHeight = other.GroundHeight;
            HeightFogFromDistance = other.HeightFogFromDistance;
            HeightFogRange = other.HeightFogRange;
        }

        public bool Equals(MassiveCloudsParameter other)
        {
            return VolumeTexture == other.VolumeTexture &&
                   Tiling == other.Tiling &&
                   Octave == other.Octave &&
                   Sculpture == other.Sculpture &&
                   Phase == other.Phase &&
                   DetailDistance == other.DetailDistance &&
                   Renderer == other.Renderer &&
                   Ramp == other.Ramp &&
                   RampTexture == other.RampTexture &&
                   RampScale == other.RampScale &&
                   RampOffset == other.RampOffset &&
                   RampStrength == other.RampStrength &&
                   Softness == other.Softness &&
                   Density == other.Density &&
                   Dissolve == other.Dissolve &&
                   Transparency == other.Transparency &&
                   Scale == other.Scale &&
                   ScrollVelocity == other.ScrollVelocity &&
                   Lighting == other.Lighting &&
                   DirectLight == other.DirectLight &&
                   Ambient == other.Ambient &&
                   LightingQuality == other.LightingQuality &&
                   LightSmoothness == other.LightSmoothness &&
                   LightScattering == other.LightScattering &&
                   Shading == other.Shading &&
                   EdgeLighting == other.EdgeLighting &&
                   GlobalLighting == other.GlobalLighting &&
                   GlobalLightingRange == other.GlobalLightingRange &&
                   Shadow == other.Shadow &&
                   ShadowSoftness == other.ShadowSoftness &&
                   ShadowQuality == other.ShadowQuality &&
                   ShadowStrength == other.ShadowStrength &&
                   ShadowThreshold == other.ShadowThreshold &&
                   VolumetricShadow == other.VolumetricShadow &&
                   VolumetricShadowDensity == other.VolumetricShadowDensity &&
                   VolumetricShadowStrength == other.VolumetricShadowStrength &&
                   Brightness == other.Brightness &&
                   Contrast == other.Contrast &&
                   RelativeHeight == other.RelativeHeight &&
                   FromHeight == other.FromHeight &&
                   ToHeight == other.ToHeight &&
                   FromDistance == other.FromDistance &&
                   Horizontal == other.Horizontal &&
                   MaxDistance == other.MaxDistance &&
                   Thickness == other.Thickness &&
                   Iteration == other.Iteration &&
                   HorizontalSoftnessTop == other.HorizontalSoftnessTop &&
                   HorizontalSoftnessBottom == other.HorizontalSoftnessBottom &&
                   HorizontalSoftnessFigure == other.HorizontalSoftnessFigure &&
                   Optimize == other.Optimize &&
                   Fade == other.Fade &&
                   HeightFog == other.HeightFog &&
                   GroundHeight == other.GroundHeight &&
                   HeightFogFromDistance == other.HeightFogFromDistance &&
                   HeightFogRange == other.HeightFogRange;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MassiveCloudsParameter && Equals((MassiveCloudsParameter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (VolumeTexture != null ? VolumeTexture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Tiling.GetHashCode();
                hashCode = (hashCode * 397) ^ Octave.GetHashCode();
                hashCode = (hashCode * 397) ^ Sculpture.GetHashCode();
                hashCode = (hashCode * 397) ^ Phase.GetHashCode();
                hashCode = (hashCode * 397) ^ DetailDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Renderer;
                hashCode = (hashCode * 397) ^ Ramp.GetHashCode();
                hashCode = (hashCode * 397) ^ (RampTexture != null ? RampTexture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ RampScale.GetHashCode();
                hashCode = (hashCode * 397) ^ RampOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ RampStrength.GetHashCode();
                hashCode = (hashCode * 397) ^ Softness.GetHashCode();
                hashCode = (hashCode * 397) ^ Density.GetHashCode();
                hashCode = (hashCode * 397) ^ Dissolve.GetHashCode();
                hashCode = (hashCode * 397) ^ Transparency.GetHashCode();
                hashCode = (hashCode * 397) ^ Scale.GetHashCode();
                hashCode = (hashCode * 397) ^ ScrollVelocity.GetHashCode();
                hashCode = (hashCode * 397) ^ Lighting.GetHashCode();
                hashCode = (hashCode * 397) ^ DirectLight.GetHashCode();
                hashCode = (hashCode * 397) ^ Ambient.GetHashCode();
                hashCode = (hashCode * 397) ^ LightingQuality.GetHashCode();
                hashCode = (hashCode * 397) ^ LightSmoothness.GetHashCode();
                hashCode = (hashCode * 397) ^ LightScattering.GetHashCode();
                hashCode = (hashCode * 397) ^ Shading.GetHashCode();
                hashCode = (hashCode * 397) ^ EdgeLighting.GetHashCode();
                hashCode = (hashCode * 397) ^ GlobalLighting.GetHashCode();
                hashCode = (hashCode * 397) ^ GlobalLightingRange.GetHashCode();
                hashCode = (hashCode * 397) ^ Shadow.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowSoftness.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowQuality.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowStrength.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowThreshold.GetHashCode();
                hashCode = (hashCode * 397) ^ VolumetricShadow.GetHashCode();
                hashCode = (hashCode * 397) ^ VolumetricShadowDensity.GetHashCode();
                hashCode = (hashCode * 397) ^ VolumetricShadowStrength.GetHashCode();
                hashCode = (hashCode * 397) ^ Brightness.GetHashCode();
                hashCode = (hashCode * 397) ^ Contrast.GetHashCode();
                hashCode = (hashCode * 397) ^ Horizontal.GetHashCode();
                hashCode = (hashCode * 397) ^ RelativeHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ HorizontalSoftnessTop.GetHashCode();
                hashCode = (hashCode * 397) ^ HorizontalSoftnessBottom.GetHashCode();
                hashCode = (hashCode * 397) ^ HorizontalSoftnessFigure.GetHashCode();
                hashCode = (hashCode * 397) ^ FromHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ ToHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ Thickness.GetHashCode();
                hashCode = (hashCode * 397) ^ FromDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ Iteration.GetHashCode();
                hashCode = (hashCode * 397) ^ Fade.GetHashCode();
                hashCode = (hashCode * 397) ^ Optimize.GetHashCode();
                hashCode = (hashCode * 397) ^ HeightFog.GetHashCode();
                hashCode = (hashCode * 397) ^ GroundHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ HeightFogFromDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ HeightFogRange.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(MassiveCloudsParameter lhs, MassiveCloudsParameter rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(MassiveCloudsParameter lhs, MassiveCloudsParameter rhs)
        {
            return !(lhs == rhs);
        }

        public MassiveCloudsParameter Lerp(MassiveCloudsParameter other, float t)
        {
            t = Mathf.SmoothStep(0, 1, t);
            return new MassiveCloudsParameter()
            {
                VolumeTexture = VolumeTexture,
                Tiling = Vector2.Lerp(other.Tiling, Tiling, t),
                Octave = Mathf.Lerp(other.Octave, Octave, t),
                Sculpture = Mathf.Lerp(other.Sculpture, Sculpture, t),
                Phase = Mathf.Lerp(other.Phase, Phase, t),
                DetailDistance = Mathf.Lerp(other.DetailDistance, DetailDistance, t),

                Renderer = Renderer,

                Ramp = Ramp,
                RampTexture = RampTexture,
                RampScale = Mathf.Lerp(other.RampScale, RampScale, t),
                RampOffset = Mathf.Lerp(other.RampOffset, RampOffset, t),
                RampStrength = Mathf.Lerp(other.RampStrength, RampStrength, t),

                Softness = Mathf.Lerp(other.Softness, Softness, t),
                Density = Mathf.Lerp(other.Density, Density, t),
                Dissolve = Mathf.Lerp(other.Dissolve, Dissolve, t),
                Transparency = Mathf.Lerp(other.Transparency, Transparency, t),
                Scale = Mathf.Lerp(other.Scale, Scale, t),

                ScrollVelocity = Vector3.Lerp(other.ScrollVelocity, ScrollVelocity, t),

                Lighting = Mathf.Lerp(other.Lighting, Lighting, t),
                DirectLight = Mathf.Lerp(other.DirectLight, DirectLight, t),
                Ambient = Mathf.Lerp(other.Ambient, Ambient, t),
                LightingQuality = Mathf.Lerp(other.LightingQuality, LightingQuality, t),

                LightSmoothness = Mathf.Lerp(other.LightSmoothness, LightSmoothness, t),
                LightScattering = Mathf.Lerp(other.LightScattering, LightScattering, t),
                Shading = Mathf.Lerp(other.Shading, Shading, t),
                EdgeLighting = Mathf.Lerp(other.EdgeLighting, EdgeLighting, t),
                GlobalLighting = Mathf.Lerp(other.GlobalLighting, GlobalLighting, t),
                GlobalLightingRange = Mathf.Lerp(other.GlobalLightingRange, GlobalLightingRange, t),

                Shadow = Shadow,
                ShadowSoftness = Mathf.Lerp(other.ShadowSoftness, ShadowSoftness, t),
                ShadowQuality = Mathf.Lerp(other.ShadowQuality, ShadowQuality, t),
                ShadowStrength = Mathf.Lerp(other.ShadowStrength, ShadowStrength, t),
                ShadowThreshold = Mathf.Lerp(other.ShadowThreshold, ShadowThreshold, t),
                VolumetricShadow = VolumetricShadow,
                VolumetricShadowDensity = Mathf.Lerp(other.VolumetricShadowDensity, VolumetricShadowDensity, t),
                VolumetricShadowStrength = Mathf.Lerp(other.VolumetricShadowStrength, VolumetricShadowStrength, t),

                Brightness = Mathf.Lerp(other.Brightness, Brightness, t),
                Contrast = Mathf.Lerp(other.Contrast, Contrast, t),

                RelativeHeight = RelativeHeight,
                FromHeight = Mathf.Lerp(other.FromHeight, FromHeight, t),
                ToHeight = Mathf.Lerp(other.ToHeight, ToHeight, t),
                FromDistance = Mathf.Lerp(other.FromDistance, FromDistance, t),
                Horizontal = Horizontal,
                MaxDistance = Mathf.Lerp(other.MaxDistance, MaxDistance, t),
                Thickness = Mathf.Lerp(other.Thickness, Thickness, t),
                Iteration = Mathf.Lerp(other.Iteration, Iteration, t),
                HorizontalSoftnessTop = Mathf.Lerp(other.HorizontalSoftnessTop, HorizontalSoftnessTop, t),
                HorizontalSoftnessBottom = Mathf.Lerp(other.HorizontalSoftnessBottom, HorizontalSoftnessBottom, t),
                HorizontalSoftnessFigure = Mathf.Lerp(other.HorizontalSoftnessFigure, HorizontalSoftnessFigure, t),
                Optimize = Mathf.Lerp(other.Optimize, Optimize, t),

                Fade = Mathf.Lerp(other.Fade, Fade, t),

                HeightFog = HeightFog,
                GroundHeight = Mathf.Lerp(other.GroundHeight, GroundHeight, t),
                HeightFogFromDistance = Mathf.Lerp(other.HeightFogFromDistance, HeightFogFromDistance, t),
                HeightFogRange = Mathf.Lerp(other.HeightFogRange, HeightFogRange, t),
            };
        }
    }

    [CreateAssetMenu(fileName = "MassiveCloudsProfile",
        menuName = "MassiveClouds/Profile", order = 1)]
    public class MassiveCloudsProfile : ScriptableObject
    {
        [SerializeField] public MassiveCloudsParameter Parameter;

        public void UpdateParameter(MassiveCloudsParameter other)
        {
            Parameter = new MassiveCloudsParameter(other);
        }

        public MassiveCloudsProfile()
        {
            Parameter = new MassiveCloudsParameter();
        }

        public Material CreateMaterial()
        {
            var mat = new Material(Shader.Find("MassiveClouds"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public Material CreateShadowMaterial()
        {
            var mat = new Material(Shader.Find("MassiveCloudsScreenSpaceShadow"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public Material CreateHeightFogMaterial()
        {
            var mat = new Material(Shader.Find("MassiveCloudsHeightFog"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public Material CreateMaterialForExport()
        {
            var mat = new Material(Shader.Find("MassiveCloudsMaterial"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public MassiveCloudsParameter Lerp(MassiveCloudsProfile from, float t)
        {
            if (from == null) return Parameter;
            return Parameter.Lerp(from.Parameter, t);
        }

        public void SetMaterial(Material mat, MassiveCloudsParameter param)
        {
            mat.SetTexture("_VolumeTex", param.VolumeTexture);
            mat.SetTextureScale("_VolumeTex", param.Tiling);

            mat.SetFloat("_Octave", param.Octave);
            mat.SetFloat("_Sculpture", param.Sculpture);
            mat.SetFloat("_Phase", param.Phase);
            mat.SetFloat("_DetailDistance", param.DetailDistance);

            mat.SetFloat("_RENDERER", ((float) param.Renderer) / 3f);
            switch (param.Renderer)
            {
                case MassiveCloudsParameter.RendererType.Authentic:
                    mat.EnableKeyword("_RENDERER_AUTHENTIC");
                    mat.DisableKeyword("_RENDERER_SURFACE");
                    mat.DisableKeyword("_RENDERER_LUCID");
                    mat.DisableKeyword("_RENDERER_SOLID");
                    break;
                case MassiveCloudsParameter.RendererType.Lucid:
                    mat.DisableKeyword("_RENDERER_AUTHENTIC");
                    mat.DisableKeyword("_RENDERER_SURFACE");
                    mat.EnableKeyword("_RENDERER_LUCID");
                    mat.DisableKeyword("_RENDERER_SOLID");
                    break;
                case MassiveCloudsParameter.RendererType.Solid:
                    mat.DisableKeyword("_RENDERER_AUTHENTIC");
                    mat.DisableKeyword("_RENDERER_SURFACE");
                    mat.DisableKeyword("_RENDERER_LUCID");
                    mat.EnableKeyword("_RENDERER_SOLID");
                    break;
                case MassiveCloudsParameter.RendererType.Surface:
                    mat.DisableKeyword("_RENDERER_AUTHENTIC");
                    mat.EnableKeyword("_RENDERER_SURFACE");
                    mat.DisableKeyword("_RENDERER_LUCID");
                    mat.DisableKeyword("_RENDERER_SOLID");
                    break;
            }

            if (param.Ramp) mat.EnableKeyword("_RAMP_ON");
            else mat.DisableKeyword("_RAMP_ON");
            mat.SetFloat("_RAMP", param.Ramp ? 1f : 0f);
            mat.SetTexture("_RampTex", param.RampTexture);
            mat.SetFloat("_RampScale", param.RampScale);
            mat.SetFloat("_RampOffset", param.RampOffset);
            mat.SetFloat("_RampStrength", param.RampStrength);

            mat.SetFloat("_Softness", param.Softness);
            mat.SetFloat("_Density", param.Density);
            mat.SetFloat("_Dissolve", param.Dissolve);
            mat.SetFloat("_Transparency", param.Transparency);
            mat.SetFloat("_Scale", param.Scale);

            mat.SetVector("_ScrollVelocity", param.ScrollVelocity);

            mat.SetFloat("_Lighting", param.Lighting);
            mat.SetFloat("_DirectLight", param.DirectLight);
            mat.SetFloat("_Ambient", param.Ambient);
            mat.SetFloat("_LightingQuality", param.LightingQuality);
            mat.SetFloat("_LightSmoothness", param.LightSmoothness);
            mat.SetFloat("_LightScattering", param.LightScattering);
            mat.SetFloat("_Shading", param.Shading);
            mat.SetFloat("_EdgeLighting", param.EdgeLighting);
            mat.SetFloat("_GlobalLighting", param.GlobalLighting);
            mat.SetFloat("_GlobalLightingRange", param.GlobalLightingRange);

            if (param.Shadow) mat.EnableKeyword("_SHADOW_ON");
            else mat.DisableKeyword("_SHADOW_ON");
            mat.SetFloat("_SHADOW", param.Shadow ? 1f : 0f);
            mat.SetFloat("_ShadowSoftness", param.ShadowSoftness);
            mat.SetFloat("_ShadowQuality", param.ShadowQuality);
            mat.SetFloat("_ShadowStrength", param.ShadowStrength);
            mat.SetFloat("_ShadowThreshold", param.ShadowThreshold);
            mat.SetFloat("_VolumetricShadow", param.VolumetricShadow ? 1f : 0f);
            mat.SetFloat("_VolumetricShadowDensity", param.VolumetricShadowDensity);
            mat.SetFloat("_VolumetricShadowStrength", param.VolumetricShadowStrength);

            mat.SetFloat("_Brightness", param.Brightness);
            mat.SetFloat("_Contrast", param.Contrast);

            mat.SetFloat("_RelativeHeight", param.RelativeHeight ? 1f : 0f);
            if (param.Horizontal) mat.EnableKeyword("_HORIZONTAL_ON");
            else mat.DisableKeyword("_HORIZONTAL_ON");
            mat.SetFloat("_HORIZONTAL", param.Horizontal ? 1f : 0f);

            mat.SetFloat("_FromHeight", param.FromHeight);
            mat.SetFloat("_FromDistance", param.FromDistance);
            mat.SetFloat("_MaxDistance", param.MaxDistance);
            if (param.Horizontal)
                mat.SetFloat("_Thickness", param.ToHeight - param.FromHeight);
            else
                mat.SetFloat("_Thickness", param.Thickness);
            mat.SetFloat("_Iteration", param.Iteration);
            mat.SetFloat("_HorizontalSoftnessTop", param.HorizontalSoftnessTop);
            mat.SetFloat("_HorizontalSoftnessBottom", param.HorizontalSoftnessBottom);
            mat.SetFloat("_HorizontalSoftnessFigure", param.HorizontalSoftnessFigure);
            mat.SetFloat("_Optimize", param.Optimize);

            mat.SetFloat("_Fade", param.Fade);

            if (param.HeightFog) mat.EnableKeyword("_HEIGHTFOG_ON");
            else mat.DisableKeyword("_HEIGHTFOG_ON");
            mat.SetFloat("_HEIGHTFOG", param.HeightFog ? 1f : 0f);

            mat.SetFloat("_GroundHeight", param.GroundHeight);
            mat.SetFloat("_HeightFogFromDistance", param.HeightFogFromDistance);
            mat.SetFloat("_HeightFogRange", param.HeightFogRange);

            // ColorSpace
            mat.SetFloat("_IsLinear", QualitySettings.activeColorSpace == ColorSpace.Linear ? 1f : 0f);
        }
    }
}