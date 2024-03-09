using Client.MirControls;
using Client.MirGraphics;
using Client.MirScenes;
using Client.MirSounds;
using Client.MirScenes.Dialogs;
using SlimDX;

namespace Client.MirObjects
{
    public abstract class MapObject
    {
        public static Font ChatFont = new Font(Settings.FontName, 10F);
        public static List<MirLabel> LabelList = new List<MirLabel>();

        public static UserObject User;
        public static MapObject MouseObject, TargetObject, MagicObject;

        private static uint mouseObjectID;
        public static uint MouseObjectID
        {
            get { return mouseObjectID; }
            set
            {
                if (mouseObjectID == value) return;
                mouseObjectID = value;
                MouseObject = MapControl.Objects.Find(x => x.ObjectID == value);
            }
        }

        private static uint targetObjectID;
        public static uint TargetObjectID
        {
            get { return targetObjectID; }
            set
            {
                if (targetObjectID == value) return;
                targetObjectID = value;
                TargetObject = MapControl.Objects.Find(x => x.ObjectID == value);
            }
        }

        private static uint magicObjectID;
        public static uint MagicObjectID
        {
            get { return magicObjectID; }
            set
            {
                if (magicObjectID == value) return;
                magicObjectID = value;
                MagicObject = MapControl.Objects.Find(x => x.ObjectID == value);
            }
        }

        public abstract ObjectType Race { get; }
        public abstract bool Blocking { get; }

        public uint ObjectID;
        public string Name = string.Empty;
        public Point CurrentLocation, MapLocation;
        public MirDirection Direction;
        public bool Dead, Hidden;
        public PoisonType Poison;
        public long DeadTime;
        public byte AI;

        public bool Blend = true;

        public long BlindTime;
        public byte BlindCount;

        private byte percentHealth;
        public virtual byte PercentHealth
        {
            get { return percentHealth; }
            set
            {
                if (percentHealth == value) return;

                percentHealth = value;
            }
        }
        public long HealthTime;

        private byte percentMana;
        public virtual byte PercentMana
        {
            get { return percentMana; }
            set
            {
                if (percentMana == value) return;

                percentMana = value;
            }
        }

        public List<QueuedAction> ActionFeed = new List<QueuedAction>();
        public QueuedAction NextAction
        {
            get { return ActionFeed.Count > 0 ? ActionFeed[0] : null; }
        }

        public List<Effect> Effects = new List<Effect>();
        public List<BuffType> Buffs = new List<BuffType>();

        public MLibrary BodyLibrary;
        public Color DrawColour = Color.White, NameColour = Color.White, LightColour = Color.White;
        public MirLabel NameLabel, ChatLabel, GuildLabel;
        public long ChatTime;
        public int DrawFrame, DrawWingFrame;
        public Point DrawLocation, Movement, FinalDrawLocation, OffSetMove;
        public Rectangle DisplayRectangle;
        public int Light, DrawY;
        public long NextMotion, NextMotion2;
        public MirAction CurrentAction;
        public byte CurrentActionLevel;
        public bool SkipFrames;
        public FrameLoop FrameLoop = null;

        //Sound
        public int StruckWeapon;

        public MirLabel TempLabel;

        public static List<MirLabel> DamageLabelList = new List<MirLabel>();
        public List<Damage> Damages = new List<Damage>();

        protected Point GlobalDisplayLocationOffset
        {
            get { return new Point(0, 0); }
        }

        protected MapObject() { }

        protected MapObject(uint objectID)
        {
            ObjectID = objectID;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != ObjectID) continue;
                ob.Remove();
            }

            MapControl.Objects.Add(this);
        }
        public void Remove()
        {
            if (MouseObject == this) MouseObjectID = 0;
            if (TargetObject == this) TargetObjectID = 0;
            if (MagicObject == this) MagicObjectID = 0;

            if (this == User.NextMagicObject)
                User.ClearMagic();

            MapControl.Objects.Remove(this);
            GameScene.Scene.MapControl.RemoveObject(this);

            if (ObjectID != GameScene.NPCID) return;

            GameScene.NPCID = 0;
            GameScene.Scene.NPCDialog.Hide();
        }

        public abstract void Process();
        public abstract void Draw();
        public abstract bool MouseOver(Point p);

        public void AddBuffEffect(BuffType type)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (!(Effects[i] is BuffEffect)) continue;
                if (((BuffEffect)(Effects[i])).BuffType == type) return;
            }

            PlayerObject ob = null;

            if (Race == ObjectType.Player)
            {
                ob = (PlayerObject)this;
            }
        }
        public void RemoveBuffEffect(BuffType type)
        {
            PlayerObject ob = null;

            if (Race == ObjectType.Player)
            {
                ob = (PlayerObject)this;
            }

            for (int i = 0; i < Effects.Count; i++)
            {
                if (!(Effects[i] is BuffEffect)) continue;
                if (((BuffEffect)(Effects[i])).BuffType != type) continue;
                Effects[i].Repeat = false;
            }
        }

        public Color ApplyDrawColour()
        {
            Color drawColour = DrawColour;
            if (drawColour == Color.Gray)
            {
                drawColour = Color.White;
                DXManager.SetGrayscale(true);
            }
            return drawColour;
        }

        public virtual Missile CreateProjectile(int baseIndex, MLibrary library, bool blend, int count, int interval, int skip, int lightDistance = 6, bool direction16 = true, Color? lightColour = null, uint targetID = 0)
        {
            return null;
        }

        public void Chat(string text)
        {
            if (ChatLabel != null && !ChatLabel.IsDisposed)
            {
                ChatLabel.Dispose();
                ChatLabel = null;
            }

            const int chatWidth = 200;
            List<string> chat = new List<string>();

            int index = 0;
            for (int i = 1; i < text.Length; i++)
                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    chat.Add(text.Substring(index, i - index - 1));
                    index = i - 1;
                }
            chat.Add(text.Substring(index, text.Length - index));

            text = chat[0];
            for (int i = 1; i < chat.Count; i++)
                text += string.Format("\n{0}", chat[i]);

            ChatLabel = new MirLabel
            {
                AutoSize = true,
                BackColour = Color.Transparent,
                ForeColour = Color.White,
                OutLine = true,
                OutLineColour = Color.Black,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Text = text,
            };
            ChatTime = CMain.Time + 5000;
        }
        public virtual void DrawChat()
        {
            if (ChatLabel == null || ChatLabel.IsDisposed) return;

            if (CMain.Time > ChatTime)
            {
                ChatLabel.Dispose();
                ChatLabel = null;
                return;
            }

            ChatLabel.ForeColour = Dead ? Color.Gray : Color.White;
            ChatLabel.Location = new Point(DisplayRectangle.X + (48 - ChatLabel.Size.Width) / 2, DisplayRectangle.Y - (60 + ChatLabel.Size.Height) - (Dead ? 35 : 0));
            ChatLabel.Draw();
        }

        public virtual void CreateLabel()
        {
            NameLabel = null;

            for (int i = 0; i < LabelList.Count; i++)
            {
                if (LabelList[i].Text != Name || LabelList[i].ForeColour != NameColour) continue;
                NameLabel = LabelList[i];
                break;
            }


            if (NameLabel != null && !NameLabel.IsDisposed) return;

            NameLabel = new MirLabel
            {
                AutoSize = true,
                BackColour = Color.Transparent,
                ForeColour = NameColour,
                OutLine = true,
                OutLineColour = Color.Black,
                Text = Name,
            };
            NameLabel.Disposing += (o, e) => LabelList.Remove(NameLabel);
            LabelList.Add(NameLabel);



        }
        public virtual void DrawName()
        {
            CreateLabel();

            if (NameLabel == null) return;
            
            NameLabel.Text = Name;
            NameLabel.Location = new Point(DisplayRectangle.X + (50 - NameLabel.Size.Width) / 2, DisplayRectangle.Y - (32 - NameLabel.Size.Height / 2) + (Dead ? 35 : 8)); //was 48 -
            NameLabel.Draw();
        }
        public virtual void DrawBlend()
        {
            DXManager.SetBlend(true, 0.3F); //0.8
            Draw();
            DXManager.SetBlend(false);
        }
        public void DrawDamages()
        {
            for (int i = Damages.Count - 1; i >= 0; i--)
            {
                Damage info = Damages[i];
                if (CMain.Time > info.ExpireTime)
                {
                    if (info.DamageLabel != null)
                    {
                        info.DamageLabel.Dispose();
                    }

                    Damages.RemoveAt(i);
                }
                else
                {
                    info.Draw(DisplayRectangle.Location);
                }
            }
        }
        public virtual bool ShouldDrawHealth()
        {
            return false;
        }
        public void DrawHealth()
        {
            string name = Name;
            if (Name.Contains("(")) name = Name.Substring(Name.IndexOf("(") + 1, Name.Length - Name.IndexOf("(") - 2);

            if (Dead) return;
            if (Race != ObjectType.Player && Race != ObjectType.Monster) return;

            if (CMain.Time >= HealthTime)
            {
                if (!ShouldDrawHealth()) return;
            }

            Libraries.Prguse.Draw(151, DisplayRectangle.X + 8, DisplayRectangle.Y - 80);
            int index = this == User ? 152 : 183;
            Libraries.Prguse.Draw(index, new Rectangle(0, 0, (int)(40 * PercentHealth / 100F), 4), new Point(DisplayRectangle.X + 8, DisplayRectangle.Y - 80), Color.White, false);
        }

        public virtual void PlayStruckSound()
        {
        }

        public void DrawPoison()
        {
            byte poisoncount = 0;
            if (Poison != PoisonType.None)
            {
                if (Poison.HasFlag(PoisonType.Green))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Green);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Red))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Red);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Slow))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Purple);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Frozen))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Blue);
                    poisoncount++;
                }
            }
        }

        public abstract void DrawBehindEffects(bool effectsEnabled);

        public abstract void DrawEffects(bool effectsEnabled);

        protected void LoopFrame(int start, int frameCount, int frameInterval, int duration)
        {
            if (FrameLoop == null)
            {
                FrameLoop = new FrameLoop
                {
                    Start = start,
                    End = start + frameCount - 1,
                    Loops = (duration / (frameInterval * frameCount)) - 1 //Remove 1 count as we've already done a loop before this is checked
                };
            }
        }
    }

    public class FrameLoop
    {
        public MirAction Action { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Loops { get; set; }

        public int CurrentCount { get; set; }
    }

}
