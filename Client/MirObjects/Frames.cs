namespace Client.MirObjects
{
    public class FrameSet : Dictionary<MirAction, Frame>
    {
        public static FrameSet Player;
        public static FrameSet DefaultNPC, DefaultMonster, DungeonMonster1, DungeonMonster2, DungeonMonster3, DungeonMonster4, DungeonMonster5, ItemMonTree, ItemMonChest, ItemMonCupboard;

        static FrameSet()
        {
            FrameSet frame;

            Player = new FrameSet();

            DefaultNPC = new FrameSet
            {
                { MirAction.Standing, new Frame(56, 1, 3, 450) }
            };

            DefaultMonster = new FrameSet
            {
                { MirAction.Standing, new Frame(16, 1, 3, 500) },
                { MirAction.Walking, new Frame(16, 4, 0, 100) },
                { MirAction.Attack1, new Frame(0, 2, 0, 100) }
            };            

            DungeonMonster1 = new FrameSet
            {
                { MirAction.Standing, new Frame(0, 1, 0, 500) },
                { MirAction.Walking, new Frame(10, 6, 4, 100) },
                { MirAction.Attack1, new Frame(90, 6, 4, 100) }
            };

            DungeonMonster2 = new FrameSet
            {
                { MirAction.Standing, new Frame(48, 1, 5, 500) },
                { MirAction.Walking, new Frame(48, 6, 0, 100) },
                { MirAction.Attack1, new Frame(0, 6, 0, 100) }
            };

            DungeonMonster3 = new FrameSet
            {
                { MirAction.Standing, new Frame(0, 1, 0, 500) },
                { MirAction.Walking, new Frame(10, 6, 4, 100) },
                { MirAction.Attack1, new Frame(90, 4, 6, 100) }
            };

            DungeonMonster4 = new FrameSet
            {
                { MirAction.Standing, new Frame(10, 8, 2, 500) },
                { MirAction.Walking, new Frame(10, 8, 2, 100) },
                { MirAction.Attack1, new Frame(90, 8, 2, 100) }
            };

            DungeonMonster5 = new FrameSet
            {
                { MirAction.Standing, new Frame(10, 8, 2, 500) },
                { MirAction.Walking, new Frame(10, 8, 2, 100) },
                { MirAction.Attack1, new Frame(90, 6, 4, 100) }
            };

            ItemMonTree = new FrameSet
            {
                { MirAction.Standing, new Frame(0, 6, -6, 500) },
                { MirAction.Die, new Frame(0, 1, -1, 1) },
                { MirAction.Dead, new Frame(7, 1, -1, 1000) },
            };

            ItemMonChest = new FrameSet
            {
                { MirAction.Standing, new Frame(0, 6, 0, 500) },
                { MirAction.Die, new Frame(0, 1, 6, 1) },
                { MirAction.Dead, new Frame(12, 6, -6, 1000) },
            };

            ItemMonCupboard = new FrameSet
            {
                { MirAction.Standing, new Frame(0, 6, -6, 500) },
                { MirAction.Die, new Frame(0, 1, -1, 1) },
                { MirAction.Dead, new Frame(6, 6, -6, 1000) },
            };


            #region Player
            Player.Add(MirAction.Standing, new Frame(56, 1, 3, 500, 0, 8, 0, 250));
            Player.Add(MirAction.Walking, new Frame(56, 4, 0, 100, 64, 6, 0, 100));
            Player.Add(MirAction.Attack1, new Frame(8, 3, 0, 100, 168, 6, 0, 100));
            Player.Add(MirAction.Spell, new Frame(56, 1, 3, 500, 328, 6, 0, 100));
            Player.Add(MirAction.Harvest, new Frame(0, 1, 0, 300, 376, 2, 0, 300));
            Player.Add(MirAction.Die, new Frame(88, 1, -1, 100, 416, 4, 0, 100));
            Player.Add(MirAction.Dead, new Frame(88, 1, -1, 1000, 419, 1, 3, 1000));
            Player.Add(MirAction.Revive, new Frame(56, 1, 3, 100, 416, 4, 0, 100) { Reverse = true });

            #endregion
        }
    }

    public class Frame
    {
        public int Start, Count, Skip, EffectStart, EffectCount, EffectSkip;
        public int Interval, EffectInterval;
        public bool Reverse, Blend;

        public int OffSet
        {
            get { return Count + Skip; }
        }

        public int EffectOffSet
        {
            get { return EffectCount + EffectSkip; }
        }

        public Frame(int start, int count, int skip, int interval, int effectstart = 0, int effectcount = 0, int effectskip = 0, int effectinterval = 0)
        {
            Start = start;
            Count = count;
            Skip = skip;
            Interval = interval;
            EffectStart = effectstart;
            EffectCount = effectcount;
            EffectSkip = effectskip;
            EffectInterval = effectinterval;
        }
    }

}
