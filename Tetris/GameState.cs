using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;

        public Block CurrentBlock 
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();
            }
        }

        public gameGrid GameGrid { get; }
        public BlockQueue Queue { get; }
        public bool GameOver { get; private set; }
        public GameState()
        {
            GameGrid = new gameGrid(22, 10);
            Queue = new BlockQueue();
            CurrentBlock = Queue.GetAndUpdate();

        }

        public bool BlockFits() 
        {
            foreach (Position p in CurrentBlock.TilePositions()) 
            {
                if (!GameGrid.isEmpty(p.Row, p.Column)) return false;
            }
            return true;
        }

        public void RotateBlockCW() 
        {
            CurrentBlock.RotateCW();
            if (!BlockFits()) 
            {
                CurrentBlock.RotateCCW();
            }
        }
        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();
            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft() 
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits()) 
            {
                CurrentBlock.Move(0, 1);
            }
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        public bool isGameOver() 
        {
            return !(GameGrid.isRowEmpty(0) && GameGrid.isRowEmpty(1));
        }

        private void PlaceBlock() 
        {
            foreach (Position p in CurrentBlock.TilePositions()) 
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.id;
            }

            GameGrid.clearFullRows();

            if (isGameOver())
            {
                GameOver = true;
            }
            else 
            {
                CurrentBlock = Queue.GetAndUpdate();
            }
        }

        public void MoveBlockDown() 
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits()) 
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
    }
}
