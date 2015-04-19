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

namespace draughts
{
    struct Position
    {
        public int x;
        public int y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public partial class MainWindow : Window
    {
        Color blockDark = Color.FromRgb(130, 81, 0);
        Color blockWhite = Color.FromRgb(254, 205, 114);
        Color draughtWhite = Color.FromRgb(250, 250, 250);
        Color draughtBlack = Color.FromRgb(100, 100, 100);
        Color kingWhite = Color.FromRgb(200, 200, 200);
        Color kingBlack = Color.FromRgb(0, 0, 0);
        Color stroke = Color.FromRgb(0, 0, 0);
        Color selected = Color.FromRgb(0, 0, 255);
        bool isSelected = false;
        bool isWin = false;
        Position selectedEllipsePosition;
        Table t;

        double BlockW, BlockH;
        public MainWindow()
        {
            InitializeComponent();
            this.UpdateLayout();
            initChessmate(blockDark, blockWhite);
            initDraughts(draughtBlack, draughtWhite);
            this.LayoutUpdated += MainWindow_LayoutUpdated;
            t = new Table(this);

        }
        private void clear()
        {
            Canv.Children.Clear();
        }
        private void initChessmate(Color black, Color white)
        {
            double k;
            k = this.Width - 15;
            BlockW = (k == 0 || double.IsNaN(k)) ? 50 : k / 8;
            k = this.Height - 35;
            BlockH = (k == 0 || double.IsNaN(k)) ? 50 : k / 8;
            for (int i = 0, shift; i < 8; i++)
            {
                shift = i % 2 == 0 ? 0 : 1;
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Fill = new SolidColorBrush(j % 2 == shift ? white : black),
                        Width = BlockW,
                        Height = BlockH
                    };
                    rect.SetValue(Canvas.LeftProperty, j * BlockW);
                    rect.SetValue(Canvas.TopProperty, i * BlockH);
                    rect.MouseDown += rectWhite_MouseDown;
                    Canv.Children.Add(rect);
                }
            }
        }

        private void initDraughts(Color black, Color white)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j += 2)
                {

                    Ellipse e = new Ellipse()
                    {
                        Fill = new SolidColorBrush(white),
                        Height = BlockH - 20,
                        Width = BlockW - 20,
                        Stroke = new SolidColorBrush(stroke),
                        StrokeThickness = 5
                    };
                    e.SetValue(Canvas.LeftProperty, BlockW * (j + (i % 2 == 0 ? 1 : 0)) + 10);
                    e.SetValue(Canvas.TopProperty, BlockH * i + 10);
                    e.MouseDown += e_MouseDown;
                    Canv.Children.Add(e);

                }
            for (int i = 7; i > 4; i--)
                for (int j = 0; j < 8; j += 2)
                {

                    Ellipse e = new Ellipse()
                    {
                        Fill = new SolidColorBrush(black),
                        Height = BlockH - 20,
                        Width = BlockW - 20,
                        Stroke = new SolidColorBrush(stroke),
                        StrokeThickness = 5
                    };
                    e.SetValue(Canvas.LeftProperty, BlockW * (j + (i % 2 == 0 ? 1 : 0)) + 10);
                    e.SetValue(Canvas.TopProperty, BlockH * i + 10);
                    e.MouseDown += e_MouseDown;
                    Canv.Children.Add(e);

                }
        }
        void e_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Ellipse).Stroke.ToString() == stroke.ToString() && ((sender as Ellipse).Fill.ToString() == (t.isTurnBlack() ? draughtBlack.ToString() : draughtWhite.ToString()) || (sender as Ellipse).Fill.ToString() == (t.isTurnBlack() ? kingBlack.ToString() : kingWhite.ToString())))
            {
                if (!isSelected)
                {
                    (sender as Ellipse).Stroke = new SolidColorBrush(selected);
                    selectedEllipsePosition = getPositionEllipse(Double.Parse((sender as Ellipse).GetValue(Canvas.LeftProperty).ToString()), Double.Parse((sender as Ellipse).GetValue(Canvas.TopProperty).ToString()));
                    isSelected = true;
                    t.startOfTurn(selectedEllipsePosition);
                }
            }
            else
            {
                if (!t.isContinueAttack())
                {
                    Position p = getPositionEllipse(Double.Parse((sender as Ellipse).GetValue(Canvas.LeftProperty).ToString()), Double.Parse((sender as Ellipse).GetValue(Canvas.TopProperty).ToString()));
                    if (isSelected && selectedEllipsePosition.x == p.x && selectedEllipsePosition.y == p.y)
                    {
                        (sender as Ellipse).Stroke = new SolidColorBrush(stroke);
                        isSelected = false;
                    }

                }
            }
        }

        void MainWindow_LayoutUpdated(object sender, EventArgs e)
        {
            this.Width = this.Height;
            repaint();
        }

        private void rectWhite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Rectangle).Fill.ToString() == blockDark.ToString() && isSelected)
            {
                Position pos = getPositionRectangle(Double.Parse((sender as Rectangle).GetValue(Canvas.LeftProperty).ToString()), Double.Parse((sender as Rectangle).GetValue(Canvas.TopProperty).ToString()));
                if (t.wantMove(pos))
                {
                    t.moveTo(pos);
                    object el = null;
                    foreach (UIElement elem in Canv.Children)
                    {
                        if (elem is Ellipse && (elem as Ellipse).Stroke.ToString() != stroke.ToString())
                        {
                            (elem as Ellipse).Stroke = new SolidColorBrush(stroke);
                            if (!t.endOfTurn())
                            {                                
                                el = elem;
                                isSelected = false;
                                e_MouseDown(el, e);
                                return;
                            }
                            else
                            {
                                if (t.notMoves())
                                {
                                    notMovesMessage();
                                    break;
                                }
                                if (isWin)
                                    break;
                            }
                        }
                    }
                    isSelected = false;
                }
            }
            if (isWin) isWin = false;
        }

        public void notMovesMessage()
        {
            MessageBox.Show(t.isTurnBlack()?"Black can't move":"White can't move");
            clear();
            repaint();
            isSelected = false;
            initChessmate(blockDark, blockWhite);
            initDraughts(draughtBlack, draughtWhite);
            t = new Table(this);
            repaint();
            isWin = true;
        }
        public void victoryMessage()
        {
            MessageBox.Show(t.isTurnBlack() ? "Black victory!" : "White victory!");
            clear();
            repaint();
            isSelected = false;
            initChessmate(blockDark, blockWhite);
            initDraughts(draughtBlack, draughtWhite);
            t = new Table(this);
            repaint();
            isWin = true;
        }
        private void repaint()
        {
            int z = 0;
            int k = 0;
            double oldBlockH = BlockH, oldBlockW = BlockW;
            BlockH = (this.Height - 35) / 8;
            BlockW = (this.Width - 15) / 8;
            if (double.IsNaN(BlockH) || double.IsNaN(BlockW) || oldBlockW == BlockW && oldBlockH == BlockH)
                return;
            foreach (UIElement elem in Canv.Children)
            {
                if (elem is Rectangle)
                {

                    (elem as Rectangle).Height = BlockH;
                    (elem as Rectangle).Width = BlockW;
                    (elem as Rectangle).SetValue(Canvas.LeftProperty, (k++) * BlockW);
                    (elem as Rectangle).SetValue(Canvas.TopProperty, z * BlockH);
                    if (k == 8)
                    {
                        k = 0;
                        z++;
                    }
                }
                if (elem is Ellipse)
                {
                    int y, x;
                    y = (int)((Double.Parse((elem as Ellipse).GetValue(Canvas.TopProperty).ToString()) - 10) / ((elem as Ellipse).Height + 20));
                    x = (int)((Double.Parse((elem as Ellipse).GetValue(Canvas.LeftProperty).ToString()) - 10) / ((elem as Ellipse).Width + 20));
                    (elem as Ellipse).Height = BlockH - 20;
                    (elem as Ellipse).Width = BlockW - 20;
                    (elem as Ellipse).SetValue(Canvas.LeftProperty, BlockW * x + 10);
                    (elem as Ellipse).SetValue(Canvas.TopProperty, BlockH * y + 10);
                }

            }
        }
        private Position getPositionEllipse(double x, double y)
        {
            Position k = new Position();
            k.y = (int)((y - 10) / BlockH);
            k.x = (int)((x - 10) / BlockW);
            return k;

        }
        private Position getPositionRectangle(double x, double y)
        {
            Position k = new Position();
            k.y = (int)(y / BlockH);
            k.x = (int)(x / BlockW);
            return k;
        }

        public void setKing()
        {
            foreach (UIElement elem in Canv.Children)
            {
                if (elem is Ellipse && (elem as Ellipse).Stroke.ToString() != stroke.ToString()) {
                    if ((elem as Ellipse).Fill.ToString() == draughtBlack.ToString())
                        (elem as Ellipse).Fill = new SolidColorBrush(kingBlack);
                    if ((elem as Ellipse).Fill.ToString() == draughtWhite.ToString())
                        (elem as Ellipse).Fill = new SolidColorBrush(kingWhite);
                }

            }
        }

        public void removeEllipse(int x, int y)
        {
            int yy, xx;
            foreach (UIElement elem in Canv.Children)
            {
                if (elem is Ellipse)
                {
                    yy = (int)((Double.Parse((elem as Ellipse).GetValue(Canvas.TopProperty).ToString()) - 10) / ((elem as Ellipse).Height + 20));
                    xx = (int)((Double.Parse((elem as Ellipse).GetValue(Canvas.LeftProperty).ToString()) - 10) / ((elem as Ellipse).Width + 20));
                    if (x == xx && y == yy)
                    {
                        Canv.Children.Remove(elem);
                        return;
                    }
                }
            }
        }
        public void turnOn(int x, int y)
        {
            foreach (UIElement elem in Canv.Children)
            {
                if (elem is Ellipse && (elem as Ellipse).Stroke.ToString() != stroke.ToString())
                {
                    (elem as Ellipse).SetValue(Canvas.LeftProperty, BlockW * x + 10);
                    (elem as Ellipse).SetValue(Canvas.TopProperty, BlockH * y + 10);
                }

            }
        }
    }
}
