using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Plugin;
using OnixRuntime.Api.Rendering;
using System.Diagnostics;

namespace BreadCrumbs {
    public class BreadCrumbs : OnixPluginBase {
        public static BreadCrumbs Instance { get; private set; } = null!;
        public static BreadCrumbsConfig Config { get; private set; } = null!;

        public BreadCrumbs(OnixPluginInitInfo initInfo) : base(initInfo) {
            Instance = this;
            // If you can clean up what the plugin leaves behind manually, please do not unload the plugin when disabling.
            base.DisablingShouldUnloadPlugin = false;
#if DEBUG
           // base.WaitForDebuggerToBeAttached();
#endif
        }

        protected override void OnLoaded() {
            Console.WriteLine($"Plugin {CurrentPluginManifest.Name} loaded!");
            Config = new BreadCrumbsConfig(PluginDisplayModule, true);
            Onix.Events.Common.Tick += OnTick;
            Onix.Events.Common.HudRender += OnHudRender;
            Onix.Events.Common.HudRenderGame += OnHudRenderGame;
            Onix.Events.Common.WorldRender += OnWorldRender;
            Onix.Events.Input.Input += OnKey;

        }

        protected override void OnEnabled() {

        }

        protected override void OnDisabled() {

        }

        protected override void OnUnloaded() {
            // Ensure every task or thread is stopped when this function returns.
            // You can give them base.PluginEjectionCancellationToken which will be cancelled when this function returns. 
            Console.WriteLine($"Plugin {CurrentPluginManifest.Name} unloaded!");
            Onix.Events.Common.Tick -= OnTick;
            Onix.Events.Common.HudRender -= OnHudRender;
            Onix.Events.Common.HudRenderGame -= OnHudRenderGame;
            Onix.Events.Common.WorldRender -= OnWorldRender;
            Onix.Events.Input.Input -= OnKey;

            foreach (var setting in Config.CurrentModule.Settings.Settings) {
                setting.RemoveFromParent();
            }
        }

        private int GetNumber(InputKey key) {
            if (key == InputKey.Type.Num0) { return 0; }
            if (key == InputKey.Type.Num1) { return 1; }
            if (key == InputKey.Type.Num2) { return 2; }
            if (key == InputKey.Type.Num3) { return 3; }
            if (key == InputKey.Type.Num4) { return 4; }
            if (key == InputKey.Type.Num5) { return 5; }
            if (key == InputKey.Type.Num6) { return 6; }
            if (key == InputKey.Type.Num7) { return 7; }
            if (key == InputKey.Type.Num9) { return 8; }
            if (key == InputKey.Type.Num0) { return 9; }

            return -1;
        }
        private bool OnKey(InputKey key, bool isDown) {
            if (isDown && key == Config.AddKey) {
                BreadCrumbs.Instance.NumberOfTrails += 1;
                BreadCrumbs.Instance.Trails.Add(new Trail($"New trail {BreadCrumbs.Instance.NumberOfTrails}"));
            }
            
            if (Onix.Input.IsDown(InputKey.Type.P) || Onix.Input.IsDown(InputKey.Type.F)) {
                int number = GetNumber(key);
                if (number != -1) {
                    int trailCounter = 0;
                    foreach (Trail trail in Trails) {
                        if (trail.Live) {
                            if (trailCounter == number) {

                                if (Onix.Input.IsDown(InputKey.Type.F)) {
                                    trail.RealStop();
                                } else {
                                    trail.RealPause(trail.PauseSetting);
                                }
                                
                            }
                            trailCounter += 1;
                        }
                    }
                }
            }

            if (SettingUpLap) {
                if (key == InputKey.Type.LMB && isDown) {
                    ResetBlocks.Add(Onix.LocalPlayer.Raycast.BlockPosition);
                    return true;
                }

                if (key == InputKey.Type.Enter) {
                    SettingUpLap = false;
                    return true;
                }
            }

            return false;

        }
        
        private void OnTick() {
            foreach (Trail trail in Trails) {
                trail.Add();
            }
        }

        private void OnHudRender(RendererCommon2D gfx, float delta) {
            int trailCounter = 0;
            foreach (Trail trail in Trails) {
                if (trail.Live) {
                    (ColorF colour, string text) = trail.GetDisplayInfo(trailCounter);
                    
                    gfx.RenderText(new Vec2(0,20*trailCounter),colour,text);
                    trailCounter += 1;
                }
            }

            if (SettingUpLap) {
                Onix.Gui.SetActionbarText("Left click a block to add it to reset selection. Press enter to confirm");
            }
        }

        private void OnHudRenderGame(RendererGame gfx, float delta) {
            
        }

        private void OnWorldRender(RendererWorld gfx, float delta) {
            foreach (Trail trail in Trails) {
                trail.RenderLine(gfx,delta);
            }

            foreach (BlockPos pos in ResetBlocks) {
                gfx.RenderBoundingBoxOutline(pos.BoundingBox,ColorF.Red);
            }
        }

        public List<Trail> Trails = new List<Trail>();
        public int NumberOfTrails = 0;
        public List<BlockPos> ResetBlocks = new();
        public bool SettingUpLap = false;
    }
}