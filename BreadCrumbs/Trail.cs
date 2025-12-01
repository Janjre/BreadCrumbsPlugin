using OnixRuntime.Api;
using OnixRuntime.Api.Errors;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient;
using OnixRuntime.Api.OnixClient.Settings;
using OnixRuntime.Api.Rendering;

namespace BreadCrumbs {
    public class Trail {
        public List<Vec3> Points;
        public bool Render;
        public bool Live;
        public bool Paused;
        public string Name;
        public OnixSetting.SettingChangedDelegate PausedTrailDelegate { get; set; }
        public OnixSetting.SettingChangedDelegate StopTrailDelegate { get; set; }
        public OnixSetting.SettingChangedDelegate DoLineDelegate { get; set; }

        public OnixSetting CategorySetting;
        public OnixSetting PauseSetting;
        public OnixSetting StopSetting;
        public OnixSetting DoLine;
        

        public Trail(string name) {
            Points = new List<Vec3>();
            Render = false;
            Live = true;
            Paused = false;
            Name = name;
            
            PausedTrailDelegate = Pause;
            StopTrailDelegate = Stop;
            DoLineDelegate = DoRenderUpdate;
            
            BreadCrumbs.Config.CurrentModule.Settings.Append(new OnixSettingCategory(BreadCrumbs.Config.CurrentModule,
                name,"A trail",2));

            DoLine = new OnixSettingBool(BreadCrumbs.Config.CurrentModule, "Draw line", true, "Should render line",
                DoLineDelegate);
            BreadCrumbs.Config.CurrentModule.Settings.Append(DoLine);

            PauseSetting = new OnixSettingButton(BreadCrumbs.Config.CurrentModule,
                "Pause", "Toggle", "Pause this trail, temporarily stopping it from recording", PausedTrailDelegate);
            BreadCrumbs.Config.CurrentModule.Settings.Append(PauseSetting);


            StopSetting = new OnixSettingButton(BreadCrumbs.Config.CurrentModule,
                "Finish", "Finish", "Stops this trail from recording", StopTrailDelegate);
            BreadCrumbs.Config.CurrentModule.Settings.Append(StopSetting);

        }

        public void DoRenderUpdate(OnixModule mod, OnixSetting setting, bool value) {
            Render = !Render;
        }
        
        public void RenderLine(RendererWorld gfx) {
            if (Render) {
                for (int n = 0; n < Points.Count-1; n++) {
                    Vec3 p1 = Points[n];
                    Vec3 p2 = Points[n+1];
                    gfx.DrawLine(p1,p2,ColorF.Red);
                }
            }
        }

        public void Pause(OnixModule mod, OnixSetting setting, bool value) {
            Paused = !Paused;
            if (Paused) {
                setting.Name = "Play";
                setting.Description = "Play this trail, starting it recording";
            } else {
                setting.Name = "Pause";
                setting.Description = "Pause this trail, temporarily stopping it from recording";
            }
            
        }
        
        

        public void Stop(OnixModule mod, OnixSetting setting, bool value) {
            Live = false;
            Paused = false;
            PauseSetting.IsHidden = true;
            StopSetting.IsHidden = true;
            
        }

        public void Add() {
            if (!Paused && Live) {
                Points.Add(Onix.LocalPlayer.Position);
            }
        }

    }
}