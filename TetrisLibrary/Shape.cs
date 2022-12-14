using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TetrisLibrary
{
	/// <summary>
	/// Represents the Tetris Shape.
	/// </summary>
    public abstract class Shape : IShape
    {
		protected Block[] blocks;
		protected int currentRotation = 0;
		protected Point[][] rotationOffset;
		private IBoard board;

		//Fires when the Shape is about to join the board pile.
		public event JoinPileHandler JoinPile;

		/// <summary>
		/// A base constructor for different Shapes.
		/// </summary>
		/// <param name="board">the Tetris board</param>
		/// <param name="blocks">the Blocks the Shape consists of</param>
		/// <param name="offset">the different offset combinations for Shape rotation</param>
		public Shape(IBoard board, Block[] blocks, Point[][] offset)
		{
			if (board == null || blocks == null)
				throw new ArgumentNullException();

			for (int i = 0; i < blocks.Length; i++)
				if(blocks[i] == null)
					throw new ArgumentNullException("One of the blocks in the blocks array is null.");

			if (offset != null)
			{
				for (int i = 0; i < offset.Length; i++)
					if (offset[i].Length < blocks.Length)
						throw new ArgumentException("Offset array length: " + offset[i].Length + 
													". Blocks array length: " + blocks.Length);
				for (int i = 0; i < offset.Length; i++)
				{
					for(int j = 0; j < offset.Length; j++)
						if(offset[i][j] == null)
							throw new ArgumentNullException("One of the offset values in the offset array is null.");
				}
			}

			this.board = board;
			this.blocks = blocks;
			this.rotationOffset = offset;
		}

		/// <summary>
		/// The length of the Shape 
		/// (i.e. the number of blocks the Shape consists of).
		/// </summary>
		/// <returns>the length of the Shape</returns>
		public virtual int Length
		{ get { return blocks.Length; } }

		/// <summary>
		/// Returns one of the Blocks of the Shape depending on the index.
		/// </summary>
		/// <param name="i">The index of the required Block</param>
		/// <returns>the Block with the given index</returns>
		public Block this[int i]
		{
			get 
			{
				checkIndex(i);
				return new Block(board, blocks[i].Colour, blocks[i].Position); 
			}
		}

		/// <summary>
		/// Drops the Shape to the bottom of the board (until there is a free space available).
		/// </summary>
		public virtual void Drop()
		{
			while (tryMoveDown())
				MoveDown();
		}

		/// <summary>
		/// Moves the current Shape down.
		/// </summary>
		public virtual void MoveDown()
		{
			if (tryMoveDown())
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].MoveDown();
			}
		}

		/// <summary>
		/// Moves the current Shape left.
		/// </summary>
		public virtual void MoveLeft()
		{
			bool moveAllow = true;

			//verifies that each Block of the Shape can move
			for (int i = 0; i < this.Length && moveAllow; i++)
			{
				if (!blocks[i].TryMoveLeft())
					moveAllow = false;
			}
			//moves the Shape
			if (moveAllow)
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].MoveLeft();
			}
		}

		/// <summary>
		/// Moves the current Shape right.
		/// </summary>
		public virtual void MoveRight()
		{
			bool moveAllow = true;

			//verifies that each Block of the Shape can move
			for (int i = 0; i < this.Length && moveAllow; i++)
			{
				if (!blocks[i].TryMoveRight())
					moveAllow = false;
			}
			//moves the Shape
			if (moveAllow)
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].MoveRight();
			}
		}

		/// <summary>
		/// Rotates the current Shape.
		/// </summary>
		public virtual void Rotate()
		{
			bool moveAllow = true;

			//verifies that each Block of the Shape can move
			for (int i = 0; i < this.Length && moveAllow; i++)
			{
				if (!blocks[i].TryRotate(rotationOffset[currentRotation][i]))
					moveAllow = false;
			}
			//moves the Shape
			if (moveAllow)
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].Rotate(rotationOffset[currentRotation][i]);

				currentRotation++;
				//reset the rotation if necessary
				if (currentRotation >= rotationOffset.Length)
					currentRotation = 0;
			}
		}

		/// <summary>
		/// Fires JoinPile event.
		/// </summary>
		protected virtual void OnJoinPile()
		{
			if(JoinPile != null)
				JoinPile();
		}

		//Checks whether the provided index corresponds to the Shape length.
		private void checkIndex(int i)
		{
			if (i < 0 || i >= this.Length)
				throw new IndexOutOfRangeException("Index: " + i + ". Size: " + this.Length);
		}

		//Verifies whether it is possible to move the Shape down;
		//if not, JoinPile event is fired since the Shape has reached the bottom.
		private bool tryMoveDown()
		{
			for (int i = 0; i < this.Length; i++)
			{
				if (!blocks[i].TryMoveDown())
				{
					OnJoinPile();
					return false;
				}
			}
			return true;
		}
	}
}
