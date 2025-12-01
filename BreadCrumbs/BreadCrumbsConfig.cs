using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.OnixClient;
using OnixRuntime.Api.OnixClient.Settings;

namespace BreadCrumbs {
    public partial class BreadCrumbsConfig : OnixModuleSettingRedirector {
        void BeginTrail() {
            BreadCrumbs.Instance.NumberOfTrails += 1;
            Console.WriteLine("HI");
            BreadCrumbs.Instance.Trails.Add(new Trail($"New trail {BreadCrumbs.Instance.NumberOfTrails}"));
            

        }
        [Button(nameof(BeginTrail), "Start")]
        [Name("New trail", "Begin a trail!")]
        public partial OnixSetting.SettingChangedDelegate StealAllSkins { get; set; }

    }
}