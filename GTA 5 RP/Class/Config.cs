﻿using Newtonsoft.Json;

namespace GTA_5_RP
{
    public class IConfig
    {
        [JsonIgnore]
        private static string? File { get; set; }

        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        [JsonProperty]
        public string? Token { get; set; }

        public bool ShouldSerializeToken() => !string.IsNullOrEmpty(Token);

        [JsonProperty("Chat ID")]
        public int ChatID { get; set; }

        public bool ShouldSerializeChatID() => ChatID > 0;

        public static (string? ErrorMessage, IConfig? Config) Load(string Directory, string _File)
        {
            if (!string.IsNullOrEmpty(Directory) && !System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            File = _File;

            if (!string.IsNullOrEmpty(File) && !System.IO.File.Exists(File))
            {
                System.IO.File.WriteAllText(File, JsonConvert.SerializeObject(new IConfig(), Formatting.Indented));
            }

            string Json;

            try
            {
                Json = System.IO.File.ReadAllText(File);
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (string.IsNullOrEmpty(Json) || Json.Length == 0)
            {
                return ("Данные равны нулю!", null);
            }

            IConfig Config;

            try
            {
                Config = JsonConvert.DeserializeObject<IConfig>(Json)!;
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (Config == null)
            {
                return ("Глобальный конфиг равен нулю!", null);
            }

            Config.Save();

            return (null, Config);
        }

        public async void Save()
        {
            if (string.IsNullOrEmpty(File) || (this == null)) return;

            string JSON = JsonConvert.SerializeObject(this, Formatting.Indented);
            string _ = File + ".new";

            await Semaphore.WaitAsync();

            try
            {
                System.IO.File.WriteAllText(_, JSON);

                if (System.IO.File.Exists(File))
                {
                    System.IO.File.Replace(_, File, null);
                }
                else
                {
                    System.IO.File.Move(_, File);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
