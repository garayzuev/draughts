using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draughts
{
    class Table
    {
        AI comp;
        List<Draught> draughts;
        List<Draught> attackers;
        bool isBlackTurn = true;
        bool isAttacking = false;
        Draught select;
        MainWindow mw;

        public Table(MainWindow mw)
        {
            this.mw = mw;
            draughts = new List<Draught>();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 4; j++)
                    draughts.Add(new Draught(false, new Position(i % 2 == 0 ? j * 2 + 1 : j * 2, i)));
            for (int i = 7; i > 4; i--)
                for (int j = 0; j < 4; j++)
                    draughts.Add(new Draught(true, new Position(i != 6 ? j * 2 : j * 2 + 1, i)));
            comp = new AI(false);

        }
        public bool isTurnBlack()
        {
            return isBlackTurn;
        }
        public void startOfTurn(Position pos)
        {
            attackers = Arbiter.getDraughtsNeedToMove(isBlackTurn, draughts);
            select = Arbiter.findDraught(pos,draughts);
        }
        public void turnAI()
        {
            Turn t = comp.doTurn(draughts);
            if (t.d.getPosition().x != -1)
            {
                select = t.d;
                if (t.isAttack)
                {
                    aiAttack(t.to);
                    while (Arbiter.getDraughtsNeedToMove(isBlackTurn, draughts) != null)
                    {
                        Turn tur = comp.doTurn(draughts);
                        if (tur.d.getPosition().x != -1)
                        {
                            select = tur.d;
                            aiAttack(tur.to);
                        }
                        else
                            mw.errorMessage();
                    }
                }
                else
                {
                    moveTo(t.to);
                }
                if (Arbiter.isVictory(isBlackTurn, draughts))
                {
                    mw.victoryMessage();
                }
            }
            else mw.errorMessage();

            isBlackTurn = !isBlackTurn;
        }
        public bool wantMove(Position to)
        {
            if (attackers != null)
            {
                if (attackers.Contains(select) && Arbiter.isAttack(select, to, draughts))
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return Arbiter.canMove(select, to, draughts);
            }
        }
        public void moveTo(Position to)
        {
            Position from = select.getPosition();
            bool isKing = select.isKing();
            if (Arbiter.isAttack(select, to, draughts))
                attack(to);
            else
                select.moveTo(to);            
            if (isKing != select.isKing())
                mw.setKing(from.x, from.y);
            mw.turnOn(from.x,from.y,to.x, to.y);
        }
        public bool endOfTurn()
        {
            if (isAttacking && Arbiter.needAttack(select, draughts))
            {
                return false;
            }
            else
            {
                if (Arbiter.isVictory(isBlackTurn, draughts))
                {
                    mw.victoryMessage();
                    return true;
                }
                isBlackTurn = !isBlackTurn;
                isAttacking = false;
                return true;
            }
        }
        public bool notMoves(bool isBlack)
        {
            foreach(Draught d in draughts)
                if (d.isBlack() == isBlack)
                {
                    foreach (Position p in d.getFreeMoves())
                        if (Arbiter.canMove(d, p, draughts))
                            return false;
                    foreach (Position p in d.getAttackMoves())
                        if (Arbiter.canAttack(d, p, draughts))
                            return false;
                }
            return true;
        }
        
        public bool isContinueAttack()
        {
            return isAttacking;
        }
        private void aiAttack(Position attacked)
        {
            Position from = select.getPosition();
            Position to = new Position();
            to.x = attacked.x - select.getPosition().x > 0 ? attacked.x + 1 : attacked.x - 1;
            to.y = attacked.y - select.getPosition().y > 0 ? attacked.y + 1 : attacked.y - 1;
            draughts.Remove(Arbiter.findDraught(attacked, draughts));
            mw.removeEllipse(attacked.x, attacked.y);
            bool isKing = select.isKing();
            select.moveTo(to);
            if (isKing != select.isKing())
                mw.setKing(from.x,from.y);
            mw.turnOn(from.x, from.y, to.x, to.y);
        }
        private void attack(Position to)
        {
            Position from = select.getPosition();
            Position attacked = new Position();
            attacked.x = to.x - select.getPosition().x > 0 ? to.x - 1 : to.x + 1;
            attacked.y = to.y - select.getPosition().y > 0 ? to.y - 1 : to.y + 1;
            draughts.Remove(Arbiter.findDraught(attacked,draughts));
            mw.removeEllipse(attacked.x, attacked.y);
            select.moveTo(to);
            isAttacking = true;
        }
    }
}
