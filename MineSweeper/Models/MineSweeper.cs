using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using Extensions;

namespace MineSweeper.Models
{
    public class MineSweeperModel : NotificationObject
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int MineCount;
        public readonly Cell[] Cells;
        public event EventHandler<bool> GameOver;
        private bool isFirst = true;
        public bool UseComplex = true;

        public MineSweeperModel(int width, int height, int mineCount)
        {
            this.Width = width;
            this.Height = height;
            this.Cells = new Cell[width * height];
            foreach (var i in Enumerable.Range(0, Cells.Length))
                Cells[i] = new Cell();
            this.MineCount = mineCount < Cells.Length ? mineCount : Cells.Length - 1;
            candidate = new SolveCellType[Cells.Length];
            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                candidate[i] = SolveCellType.Undef;
            }
        }

        public void Retry()
        {
            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                Cells[i].Flag = false;
                Cells[i].Open = false;
            }
            solverCount = 0;
            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                candidate[i] = SolveCellType.Undef;
            }
        }

        public void Open(int index)
        {
            if (isFirst)
                Init(index);

            if (Cells[index].Open == true)
                return;

            if (Cells[index].Type == CellType.Mine)
            {

                System.Diagnostics.Debug.WriteLine("Miss Index:{0}",index);
                if (GameOver != null)
                    GameOver(this, false);
                return;
            }

            Cells[index].Open = true;

            if (Cells.Count(c => !c.Open) == MineCount)
            {
                if (GameOver != null)
                    GameOver(this, true);
                return;
            }

            if (Cells[index].NeighborMineCount == 0)
                foreach (var i in Neighbors(index))
                    Open(i);
        }

        private void Init(int index)
        {
            isFirst = false;
            var nine = Neighbors(index);
            nine.Add(index);
            nine.ForEach(i => Cells[i].Type = CellType.Empty);
            var rand = new Random();
            var cellTypes = Enumerable.Range(0, Cells.Length)
                .Where(i => !nine.Contains(i)).OrderBy(_ => rand.NextDouble())
                .Select((i, n) => n < MineCount ? new { i, type = CellType.Mine } : new { i, type = CellType.Empty });
            foreach (var r in cellTypes)
                Cells[r.i].Type = r.type;

            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                var count = 0;
                foreach (var j in Neighbors(i))
                    if (Cells[j].Type == CellType.Mine)
                        count++;
                Cells[i].NeighborMineCount = count;
            }
        }

        private List<int> Neighbors(int index)
        {
            var x = index % Width;
            var y = index / Width;
            var dx = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };
            var dy = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
            var res = new List<int>();
            foreach (var i in Enumerable.Range(0, 8))
            {
                var nx = x + dx[i];
                var ny = y + dy[i];
                if (0 <= nx && nx < Width && 0 <= ny && ny < Height)
                    res.Add(ny * Width + nx);
            }
            return res;
        }

        private int solverCount = 0;

        public bool Solve()
        {
            while (SolveOneStep()){};
            return Cells.Count(c => !c.Open) == MineCount;
        }

        private SolveCellType[] candidate;
        public bool SolveOneStep()
        {
            var res = SolveTrivial();
            if (res)
                return true;
            if (!UseComplex)
                return false;
            solverCount++;
            return SolveComplex();
        }

        private bool UpdateBoard(bool[] canOpen, bool[] canFlag)
        {
            canFlag.ForEach((f, i) =>
            {
                Cells[i].Flag = f;
                candidate[i] = f ? SolveCellType.Mine : candidate[i];
            });
            if (!canOpen.Any(o => o))
                return false;
            canOpen.ForEach((o, i) =>
            {
                if (o)
                {
                    Open(i);
                }
            });
            return true;
        }

        private void UpdateCanOpenOrFlag(SolveCellType[] candidate, bool[] canOpen, bool[] canFlag)
        {
            var min = candidate.Count(c => c == SolveCellType.Mine);
            var max = candidate.Count(c => c == SolveCellType.Mine || c == SolveCellType.Uncertain || c == SolveCellType.Undef);
            if (min > MineCount || max < MineCount)
                return;

            var betterCandidate = candidate;

            if (min == MineCount)
                betterCandidate = candidate.Select(c => (c == SolveCellType.Uncertain || c == SolveCellType.Undef) ? SolveCellType.Empty : c).ToArray();

            if (max == MineCount)
                betterCandidate = candidate.Select(c => (c == SolveCellType.Uncertain || c == SolveCellType.Undef) ? SolveCellType.Mine : c).ToArray();

            betterCandidate.ForEach((t, i) =>
            {
                if (t != SolveCellType.Empty)
                    canOpen[i] = false;
                if (t != SolveCellType.Mine)
                    canFlag[i] = false;
            });
        }

        private bool SolveTrivial()
        {
            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                if (candidate[i] == SolveCellType.Uncertain)
                    candidate[i] = SolveCellType.Undef;

                var ns = Neighbors(i);
                if(Cells[i].Open)
                {
                    candidate[i] = SolveCellType.Empty;
                    if (ns.Count(ni => !Cells[ni].Open) == Cells[i].SafeNeighborMineCount)
                    {
                        ns.ForEach(ni =>
                        {
                            if (!Cells[ni].Open)
                                candidate[ni] = SolveCellType.Mine;
                        });
                    }
                }
                else
                {
                    if (ns.All(ni => !Cells[ni].Open))
                    {
                        candidate[i] = SolveCellType.Uncertain;
                    }
                }
            }
            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                var ns = Neighbors(i);
                if (Cells[i].Open)
                {
                    if (ns.Count(ni => candidate[ni] == SolveCellType.Mine) == Cells[i].SafeNeighborMineCount)
                    {
                        ns.ForEach(ni =>
                        {
                            if (candidate[ni] == SolveCellType.Undef)
                                candidate[ni] = SolveCellType.Empty;
                        });
                    }
                }
            }
            var canOpen = new bool[Cells.Length];
            var canFlag = new bool[Cells.Length];
            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                canOpen[i] = !Cells[i].Open;
                canFlag[i] = true;
            }
            UpdateCanOpenOrFlag(candidate, canOpen, canFlag);
            return UpdateBoard(canOpen, canFlag);
        }

        private bool SolveComplex()
        {
            var canOpen = new bool[Cells.Length];
            var canFlag = new bool[Cells.Length];
            foreach (var i in Enumerable.Range(0, Cells.Length))
            {
                canOpen[i] = !Cells[i].Open;
                canFlag[i] = true;
            }
            SolveRec(0, candidate, canOpen, canFlag);
            return UpdateBoard(canOpen, canFlag);
        }

        private void SolveRec(int index, SolveCellType[] candidate, bool[] canOpen, bool[] canFlag)
        {
            if (index == Cells.Length)
            {
                UpdateCanOpenOrFlag(candidate, canOpen, canFlag);
                return;
            }


            if (candidate[index] != SolveCellType.Undef)
            {
                SolveRec(index + 1, candidate, canOpen, canFlag);
                return;
            }

            //可能性のある解
            var mine = true;
            var empty = true;
            foreach (var n in Neighbors(index))
            {
                if(!Cells[n].Open)
                    continue;

                var ms = Neighbors(n);
                if (ms.Count(i => candidate[i] == SolveCellType.Mine) >= Cells[n].SafeNeighborMineCount)
                    mine = false;
                if (ms.Count(i => candidate[i] == SolveCellType.Empty) >= ms.Count - Cells[n].SafeNeighborMineCount)
                    empty = false;

                if (!(mine || empty))
                    return;
            }


            if(mine)
            {
                candidate[index] = SolveCellType.Mine;
                SolveRec(index + 1, candidate, canOpen, canFlag);
            }
            if (empty)
            {
                candidate[index] = SolveCellType.Empty;
                SolveRec(index + 1, candidate, canOpen, canFlag);
            }
            candidate[index] = SolveCellType.Undef;
            return;
        }
    }
}
