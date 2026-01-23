using Client.MirSounds.Libraries;
using SlimDX.Direct3D9;
using SlimDX.DirectSound;
using System;

namespace Client.MirSounds
{
    static class SoundManager
    {
        private static readonly List<ISoundLibrary> Sounds = new List<ISoundLibrary>();
        private static readonly Dictionary<int, string> IndexList = new Dictionary<int, string>();

        private static readonly List<KeyValuePair<long, int>> DelayList = new List<KeyValuePair<long, int>>();

        public static readonly List<string> SupportedFileTypes;

        public static ISoundLibrary Music;
        private static long _checkSoundTime;

        private static int _vol;
        public static int Vol
        {
            get { return _vol; }
            set
            {
                if (_vol == value) return;
                _vol = value;
                AdjustAllVolumes();
            }
        }

        private static int _musicVol;
        public static int MusicVol
        {
            get { return _musicVol; }
            set
            {
                if (_musicVol == value) return;
                _musicVol = value;
                AdjustMusicVolume();
            }
        }

        static SoundManager()
        {
            SupportedFileTypes = new List<string>
            {
                ".wav",
                ".mp3"
            };
        }

        public static void ProcessDelayedSounds()
        {
            if (DelayList.Count == 0) return;

            var sounds = DelayList.Where(x => x.Key <= CMain.Time).ToList();

            foreach (var sound in sounds)
            {
                DelayList.Remove(sound);

                PlaySound(sound.Value);
            }
        }

        public static void Create()
        {
            if (Program.Form == null || Program.Form.IsDisposed) return;

            LoadSoundList();
        }
        public static void LoadSoundList()
        {
            string fileName = Path.Combine(Settings.SoundPath, "SoundList.lst");

            if (!File.Exists(fileName)) return;

            string[] lines = File.ReadAllLines(fileName);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] split = lines[i].Replace(" ", "").Split(':', '\t');

                int index;
                if (split.Length <= 1 || !int.TryParse(split[0], out index)) continue;

                if (!IndexList.ContainsKey(index))
                    IndexList.Add(index, split[split.Length - 1]);
            }
        }

        public static void StopSound(int index)
        {
            for (int i = 0; i < Sounds.Count; i++)
            {
                if (Sounds[i].Index != index) continue;

                Sounds[i].Stop();
                return;
            }
        }

        public static void PlaySound(int index, bool loop = false, int delay = 0)
        {
            if (delay > 0)
            {
                DelayList.Add(new KeyValuePair<long, int>(CMain.Time + delay, index));
                return;
            }

            CheckSoundTimeOut();

            for (int i = 0; i < Sounds.Count; i++)
            {
                if (Sounds[i].Index != index) continue;
                Sounds[i].Play(Vol);
                return;
            }

            string filename;
            filename = string.Format(index.ToString());
            Sounds.Add(GetSound(index, filename, Vol, loop));
        }

        public static void PlayMusic(int index, bool loop = false)
        {
            if (IndexList.TryGetValue(index, out string value))
            {
                Music = GetSound(index, value, MusicVol, loop);
            }
        }

        public static void StopMusic()
        {
            Music?.Stop();
            Music?.Dispose();
        }

        static void AdjustMusicVolume()
        {
            Music?.SetVolume(MusicVol);
        }

        static void AdjustAllVolumes()
        {
            for (int i = 0; i < Sounds.Count; i++)
            {
                Sounds[i].SetVolume(Vol);
            }
        }

        static void CheckSoundTimeOut()
        {
            if (CMain.Time >= _checkSoundTime)
            {
                _checkSoundTime = CMain.Time + 30 * 1000;

                for (int i = Sounds.Count - 1; i >= 0; i--)
                {
                    var sound = Sounds[i];

                    if (!sound.IsPlaying())
                    {
                        if (CMain.Time >= sound.ExpireTime)
                        {
                            sound.Dispose();
                            Sounds.RemoveAt(i);
                            continue;
                        }
                    }
                }
            }
        }

        static ISoundLibrary GetSound(int index, string fileName, int volume, bool loop)
        {
            var sound = NAudioLibrary.TryCreate(index, fileName, volume, loop);

            return sound == null ? new NullLibrary(index, fileName, loop) : sound;
        }

        public static void Dispose()
        {
            DelayList.Clear();

            for (int i = Sounds.Count - 1; i >= 0; i--)
            {
                Sounds[i]?.Dispose();
            }

            Music?.Dispose();
        }
    }

    public static class SoundList
    {
        public static int
            None = 0,

            IntroMusic = 10146,
            SelectMusic = 10147,
            LevelUp = 10156,

            ButtonA = 10103,
            ButtonB = 10104,
            ButtonC = 10105,
            Gold = 10106,
            EatDrug = 10107,
            ClickDrug = 10108,
            ClickWeapon = 10111,
            ClickArmour = 10112,
            ClickRing = 10113,
            ClickBracelet = 10114,
            ClickNecklace = 10115,
            ClickHelmet = 10116,
            ClickItem = 10118,

            SwingWood = 20,
            SwingSword = 21,
            SwingFist = 30,

            StruckBodySword = 15,
            StruckBodyFist = 10,

            Wolf = 2,
            Deer = 3,
            Bull = 4,
            Doe = 5,
            Hen = 6,
            Tiger = 7,



            HealingEnd = 200,

            SlayingEnd = 201,

            IceBallExplode = 204,

            ThunderboltEnd = 205,

            Thunderbolt2End = 206,

            ThunderBallExplode = 207,

            FireballFly = 208,
            FireballExplode = 209,

            IceRockEnd = 220;

            
    }
}
