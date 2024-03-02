using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace GTA_5_RP
{
    public partial class Program
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static bool Unique;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        private static readonly string ConfigDirectory = "config";

        private static string Directory = "";
        private static string UserFile = "";
        private static string StorageFile = "";

        private static Telegram? Telegram;
        private static IStorage? Storage;

        #region VIP

        private static readonly DateTime END_VIP = new(2024, 01, 11, 07, 00, 00);

        private static bool VIP => DateTime.Now <= END_VIP;

        #endregion

        #region X2

        private static readonly DateTime START_X2 = new(2023, 02, 10, 07, 00, 00);
        private static readonly DateTime END_X2 = new(2024, 02, 12, 07, 00, 00);

        private static bool X2 => DateTime.Now >= START_X2 && DateTime.Now <= END_X2;

        #endregion

        private const uint WM_SETICON = 0x0080;

        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        public const int SWP_NOSIZE = 0x0001;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        #region Message

        public enum EMessage : byte
        {
            DISABLED,
            ENABLED,
            MUTE
        }

        private static EMessage Message;

        #endregion

        #region Security

        public class ISecurity
        {
            public bool Active { get; set; }

            public Thread? Thread { get; set; }

            public DateTime? Date { get; set; }
            public TimeSpan? Header { get; set; }

            public void DoThread(TimeSpan Time)
            {
                Thread = new Thread(() =>
                {
                    try
                    {
                        Date = DateTime.Now.Add(Time);

                        for (var N = DateTime.Now; N <= Date; N = N.AddSeconds(1))
                        {
                            Header = Date - N;

                            Thread.Sleep(1000);
                        }

                        Active = true;
                    }
                    catch (ThreadInterruptedException) { }
                    finally
                    {
                        Date = null;
                        Header = null;
                    }
                });

                Thread.Start();
            }
        }

        private static readonly ISecurity Security = new();

        #endregion

        #region User

        public class IUser
        {
            public string Token { get; set; }

            public bool Background { get; set; }
            public bool Message { get; set; }

            public Dictionary<string, bool> Security { get; set; }

            public IUser(string Token)
            {
                this.Token = Token;

                Background = true;
                Message = true;

                Security = new();
            }

            public string Login { get; set; } = "UNDEFINED";
            public string Name { get; set; } = "UNDEFINED";

            #region History

            public class IHistory
            {
                public string ID { get; set; }

                public IHistory(string ID, long LastMessageID)
                {
                    this.ID = ID;

                    Update(LastMessageID);
                }

                public long LastMessageID { get; set; }
                public DateTime DateTime { get; set; }

                public void Update(long LastMessageID)
                {
                    this.LastMessageID = LastMessageID;

                    DateTime = ToDateTime(LastMessageID);
                }
            }

            public List<IHistory>? History { get; set; }

            #endregion

            public int? Error { get; set; }

            public double? Time { get; set; }

            public override string ToString()
            {
                return $"[{(Background ? "X" : "~")}] {Name}{(Error.HasValue ? $" | ERROR: {Error}" : "")}";
            }
        }

        private static readonly List<IUser> UserList = new();

        #endregion

        #region Separate

        public class ISeparate
        {
            public string ID { get; set; }

            public ISeparate(string ID)
            {
                this.ID = ID;
            }

            public bool? Enabled { get; set; }

            public DateTime? Date { get; set; }

            #region Position 

            public enum EPosition : byte
            {
                ACTIVE,
                INACTIVE,
                NEXT,
                UNABLE
            }

            public EPosition Position { get; set; }

            #endregion

            public string? Content { get; set; }

            #region Car

            public class ICar
            {
                public string Key { get; set; }

                #region List

                public class IList
                {
                    [JsonProperty]
                    public string Name { get; set; }

                    [JsonProperty]
                    public string Alternative { get; set; }

                    [JsonProperty]
                    public List<string> IDs { get; set; }

                    [JsonProperty]
                    public List<string> Description { get; set; }

                    [JsonProperty]
                    public Dictionary<int, decimal> Value { get; set; }

                    [JsonProperty]
                    public string Spoiler { get; set; }

                    [JsonConstructor]
                    public IList(string Name, string Alternative, List<string> IDs, List<string> Description, Dictionary<int, decimal> Value, string Spoiler)
                    {
                        Certificate ??= new();

                        foreach (string ID in IDs)
                        {
                            Certificate.Add(new(ID, Name));
                        }

                        this.Name = Name;
                        this.Alternative = Alternative;
                        this.IDs = IDs;
                        this.Description = Description;
                        this.Value = Value;
                        this.Spoiler = Spoiler;
                    }

                    public enum ETuning : byte
                    {
                        NONE,
                        FT,
                        FFT
                    }

                    [JsonProperty]
                    public ETuning Tuning { get; set; }

                    [JsonProperty]
                    public string Image { get; set; } = "";

                    #region Certificate

                    public class ICertificate
                    {
                        public string ID { get; set; }
                        public string Name { get; set; }
                        public bool Active { get; set; }

                        public ICertificate(string ID, string Name)
                        {
                            this.ID = ID;
                            this.Name = $"{ID} - {Name}";

                            Active = true;
                        }

                        #region History

                        public class IHistory
                        {
                            public int Hour { get; set; }
                            public decimal Price { get; set; }

                            public IHistory(int Hour, decimal Price)
                            {
                                this.Hour = Hour;
                                this.Price = Price;
                            }
                        }

                        public List<IHistory>? History { get; set; }

                        #endregion

                        public Thread? Thread { get; set; }

                        public DateTime? Date { get; set; }
                        public TimeSpan? Header { get; set; }

                        public void DoThread(TimeSpan Time)
                        {
                            Thread = new Thread(() =>
                            {
                                try
                                {
                                    Active = false;
                                    Date = DateTime.Now.Add(Time);

                                    if (Storage is not null)
                                    {
                                        if (Storage.Separate.TryAdd(Name, Date.Value))
                                        {
                                            Storage.Save(StorageFile);
                                        }
                                    }

                                    for (var N = DateTime.Now; N <= Date; N = N.AddSeconds(1))
                                    {
                                        Header = Date - N;

                                        Thread.Sleep(1000);
                                    }

                                    if (Message > EMessage.DISABLED)
                                    {
                                        if (Telegram is not null)
                                        {
                                            _ = Telegram.SendMessage($"\"{Name}\" освободился!", Message == EMessage.MUTE);
                                        }
                                    }
                                }
                                catch (ThreadInterruptedException) { }
                                finally
                                {
                                    Active = true;
                                    Date = null;
                                    Header = null;

                                    if (Storage is not null)
                                    {
                                        if (Storage.Separate.ContainsKey(Name))
                                        {
                                            Storage.Separate.Remove(Name);
                                            Storage.Save(StorageFile);
                                        }
                                    }
                                }
                            });

                            Thread.Start();
                        }

                        public string Format(string? X = "")
                        {
                            string A = $"[{(Active ? "√" : "X")}]";
                            string H = "";

                            if (Header.HasValue)
                            {
                                H = $"~ {DoTime(Header.Value)}";
                            }

                            return string.Join(" ", new string[] { A, string.IsNullOrEmpty(X) ? ID : X, H });
                        }
                    }

                    [JsonIgnore]
                    public List<ICertificate> Certificate { get; set; }

                    #endregion
                }

                public List<IList> List { get; set; }

                #endregion

                public ICar(string Key, List<IList> List)
                {
                    string N = Path.GetDirectoryName(Key)!;

                    foreach (var Value in List)
                    {
                        if (string.IsNullOrEmpty(Value.Image)) continue;

                        Value.Image = Path.Combine(N, Value.Image);
                    }

                    this.Key = Key;
                    this.List = List;
                }
            }

            public ICar? Car { get; set; }

            #endregion

            public List<string>? Image { get; set; }

            public string? GatherContent()
            {
                if (Car is not null && Car.List.Count > 0)
                {
                    List<string> List = new();

                    foreach (var Value in Car.List)
                    {
                        List.Add("⠀");
                        List.Add($"# :{(Value.Certificate.Any(x => x.Active) ? "green" : "red")}_square: {Value.Name}{(Value.Tuning > 0 ? $" [{Value.Tuning}]" : "")} || {Value.Alternative} ||");
                        List.Add("⠀");

                        #region Description

                        List.Add("```cs");

                        foreach (string T1 in Value.Description)
                        {
                            List.Add($" {T1}");
                        }

                        List.Add("```");

                        #endregion

                        #region Value

                        List.Add("```cs");

                        foreach (var Pair in Value.Value)
                        {
                            if (Pair.Key > 99 && X2) continue;

                            List.Add($" {Pair.Key} {(Pair.Key == 1 ? "час" : Pair.Key < 5 ? "часа" : "часов")} - {Pair.Value}$");
                        }

                        List.Add("```");

                        #endregion
                    }

                    List.Add("⠀");
                    List.Add($"|| сдам, аренда, {string.Join(", ", Car.List.Select(x => x.Spoiler))} ||");

                    return string.Join(Environment.NewLine, List);
                }

                if (string.IsNullOrEmpty(Content))
                {
                    return null;
                }

                return File.ReadAllText(Content);
            }

            public List<string>? GatherImage()
            {
                if (Car is not null && Car.List.Count > 0)
                {
                    var List = Car.List
                        .Where(x => x.Certificate.Any(v => v.Active))
                        .Select(x => x.Image)
                        .ToList();

                    if (List.Count == 1)
                    {
                        List.RemoveAll(x => string.IsNullOrEmpty(x));

                        return List;
                    }
                }

                if (Image == null || Image.Count == 0)
                {
                    return null;
                }

                return Image;
            }

            public bool Can
            {
                get
                {
                    bool Has = Enabled.HasValue && Enabled.Value;
                    bool Any = Car is not null && Car.List.SelectMany(x => x.Certificate).Any(x => x.Active);

                    return Has || Any;
                }
            }
        }

        private static List<ISeparate> SeparateList = new();

        #endregion

        #region Notice

        public class INotice
        {
            public string Name { get; set; } = "";
            public int Group { get; set; }

            public TimeSpan? Time { get; set; }

            public bool Beep { get; set; }
            public bool Dump { get; set; } = true;

            public bool Notification { get; set; } = true;

            public List<INotice>? List { get; set; }

            public bool Active { get; set; }

            public void Modify()
            {
                if (Active)
                {
                    Factory();

                    Active = false;
                }
                else
                {
                    Active = true;
                }
            }

            public CancellationTokenSource? Source { get; set; }

            public DateTime? Date { get; set; }
            public TimeSpan? Header { get; set; }

            private void Factory()
            {
                Source?.Cancel();

                Date = null;
                Header = null;
            }

            public Dictionary<string, INotice> Recursion(string Name, List<INotice> List)
            {
                var Dictionary = new Dictionary<string, INotice>();

                foreach (var Notice in List)
                {
                    string N = $"{Name} - {Notice.Name}";

                    if (Notice.List == null || Notice.List.Count == 0)
                    {
                        Dictionary.Add(N, Notice);

                        continue;
                    }

                    foreach (var T in Recursion(N, Notice.List))
                    {
                        Dictionary.Add(T.Key, T.Value);
                    }
                }

                return Dictionary;
            }

            public char Condition
            {
                get
                {
                    if (Active)
                    {
                        return 'X';
                    }
                    else if (Time.HasValue)
                    {
                        return '√';
                    }
                    else if (List is not null && Recursion(Name, List).Any(x => x.Value.Active))
                    {
                        return 'X';
                    }

                    return '~';
                }
            }

            public override string ToString()
            {
                string H = "";

                if (Active)
                {
                    if (Header.HasValue)
                    {
                        H = $"~ {DoTime(Header.Value)}";
                    }

                }
                else
                {
                    if (Date.HasValue)
                    {
                        H = $"- {Date.Value:hh:mm:ss}";
                    }
                }

                return string.Join(" ", new string[] { $"[{Condition}]", Name, H });
            }
        }

        private static readonly List<INotice> NoticeList = new();

        private static Dictionary<string, INotice> NoticeDictionary
        {
            get
            {
                var Dictionary = new Dictionary<string, INotice>();

                foreach (var X in NoticeList)
                {
                    if (X == null) continue;

                    if (X.Time.HasValue)
                    {
                        Dictionary.Add(X.Name, X);
                    }

                    if (X.List == null || X.List.Count == 0) continue;

                    foreach (var T in X.Recursion(X.Name, X.List))
                    {
                        Dictionary.Add(T.Key, T.Value);
                    }
                }

                return Dictionary;
            }
        }

        #endregion

        #region Incentive

        public class IIncentive
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public int Start { get; set; }
            public int Per { get; set; }

            public int Count { get; set; }

            public IIncentive(string Name, int Value, int Start = 0, int Per = 0, int Count = 1)
            {
                this.Name = Name;

                if (VIP)
                {
                    Value *= 2;
                }

                if (X2)
                {
                    Value *= 2;
                }

                this.Value = Value;

                this.Start = Start;
                this.Per = Per;

                this.Count = Count;
            }

            public int Index { get; set; }

            public Thread? Thread { get; set; }

            public DateTime? Date { get; set; }

            public void DoThread(DateTime X)
            {
                Thread = new Thread(() =>
                {
                    try
                    {
                        Date = X;

                        if (Storage is not null)
                        {
                            if (Storage.Incentive.TryAdd(Name, Date.Value))
                            {
                                Storage.Save(StorageFile);
                            }
                        }

                        for (var N = DateTime.Now; N <= Date; N = N.AddMinutes(1))
                        {
                            Thread.Sleep(60000);
                        }

                        Index = 0;
                    }
                    catch (ThreadInterruptedException) { }
                    finally
                    {
                        Date = null;

                        if (Storage is not null)
                        {
                            if (Storage.Incentive.ContainsKey(Name))
                            {
                                Storage.Incentive.Remove(Name);
                                Storage.Save(StorageFile);
                            }
                        }
                    }
                });

                Thread.Start();
            }

            public override string ToString()
            {
                int[] Array = new int[] { Start, Per }.Where(x => x > 0).ToArray();

                return $"[{(Index == Count ? "√" : "X")}] {Name} - {Value} BP{(Array.Length == 0 ? "" : " = ")}{string.Join(" | ", Array.Select(N => N + "$"))}{(Count > 1 ? $" ({Index}/{Count})" : "")}";
            }
        }

        private static readonly List<IIncentive> IncentiveList = new();

        #endregion

        #region Enumerator

        public class IEnumerator : System.Collections.IEnumerator
        {
            private readonly int[] List;

            public IEnumerator(int[] List)
            {
                this.List = List;
            }

            public int Position { get; set; }

            public bool MoveNext() => ++Position >= List.Length;

            public void Reset()
            {
                Position = 0;
            }

            object System.Collections.IEnumerator.Current => Current;

            public int Current
            {
                get => List[Position];
            }
        }


        #endregion


        [DllImport("shell32.dll", SetLastError = true)]
        static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        [SupportedOSPlatform("Windows")]
        public static void Main(string[] A)
        {
            if (!System.IO.Directory.Exists(ConfigDirectory)) System.IO.Directory.CreateDirectory(ConfigDirectory);

            DoBar();

            var Execute = Assembly.GetExecutingAssembly().GetName();

            if (Execute == null || string.IsNullOrEmpty(Execute.Name) || Execute.Version == null) return;

            var List = new List<string>
            {
                Execute.Name,
                Execute.Version.ToString()
            };

            if (A.Length > 0)
            {
                List.Add("X");
            }

            _ = new Mutex(true, string.Join("-", List), out Unique);

            switch (A.Length)
            {
                case 2 when Unique:

                    string[] Directories = System.IO.Directory.GetDirectories(ConfigDirectory);

                    if (Directories.Length == 0) return;

                    foreach (var T in Directories)
                    {
                        var Process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = Environment.ProcessPath,
                                UseShellExecute = true,
                                Arguments = Path.GetFileName(T) + " " + string.Join(" ", A)
                            }
                        };

                        Process.Start();
                    }

                    Environment.Exit(0);

                    break;

                case 3:

                    Directory = Path.Combine(ConfigDirectory, A[0]);

                    if (System.IO.Directory.Exists(Directory) && int.TryParse(Path.GetFileName(Directory), out int Index))
                    {
                        UserFile = Path.Combine(Directory, "!.txt");
                        StorageFile = Path.Combine(Directory, "!.json");

                        Telegram = new Telegram(A[1], int.Parse(A[2]));

                        #region User

                        if (!File.Exists(UserFile)) File.Create(UserFile);

                        string[] T_User = File.ReadAllLines(UserFile);

                        if (T_User.Length == 0) return;

                        foreach (string Token in T_User)
                        {
                            if (Regex.IsMatch(Token, @"^([a-zA-Z0-9_-]{24}\.[a-zA-Z0-9_-]{6}\.[a-zA-Z0-9_-]{38})$"))
                            {
                                UserList.Add(new IUser(Token));
                            }
                        }

                        if (UserList.Count == 0) return;

                        #endregion

                        #region Storage

                        if (!File.Exists(StorageFile)) File.WriteAllText(StorageFile, JsonConvert.SerializeObject(new IStorage(), Formatting.Indented));

                        string T_Storage = File.ReadAllText(StorageFile);

                        if (string.IsNullOrEmpty(T_Storage)) return;

                        Storage = JsonConvert.DeserializeObject<IStorage>(T_Storage);

                        if (Storage == null) return;

                        #endregion

                        Destruction(Debug());

                        int X = 100;
                        int Y = 100;

                        if (Index > 1)
                        {
                            X = Index * 180;
                        }

                        SetWindowPos(GetConsoleWindow(), IntPtr.Zero, X, Y, 0, 0, SWP_NOSIZE);

                        using (Bitmap Bitmap = new(32, 32))
                        {
                            var Icon = Properties.Resources.Icon;

                            using Graphics Graphics = System.Drawing.Graphics.FromImage(Bitmap);

                            Graphics.DrawIcon(Icon, 0, 0);
                            Graphics.DrawString(
                                Index.ToString(),
                                new Font("Calibri", 12, FontStyle.Regular),
                                Brushes.White,
                                new Rectangle(0, 0, Icon.Width, Icon.Height),
                                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                            IntPtr Hicon = Bitmap.GetHicon();

                            var Current = Process.GetCurrentProcess();

                            SendMessage(Current.MainWindowHandle, WM_SETICON, ICON_SMALL, Hicon);
                            SendMessage(Current.MainWindowHandle, WM_SETICON, ICON_BIG, Hicon);
                        }

                        Init();
                    }

                    break;
            }

            Console.ReadLine();
        }

        private static void Init()
        {
            string[] Directories = System.IO.Directory.GetDirectories(Directory);

            if (Directories.Length == 0) return;

            foreach (string Directory in Directories)
            {
                string Name = Path.GetFileName(Directory);

                if (Regex.IsMatch(Name, @"^\d+$"))
                {
                    string[] Files = System.IO.Directory.GetFiles(Directory);

                    if (Files.Length == 0) continue;

                    var X = new ISeparate(Name);

                    foreach (string File in Files.Where(x => x.EndsWith(".txt")))
                    {
                        X.Content = File;
                    }

                    foreach (string File in Files.Where(x => x.EndsWith(".json")))
                    {
                        var CarList = JsonConvert.DeserializeObject<List<ISeparate.ICar.IList>>(System.IO.File.ReadAllText(File));

                        if (CarList == null) continue;

                        X.Car = new(File, CarList);
                    }

                    foreach (string File in Files.Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".png") || x.EndsWith(".webp") || x.EndsWith(".gif")))
                    {
                        X.Image ??= new();
                        X.Image.Add(File);
                    }

                    SeparateList.Add(X);
                }
            }

            var FileSystemWatcher = new FileSystemWatcher(Directory)
            {
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            FileSystemWatcher.Changed += OnChanged;
            FileSystemWatcher.Created += OnCreated;
            FileSystemWatcher.Deleted += OnDeleted;
            FileSystemWatcher.Renamed += OnRenamed;

            Current = new ConcurrentDictionary<string, object>();

            FileSystemWatcher.EnableRaisingEvents = true;
            FileSystemWatcher.IncludeSubdirectories = true;

            if (SeparateList.Count == 0) return;

            Begin();

            foreach (string Value in SeparateList.Select(v => v.ID))
            {
                UserList.ForEach(x => x.Security.Add(Value, true));
            }

            new Thread(() => DoUser(UserList)).Start();
            new Thread(() => DoMessage(UserList)).Start();
            new Thread(() => DoSeparate(UserList)).Start();

            if (Unique)
            {
                NoticeList.Add(new INotice
                {
                    Name = "Почта",
                    Group = 2,
                    Time = TimeSpan.FromMinutes(10),
                    Beep = true,
                    Notification = false
                });

                for (int i = 1; i <= 3; i++)
                {
                    NoticeList.Add(new INotice
                    {
                        Name = "Аккаунт #" + i,
                        Group = 1,
                        List = new List<INotice>
                        {
                            new INotice { Name = "Задание", Time = TimeSpan.FromHours(2) },
                            new INotice { Name = "Событие", Time = TimeSpan.FromHours(3) },

                            new INotice
                            {
                                Name = "Неофициальная организация",
                                List = new List<INotice>
                                {
                                    new INotice { Name = "Большой улов", Time = TimeSpan.FromHours(24), Dump = false },

                                    new INotice { Name = "Грандиозная уборка", Time = TimeSpan.FromHours(26), Dump = false },
                                    new INotice { Name = "Мясной день", Time = TimeSpan.FromHours(26), Dump = false },

                                    new INotice { Name = "Долгожданная встреча", Time = TimeSpan.FromHours(20), Dump = false },
                                    new INotice { Name = "Обновляем гардероб", Time = TimeSpan.FromHours(20), Dump = false }
                                }}
                        }
                    });
                }

                NoticeList.Add(new INotice
                {
                    Name = "Преступный синдикат",
                    Group = 2,
                    List = new List<INotice>
                    {
                        new INotice { Name = "Угон авто", Time = TimeSpan.FromHours(1.5) },
                        new INotice { Name = "Работа сутенёром", Time = TimeSpan.FromHours(1.5) }
                    }
                });

                NoticeList.Add(new INotice
                {
                    Name = "Неофициальная организация",
                    Group = 2,
                    Time = TimeSpan.FromHours(2),
                    List = new List<INotice>
                    {

                        new INotice { Name = "Скользкая дорожка", Time = TimeSpan.FromHours(3) },
                        new INotice { Name = "Мотивированное волонтерство", Time = TimeSpan.FromHours(3) },

                        new INotice { Name = "Долгожданная встреча", Time = TimeSpan.FromHours(4) },
                        new INotice { Name = "Обновляем гардероб", Time = TimeSpan.FromHours(4) }
                    }
                });

                new Thread(() => DoNotice()).Start();

                IncentiveList.AddRange(new List<IIncentive>
                {
                    new IIncentive("Лотерейный билет", 1, 1500),
                    new IIncentive("Тренировка в тире", 1, 500),
                    new IIncentive("Уличная гонка", 1, 1000),
                    new IIncentive("Аренда Киностудии", 2, (X2 ? 2500 : 10000)),
                    new IIncentive("Кинотеатр", 1, 100, 50, 5),
                    new IIncentive("Арена Maze Bank", 1, Per: 100, Count: 3),
                    new IIncentive("Тренеровочный комплекс", 1, Per: 100, Count: 5),
                    new IIncentive("Картинг", 1, 500),
                    new IIncentive("Танец", 2, Count: 3),
                    new IIncentive("Ферма", 1, Count: 10),
                    new IIncentive("Качалка", 1, 750, Count: 20),
                    new IIncentive("Почта", 1, Count: 10),
                    new IIncentive("Пожарный", 1, Count: 25),
                    new IIncentive("Контрабанда", 2, Count: 5),
                    new IIncentive("Казино", 2)
                });
            }

            new Thread(() => DoStorage()).Start();

            int Index = 0;

        RETRY:

            BAR_INCENTIVE = false;

            Console.Clear();
            Console.WriteLine("\n\n");

            var Selection = new List<string>
            {
                "[~] Список",
                "[~] Пользователи",
                "",
                $"[{(Message == EMessage.ENABLED ? "√" : Message == EMessage.DISABLED ? "X" : "?")}] Сообщения",
                $"[{(Security.Header.HasValue ? "?" : Security.Active ? "√" : "X")}] Темы",
            };

            if (Unique)
            {
                Selection.AddRange(new string[]
                {
                    "",
                    "[~] Уведомления",
                    "[~] Выгода"
                });
            }

            var Case = Support.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Oem3);

            if (Case.Key == ConsoleKey.F5) goto RETRY;

            if (Selection[Case.Index].Contains("Список"))
            {
                Index = 0;

                if (Case.Key == ConsoleKey.Enter)
                {

                GOTO_SEPARATE:

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    Selection = SeparateList
                        .Select(x =>
                        {
                            string T = $"[{(x.Enabled.HasValue ? x.Enabled.Value ? "√" : "X" : "~")}] {x.ID}";

                            if (x.Date.HasValue)
                            {
                                var TS = DateTime.Now - x.Date.Value;

                                if (TS < TimeSpan.Zero)
                                {
                                    T += $" ~ {TS:hh\\:mm\\:ss}";
                                }
                            }
                            else if (x.Position > 0)
                            {
                                T += $" - {x.Position}";
                            }

                            return T;
                        })
                        .ToList();

                    Case = Support.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                    Index = Case.Index;

                    if (Case.Key == ConsoleKey.F5) goto GOTO_SEPARATE;

                    if (Case.Key == ConsoleKey.Escape) goto RETRY;

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        var Separate = SeparateList[Case.Index];

                        if (Separate.Car == null)
                        {
                            Separate.Enabled = !Separate.Enabled;
                        }
                        else
                        {

                            Index = 0;

                        LIST:

                            Console.Clear();
                            Console.WriteLine("\n\n");

                            Selection = Separate.Car.List
                                .Select(x =>
                                {
                                    if (x.Certificate.Count == 1)
                                    {
                                        return x.Certificate[0].Format(x.Name);
                                    }

                                    int Count = x.Certificate.Count(x => x.Active);

                                    return $"[{(Count == 0 ? "X" : Count == x.Certificate.Count ? "√" : "?")}] {x.Name}";
                                })
                                .ToList();

                            Case = Support.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                            Index = Case.Index;

                            if (Case.Key == ConsoleKey.F5) goto LIST;

                            if (Case.Key == ConsoleKey.Escape) goto GOTO_SEPARATE;

                            var Car = Separate.Car.List[Case.Index];

                            if (Car.Certificate.Count == 1)
                            {
                                DO(Car.Certificate[0]);

                                goto LIST;
                            }
                            else
                            {
                                Index = 0;

                            CERTIFICATE:

                                Console.Clear();
                                Console.WriteLine("\n\n");

                                Selection = Car.Certificate
                                    .Select(x => x.Format())
                                    .ToList();

                                Case = Support.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                                Index = Case.Index;

                                if (Case.Key == ConsoleKey.F5) goto CERTIFICATE;

                                if (Case.Key == ConsoleKey.Escape) goto LIST;

                                DO(Car.Certificate[Case.Index]);

                                goto CERTIFICATE;
                            }

                            void DO(ISeparate.ICar.IList.ICertificate Certificate)
                            {
                                if (Case.Key == ConsoleKey.OemMinus || Case.Key == ConsoleKey.OemPlus)
                                {
                                    if (Certificate.Date.HasValue)
                                    {
                                        var Latency = DoLatency($"У{(Case.Key == ConsoleKey.OemMinus ? "меньш" : Case.Key == ConsoleKey.OemPlus ? "велич" : "")}ить на: ");

                                        if (Latency.HasValue)
                                        {
                                            if (Case.Key == ConsoleKey.OemMinus)
                                            {
                                                Certificate.Date = Certificate.Date.Value.Add(-Latency.Value);
                                            }
                                            else if (Case.Key == ConsoleKey.OemPlus)
                                            {
                                                Certificate.Date = Certificate.Date.Value.Add(Latency.Value);
                                            }

                                            if (Storage is not null)
                                            {
                                                if (Storage.Separate.ContainsKey(Certificate.Name))
                                                {
                                                    Storage.Separate[Certificate.Name] = Certificate.Date.Value;
                                                    Storage.Save(StorageFile);
                                                }
                                            }
                                        }
                                    }

                                    return;
                                }

                                if (Case.Key == ConsoleKey.Enter)
                                {
                                    if (Certificate.Active)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n\n");

                                        Selection = new List<string>
                                        {
                                            "Время",
                                            "Дата"
                                        };

                                        Case = Support.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.Escape);

                                        if (Case.Key == ConsoleKey.Escape) return;

                                        if (Case.Key == ConsoleKey.Enter)
                                        {
                                            switch (Case.Index)
                                            {
                                                case 0:
                                                    var Latency = DoLatency("Аренда: ");

                                                    if (Latency.HasValue)
                                                    {
                                                        Certificate.DoThread(Latency.Value);
                                                    }

                                                    break;

                                                case 1:
                                                    Console.Clear();
                                                    Console.Write("Дата: ");

                                                    if (Support.Read(out string D))
                                                    {
                                                        var FORMAT = new List<string>();

                                                        if (D.Contains(':')) FORMAT.Add("HH:mm");
                                                        if (D.Contains('.')) FORMAT.Add("dd.MM");

                                                        if (FORMAT.Count > 0 && DateTime.TryParseExact(D, string.Join(" ", FORMAT), CultureInfo.CurrentCulture, DateTimeStyles.None, out var Date))
                                                        {
                                                            if (Date < DateTime.Now)
                                                            {
                                                                Date = Date.AddDays(1);
                                                            }

                                                            Certificate.DoThread(Date - DateTime.Now);
                                                        }
                                                        else
                                                        {
                                                            goto case 1;
                                                        }
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Certificate.Thread is not null)
                                        {
                                            try
                                            {
                                                Certificate.Thread.Interrupt();
                                                Certificate.Thread = null;
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    goto GOTO_SEPARATE;
                }
            }
            else if (Selection[Case.Index].Contains("Пользователи"))
            {
                Index = 0;

                if (Case.Key == ConsoleKey.Enter)
                {

                GOTO_USER:

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    Selection = UserList
                        .Select(x => x.ToString())
                        .ToList();

                    Case = Support.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                    Index = Case.Index;

                    if (Case.Key == ConsoleKey.F5) goto GOTO_USER;

                    if (Case.Key == ConsoleKey.Escape) goto RETRY;

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        var User = UserList[Case.Index];

                        if (User.Background)
                        {
                            goto GOTO_USER;
                        }

                    USER:

                        Console.Clear();
                        Console.WriteLine("\n\n");

                        Selection = new List<string>
                        {
                            $"[{(User.Message ? "√" : "X")}] Сообщения", "[~] Темы"
                        };

                        Case = Support.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                        if (Case.Key == ConsoleKey.F5) goto USER;

                        if (Case.Key == ConsoleKey.Escape) goto GOTO_USER;

                        if (Case.Key == ConsoleKey.Enter)
                        {
                            switch (Case.Index)
                            {
                                case 0:
                                    User.Message = !User.Message;

                                    goto USER;

                                case 1:

                                    Console.Clear();
                                    Console.WriteLine("\n\n");

                                    Selection = User.Security
                                        .Select(x => $"[{(x.Value ? "√" : "X")}] {x.Key}")
                                        .ToList();

                                    Case = Support.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                                    if (Case.Key == ConsoleKey.F5) goto case 1;

                                    if (Case.Key == ConsoleKey.Escape) goto USER;

                                    if (Case.Key == ConsoleKey.Enter)
                                    {
                                        foreach (var Thread in User.Security)
                                        {
                                            if (Selection[Case.Index].Contains(Thread.Key))
                                            {
                                                User.Security[Thread.Key] = !User.Security[Thread.Key];

                                                break;
                                            }
                                        }

                                    }

                                    goto case 1;
                            }
                        }
                    }

                    goto GOTO_USER;
                }
            }
            else if (Selection[Case.Index].Contains("Сообщения"))
            {
                if (Case.Key == ConsoleKey.Enter)
                {
                    Index = Case.Index;

                    if (UserList.Any(x => x.Background)) goto RETRY;

                    var List = Enum
                        .GetValues(typeof(EMessage))
                        .Cast<EMessage>()
                        .ToList();

                    int Value = List.IndexOf(Message) + 1;

                    Message = List.Count == Value
                        ? List[0]
                        : List[Value];
                }
            }
            else if (Selection[Case.Index].Contains("Темы"))
            {
                Index = Case.Index;

                if (UserList.Any(x => x.Background)) goto RETRY;

                if (Case.Key == ConsoleKey.Enter)
                {
                    if (Security.Active)
                    {
                        if (Security.Thread is not null)
                        {
                            try
                            {
                                Security.Thread.Interrupt();
                                Security.Thread = null;
                            }
                            catch { }
                        }

                        Security.Active = false;
                    }
                    else
                    {
                        Security.Active = true;
                    }
                }
                else
                {
                    if (Security.Active) goto RETRY;

                    var Latency = DoLatency("Через: ");

                    if (Latency.HasValue)
                    {
                        Security.DoThread(Latency.Value);
                    }
                }
            }
            else if (Selection[Case.Index].Contains("Уведомления"))
            {
                if (Case.Key == ConsoleKey.Enter)
                {
                    List<INotice> List = NoticeList;

                    List<List<INotice>>? OLD_LIST = null;
                    INotice? OLD_VALUE = null;

                    IEnumerator? Enumerator = null;
                    int? Position = null;

                    List<string> Header = new();

                GOTO_NOTICE:

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    int Max = List.Max(x => x.Group);

                    if (Max == 0)
                    {
                        Enumerator = null;
                    }
                    else
                    {
                        if (Enumerator == null)
                        {
                            Enumerator = new IEnumerator(new int[Max]
                                .Select((x, i) => i + 1)
                                .ToArray());

                            if (Position.HasValue)
                            {
                                Enumerator.Position = Position.Value;
                            }
                        }
                    }

                    var Cluster = List
                        .Where(x => Enumerator == null || Enumerator.Current == x.Group)
                        .ToList();

                    Selection = Cluster
                        .Select(x => x.ToString())
                        .ToList();

                    Case = Support.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.Tab, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                    if (Case.Key == ConsoleKey.Tab)
                    {
                        if (Enumerator is not null)
                        {
                            if (Enumerator.MoveNext())
                            {
                                Enumerator.Reset();
                            }

                            Position = Enumerator.Position;
                        }

                        goto GOTO_NOTICE;
                    }

                    if (Case.Key == ConsoleKey.F5) goto GOTO_NOTICE;

                    if (Case.Key == ConsoleKey.Escape)
                    {
                        if (OLD_LIST == null || OLD_LIST.Count == 0) goto RETRY;

                        List = OLD_LIST.Last();
                        OLD_LIST.Remove(List);

                        Header.Remove(Header.Last());

                        goto GOTO_NOTICE;
                    }

                    var Notice = Cluster[Case.Index];

                    if (Case.Key == ConsoleKey.OemMinus || Case.Key == ConsoleKey.OemPlus)
                    {
                        if (Notice.Date.HasValue)
                        {
                            var Latency = DoLatency($"У{(Case.Key == ConsoleKey.OemMinus ? "меньш" : Case.Key == ConsoleKey.OemPlus ? "велич" : "")}ить на: ");

                            if (Latency.HasValue)
                            {
                                if (Case.Key == ConsoleKey.OemMinus)
                                {
                                    Notice.Date = Notice.Date.Value.Add(-Latency.Value);
                                }
                                else if (Case.Key == ConsoleKey.OemPlus)
                                {
                                    Notice.Date = Notice.Date.Value.Add(Latency.Value);
                                }

                                if (Storage is not null)
                                {
                                    string Name = string.Join(" - ", Header);

                                    if (Storage.Notice.ContainsKey(Name))
                                    {
                                        Storage.Notice[Name] = Notice.Date.Value;
                                        Storage.Save(StorageFile);
                                    }
                                }
                            }
                        }

                        goto GOTO_NOTICE;
                    }

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        if (Notice.List == null || Notice.List.Count == 0)
                        {
                            Notice.Modify();

                            if (OLD_VALUE == null || OLD_VALUE.List == null) goto GOTO_NOTICE;

                            if (OLD_VALUE.Time.HasValue)
                            {
                                if (OLD_VALUE.Active)
                                {
                                    int Count = OLD_VALUE.List.Count(x => x.Active);

                                    if (Count == 0)
                                    {
                                        OLD_VALUE.Modify();
                                    }
                                    else
                                    {
                                        if (Count > 1)
                                        {
                                            OLD_VALUE.Date = DateTime.Now.Add(OLD_VALUE.Time.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (Notice.Active)
                                    {
                                        OLD_VALUE.Modify();
                                    }
                                }
                            }
                        }
                        else
                        {
                            Header.Add(Notice.Name);

                            OLD_LIST ??= new();
                            OLD_LIST.Add(List);

                            OLD_VALUE = Notice;

                            List = Notice.List;
                        }
                    }

                    goto GOTO_NOTICE;
                }
            }
            else if (Selection[Case.Index].Contains("Выгода"))
            {
                Index = 0;

                if (Case.Key == ConsoleKey.Enter)
                {

                GOTO_INCENTIVE:

                    BAR_INCENTIVE = true;

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    Selection = IncentiveList
                        .Select(x => x.ToString())
                        .ToList();

                    Case = Support.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.Enter, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                    Index = Case.Index;

                    if (Case.Key == ConsoleKey.F5) goto GOTO_INCENTIVE;

                    if (Case.Key == ConsoleKey.Escape) goto RETRY;

                    var Incentive = IncentiveList[Case.Index];

                    if (Case.Key == ConsoleKey.Enter)
                    {

                    }
                    else if (Case.Key == ConsoleKey.OemMinus)
                    {
                        if (Incentive.Index > 0)
                        {
                            Incentive.Index -= 1;
                        }

                        if (Incentive.Index == 0)
                        {
                            if (Incentive.Thread is not null)
                            {
                                try
                                {
                                    Incentive.Thread.Interrupt();
                                    Incentive.Thread = null;
                                }
                                catch { }
                            }
                        }
                    }
                    else if (Case.Key == ConsoleKey.OemPlus)
                    {
                        if (Incentive.Index < Incentive.Count)
                        {
                            Incentive.Index += 1;
                        }

                        if (Incentive.Index == Incentive.Count)
                        {
                            Incentive.DoThread(DoDump());
                        }
                    }

                    goto GOTO_INCENTIVE;
                }
            }

            goto RETRY;
        }

        private static void Begin()
        {
            foreach (var Separate in SeparateList)
            {
                if (Separate.Car == null)
                {
                    Separate.Enabled = true;
                }
                else
                {
                    if (Separate.Image is not null && Separate.Image.Count > 0)
                    {
                        foreach (var Car in Separate.Car.List)
                        {
                            if (string.IsNullOrEmpty(Car.Image)) continue;

                            Separate.Image.RemoveAll(Image => Image == Car.Image);
                        }
                    }
                }

                Destruction(Debug(Separate.ID));
            }

            SeparateList = SeparateList
                .OrderBy(x => x.Car == null)
                .ToList();
        }

        private static async void DoUser(List<IUser> UserList)
        {
            foreach (IUser T in UserList)
            {
                try
                {
                    var Response = await Discord.User(T);

                    if (Response == null)
                    {

                    }
                    else
                    {
                        if (Response.Code.HasValue)
                        {

                        }
                        else
                        {
                            T.Login = string.IsNullOrEmpty(Response.Login)
                                ? "NULL"
                                : Response.Login;

                            T.Name = string.IsNullOrEmpty(Response.Name)
                                ? "NULL"
                                : Response.Name;
                        }
                    }
                }
                finally
                {
                    T.Background = false;
                }

                await Task.Delay(2500);
            }
        }

        private static async void DoMessage(List<IUser> UserList)
        {
            while (true)
            {
                if (UserList.Any(x => x.Message) && Message > EMessage.DISABLED)
                {
                    var List = UserList
                        .Where(x => x.Message)
                        .ToList();

                    foreach (var User in List)
                    {
                        var Response = await Discord.Message(User);

                        if (Response == null)
                        {
                            User.Message = false;
                        }
                        else
                        {
                            var History = Response
                                .Where(x => x.Type == 1)
                                .ToList();

                            if (History.Count > 0)
                            {
                                if (User.History == null)
                                {
                                    User.History = History
                                        .Select(x => new IUser.IHistory(x.ID, x.LastMessageID))
                                        .ToList();
                                }
                                else
                                {
                                    foreach (var X in History)
                                    {
                                        var T = User.History.FirstOrDefault(x => x.ID == X.ID);

                                        if (T == null)
                                        {
                                            User.History.Add(new IUser.IHistory(X.ID, X.LastMessageID));

                                            string TEXT = $"[{User.Name}] У Вас одно новое сообщение!";

                                            try
                                            {
                                                foreach (var Recipient in X.Recipient)
                                                {
                                                    var Seek = await Discord.Seek(Recipient.ID, User);

                                                    if (Seek == null) continue;

                                                    var Message = Seek.Message
                                                        .SelectMany(x => x)
                                                        .ToList();

                                                    if (Message.Count == 0) continue;

                                                    var Group = Message
                                                        .GroupBy(x => x.ID)
                                                        .Select(x => (x.Key, Date: x.Max(x => x.Date), Count: x.Count()))
                                                        .ToList();

                                                    if (Group.Count == 0) continue;

                                                    var Dictionary = new Dictionary<string, List<Discord.ISeekResponse.IMessage>>();

                                                    foreach (var Pair in Discord.GTA_5_RP)
                                                    {
                                                        foreach ((string Key, DateTime Date, int Count) in Group)
                                                        {
                                                            if (Pair.Value.Contains(Key))
                                                            {
                                                                if (Dictionary.ContainsKey(Pair.Key))
                                                                {
                                                                    Dictionary[Pair.Key].Add(new(Key, Date, Count));
                                                                }
                                                                else
                                                                {
                                                                    Dictionary.Add(Pair.Key, new List<Discord.ISeekResponse.IMessage> { new(Key, Date, Count) });
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (Dictionary.Count > 0)
                                                    {
                                                        TEXT += Environment.NewLine + string.Join("",
                                                            Dictionary
                                                                .Select(x => (
                                                                    x.Key,
                                                                    Count: x.Value.Sum(v => v.Count),
                                                                    Date: x.Value.Max(v => v.Date)
                                                                ))
                                                                .OrderBy(x => x.Date)
                                                                .Reverse()
                                                                .Select(x => $"{Environment.NewLine}{x.Key} ~ {x.Count} ({x.Date:MM.dd hh:mm})")
                                                                .ToList()
                                                            );
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                if (Telegram is not null)
                                                {
                                                    await Telegram.SendMessage(TEXT, Message == EMessage.MUTE);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (T.LastMessageID == X.LastMessageID) continue;

                                            var DateTime = ToDateTime(X.LastMessageID).Subtract(T.DateTime);

                                            if (DateTime.TotalSeconds >= 2.5 * 60)
                                            {
                                                T.Update(X.LastMessageID);

                                                if (Telegram is not null)
                                                {
                                                    await Telegram.SendMessage($"[{User.Name}] Вы получили сообщение!", Message == EMessage.MUTE);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            await Task.Delay(5000);
                        }
                    }

                    await Task.Delay(60 * 1000);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async void DoSeparate(List<IUser> UserList)
        {
            var _ = SeparateList.Select(X =>
            {
                return Task.Run(async () =>
                {
                    while (true)
                    {
                        if (UserList.Any(x => x.Security[X.ID]) && Security.Active)
                        {
                            if (X.Can)
                            {
                                var List = UserList
                                    .Where(x => x.Security[X.ID])
                                    .OrderBy(x => x.Time)
                                    .ToList();

                                for (int i = 0; i < List.Count && Security.Active && X.Can; i++)
                                {
                                    double Average = ToAverage(List);

                                    var Read = await Discord.ThreadRead(X, List[i]);

                                    if (Read is not null)
                                    {
                                        var T = Read
                                            .Select((x, i) => (Value: x, Index: i))
                                            .Where(x => List[i].Login == x.Value.Author.Login)
                                            .ToList();

                                        if (T.Any())
                                        {
                                            if (T.Min(x => x.Index) <= 10)
                                            {
                                                List[i].Time = null;

                                                await Task.Delay(ToTimeSpan(
                                                    X,
                                                    Average / 2
                                                ));

                                                continue;
                                            }
                                        }
                                    }

                                    await Task.Delay(2500);

                                    var Write = await Discord.ThreadWrite(X, List[i]);

                                    if (Write == null)
                                    {
                                        List[i].Security[X.ID] = false;
                                        List[i].Error = 0;
                                    }
                                    else
                                    {
                                        if (Write.Code.HasValue)
                                        {
                                            if (Write.Code == 20016)
                                            {
                                                List[i].Time = Math.Ceiling(Write.Retry / 60f);

                                                if (List[i].Time < Average && List.Count - 1 > i)
                                                {
                                                    X.Date = null;
                                                    X.Position = ISeparate.EPosition.NEXT;

                                                    await Task.Delay(30 * 1000);
                                                }
                                                else
                                                {
                                                    double N = List.Where(x => x.Time.HasValue).Min(x => x.Time!.Value);

                                                    await Task.Delay(ToTimeSpan(
                                                        X,
                                                        N > Average
                                                            ? Average
                                                            : N
                                                    ));
                                                }
                                            }
                                            else
                                            {
                                                List[i].Security[X.ID] = false;
                                                List[i].Error = Write.Code;
                                            }
                                        }
                                        else
                                        {
                                            List[i].Error = null;
                                            List[i].Time = null;

                                            await Task.Delay(ToTimeSpan(
                                                X,
                                                Average
                                            ));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                X.Date = null;
                                X.Position = ISeparate.EPosition.UNABLE;

                                await Task.Delay(15 * 1000);
                            }
                        }
                        else
                        {
                            X.Date = null;
                            X.Position = ISeparate.EPosition.INACTIVE;

                            await Task.Delay(1000);
                        }
                    }
                });
            });

            await Task.WhenAll(_);
        }

        private static void DoStorage(bool X = true)
        {
            if (Storage == null) return;

            try
            {
                foreach (var Pair in Storage.Separate.ToList())
                {
                    if (Pair.Value > DateTime.Now)
                    {
                        foreach (var Separate in SeparateList)
                        {
                            if (Separate.Car == null) continue;

                            foreach (var Certificate in Separate.Car.List.SelectMany(x => x.Certificate))
                            {
                                if (Certificate.Name == Pair.Key)
                                {
                                    Certificate.DoThread(Pair.Value - DateTime.Now);
                                }
                            }
                        }
                    }
                    else
                    {
                        Storage.Separate.Remove(Pair.Key);
                    }
                }

                if (X)
                {
                    foreach (var Pair in Storage.Notice.ToList())
                    {
                        if (Pair.Value > DateTime.Now)
                        {
                            foreach (var Notice in NoticeDictionary)
                            {
                                if (Notice.Key == Pair.Key)
                                {
                                    Notice.Value.Modify();

                                    while (Notice.Value.Date == null) { }

                                    Notice.Value.Date = DateTime.Now + (Pair.Value - DateTime.Now);
                                }
                            }
                        }
                        else
                        {
                            Storage.Notice.Remove(Pair.Key);
                        }
                    }

                    foreach (var Pair in Storage.Incentive.ToList())
                    {
                        if (DateTime.Now < Pair.Value)
                        {
                            foreach (var Incentive in IncentiveList)
                            {
                                if (Incentive.Name == Pair.Key)
                                {
                                    Incentive.Index = Incentive.Count;

                                    Incentive.DoThread(Pair.Value);
                                }
                            }
                        }
                        else
                        {
                            Storage.Incentive.Remove(Pair.Key);
                        }
                    }
                }
            }
            finally
            {
                Storage.Save(StorageFile);
            }
        }

        private static async void DoNotice()
        {
            var _ = NoticeDictionary.Select(X =>
            {
                return Task.Run(async () =>
                {
                    while (true)
                    {
                        if (X.Value.Active)
                        {
                            if (X.Value.Time.HasValue)
                            {
                                X.Value.Date = DateTime.Now.Add(X.Value.Time.Value);

                                X.Value.Source = new();

                                if (Storage is not null)
                                {
                                    if (Storage.Notice.TryAdd(X.Key, X.Value.Date.Value))
                                    {
                                        Storage.Save(StorageFile);
                                    }
                                }

                                var Date = DoDump();

                                if (X.Value.Dump && X.Value.Date > Date)
                                {
                                    X.Value.Date = Date;
                                }

                                try
                                {
                                    for (var N = DateTime.Now; N <= X.Value.Date; N = N.AddSeconds(1))
                                    {
                                        if (X.Value.Source.IsCancellationRequested) break;

                                        X.Value.Header = X.Value.Date - N;

                                        await Task.Delay(1000);
                                    }

                                    X.Value.Active = false;

                                    if (X.Value.Beep)
                                    {
                                        await Task.Delay(2500, X.Value.Source.Token);

                                        Console.Beep();
                                    }

                                    X.Value.Source.Token.ThrowIfCancellationRequested();

                                    if (X.Value.Notification && Message > EMessage.DISABLED)
                                    {
                                        string[] Array = X.Key.Split(" - ");

                                        if (Telegram is not null)
                                        {
                                            await Telegram.SendMessage($"\"{(Regex.IsMatch(Array[0], "Аккаунт #[0-9]+") ? Array.First() + " - " : "")}{Array.Last()}\" доступно!", Message == EMessage.MUTE);
                                        }
                                    }
                                }
                                catch (OperationCanceledException) { }
                                catch (ObjectDisposedException) { }
                                finally
                                {
                                    if (Storage is not null)
                                    {
                                        if (Storage.Notice.ContainsKey(X.Key))
                                        {
                                            Storage.Notice.Remove(X.Key);
                                            Storage.Save(StorageFile);
                                        }
                                    }

                                    X.Value.Source.Dispose();
                                    X.Value.Source = null;
                                }
                            }
                        }
                        else
                        {
                            await Task.Delay(1000);
                        }
                    }
                });
            });

            await Task.WhenAll(_);
        }

        #region Can

        private static ConcurrentDictionary<string, object>? Current;

        private static async Task<bool> Write(string File)
        {
            if (Current == null) return false;

            object T = new();

            Current[File] = T;

            await Task.Delay(1000).ConfigureAwait(false);

            return Current.TryGetValue(File, out var X) && (T == X) && Current.TryRemove(File, out _);
        }

        private async static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FullPath) || string.IsNullOrEmpty(e.Name)) return;

            if (await Write(e.Name))
            {
                string? DirectoryName = Path.GetDirectoryName(e.FullPath);

                if (string.IsNullOrEmpty(DirectoryName)) return;

                string? FileName = Path.GetFileName(DirectoryName);

                if (string.IsNullOrEmpty(FileName)) return;

                var Separate = SeparateList
                    .Where(x => x.ID == FileName)
                    .FirstOrDefault(x =>
                    {
                        return x.Car switch
                        {
                            null => false,

                            _ => x.Car.Key == e.FullPath
                        };
                    });

                if (Separate == null) return;

                var CarList = JsonConvert.DeserializeObject<List<ISeparate.ICar.IList>>(File.ReadAllText(e.FullPath));

                if (CarList == null) return;

                Separate.Car = new(e.FullPath, CarList);

                new Thread(() => DoStorage(false)).Start();
            }
        }


        private async static Task OnCreatedFile(string Name, string FullPath)
        {
            var X = await Write(FullPath);

            if (X)
            {
                if (Path.HasExtension(FullPath))
                {
                    string? DirectoryName = Path.GetDirectoryName(FullPath);

                    if (string.IsNullOrEmpty(DirectoryName)) return;

                    string? FileName = Path.GetFileName(DirectoryName);

                    if (string.IsNullOrEmpty(FileName)) return;

                    var Separate = SeparateList.FirstOrDefault(x => x.ID == FileName);

                    if (Separate == null) return;

                    switch (Path.GetExtension(FullPath).ToUpper())
                    {
                        case ".TXT":

                            Separate.Content = FullPath;

                            break;

                        case ".JSON":

                            var CarList = JsonConvert.DeserializeObject<List<ISeparate.ICar.IList>>(File.ReadAllText(FullPath));

                            if (CarList == null) return;

                            Separate.Car = new(FullPath, CarList);

                            break;

                        default:

                            Separate.Image ??= new();
                            Separate.Image.Add(FullPath);

                            break;
                    }

                    Begin();
                }
                else
                {
                    if (Regex.IsMatch(Name, @"^\d+$"))
                    {
                        var Separate = new ISeparate(Name);

                        SeparateList.Add(Separate);

                        UserList.ForEach(x => x.Security.Add(Separate.ID, true));
                    }
                }
            }
        }

        private async static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FullPath) || string.IsNullOrEmpty(e.Name)) return;

            await OnCreatedFile(e.Name, e.FullPath);
        }

        private static async Task OnDeletedFile(string Name, string FullPath)
        {
            if (await Write(FullPath))
            {
                if (Path.HasExtension(FullPath))
                {
                    string? DirectoryName = Path.GetDirectoryName(Name);

                    if (string.IsNullOrEmpty(DirectoryName)) return;

                    var Separate = SeparateList.FirstOrDefault(x => x.ID == DirectoryName);

                    if (Separate == null) return;

                    switch (Path.GetExtension(FullPath).ToUpper())
                    {
                        case ".TXT":

                            if (string.IsNullOrEmpty(Separate.Content)) return;

                            if (Separate.Content == FullPath)
                            {
                                Separate.Content = null;
                            }

                            break;

                        case ".JSON":

                            if (Separate.Car == null) return;

                            if (Separate.Car.Key == FullPath)
                            {
                                Separate.Car = null;
                            }

                            break;

                        default:

                            if (Separate.Image == null || Separate.Image.Count == 0) return;

                            Separate.Image.RemoveAll(Image => Image == FullPath);

                            break;
                    }
                }
                else
                {
                    if (SeparateList.RemoveAll(x => x.ID == Name) > 0)
                    {
                        UserList.ForEach(x => x.Security.Remove(Name));
                    }
                }
            }
        }

        private async static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FullPath) || string.IsNullOrEmpty(e.Name)) return;

            await OnDeletedFile(e.Name, e.FullPath);
        }

        private static async void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.OldName) && !string.IsNullOrEmpty(e.OldFullPath))
            {
                await OnDeletedFile(e.OldName, e.OldFullPath);
            }

            if (!string.IsNullOrEmpty(e.Name) && !string.IsNullOrEmpty(e.FullPath))
            {
                await OnCreatedFile(e.Name, e.FullPath);
            }
        }

        #endregion

        private static bool BAR_INCENTIVE = false;

        private static void DoBar()
        {
            var Thread = new Thread(() =>
            {
                while (true)
                {
                    if (BAR_INCENTIVE)
                    {
                        var N = IncentiveList
                            .Where(x => x.Index == x.Count)
                            .ToList();

                        if (N.Count > 0)
                        {
                            Console.Title = $"$ {N.Sum(x => x.Value)} BP - {N.Sum(x => x.Start + (x.Per * x.Count))}$";
                        }
                    }
                    else
                    {
                        Console.Title = "$ ";
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            });

            Thread.Start();
        }

        private static string DoTime(TimeSpan TS)
        {
            string F = $"hh\\:mm\\:ss";

            if (TS.Days > 0) F = F.Insert(0, "dd\\.");

            return TS.ToString(F);
        }

        private static DateTime DoDump(DateTime? Date = null)
        {
            var X = (Date ?? DateTime.Now).Date.AddHours(7);

            if (DateTime.Now > X) X = X.AddDays(1);

            return X;
        }

        private static TimeSpan? DoLatency(string Value)
        {

        LATENCY:

            Console.Clear();
            Console.Write(Value);

            if (Support.Read(out string N))
            {
                if (double.TryParse(N, out double X) && X > 0)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n");

                    var Selection = new List<string>
                    {
                        $"Час{(X == 1 ? "" : X < 5 ? "а" : "ов")}",
                        $"Минут{(X == 1 ? "у" : X < 5 ? "ы" : "")}",
                        $"Секунд{(X == 1 ? "у" : X < 5 ? "ы" : "")}"
                    };

                    var Case = Support.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.Escape);

                    if (Case.Key == ConsoleKey.Escape) return null;

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        TimeSpan? Latency = null;

                        switch (Case.Index)
                        {
                            case 0:
                                Latency = TimeSpan.FromHours(X);

                                break;

                            case 1:
                                Latency = TimeSpan.FromMinutes(X);

                                break;
                            case 2:
                                Latency = TimeSpan.FromSeconds(X);

                                break;
                        }

                        return Latency;
                    }
                }
                else
                {
                    goto LATENCY;
                }
            }

            return null;
        }

        private static void Destruction(string _)
        {
            string[] Files = System.IO.Directory.GetFiles(_);

            if (Files.Length == 0) return;

            foreach (string File in Files)
            {
                if (System.IO.File.Exists(File))
                {
                    var CreationTime = System.IO.File.GetCreationTime(File);

                    var N = (DateTime.Now - CreationTime).TotalDays;

                    if (N > 7)
                    {
                        System.IO.File.Delete(File);
                    }
                }
            }

        }

        public static string Debug(string? ID = null)
        {
            string X = Path.Combine(Directory, string.IsNullOrEmpty(ID) ? "" : ID, "debug");

            if (!System.IO.Directory.Exists(X)) System.IO.Directory.CreateDirectory(X);

            return X;
        }

        public static string Now(string Debug) => Path.Combine(Debug, $"~ {DateTime.Now:dd MMM}.txt");

        private static readonly Random Random = new();

        private static double ToAverage(List<IUser> List)
        {
            return Math.Ceiling(60d / List.Count)
                + 1d
                + Random.NextDouble();
        }

        private static TimeSpan ToTimeSpan(ISeparate X, double N)
        {
            var TS = TimeSpan.FromMinutes(N);

            X.Date = DateTime.Now + TS;
            X.Position = ISeparate.EPosition.ACTIVE;

            return TS;
        }

        private static DateTime ToDateTime(long Snowflake)
        {
            var MS = new DateTimeOffset(2015, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
            var TZ = Snowflake >> 22;

            return DateTimeOffset
                .FromUnixTimeMilliseconds(MS + TZ)
                .ToLocalTime()
                .DateTime;
        }
    }
}