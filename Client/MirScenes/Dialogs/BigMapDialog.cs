using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using SlimDX;
using Font = System.Drawing.Font;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class BigMapDialog : MirImageControl
    {
        public MirButton CloseButton;
        public BigMapViewPort ViewPort;

        public BigMapDialog()
        {
            Index = 170;
            Library = Libraries.Prguse;
            Sort = true;
            Location = Center.Subtract(0, 100);
            NotControl = false;

            ViewPort = new BigMapViewPort()
            {
                Parent = this
            };

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(Size.Width - 25, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();
        }

        public void Toggle()
        {
            if (Visible)
                Hide();
            else
                Show();
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseButton = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    public sealed class BigMapViewPort : MirControl
    {
        BigMapDialog ParentDialog;
        float ScaleX;
        float ScaleY;
        public MirImageControl UserRadarDot;

        int BigMap_MouseCoordsProcessing_OffsetX, BigMap_MouseCoordsProcessing_OffsetY;

        public MirImageControl[] Players;
        public static Dictionary<string, Point> PlayerLocations = new Dictionary<string, Point>();

        public BigMapViewPort()
        {
            NotControl = false;
            Size = new Size(359, 286);

            UserRadarDot = new MirImageControl
            {
                Library = Libraries.Prguse2,
                Index = 1350,
                Parent = this,
                Visible = false,
                NotControl = true
            };

            Players = new MirImageControl[Globals.MaxGroup];
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = new MirImageControl
                {
                    Index = 1350,
                    Library = Libraries.Prguse2,
                    Parent = this,
                    NotControl = false,
                    Visible = false,
                };
            }

            AfterDraw += (o, e) => OnBeforeDraw();
            ParentChanged += (o, e) => SetParent();
            MouseDown += OnMouseClick;
        }

        private void SetParent()
        {
            ParentDialog = (BigMapDialog)Parent;
        }


        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            int X = (int)((e.Location.X - BigMap_MouseCoordsProcessing_OffsetX) / ScaleX);
            int Y = (int)((e.Location.Y - BigMap_MouseCoordsProcessing_OffsetY) / ScaleY);

            var path = GameScene.Scene.MapControl.PathFinder.FindPath(MapObject.User.CurrentLocation, new Point(X, Y));

            if (path == null || path.Count == 0)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("Could not find suitable path.", ChatType.System);
            }
            else
            {
                GameScene.Scene.MapControl.CurrentPath = path;
                GameScene.Scene.MapControl.AutoPath = true;
            }
        }


        private void OnBeforeDraw()
        {
            if (!Parent.Visible) return;
            var texture = GameScene.Scene.MapControl.MiniMapTexture;
            if (texture == null) return;

            int index = GameScene.Scene.MapControl.MiniMap;

            Size = index > 0 ? Libraries.MiniMap.GetSize(index) : new Size(GameScene.Scene.MapControl.Width, GameScene.Scene.MapControl.Height);
            Rectangle viewRect = new Rectangle(0, 0, Math.Min(359, Size.Width), Math.Min(286, Size.Height));

            viewRect.X = 8 + (359 - viewRect.Width) / 2;
            viewRect.Y = 8 + (286 - viewRect.Height) / 2;

            Location = viewRect.Location;
            Size = viewRect.Size;

            BigMap_MouseCoordsProcessing_OffsetX = DisplayLocation.X;
            BigMap_MouseCoordsProcessing_OffsetY = DisplayLocation.Y;

            ScaleX = Size.Width / (float)GameScene.Scene.MapControl.Width;
            ScaleY = Size.Height / (float)GameScene.Scene.MapControl.Height;

            viewRect.Location = new Point(
                (int)(ScaleX * MapObject.User.CurrentLocation.X) - viewRect.Width / 2,
                (int)(ScaleY * MapObject.User.CurrentLocation.Y) - viewRect.Height / 2);

            if (viewRect.Right >= Size.Width)
                viewRect.X = Size.Width - viewRect.Width;
            if (viewRect.Bottom >= Size.Height)
                viewRect.Y = Size.Height - viewRect.Height;

            if (viewRect.X < 0) viewRect.X = 0;
            if (viewRect.Y < 0) viewRect.Y = 0;

            if (index > 0)
                Libraries.MiniMap.Draw(index, DisplayLocation, Size, Color.FromArgb(255, 255, 255));
            else
                DXManager.Draw(texture, null, new Vector3(DisplayRectangle.X, DisplayRectangle.Y, 0.0F), Color.White);

            int startPointX = (int)(viewRect.X / ScaleX);
            int startPointY = (int)(viewRect.Y / ScaleY);

            var map = GameScene.Scene.MapControl;
                float x;
                float y;
                for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
                {
                    MapObject ob = MapControl.Objects[i];

                    if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell) continue;
                    x = ((ob.CurrentLocation.X - startPointX) * ScaleX) + DisplayLocation.X;
                    y = ((ob.CurrentLocation.Y - startPointY) * ScaleY) + DisplayLocation.Y;

                    Color colour;

                if (GroupDialog.GroupList.Contains(ob.Name) || ob.Name.EndsWith(string.Format("({0})", MapObject.User.Name)))
                    colour = Color.Blue;
                else if (ob is PlayerObject)
                    colour = Color.Blue;
                else if (ob is NPCObject || ob.AI == 6)
                    colour = Color.FromArgb(0, 255, 50);
                else
                    colour = Color.FromArgb(255, 0, 0);

                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 2, 2), new Vector3((float)(x - 0.5), (float)(y - 0.5), 0.0F), colour);
                }

                x = MapObject.User.CurrentLocation.X * ScaleX;
                y = MapObject.User.CurrentLocation.Y * ScaleY;
                var s = UserRadarDot.Size;
                UserRadarDot.Location = new Point((int)x - s.Width / 2, (int)y - s.Height / 2);

                if (GroupDialog.GroupList.Count > 0)
                {
                    for (int i = 0; i < GameScene.Scene.GroupDialog.GroupMembers.Length; i++)
                    {
                        string groupMembersName = GameScene.Scene.GroupDialog.GroupMembers[i].Text;
                        Players[i].Visible = false;

                        foreach (var groupMembersMap in GroupDialog.GroupMembersMap.Where(x => x.Key == groupMembersName && x.Value == map.Title))
                        {
                            foreach (var groupMemberLocation in PlayerLocations.Where(x => x.Key == groupMembersMap.Key))
                            {

                                float alteredX = ((groupMemberLocation.Value.X - startPointX) * ScaleX);
                                float alteredY = ((groupMemberLocation.Value.Y - startPointY) * ScaleY);

                                if (groupMembersName != MapObject.User.Name)
                                    Players[i].Visible = true;

                                Players[i].Hint = groupMemberLocation.Key;
                                Players[i].Location = new Point((int)(alteredX - 0.5F) - 3, (int)(alteredY - 0.5F) - 3);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Players.Length; i++)
                    {
                        Players[i].Visible = false;
                    }
                }

        }
    }

}
