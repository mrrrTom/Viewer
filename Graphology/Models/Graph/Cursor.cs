using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphology.Models.Graph
{
    internal class Cursor
    {
        private readonly int _cols;
        private readonly int _rows;
        private int _curRow = 0;
        private int _curCol = -1;
        private Queue<Position<int>> _queue = new Queue<Position<int>>();
        private HashSet<Position<int>> _usedIndexes = new HashSet<Position<int>>();

        public Cursor(int rows, int cols)
        {
            _cols = cols;
            _rows = rows;
        }

        private Position<int> GetNext()
        {
            do
            {
                var x = _curCol + 1;
                if (x < _cols)
                {
                    _curCol = x;
                }
                else
                {
                    _curRow++;
                    _curCol = 0;
                }
            }
            while (_usedIndexes.Contains(new Position<int>(_curRow, _curCol)));
            _usedIndexes.Add(new Position<int>(_curRow, _curCol));
            return GetCurrent();
        }

        public Queue<Position<int>> GetNeigbours(int count)
        {
            var result = new Queue<Position<int>>();
            FillNeigbourQueue();
            for (var i = 0; i < count; i++)
            {
                var indexes = _queue.Dequeue();
                _usedIndexes.Add(indexes);
                result.Enqueue(indexes);
            }

            _queue.Clear();
            return result;
        }

        private void FillNeigbourQueue()
        {
            var maxStep = Math.Max(_cols - _curCol, _rows - _curRow);
            for (var s = 1; s <= maxStep; s++)
            {
                var circleValues = GetCircle(s);
                foreach (var v in circleValues)
                {
                    if (_usedIndexes.Contains(v)) continue;
                    if (v.Y >= _cols) continue;
                    if (v.X >= _rows) continue;
                    if (v.X < _curRow) continue;
                    if (v.Y < 0) continue;
                    if (v.X < 0) continue;
                    if (v.X == _curRow && v.Y < _curCol) continue;
                    _queue.Enqueue(v);
                }
            }
        }

        private List<Position<int>> GetCircle(int step)
        {
            var result = new List<Position<int>>();
            var newColPos = _curCol + step;
            var newColNeg = _curCol - step;
            var newRowPos = _curRow + step;
            var newRowNeg = _curRow - step;
            for (var row = newRowNeg; row <= newRowPos; row++)
            {
                result.Add(new Position<int>(row, newColPos));
                result.Add(new Position<int>(row, newColNeg));
            }
            for (int col = newColNeg + 1; col <= newColPos - 1; col++)
            {
                result.Add(new Position<int>(newRowNeg, col));
                result.Add(new Position<int>(newRowPos, col));
            }

            return result;
        }

        public Position<int> GetNextInRow()
        {
            _queue.Clear();
            return GetNext();
        }

        public Position<int> GetCurrent()
        {
            return new Position<int>(_curRow, _curCol);
        }
    }
}
