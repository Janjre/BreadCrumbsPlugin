using OnixRuntime.Api;
using OnixRuntime.Plugin;
using OnixRuntime.Api.Rendering;

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
        }

        private void OnTick() {
            foreach (Trail trail in Trails) {
                trail.Add();
            }
        }

        private void OnHudRender(RendererCommon2D gfx, float delta) {
        }

        private void OnHudRenderGame(RendererGame gfx, float delta) {
            
        }

        private void OnWorldRender(RendererWorld gfx, float delta) {
            foreach (Trail trail in Trails) {
                trail.RenderLine(gfx);
            }
        }

        public List<Trail> Trails = new List<Trail>();
        public int NumberOfTrails = 0;
    }
}