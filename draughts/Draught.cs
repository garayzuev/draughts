using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace draughts
{


    class Draught
    {
        bool _isBlack;
        bool _isKing;
        Position pos;
        bool isSelected;

        public Draught(bool isBlack, Position pos)
        {
            this._isBlack = isBlack;
            _isKing = false;
            isSelected = false;            
            moveTo(pos);
        }


        public void select()
        {
            isSelected = true;
        }
        
        public bool canMoveWithAttack(Position pos)
        {
            if (!isKing())
            {
                return (Math.Abs(this.pos.y - pos.y) == 2 && Math.Abs(this.pos.x - pos.x) == 2);                    
            }
            return canMoveTo(pos);
        }
        

        public List<Position> getAttackMoves()
        {
            List<Position> list = getFreeMoves();
            if (list == null)
                list = new List<Position>();
            if (isBlack())
            {
                if (pos.y + 1 >= 0 && pos.y + 1 < 8 && pos.x - 1 >= 0 && pos.x - 1 < 8) //Down left corner
                    list.Add(new Position(pos.x - 1, pos.y + 1));
                if (pos.y + 1 >= 0 && pos.y + 1 < 8 && pos.x + 1 < 8 && pos.x + 1 >= 0) //Down right corner
                    list.Add(new Position(pos.x + 1, pos.y + 1));
               
            }
            else
            {
                if (pos.y - 1 < 8 && pos.y - 1 >= 0 && pos.x - 1 >= 0 && pos.x - 1 < 8) //Up left corner
                    list.Add(new Position(pos.x - 1, pos.y - 1));
                if (pos.y - 1 < 8 && pos.y - 1 >= 0 && pos.x + 1 < 8 && pos.x + 1 >= 0) //Up right corner
                    list.Add(new Position(pos.x + 1, pos.y - 1));                
            }
            return list.Count != 0 ? list : null;
        }

        public List<Position> getFreeMoves()
        {
            List<Position> list = new List<Position>();
            if (isKing())
                for (int i = 1; i < 8; i++)
                {
                    if (pos.y - i < 8 && pos.y - i >= 0 && pos.x - i >= 0 && pos.x - i < 8) //Up left corner
                        list.Add(new Position(pos.x - i, pos.y - i));
                    if (pos.y - i < 8 && pos.y - i >= 0 && pos.x + i < 8 && pos.x + i >= 0) //Up right corner
                        list.Add(new Position(pos.x + i, pos.y - i));
                    if (pos.y + i >= 0 && pos.y + i < 8 && pos.x - i >= 0 && pos.x - i < 8) //Down left corner
                        list.Add(new Position(pos.x - i, pos.y + i));
                    if (pos.y + i >= 0 && pos.y + i < 8 && pos.x + i < 8 && pos.x + i >= 0) //Down right corner
                        list.Add(new Position(pos.x + i, pos.y + i));
                }
            else
            {
                if (isBlack())
                {
                    if (pos.y - 1 < 8 && pos.y - 1 >= 0 && pos.x - 1 >= 0 && pos.x - 1 < 8) //Up left corner
                        list.Add(new Position(pos.x - 1, pos.y - 1));
                    if (pos.y - 1 < 8 && pos.y - 1 >= 0 && pos.x + 1 < 8 && pos.x + 1 >= 0) //Up right corner
                        list.Add(new Position(pos.x + 1, pos.y - 1));
                }
                else
                {

                    if (pos.y + 1 >= 0 && pos.y + 1 < 8 && pos.x - 1 >= 0 && pos.x - 1 < 8) //Down left corner
                        list.Add(new Position(pos.x - 1, pos.y + 1));
                    if (pos.y + 1 >= 0 && pos.y + 1 < 8 && pos.x + 1 < 8 && pos.x + 1 >= 0) //Down right corner
                        list.Add(new Position(pos.x + 1, pos.y + 1));
                }
            }
            return list.Count != 0 ? list : null;
        }
        public void moveTo(Position pos)
        {
            this.pos = pos;
            checkKing();            
        }

        
        public bool canAttackTo(Position pos)
        {
            return getAttackMoves().Contains(pos);
        }
        public bool canMoveTo(Position pos)
        {
            return getFreeMoves().Contains(pos);
        }
        public List<Position> getRoutTo(Position pos)
        {
            if (!canMoveWithAttack(pos))
                return null;
            if (Math.Abs(this.pos.x - pos.x) != Math.Abs(this.pos.y - pos.y))
                return null;
            if (!isKing() && Math.Abs(this.pos.x - pos.x) > 2)
                return null;
            List<Position> posi = new List<Position>();
            for (int i = 1, x, y; i <= Math.Abs(this.pos.x - pos.x); i++)
            {
                x = this.pos.x - pos.x > 0 ? this.pos.x - i : this.pos.x + i;
                y = this.pos.y - pos.y > 0 ? this.pos.y - i : this.pos.y + i;
                posi.Add(new Position(x, y));
            }
            return posi;
        }
        public Boolean isMyPosition(Position pos)
        {
            if (this.pos.x == pos.x && this.pos.y == pos.y)
                return true;
            else
                return false;

        }
        
        public Position getPosition()
        {
            return pos;
        }

        public bool isBlack()
        {
            return _isBlack;
        }

        public bool isKing()
        {
            return _isKing;
        }
        private void checkKing()
        {
            if (isBlack() && pos.y == 0)
            {
                _isKing = true;
            }
            if (!isBlack() && pos.y == 7)
            {
                _isKing = true;
            }
        }
        public Draught clone()
        {
            Draught d = new Draught(isBlack(), this.pos);
            d._isKing = this._isKing;
            return d;
        }

    }
}
