using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(GaussianBlurRenderer), PostProcessEvent.AfterStack, "Custom/Gaussian Blur")]
public class GaussianBlur : PostProcessEffectSettings
{ 
    [Range(0,1)]
    public FloatParameter delta = new FloatParameter { value = 1 };
    public FloatParameter blur = new FloatParameter { value = 1 };
}

public class GaussianBlurRenderer : PostProcessEffectRenderer<GaussianBlur>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/GaussianBlur"));
        sheet.properties.SetFloat("_Delta", settings.delta/100);
        sheet.properties.SetFloat("_Blur", settings.blur);
        sheet.properties.SetFloat("_Compensator", 4);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
