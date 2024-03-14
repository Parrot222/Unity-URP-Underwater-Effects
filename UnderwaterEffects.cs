using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class UnderwaterEffects : ScriptableRendererFeature
{
  [System.Serializable]
  public class MyFeatureSettings
  {
    // we're free to put whatever we want here, public fields will be exposed in the inspector
    public bool IsEnabled = true;
    public RenderPassEvent WhenToInsert = RenderPassEvent.AfterRenderingOpaques;
    public Color color;
    public float distance = 10;
    [Range(0, 1)]
    public float alpha;
    public float refraction = 0.01f;
    public Texture normalmap;
    public Vector4 UV = new Vector4(1,1,0.2f,0.1f);
  }

  Material MaterialToBlit;

  // MUST be named "settings" (lowercase) to be shown in the Render Features inspector
  public MyFeatureSettings settings = new MyFeatureSettings();

  RenderTargetHandle renderTextureHandle;
  UnderwaterPass underwaterPass;

  public override void Create()
  {
    MaterialToBlit = new Material(Shader.Find("Paro222/UnderwaterEffects"));
    underwaterPass = new UnderwaterPass(
      "Underwater Effects",
      settings.WhenToInsert,
      MaterialToBlit,
      settings.distance,
      settings.color,
      settings.alpha,
      settings.refraction,
      settings.normalmap,
      settings.UV
    );
  }

  // called every frame once per camera
  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    if (!settings.IsEnabled)
    {
      // we can do nothing this frame if we want
      return;
    }

    // Gather up and pass any extra information our pass will need.
    // In this case we're getting the camera's color buffer target
    var cameraColorTargetIdent = renderer.cameraColorTarget;
    underwaterPass.Setup(cameraColorTargetIdent);

    // Ask the renderer to add our pass.
    // Could queue up multiple passes and/or pick passes to use
    renderer.EnqueuePass(underwaterPass);
  }
}