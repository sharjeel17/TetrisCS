﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public int Score { get; private set; }

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

            Score += GameGrid.clearFullRows();

            if (isGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = Queue.GetAndUpdate();
            }
        }

        //move block down if it fits
        //called on down key
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        //calculate distance from current tile (row,col) to the bottom
        private int TileDropDistance(Position p)
        {
            int drop = 0;
            while (GameGrid.isEmpty(p.Row + drop + 1, p.Column)) drop++;
            return drop;
        }

        //check each tile (row,col) of the current block
        //and take the minimum distance
        public int BlockDropDistance() 
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions()) 
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }
            return drop;
        }

        //drop block instantly
        //called from Spacebar key
        public void DropBlock() 
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

    }
}
