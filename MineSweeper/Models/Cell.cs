using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace MineSweeper.Models
{
    public enum CellType
    {
        Mine, Empty, Undef
    }

    public enum SolveCellType
    {
        Mine, Empty, Undef, Uncertain
    }

    public enum AnsCellType
    {
        Flag, CanOpen, CanNotOpen
    }

    public class Cell : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */


        #region Open変更通知プロパティ
        private bool _Open = false;

        public bool Open
        {
            get
            { return _Open; }
            set
            { 
                if (_Open == value)
                    return;
                _Open = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region NeighborMineCount変更通知プロパティ
        private int _NeighborMineCount = 0;

        public int NeighborMineCount
        {
            get
            { return _NeighborMineCount; }
            set
            { 
                if (_NeighborMineCount == value)
                    return;
                _NeighborMineCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Type変更通知プロパティ
        private CellType _Type = CellType.Undef;

        public CellType Type
        {
            get
            { return _Type; }
            set
            { 
                if (_Type == value)
                    return;
                _Type = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Flag変更通知プロパティ
        private bool _Flag;

        public bool Flag
        {
            get
            { return _Flag; }
            set
            { 
                if (_Flag == value)
                    return;
                _Flag = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public int SafeNeighborMineCount
        {
            get {
                if (Open == false)
                    throw new InvalidOperationException();
                return NeighborMineCount;
            }
        }
    }
}
