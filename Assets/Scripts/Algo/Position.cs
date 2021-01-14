using System;

namespace Pylos
{
    public class Position
    {
        private int _stage;
        private int _i;
        private int _j;

        public int I
        {
            get => _i;
            set => _i = value;
        }

        public int Stage
        {
            get => _stage;
            set => _stage = value;
        }

        public int J
        {
            get => _j;
            set => _j = value;
        }

        public Position(int stage, int i, int j)
        {
            this._stage = stage;
            this._i = i;
            this._j = j;
        }

        public Position Clone()
        {
            return new Position(_stage, _i, _j);
        }

        public void Display()
        {
            Console.WriteLine("Stage :"+_stage+" Emplacement ["+_i+";"+_j+"]");
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Position position = (Position) obj;
                return (this.I == position.I && this.J == position.J && this.Stage == position.Stage) ;
            }
        }

        public bool CheckSuperimposing(Position position)
        {   Console.WriteLine("Emplacement où monter :");
            Display();
            Console.WriteLine("Emplacement de départ :");
            position.Display();
            return (_i != position._i || _j != position._j)
                   && (_i + 1 != position._i || _j != position._j)
                   && (_i != position._i || _j+1 != position._j )
                   && (_i + 1 != position._i || _j + 1!= position._j );
        }
    }
}