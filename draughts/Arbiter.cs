using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draughts
{
    class Arbiter
    {
        
        public static bool isVictory(bool isBlack, List<Draught> list)
        {
            foreach(Draught d in list){
                if (d.isBlack() == !isBlack)
                    return false;
            }
            return true;
        }

        public static List<Draught> getDraughtsNeedToMove(bool isBlack, List<Draught> list)
        {
            List<Draught> res = new List<Draught>();
            foreach (Draught d in list)
                if (d.isBlack() == isBlack && needAttack(d, list))
                    res.Add(d);
            if (res.Count() > 0) return res;
            else return null;
        }

        public static bool needAttack(Draught d, List<Draught> list)
        {
            List<Position> listPos = d.getAttackMoves();
            foreach (Draught dr in list)
                foreach (Position p in listPos)
                    if (dr != d && dr.isBlack() != d.isBlack() && dr.isMyPosition(p) && canAttack(d, p, list))
                        return true;
            return false;
        }

        public static bool canAttack(Draught from, Position attacked, List<Draught> list)
        {
            Position to = new Position((from.getPosition().x - attacked.x > 0 ? attacked.x - 1 : attacked.x + 1), (from.getPosition().y - attacked.y > 0 ? attacked.y - 1 : attacked.y + 1));
            if (to.x > 7 || to.x < 0 || to.y > 7 || to.y < 0)
                return false;
            List<Position> pos = from.getRoutTo(to);
            if (pos == null)
                return false;
            bool isFind = false;
            foreach (Draught d in list)
            {
                if (d.isMyPosition(attacked) && d.isBlack() != from.isBlack())
                    isFind = true;
            }
            if (!isFind)
                return false;
            foreach (Draught d in list)
                foreach(Position p in pos)
                    if (!d.isMyPosition(attacked) && d.isMyPosition(p) )
                        return false;
            return true;
        }

        public static bool canMove(Draught from, Position to, List<Draught> list)
        {
            if (needAttack(from, list))
                return isAttack(from, to, list);
            else
            {
                foreach (Draught d in list)
                    if (d.isMyPosition(to))
                        return false;
                return from.canMoveTo(to);
            }
                
        }

        public static bool isAttack(Draught attacker, Position to, List<Draught> list)
        {
            if(!attacker.canMoveWithAttack(to))
                return false;
            Position attacked = new Position();
            attacked.x = to.x - attacker.getPosition().x > 0 ? to.x - 1 : to.x + 1;
            attacked.y = to.y - attacker.getPosition().y > 0 ? to.y - 1 : to.y + 1;
            if (attacker.isMyPosition(attacked))
                return false;
            foreach (Draught d in list)
            {
                if(d.isMyPosition(attacked))
                    return canAttack(attacker, attacked, list);
            }

            return false;
        }

        public static Draught findDraught(Position pos, List<Draught> draughts)
        {
            foreach (Draught d in draughts)
            {
                if (d.isMyPosition(pos))
                    return d;
            }
            return null;
        }
    }
}
