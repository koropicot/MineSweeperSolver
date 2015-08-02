using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MineSweeper.Views
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Cell"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Cell;assembly=Cell"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class Cell : Control
    {

        #region NeighborMineCount
        public int NeighborMineCount
        {
            get { return (int)GetValue(NeighborMineCountProperty); }
            set { SetValue(NeighborMineCountProperty, value); }
        }

        public static readonly DependencyProperty NeighborMineCountProperty =
            DependencyProperty.Register("NeighborMineCount", typeof(int), typeof(Cell), new PropertyMetadata(0));
        #endregion

        #region State
        public string State
        {
            get { return (string)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(string), typeof(Cell), new PropertyMetadata("Hide"));
        #endregion

        #region ShowMine
        public bool ShowMine
        {
            get { return (bool)GetValue(ShowMineProperty); }
            set { SetValue(ShowMineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMineProperty =
            DependencyProperty.Register("ShowMine", typeof(bool), typeof(Cell), new PropertyMetadata(false));
        #endregion

        static Cell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Cell), new FrameworkPropertyMetadata(typeof(Cell)));
        }

        public event EventHandler OnOpen;


        #region OpenCommand
        private ViewModelCommand _OpenCommand;

        public ViewModelCommand OpenCommand
        {
            get
            {
                if (_OpenCommand == null)
                {
                    _OpenCommand = new ViewModelCommand(Open);
                }
                return _OpenCommand;
            }
        }

        public void Open()
        {
            if (OnOpen != null)
                OnOpen(this, new EventArgs());
        }
        #endregion

    }
}
