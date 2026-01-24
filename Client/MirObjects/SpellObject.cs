using Client.MirGraphics;
using Client.MirScenes;
using Client.MirSounds;
using S = ServerPackets;

namespace Client.MirObjects
{
    class SpellObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Spell; }
        }

        public override bool Blocking
        {
            get { return false; }
        }

        public Spell Spell;
        public Point AnimationOffset = new(0, 0);
        public int FrameCount, FrameInterval, FrameIndex;
        public bool Repeat, Ended;
        

        public SpellObject(uint objectID) : base(objectID)
        {
        }

        public void Load(S.ObjectSpell info)
        {
            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);
            Spell = info.Spell;
            Direction = info.Direction;
            Repeat = true;
            Ended = false;

            switch (Spell)
            {
                case Spell.TrapHexagon:
                    BodyLibrary = Libraries.Magic;
                    DrawFrame = 1390;
                    FrameInterval = 100;
                    FrameCount = 10;
                    Blend = true;
                    break;
                case Spell.FireWall:
                    BodyLibrary = Libraries.Magic;
                    DrawFrame = 438;
                    FrameInterval = 120;
                    FrameCount = 10;
                    Light = 3;
                    break;
            }

            NextMotion = CMain.Time + FrameInterval;
            NextMotion -= NextMotion % 100;
        }

        public override void Process()
        {
            if (CMain.Time >= NextMotion)
            {
                if (++FrameIndex >= FrameCount && Repeat)
                {
                    FrameIndex = 0;
                    Ended = true;
                }

                NextMotion = CMain.Time + FrameInterval;
            }

            DrawLocation = new Point((CurrentLocation.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth, (CurrentLocation.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);
            DrawLocation.Offset(GlobalDisplayLocationOffset);
            DrawLocation.Offset(User.OffSetMove);
        }

        public override void Draw()
        {
            if (FrameIndex >= FrameCount && !Repeat) return;
            if (BodyLibrary == null) return;

            if (Blend)
            {
                BodyLibrary.DrawBlend(
                    DrawFrame + FrameIndex,
                    AnimationOffset == default ? DrawLocation : GetDrawWithOffset(),
                    DrawColour, true,
                    0.8F);
            }
            else
            {
                BodyLibrary.Draw(DrawFrame + FrameIndex,
                    AnimationOffset == default ? DrawLocation : GetDrawWithOffset(),
                    DrawColour,
                    true);
            }
        }

        public override bool MouseOver(Point p)
        {
            return false;
        }

        public override void DrawBehindEffects(bool effectsEnabled)
        {
        }

        public override void DrawEffects(bool effectsEnabled)
        { 
        }

        private Point GetDrawWithOffset()
        {
            Point newDrawLocation = new (
                (CurrentLocation.X + AnimationOffset.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth,
                (CurrentLocation.Y + AnimationOffset.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);

            newDrawLocation.Offset(GlobalDisplayLocationOffset);
            newDrawLocation.Offset(User.OffSetMove);

            return newDrawLocation;
        }
    }
}
