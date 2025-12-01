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
        public OnixSetting.SettingChangedDelegate RemoveTrailDelegate { get; set; }

        public OnixSetting CategorySetting;
        public OnixSetting PauseSetting;
        public OnixSetting StopSetting;
        public OnixSetting DoLine;
        public OnixSetting RemoveTrailSetting;
        

        public Trail(string name) {
            Points = new List<Vec3>();
            Render = false;
            Live = true;
            Paused = false;
            Name = name;
            
            PausedTrailDelegate = Pause;
            StopTrailDelegate = Stop;
            DoLineDelegate = DoRenderUpdate;
            RemoveTrailDelegate = Remove;


            CategorySetting = new OnixSettingCategory(BreadCrumbs.Config.CurrentModule,
                name, "A trail", 4);
            BreadCrumbs.Config.CurrentModule.Settings.Append(CategorySetting);
            
            
            
            DoLine = new OnixSettingBool(BreadCrumbs.Config.CurrentModule, "Draw line", false, "Should render line",
                DoLineDelegate);
            
            BreadCrumbs.Config.CurrentModule.Settings.Append(DoLine);

            PauseSetting = new OnixSettingButton(BreadCrumbs.Config.CurrentModule,
                "Pause", "Toggle", "Pause this trail, temporarily stopping it from recording", PausedTrailDelegate);
            BreadCrumbs.Config.CurrentModule.Settings.Append(PauseSetting);


            StopSetting = new OnixSettingButton(BreadCrumbs.Config.CurrentModule,
                "Finish", "Finish", "Stops this trail from recording", StopTrailDelegate);
            BreadCrumbs.Config.CurrentModule.Settings.Append(StopSetting);
            
            RemoveTrailSetting = new OnixSettingButton(BreadCrumbs.Config.CurrentModule,
                "Remove", "Remove", "Removes this trail", RemoveTrailDelegate);
            BreadCrumbs.Config.CurrentModule.Settings.Append(StopSetting);

        }

        public void DoRenderUpdate(OnixModule mod, OnixSetting setting, bool value) {
            Console.WriteLine("INVOKED");
            Render = !Render;
        }
        
        public void RenderLine(RendererWorld gfx) {
            if (!Render) {
                for (int n = 0; n < Points.Count - 1; n++)
                {
                    Vec3 p1 = Points[n];
                    Vec3 p2 = Points[n + 1];

                    int left  = Math.Max(n - 10, 0);
                    int right = Math.Min(n + 10, Points.Count - 1);

                    Vec3 p11 = Points[left];
                    Vec3 p22 = Points[right];

                    int span = right - left;
                    float idealSpan = 20f;   // normal is 10 left + 10 right
                    float scale = span / idealSpan;

                    float raw = (p22 - p11).Length;
                    float normalized = raw / Math.Max(scale, 0.0001f);

                    float t = Math.Clamp(normalized * 0.15f, 0f, 1f);

                    Vec3 slow = new Vec3(1, 0, 0);
                    Vec3 mid  = new Vec3(1, 1, 0);
                    Vec3 fast = new Vec3(0, 1, 0);

                    Vec3 colour = (t < 0.5f)
                        ? slow.Lerp(mid, t * 2f)
                        : mid.Lerp(fast, (t - 0.5f) * 2f);

                    gfx.DrawLine(
                        p1.WithY(p1.Y + 0.01f),
                        p2.WithY(p2.Y + 0.01f),
                        new ColorF(colour.X, colour.Y, colour.Z)
                    );
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

        public void Remove(OnixModule mod, OnixSetting setting, bool value) {
            StopSetting.IsHidden = true;
            PauseSetting.IsHidden = true;
            Live = false;
            Render = false;
            Points = new();
            DoLine.IsHidden = true;
            RemoveTrailSetting.IsHidden = true;
            CategorySetting.IsHidden = true;

        }

        public void Add() {
            if (!Paused && Live) {
                Points.Add(Onix.LocalPlayer.Position);
            }
        }

    }
}