using BepInEx;
using GorillaNetworking;
using GorillaTagScripts;
using System.IO;
using System.Reflection;
using UnityEngine;
#nullable disable
namespace OutfitSwapV2;
[BepInPlugin("whongt.outfitswapv2", "OutfitSwapV2", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
  private float LastPressForward;
  private float LastPressBackward;
  private bool WasPressingForward;
  private bool WasPressingBackward;
  private void Update()
  {
    int maxOutfits = SubscriptionManager.IsLocalSubscribed() ? 10 : 4;

    // forward: right controller primary button
    bool pressingForward = ControllerInputPoller.instance.rightControllerPrimaryButton;
    if (pressingForward && !this.WasPressingForward)
    {
      if ((double) Time.time - (double) this.LastPressForward < 0.30000001192092896)
      {
        Plugin.Wear(CosmeticsController.SelectedOutfit + 1 >= maxOutfits ? 0 : CosmeticsController.SelectedOutfit + 1);
        this.LastPressForward = 0.0f;
      }
      else
        this.LastPressForward = Time.time;
    }
    this.WasPressingForward = pressingForward;

    // backward: left controller primary button
    bool pressingBackward = ControllerInputPoller.instance.leftControllerPrimaryButton;
    if (pressingBackward && !this.WasPressingBackward)
    {
      if ((double) Time.time - (double) this.LastPressBackward < 0.30000001192092896)
      {
        Plugin.Wear(CosmeticsController.SelectedOutfit - 1 < 0 ? maxOutfits - 1 : CosmeticsController.SelectedOutfit - 1);
        this.LastPressBackward = 0.0f;
      }
      else
        this.LastPressBackward = Time.time;
    }
    this.WasPressingBackward = pressingBackward;
  }
  private static void Wear(int outfitIndex)
  {
    GorillaTagger.Instance.StartVibration(false, 0.08f, 0.1f);
    CosmeticsController.instance.LoadSavedOutfit(outfitIndex);
  }
  public static void ApplyHarmonyPatches()
  {
    Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Harmony.PatchInfo.bin");
    MemoryStream destination = new MemoryStream();
    manifestResourceStream.CopyTo((Stream) destination);
    Assembly.Load(destination.ToArray()).GetType("HarmonyX.Internal.PatchProcessor").GetMethod("Initialize").Invoke((object) null, (object[]) null);
  }
  public void Start() => Plugin.ApplyHarmonyPatches();
}