using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using MineSweeper.Models;

namespace MineSweeper.ViewModels
{
    public class CellViewModel : ViewModel, IDisposable
    {
        private readonly int index;
        private Cell cell;
        public event EventHandler<int> OnOpen;
        public event EventHandler<bool> FlagChanged;
        public CellViewModel(Cell cell, int i)
        {
            this.cell = cell;
            cell.PropertyChanged += cell_PropertyChanged;
            State = cell.Open ? "Open" : "Hide";
            NeighborMineCount = cell.NeighborMineCount;
            this.index = i;
        }

        void cell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cell = (Cell)sender;

            switch (e.PropertyName)
            {
                case "Open":
                    if (cell.Open && cell.Flag)
                        ToggleFlag();
                    State = (cell.Open) ? "Open" : "Hide";
                    break;
                case "NeighborMineCount":
                    NeighborMineCount = cell.NeighborMineCount;
                    break;
                case "Flag":
                    State = cell.Flag ? "Flag" : "Hide";
                    if (FlagChanged != null)
                        FlagChanged(this, cell.Flag);
                    break;
            }
        }

        public void ToggleFlag()
        {
            if(!cell.Open)
                cell.Flag = !cell.Flag;
        }

        public void Open()
        {
            if (OnOpen != null)
                OnOpen(this, index);
        }


        #region State変更通知プロパティ
        private string _State;

        public string State
        {
            get
            { return _State; }
            set
            { 
                if (_State == value)
                    return;
                _State = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region NeighborMineCount変更通知プロパティ
        private int _NeighborMineCount;

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

        #region ShowMine変更通知プロパティ
        private bool _ShowMine;

        public bool ShowMine
        {
            get
            { return _ShowMine; }
            set
            { 
                if (_ShowMine == value)
                    return;
                _ShowMine = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public void Show()
        {
            ShowMine = cell.Type == CellType.Mine;
        }

        public void UnShow()
        {
            ShowMine = false;
        }


        public void Initialize()
        {
        }

        public new void Dispose()
        {
            base.Dispose();
            if (cell != null)
            {
                cell.PropertyChanged -= cell_PropertyChanged;
                cell = null;
            }
        }
    }
}
