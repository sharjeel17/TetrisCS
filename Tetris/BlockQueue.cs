using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock(),

        };

        private readonly Random rnd = new Random();
        public Block NextBlock { get; private set; }

        private Block RandomBlock() 
        {
            return blocks[rnd.Next(blocks.Length)];
        }
        public BlockQueue() { 
            NextBlock = RandomBlock();
        }

        public Block GetAndUpdate() 
        {
            Block block = NextBlock;
            NextBlock = RandomBlock();
            return block;
        }
    }
}
