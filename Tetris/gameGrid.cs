using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris
{
    public class gameGrid
    {
        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        //indexer
        public int this[int r, int c] 
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        //constructor
        public gameGrid(int rows, int cols) 
        {
            this.Rows = rows;
            this.Columns = cols;
            grid = new int[rows, cols];
        }

        //check boundary
        public bool isInside(int row, int col) 
        {
            return (row >= 0 && row < Rows) && (col >= 0 && col < Columns);
        }

        //check if row col is 0/empty or not
        public bool isEmpty(int row, int col) 
        {
            return isInside(row, col) && grid[row, col] == 0; 
        }

        //check if row is full
        public bool isRowFull(int row) 
        {
            for (int col = 0; col < Columns; col++) 
            {
                if (this.grid[row, col] == 0) return false;
            }
            return true;
        }

        //check if row is empty
        public bool isRowEmpty(int row)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (this.grid[row, col] != 0) return false;
            }
            return true;
        }

        //clear row
        public void clearRow(int row) 
        {
            for (int col = 0; col < Columns; col++) 
            {
                this.grid[row, col] = 0;
            }
        }

        //move row down by numRows
        public void moveRowDown(int row, int numRows) 
        {
            for (int col = 0; col < Columns; col++) 
            {
                grid[row + numRows, col] = grid[row, col];
                grid[row, col] = 0;
            }
        }

        public int clearFullRows() 
        {
            int cleared = 0;
            for (int row = Rows - 1; row >= 0; row--) 
            {
                if (isRowFull(row))
                {
                    clearRow(row);
                    cleared++;
                }

                else if (cleared > 0) moveRowDown(row, cleared);
            }
            return cleared;
        }

    }
}
