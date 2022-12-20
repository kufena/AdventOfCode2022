using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day17
{
    public abstract class Shapes
    {
        public long height { get; set; }
        public long width { get; set; }

        public long[] topProfile { get; set; }
        public long[] botProfile { get; set; }
        public long leftshifth = 0;
        public long rightshifth = 0;
        public static Shapes[] ShapeList = new Shapes[] { new Line(), new Cross(), new Ell(), new Vert(), new Square() };

        public abstract bool IsClear(long[][] board, long x, long y, long bwidth, long bheight);
        public abstract void Place(long[][] board, long x, long y, long bwidth, long bheight);

    }

    public class Line : Shapes
    {
        public Line()
        {
            height = 1;
            width = 4;
            topProfile = new long[4] { 0, 0, 0, 0 };
            botProfile = new long[4] { 0, 0, 0, 0 };
        }

        public override bool IsClear(long[][] board, long x, long y, long bwidth, long bheight)
        {
            if (x + (width - 1) >= 7) return false;
            if (x < 0) return false;

            for (long i = x; i < x + width; i++) if (board[i][y] != 0) return false;
            return true;
        }
        public override void Place(long[][] board, long x, long y, long bwidth, long bheight) 
        {
            for (long i = x; i < x + width; i++) board[i][y] = 1;
        }

    }

    public class Cross : Shapes 
    {
        public Cross()
        {
            height = 3;
            width = 3;
            topProfile = new long[3] { 1, 0, 1 };
            botProfile = new long[3] { 1, 0, 1 };
            //leftshifth = 1;
            //rightshifth = 1;
        }

        public override bool IsClear(long[][] board, long x, long y, long bwidth, long bheight)
        {
            if (x + (width - 1) >= 7) return false;
            if (x < 0) return false;
            return (board[x + 1][y] == 0 &&
            board[x][y + 1] == 0 &&
            board[x + 1][y + 1] == 0 &&
            board[x + 2][y + 1] == 0 &&
            board[x + 1][y + 2] == 0);

        }

        public override void Place(long[][] board, long x, long y, long bwidth, long bheight)
        {
            board[x + 1][y] = 1;
            board[x][y + 1] = 1; 
            board[x + 1][y + 1] = 1;
            board[x + 2][y+ 1] = 1;
            board[x + 1][y + 2] = 1;
        }
    }

    public class Ell : Shapes
    {
        public Ell()
        {
            height = 3;
            width = 3;
            topProfile = new long[3] { 2, 2, 0 };
            botProfile = new long[3] { 0, 0, 0 };
        }

        public override bool IsClear(long[][] board, long x, long y, long bwidth, long bheight)
        {
            if (x + (width - 1) >= 7) return false;
            if (x < 0) return false;
            return (board[x][y] == 0 &&
                board[x + 1][y] == 0 &&
                board[x + 2][y] == 0 &&
                board[x + 2][y + 1] == 0 &&
                board[x + 2][y + 2] == 0);
        }

        public override void Place(long[][] board, long x, long y, long bwidth, long bheight)
        {
            board[x][y] = 1;
            board[x + 1][y] = 1;
            board[x + 2][y] = 1;
            board[x + 2][y + 1] = 1;
            board[x + 2][y + 2] = 1;
        }

    }

    public class Vert : Shapes
    {
        public Vert()
        {
            height = 4;
            width = 1;
            topProfile = new long[1] { 0 };
            botProfile = new long[1] { 0 };
        }
        public override bool IsClear(long[][] board, long x, long y, long bwidth, long bheight)
        {
            if (x + (width - 1) >= 7) return false;
            if (x < 0) return false;
            return board[x][y] == 0 &&
                board[x][y + 1] == 0 &&
                board[x][y + 2] == 0 &&
                board[x][y + 3] == 0;
        }

        public override void Place(long[][] board, long x, long y, long bwidth, long bheight)
        {
            if (!IsClear(board, x, y, bwidth, bheight))
                throw new Exception("it isn't clear after all!");
            board[x][y] = 1;
            board[x][y + 1] = 1;
            board[x][y + 2] = 1;
            board[x][y + 3] = 1;
        }
    }

    public class Square : Shapes
    {
        public Square()
        {
            height = 2;
            width = 2;
        topProfile = new long[2] { 0,0 };
        botProfile = new long[2] { 0,0 };
        }

        public override bool IsClear(long[][] board, long x, long y, long bwidth, long bheight)
        {
            if (x + (width - 1) >= 7) return false;
            if (x < 0) return false;
            return board[x][y] == 0 &&
                board[x][y + 1] == 0 &&
                board[x + 1][y] == 0 &&
                board[x + 1][y + 1] == 0;
        }

        public override void Place(long[][] board, long x, long y, long bwidth, long bheight)
        {
            board[x][y] = 1;
            board[x + 1][y] = 1;
            board[x][y + 1] = 1;
            board[x + 1][y + 1] = 1;
        }

    }
}
