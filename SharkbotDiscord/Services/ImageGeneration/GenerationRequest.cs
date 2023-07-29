namespace SharkbotDiscord.Services.ImageGeneration
{
    public class GenerationRequest
    {
        public bool enable_hr { get; set; }
        public int denoising_strength { get; set; }
        public int firstphase_width { get; set; }
        public int firstphase_height { get; set; }
        public int hr_scale { get; set; }
        public string hr_upscaler { get; set; }
        public int hr_second_pass_steps { get; set; }
        public int hr_resize_x { get; set; }
        public int hr_resize_y { get; set; }
        public string hr_sampler_name { get; set; }
        public string hr_prompt { get; set; }
        public string hr_negative_prompt { get; set; }
        public string prompt { get; set; }
        public List<string> styles { get; set; }
        public int seed { get; set; }
        public int subseed { get; set; }
        public int subseed_strength { get; set; }
        public int seed_resize_from_h { get; set; }
        public int seed_resize_from_w { get; set; }
        public string sampler_name { get; set; }
        public int batch_size { get; set; }
        public int n_iter { get; set; }
        public int steps { get; set; }
        public int cfg_scale { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool restore_faces { get; set; }
        public bool tiling { get; set; }
        public bool do_not_save_samples { get; set; }
        public bool do_not_save_grid { get; set; }
        public string negative_prompt { get; set; }
        public int eta { get; set; }
        public int s_min_uncond { get; set; }
        public int s_churn { get; set; }
        public int s_tmax { get; set; }
        public int s_tmin { get; set; }
        public int s_noise { get; set; }
        public OverrideSettings override_settings { get; set; }
        public bool override_settings_restore_afterwards { get; set; }
        public List<object> script_args { get; set; }
        public string sampler_index { get; set; }
        public string script_name { get; set; }
        public bool send_images { get; set; }
        public bool save_images { get; set; }
        public AlwaysonScripts alwayson_scripts { get; set; }
    }

    public class AlwaysonScripts
    {
    }

    public class OverrideSettings
    {
    }
}
