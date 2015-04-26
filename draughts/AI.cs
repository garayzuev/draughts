using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draughts
{
    struct Turn {
        public Draught d;
        public bool isAttack;
        public Position to;

        
        public Turn(Draught d, bool isAttack, Position to)
        {
            this.d = d;
            this.isAttack = isAttack;
            this.to = to;
        }
    }
    class AI
    {
        const int POINTS_FOR_KING                            =   8;
        const int POINTS_FOR_EAT                             =   4;
        const int POINTS_FOR_SAVE                            =   2;
        const int POINTS_FOR_MOVE_WITHOUT_NEXT_EATEN         =   1;
        const int POINTS_FOR_MOVE_FORW                       =   1;

        List<Draught> draughts;
        Dictionary<Turn, int> sortedMoves;
        bool isBlack;

        public AI(bool isBlack)
        {
            this.isBlack = isBlack;
            sortedMoves = new Dictionary<Turn, int>();
        }

        public Turn doTurn(List<Draught> list){
            draughts = list;
            sortedMoves.Clear();
            findAllMoves();
            if (sortedMoves.Count > 0)
            {
                int max = 0;
                foreach(int i in sortedMoves.Values){
                    max = max < i ? i : max; 
                }
                return findInDictionary(max);
            }
            else
            {
                foreach (Draught d in draughts)
                {
                    foreach (Position p in d.getFreeMoves())
                    {
                        if (Arbiter.canMove(d, p, draughts))
                        {
                            return new Turn(d, false, p);
                        }
                    }
                }
                return new Turn(new Draught(isBlack,new Position(-1,-1)),false,new Position(-1,-1));
            }
        }
        private void findAllMoves()
        {

            if (isNeedAttack()) return;
            freeMoves();

        }
        private void freeMoves()
        {
            foreach (Draught d in draughts)
            {
                if(d.isBlack() == isBlack)

                    foreach (Position p in d.getFreeMoves())
                    {
                        if (Arbiter.findDraught(p, draughts) == null)
                        {
                            List<Draught> newList = copyList(draughts);
                            Arbiter.findDraught(d.getPosition(), newList).moveTo(p);                            
                            int points = checkDraught(p, newList);
                            if (d.isKing() != Arbiter.findDraught(p, newList).isKing())
                                points += POINTS_FOR_KING;
                            sortedMoves.Add(new Turn(d, false, p), points);
                        }
                    }
            }
        }
        private bool isNeedAttack()
        {
            List <Draught> list = Arbiter.getDraughtsNeedToMove(isBlack, draughts);
            if (list != null && list.Count > 0)
            {
                foreach (Draught d in list)
                {
                    int points = attack(d, draughts);

                    List<Position> pos = d.getAttackMoves();
                    foreach (Position p in pos)
                        if (Arbiter.canAttack(d, p, draughts))
                        {
                            sortedMoves.Add(new Turn(d, true, p), points);
                            break;
                        }
                }
                return true;

            }
            else
                return false;
        }

        private int attack(Draught d, List<Draught> list)
        {
            List<Position> pos = d.getAttackMoves();
            foreach (Position p in pos)
            {
                if (Arbiter.canAttack(d, p, list))
                {
                    Position to = new Position();
                    to.x = p.x - d.getPosition().x > 0 ? p.x + 1 : p.x - 1;
                    to.y = p.y - d.getPosition().y > 0 ? p.y + 1 : p.y - 1;

                    List<Draught> newList = copyList(list);
                    newList.Remove(Arbiter.findDraught(p, newList));
                    Arbiter.findDraught(d.getPosition(), newList).moveTo(to);
                    return POINTS_FOR_EAT + (Arbiter.findDraught(to, newList).isKing() != d.isKing()?POINTS_FOR_KING:0) + attack(Arbiter.findDraught(to, newList), newList);
                }
            }
            return checkDraught(d.getPosition(),list);
        }
        private int checkDraught(Position pos, List<Draught> list)
        {
            int points = POINTS_FOR_MOVE_WITHOUT_NEXT_EATEN;
            foreach (Draught dr in list)
            {
                if (dr.isBlack()!=isBlack && Arbiter.canAttack(dr, pos, list))
                {
                    points -= POINTS_FOR_MOVE_WITHOUT_NEXT_EATEN;
                    break;
                }
            }
            if (isNear(pos, list))
                points += POINTS_FOR_SAVE;
            return points;

        }
        private bool isNear(Position p, List<Draught> list)
        {
            foreach (Draught dr in list)
            {
                if (dr.isBlack() != isBlack)
                    continue;
                Position pos = dr.getPosition();
                if (p.x - 1 == pos.x && p.y - 1 == pos.y ||
                    p.x + 1 == pos.x && p.y - 1 == pos.y ||
                    p.x - 1 == pos.x && p.y + 1 == pos.y ||
                    p.x + 1 == pos.x && p.y + 1 == pos.y)
                    return true;
            }
            return false;
        }
        private Turn findInDictionary(int value)
        {
            foreach (KeyValuePair<Turn, int> kvp in sortedMoves)
            {
                if (kvp.Value == value)
                    return kvp.Key;
            }
            return new Turn();
        }
        private List<Draught> copyList(List<Draught> list)
        {
            List<Draught> newList = new List<Draught>();
            foreach (Draught d in list)
                newList.Add(d.clone());
            return newList;
        }
    }
}
