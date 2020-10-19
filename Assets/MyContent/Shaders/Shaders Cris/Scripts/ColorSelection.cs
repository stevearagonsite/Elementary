using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ColorSelectionRenderer), PostProcessEvent.AfterStack, "Custom/ColorSelection")]
public class ColorSelection : PostProcessEffectSettings
{
    //offset
    //filterColor
    public ColorParameter filterColor = new ColorParameter { value = new Color(0, 0, 0, 0) };
    public Vector3Parameter offset = new Vector3Parameter { value = new Vector3(1,1,1) };
}

public class ColorSelectionRenderer : PostProcessEffectRenderer<ColorSelection>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ColorSelection"));
        sheet.properties.SetColor("_FilterColor", settings.filterColor);
        sheet.properties.SetVector("_Offset", settings.offset);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
