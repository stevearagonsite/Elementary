using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


[Serializable]
[PostProcess(typeof(EdgeDetectionRenderer), PostProcessEvent.AfterStack, "Custom/Edge Detection")]
public class EdgeDetection : PostProcessEffectSettings
{
    public FloatParameter deltaX = new FloatParameter { value = 1 };
    public FloatParameter deltaY = new FloatParameter { value = 1 };
    public FloatParameter exponent = new FloatParameter { value = 1 };
    public ColorParameter edgeColor = new ColorParameter { value = Color.white };

}

public class EdgeDetectionRenderer : PostProcessEffectRenderer<EdgeDetection>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/EdgeDetection"));
        sheet.properties.SetFloat("_DeltaX", settings.deltaX / 100);
        sheet.properties.SetFloat("_DeltaY", settings.deltaY / 100);
        sheet.properties.SetFloat("_Exponent", settings.exponent);
        sheet.properties.SetColor("_EdgeColor", settings.edgeColor);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}