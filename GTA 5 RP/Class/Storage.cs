using Newtonsoft.Json;

namespace GTA_5_RP
{
    public class IStorage
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        [JsonProperty("Список")]
        public Dictionary<string, DateTime> Separate { get; set; } = new();

        [JsonProperty("Уведомления")]
        public Dictionary<string, DateTime> Notice { get; set; } = new();

        public static bool ShouldSerializeNotice() => Program.Unique;

        [JsonProperty("Выгода")]
        public Dictionary<string, DateTime> Incentive { get; set; } = new();

        public static bool ShouldSerializeIncentive() => Program.Unique;

        public async void Save(string File)
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
