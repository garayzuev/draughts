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
        Color draughtWhite = Color.FromRgb(200, 200, 200);
        Color draughtBlack = Color.FromRgb(100, 100, 100);
        Color kingWhite = Color.FromRgb(250, 250, 250);
        Color kingBlack = Color.FromRgb(0, 0, 0);
        Color stroke = Color.FromRgb(0, 0, 0);
        Color selected = Color.FromRgb(0, 0, 255);
        bool isSelected = false;
        bool isGameOver = false;
        Position selectedEllipsePosition;
        Table t;
        double BlockW, BlockH;
        public MainWindow()
        {
            InitializeComponent();
            this.UpdateLayout();
            this.LayoutUpdated += MainWindow_LayoutUpdated;
            initContextMenu();
            this.MinHeight = 200 + 35;
            this.MinWidth = 200 + 15;
            this.Title = "Draughts";
            newGame();
        }

        private void initContextMenu()
        {
            ContextMenu contextMenu1 = new ContextMenu();
            MenuItem item1 = new MenuItem();
            item1.DataContext = "New game";
            item1.Header = "New game";
            MenuItem item2 = new MenuItem();
            item2.DataContext = "Save game";
            item2.Header = "Save game";
            MenuItem item3 = new MenuItem();
            item3.DataContext = "Load game";
            item3.Header = "Load game";
            MenuItem item4 = new MenuItem();
            item4.DataContext = "Quit";
            item4.Header = "Quit";
            contextMenu1.Items.Add(item1);
            contextMenu1.Items.Add(item2);
            contextMenu1.Items.Add(item3);
            contextMenu1.Items.Add(item4);

            this.ContextMenu = contextMenu1;
            item1.Click += item1_Click;
            item2.Click += item2_Click;
            item3.Click += item3_Click;
            item4.Click += item4_Click;
        }

        void item3_Click(object sender, RoutedEventArgs e)
        {
            System.IO.StreamReader fileR;
            try { fileR = new System.IO.StreamReader("hello.txt"); }
            catch (System.IO.FileNotFoundException) { errorMessage(); return; }
            catch (System.IO.DirectoryNotFoundException) { errorMessage(); return; }
            List<Draught> list = new List<Draught>();
            while (!fileR.EndOfStream)
            {
                String s = fileR.ReadLine();
                bool isKing;
                bool isBlack;
                Position pos;
                if (s.Contains("true")) isKing = true;
                else if (s.Contains("false")) isKing = false;
                else { errorMessage(); fileR.Close(); return; }
                if (s.Contains("black")) isBlack = true;
                else if (s.Contains("white")) isBlack = false;
                else { errorMessage(); fileR.Close(); return; }

                int st = s.LastIndexOf("(");
                int end = s.LastIndexOf(")");
                if (end - st != 4 || s[st + 2] != ',') { errorMessage(); fileR.Close(); return; }
                int x, y;
                try
                {
                    x = int.Parse(s[st + 1].ToString());
                    y = int.Parse(s[st + 3].ToString());
                    if (x > 7 || y > 7) { errorMessage(); fileR.Close(); return; }
                }
                catch (System.ArgumentNullException) { errorMessage(); fileR.Close(); return; }
                catch (System.FormatException) { errorMessage(); fileR.Close(); return; }
                catch (System.OverflowException) { errorMessage(); fileR.Close(); return; }
                pos = new Position(x, y);
                if (Arbiter.findDraught(pos, list) != null) { errorMessage(); fileR.Close(); return; }
                list.Add(new Draught(isBlack, isKing, pos));
            }
            fileR.Close();
            clear();
            isSelected = false;
            initChessmate(blockDark, blockWhite);
            setEllipseFromFile(list, draughtWhite, draughtBlack, kingWhite, kingBlack);
            t = new Table(this, list);
        }

        private void setEllipseFromFile(List<Draught> list, Color white, Color black, Color whiteKing, Color blackKing)
        {
            foreach (Draught d in list)
            {
                Ellipse e = new Ellipse()
                {
                    Fill = new SolidColorBrush(d.isKing() ? (d.isBlack() ? blackKing : whiteKing) : (d.isBlack() ? black : white)),
                    Height = BlockH - 20,
                    Width = BlockW - 20,
                    Stroke = new SolidColorBrush(stroke),
                    StrokeThickness = 5
                };
                e.SetValue(Canvas.LeftProperty, BlockW * (d.getPosition().x) + 10);
                e.SetValue(Canvas.TopProperty, BlockH * d.getPosition().y + 10);
                e.MouseDown += ellipse_MouseDown;
                Canv.Children.Add(e);
            }
        }
        void item2_Click(object sender, RoutedEventArgs e)
        {

            System.IO.StreamWriter fileRw = new System.IO.StreamWriter("hello.txt");
            foreach (Draught d in t.getDraughts())
            {
                fileRw.Write(d.ToString());
            }
            fileRw.Close();
        }

        void item1_Click(object sender, RoutedEventArgs e)
        {
            newGame();
        }

        void item4_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }




        void newGame()
        {
            clear();
            repaint();
            isSelected = false;
            initChessmate(blockDark, blockWhite);
            initDraughts(draughtBlack, draughtWhite);
            t = new Table(this);
            repaint();
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
                    e.MouseDown += ellipse_MouseDown;
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
                    e.MouseDown += ellipse_MouseDown;
                    Canv.Children.Add(e);

                }
        }
        void ellipse_MouseDown(object sender, MouseButtonEventArgs e)
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
                    Ellipse selectedEllipse = findSelectedEllipse();

                    selectedEllipse.Stroke = new SolidColorBrush(stroke);
                    if (!t.endOfTurn())
                    {
                        el = selectedEllipse;
                        isSelected = false;
                        ellipse_MouseDown(el, e);
                        return;
                    }
                    else
                    {
                        if (t.notMoves(t.isTurnBlack()))
                            notMovesMessage();

                        if (isGameOver)
                            repaint();
                        if(!isGameOver)
                            t.turnAI();
                        if (t.notMoves(!t.isTurnBlack()))
                            notMovesMessage();
                        if (isGameOver)
                            repaint();
                    }
                    isSelected = false;
                }
            }
            if (isGameOver) isGameOver = false;
        }
        public void errorMessage()
        {
            MessageBox.Show("ERROR!!!");
        }
        public void notMovesMessage()
        {
            MessageBox.Show(t.isTurnBlack() ? "Black can't move" : "White can't move");
            newGame();
            isGameOver = true;
        }
        public void victoryMessage()
        {
            MessageBox.Show(t.isTurnBlack() ? "Black victory!" : "White victory!");
            newGame();
            isGameOver = true;
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

        private Ellipse findSelectedEllipse()
        {
            foreach (UIElement elem in Canv.Children)
                if (elem is Ellipse && (elem as Ellipse).Stroke.ToString() != stroke.ToString())
                    return ((elem as Ellipse));
            return null;
        }
        private Ellipse findEllipseByPos(int x, int y)
        {
            foreach (UIElement elem in Canv.Children)
            {
                if (elem is Ellipse)
                {
                    Position pos = getPositionEllipse(Double.Parse((elem as Ellipse).GetValue(Canvas.LeftProperty).ToString()), Double.Parse((elem as Ellipse).GetValue(Canvas.TopProperty).ToString()));
                    if (x == pos.x && y == pos.y)
                    {
                        return (elem as Ellipse);
                    }
                }
            }
            return null;
        }
        public void setKing(int x, int y)
        {
            Ellipse elem = findEllipseByPos(x, y);
            if (elem != null)
            {
                if (elem.Fill.ToString() == draughtBlack.ToString())
                    elem.Fill = new SolidColorBrush(kingBlack);
                if (elem.Fill.ToString() == draughtWhite.ToString())
                    elem.Fill = new SolidColorBrush(kingWhite);
            }
        }

        public void removeEllipse(int x, int y)
        {
            Ellipse elem = findEllipseByPos(x, y);
            if (elem != null)
                Canv.Children.Remove(elem);
        }
        public void turnOn(int from_x, int from_y, int to_x, int to_y)
        {
            Ellipse elem = findEllipseByPos(from_x, from_y);
            if (elem != null)
            {
                elem.SetValue(Canvas.LeftProperty, BlockW * to_x + 10);
                elem.SetValue(Canvas.TopProperty, BlockH * to_y + 10);
            }
        }
    }
}
