using Server.MirDatabase;

namespace Server.MirObjects.Monsters
{
    public class PassiveMonster : MonsterObject
    {
        protected internal PassiveMonster(MonsterInfo info)
            : base(info)
        {
        }

        protected override void FindTarget()
        {
        }
    }
}
