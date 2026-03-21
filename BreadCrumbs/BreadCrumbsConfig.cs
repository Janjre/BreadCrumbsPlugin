using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.OnixClient;


namespace BreadCrumbs {
    public partial class BreadCrumbsConfig : OnixModuleSettingRedirector {
        [Value(InputKey.Type.Execute)]
        [Name("Start new trail", "Start a new trail from anywhere")]
        
        public partial InputKey AddKey { get; set; }
        
        
        
        [Value(true)]
        [Name("Delete old trail points", "Use the slider below to change how many points there can be in a trail at one time.")]
        public partial bool ShortenPoints { get; set; }
        
        [Value(3_500)]
        [MinMax(0, 10_000)]
        [Name("Maximum trail length", "How long the list of points in a trail should be. A point is added every tick.")]
        public partial int MaxLength { get; set; }
        
        
        void BeginTrail() {
            BreadCrumbs.Instance.NumberOfTrails += 1;
            BreadCrumbs.Instance.Trails.Add(new Trail($"New trail {BreadCrumbs.Instance.NumberOfTrails}"));
        }
        [Button(nameof(BeginTrail), "Start")]
        [Name("New trail", "Begin a trail!")]
        public partial OnixSetting.SettingChangedDelegate BeginTrailsDelegateInSettings { get; set; }
        

    }
}