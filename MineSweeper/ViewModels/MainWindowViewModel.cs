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
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        const int CellSize = 18;

        #region Rows変更通知プロパティ
        private int _Rows;

        public int Rows
        {
            get
            { return _Rows; }
            set
            { 
                if (_Rows == value)
                    return;
                _Rows = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Columns変更通知プロパティ
        private int _Columns;

        public int Columns
        {
            get
            { return _Columns; }
            set
            { 
                if (_Columns == value)
                    return;
                _Columns = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region BoardRows変更通知プロパティ
        private int _BoardRows;

        public int BoardRows
        {
            get
            { return _BoardRows; }
            set
            { 
                if (_BoardRows == value)
                    return;
                _BoardRows = value;
                BoardHeight = value * CellSize;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region BoardColumns変更通知プロパティ
        private int _BoardColumns;

        public int BoardColumns
        {
            get
            { return _BoardColumns; }
            set
            { 
                if (_BoardColumns == value)
                    return;
                _BoardColumns = value;
                BoardWidth = value * CellSize;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region BoardWidth変更通知プロパティ
        private int _BoardWidth;

        public int BoardWidth
        {
            get
            { return _BoardWidth; }
            set
            { 
                if (_BoardWidth == value)
                    return;
                _BoardWidth = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region BoardHeight変更通知プロパティ
        private int _BoardHeight;

        public int BoardHeight
        {
            get
            { return _BoardHeight; }
            set
            { 
                if (_BoardHeight == value)
                    return;
                _BoardHeight = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region MineCount変更通知プロパティ
        private int _MineCount;

        public int MineCount
        {
            get
            { return _MineCount; }
            set
            { 
                if (_MineCount == value)
                    return;
                _MineCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Cells変更通知プロパティ
        private List<CellViewModel> _Cells;

        public List<CellViewModel> Cells
        {
            get
            { return _Cells; }
            set
            { 
                if (_Cells == value)
                    return;
                _Cells = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region RemainMineCount変更通知プロパティ
        private int _RemainMineCount;

        public int RemainMineCount
        {
            get
            { return _RemainMineCount; }
            set
            { 
                if (_RemainMineCount == value)
                    return;
                _RemainMineCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region UseComplex変更通知プロパティ
        private bool _UseComplex = true;

        public bool UseComplex
        {
            get
            { return _UseComplex; }
            set
            { 
                if (_UseComplex == value)
                    return;
                _UseComplex = value;
                mineSweeper.UseComplex = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        private MineSweeperModel mineSweeper;

        public MainWindowViewModel() : base()
        {
            Rows = 16;
            Columns = 16;
            MineCount = 40;
            Reset();
        }

        public void Initialize()
        {
        }

        public void Reset()
        {
            mineSweeper = new MineSweeperModel(Columns, Rows, MineCount);
            RemainMineCount = MineCount;
            if (Cells != null)
                foreach (var c in Cells)
                    c.Dispose();
           Cells = mineSweeper.Cells.Select((c, i) => {
                var cvm = new CellViewModel(c, i);
                cvm.OnOpen += (_, index) => mineSweeper.Open(index);
                cvm.FlagChanged += (_, dec) =>
                {
                    RemainMineCount = dec ? RemainMineCount - 1 : RemainMineCount + 1;
                };
                return cvm;
            }).ToList();
            mineSweeper.GameOver += (s, clear) => {
                ShowMine();
                if(clear)
                    Messenger.Raise(new InformationMessage("Game Clear!", "", "Info"));
                else
                    Messenger.Raise(new InformationMessage("Miss!", "", "Info"));
            };
            BoardRows = Rows;
            BoardColumns = Columns;
        }

        public void ShowMine()
        {
            foreach (var c in Cells)
                c.Show();
        }

        public void SolveOneStep()
        {
            var res = mineSweeper.SolveOneStep();
            if(!res)
                Messenger.Raise(new InformationMessage("Fail to solve.", "", "Info"));
        }

        public void Solve()
        {
            var res = mineSweeper.Solve();
            if (!res)
                Messenger.Raise(new InformationMessage("Fail to solve.", "", "Info"));
        }

        public void Retry()
        {
            foreach (var c in Cells)
                c.UnShow();
            mineSweeper.Retry();
        }
    }
}
