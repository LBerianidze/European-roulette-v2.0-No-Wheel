using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Color = System.Drawing.Color;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridCell = System.Windows.Controls.DataGridCell;
using MessageBox = System.Windows.MessageBox;
using ScrollEventArgs = System.Windows.Forms.ScrollEventArgs;
using Size = System.Drawing.Size;

namespace European_roulette_v2._0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataGridView droppedInSixLinesGridView;
        private UserControl1 favouritessGridView;

        private List<int> spinNumbers = new List<int>();
        private int[] part1Sector1 = { 35, 3, 26, 0, 32, 15, 19 };
        private int[] part1Sector2 = { 4, 21, 2, 25, 17, 34 };
        private int[] part1Sector3 = { 6, 27, 13, 36, 11, 30 };
        private int[] part1Sector4 = { 8, 23, 10, 5, 24, 16 };
        private int[] part1Sector5 = { 33, 1, 20, 14, 31, 9 };
        private int[] part1Sector6 = { 22, 18, 29, 7, 28, 12 };

        private List<int> part1SixLines = new List<int>();
        private ObservableCollection<object[]> firstSummaryDataGridItems = new ObservableCollection<object[]>();
        private ObservableCollection<object[]> favsSummaryDataGridItems = new ObservableCollection<object[]>();

        private bool allowMoveBetFirst = false;
        private int betSelectedIndexFirst = -1;

        private bool allowMoveBetSecond = false;
        private int betSelectedIndexSecond = -1;

        private int secondsTillStart = 0;
        private Timer stopWatch = new Timer();
        private bool isNumbersListSaved = false;
        private void CreateDroppedSixLineGridView()
        {
            this.droppedInSixLinesGridView = new DataGridView
            {
                ColumnHeadersVisible = false,
                RowHeadersVisible = false,
                ScrollBars = ScrollBars.None,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.Gray,
                Size = new Size(1040, 50),
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = false

            };
            Type dgvType = this.droppedInSixLinesGridView.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.droppedInSixLinesGridView, true, null);

            this.droppedInSixLinesGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.droppedInSixLinesGridView.Font = new Font("Arial", 11);
            this.droppedSixLinesGridContainer.Child = this.droppedInSixLinesGridView;
        }
        private void CreateFavouritesGridView()
        {
            this.favouritessGridView = new UserControl1
            {
                ColumnHeadersVisible = false,
                RowHeadersVisible = false,
                ScrollBars = ScrollBars.Horizontal,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.Gray,
                Size = new Size(1040, 50),
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = false
            };
            //Type dgvType = this.favouritessGridView.GetType();
            //PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            //    BindingFlags.Instance | BindingFlags.NonPublic);
            //pi.SetValue(this.favouritessGridView, true, null);

            this.favouritessGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.favouritessGridView.Font = new Font("Arial", 11);
            this.favouritessGridView.UpdateHorizontalWidth();
            this.favouritessGridView.Scroll += this.Part1BonusesGridViewScroll;
            this.favouritesGridContainer.Child = this.favouritessGridView;

        }
        private void FillBetGrids()
        {
            ObservableCollection<object[]> betGridCells = new ObservableCollection<object[]>
                {
                    new object[100],
                    new object[100]
                };
            for (int i = 0; i < 100; i++)
            {
                DataGridTextColumn dataGridTextColumn = new DataGridTextColumn();
                this.betGrid.Columns.Add(dataGridTextColumn);
                Binding binding = new Binding($"[{i}]");
                dataGridTextColumn.Binding = binding;
            }
            this.betGrid.ItemsSource = betGridCells;

            ObservableCollection<object[]> betGrid2Cells = new ObservableCollection<object[]>
                {
                    new object[100],
                    new object[100]
                };
            for (int i = 0; i < 100; i++)
            {
                DataGridTextColumn dataGridTextColumn = new DataGridTextColumn();
                this.betGrid2.Columns.Add(dataGridTextColumn);
                Binding binding = new Binding($"[{i}]");
                dataGridTextColumn.Binding = binding;
            }
            this.betGrid2.ItemsSource = betGridCells;
        }
        private void FillSummaryGrids()
        {
            for (int i = 0; i < 3; i++)
            {
                this.firstSummaryDataGridItems.Add(new object[5]);
            }
            this.part1SummaryDataGrid.ItemsSource = this.firstSummaryDataGridItems;
            for (int i = 0; i < 3; i++)
            {
                this.favsSummaryDataGridItems.Add(new object[3]);
            }
            this.favsSummaryDataGrid.ItemsSource = this.favsSummaryDataGridItems;
        }
        public MainWindow()
        {
            this.InitializeComponent();
            CreateDroppedSixLineGridView();
            CreateFavouritesGridView();
            FillBetGrids();
            FillSummaryGrids();
            this.stopWatch.Tick += this.T_Tick;
            this.stopWatch.Interval = 1000;
            this.stopWatch.Start();

        }
        private void T_Tick(object sender, EventArgs e)
        {
            this.secondsTillStart++;
            TimeSpan timeSpan = TimeSpan.FromSeconds(this.secondsTillStart);
            string sttimer = timeSpan.ToString(@"hh\:mm\:ss");
            System.Windows.Threading.Dispatcher dispatcher = this.Dispatcher;
            dispatcher?.BeginInvoke(new Action(() => { this.start_timer.Text = sttimer; }));
        }
        private void Part1BonusesGridViewScroll(object sender, ScrollEventArgs e)
        {
            this.droppedInSixLinesGridView.HorizontalScrollingOffset = this.favouritessGridView.HorizontalScrollingOffset;
        }
        private void UpdateTable()
        {
            this.droppedInSixLinesGridView.Rows.Clear();
            this.droppedInSixLinesGridView.Columns.Clear();
            this.part1SixLines.Clear();
            int divided = (this.spinNumbers.Count) % 6;
            int columnsCount = divided == 0 ? this.spinNumbers.Count / 6 : (this.spinNumbers.Count / 6) + 1;
            for (int i = 0, j = columnsCount; i < columnsCount; i++, j++)
            {
                DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn { HeaderText = "Column" + i, Name = "Column" + i, Width = 25 };
                DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn { HeaderText = "Column" + j, Name = "Column" + j, Width = 25 };
                this.droppedInSixLinesGridView.Columns.Add(column1);
            }
            if (this.spinNumbers.Count != 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    this.droppedInSixLinesGridView.Rows.Add("");
                }
            }
            for (int i = 0, rowCounter = 0, cellCounter = 0; i <= this.spinNumbers.ToArray().GetUpperBound(0); i++)
            {
                int sixLine = this.SixLine(this.spinNumbers[i]);
                this.part1SixLines.Add(sixLine);
                if (sixLine != 0)
                {
                    this.droppedInSixLinesGridView.Rows[sixLine - 1].Cells[cellCounter].Value = sixLine;
                }
                rowCounter++;
                if (rowCounter > 5)
                {
                    rowCounter = 0;
                    cellCounter++;
                }
            }
            if (this.droppedInSixLinesGridView.ColumnCount != 0)
            {
                this.droppedInSixLinesGridView.Rows[0].Cells[0].Selected = false;
                this.droppedInSixLinesGridView.Rows[(this.spinNumbers.Count - ((columnsCount - 1) * 6)) - 1].Cells[columnsCount - 1].Selected = true;

                this.droppedInSixLinesGridView.FirstDisplayedScrollingColumnIndex = this.droppedInSixLinesGridView.ColumnCount - 1;
            }
        }
        private int SixLine(int v)
        {
            if (this.part1Sector1.Contains(v))
            {
                return 1;
            }
            if (this.part1Sector2.Contains(v))
            {
                return 2;
            }
            if (this.part1Sector3.Contains(v))
            {
                return 3;
            }
            if (this.part1Sector4.Contains(v))
            {
                return 4;
            }
            if (this.part1Sector5.Contains(v))
            {
                return 5;
            }
            if (this.part1Sector6.Contains(v))
            {
                return 6;
            }
            return 0;
        }
        private List<T> Reverse<T>(List<T> source)
        {
            return source.ToArray().Reverse().ToList();
        }
        bool cancelled = true;
        System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(255, 255, 255);
        private void DoActions()
        {
            for (int i = 0; i <= 36; i++)
            {
                Button found = this.FindName("id" + i) as Button;
                found.Background = new SolidColorBrush(color); ;
            }
            if (this.spinNumbers.Count != 0)
            {
                Button button = this.FindName("id" + this.spinNumbers.Last()) as Button;

                button.SetValue(BackgroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(20, 255, 0, 0)));
            }
            this.isNumbersListSaved = false;
            this.numberTextBox.Text = (string.Join(", ", this.Reverse(this.spinNumbers)));
            this.sum.Text = this.spinNumbers.Count.ToString();
            this.UpdateTable();
            this.UpdateSummaryGrid();
            this.CalculateSuccessRate();
            this.FillFavourites();

            if (this.allowMoveBetFirst && !cancelled)
            {
                this.betGrid.UnselectAllCells();
                this.betGrid.GetCell(this.betGrid.GetRow(1), ++this.betSelectedIndexFirst).IsSelected = true;
                this.betGrid.ScrollIntoView(this.betGrid.Items[1], this.betGrid.Columns[this.betSelectedIndexFirst]);
            }
            if (this.allowMoveBetSecond && !cancelled)
            {
                this.betGrid2.UnselectAllCells();
                this.betGrid2.GetCell(this.betGrid2.GetRow(1), ++this.betSelectedIndexSecond).IsSelected = true;
                this.betGrid2.ScrollIntoView(this.betGrid2.Items[1], this.betGrid2.Columns[this.betSelectedIndexSecond]);
            }
            cancelled = false;
        }
        private int[] GetLastSelaectedBets(List<int> _part1SixLines, int amount)
        {
            if (_part1SixLines.Count >= amount)
            {
                int[] values = new int[amount];
                for (int i = 6; i <= amount; i += 6)
                {
                    values[i - 6] = _part1SixLines.Take(i).Count(t => t == 1);
                    values[i - 5] = _part1SixLines.Take(i).Count(t => t == 2);
                    values[i - 4] = _part1SixLines.Take(i).Count(t => t == 3);
                    values[i - 3] = _part1SixLines.Take(i).Count(t => t == 4);
                    values[i - 2] = _part1SixLines.Take(i).Count(t => t == 5);
                    values[i - 1] = _part1SixLines.Take(i).Count(t => t == 6);
                }
                int[] sum = new int[6];
                for (int i = 0; i < amount; i++)
                {
                    sum[i % 6] += values[i];
                }
                var sorted = sum.Select(((i, i1) => new { index = i1, value = i })).OrderByDescending(t => t.value).ToList();
                return new int[6] { sorted[0].index + 1, sorted[1].index + 1, sorted[2].index + 1, sorted[3].index + 1, sorted[4].index + 1, sorted[5].index + 1 };
            }
            return new int[0];
        }
        private int[] GetLastSelaectedFavs(List<int> _part1SixLines, int amount)
        {
            if (_part1SixLines.Count >= amount)
            {
                int[] values = new int[amount];
                for (int i = 5; i <= amount; i += 5)
                {
                    values[i - 5] = _part1SixLines.Take(i).Count(t => t == 12);
                    values[i - 4] = _part1SixLines.Take(i).Count(t => t == 18);
                    values[i - 3] = _part1SixLines.Take(i).Count(t => t == 24);
                    values[i - 2] = _part1SixLines.Take(i).Count(t => t == 30);
                    values[i - 1] = _part1SixLines.Take(i).Count(t => t == 36);
                }
                int[] sum = new int[5];
                for (int i = 0; i < amount; i++)
                {
                    sum[i % 5] += values[i];
                }
                var sorted = sum.Select(((i, i1) => new { index = i1, value = i })).OrderByDescending(t => t.value).ToList();
                return new int[5] { (sorted[0].index + 2) * 6, (sorted[1].index + 2) * 6, (sorted[2].index + 2) * 6, (sorted[3].index + 2) * 6, (sorted[4].index + 2) * 6 };
            }
            return new int[0];
        }
        List<int[]> favourites = new List<int[]>();
        List<int> favs1 = new List<int>();
        private void FillFavourites()
        {
            if (this.spinNumbers.Count < 13)
            {
                return;
            }
            favouritessGridView.SuspendLayout();
            this.favouritessGridView.Rows.Clear();
            this.favouritessGridView.Columns.Clear();
            int divided = (this.spinNumbers.Count) % 6;
            int columnsCount = divided == 0 ? this.spinNumbers.Count / 6 : (this.spinNumbers.Count / 6) + 1;
            for (int i = 0, j = columnsCount; i < columnsCount; i++, j++)
            {
                DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn { HeaderText = "Column" + j, Name = "Column" + j, Width = 25 };
                this.favouritessGridView.Columns.Add(column2);
            }
            if (this.spinNumbers.Count != 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    this.favouritessGridView.Rows.Add("");
                }
            }

            List<int> favs = new List<int>();

            for (int i = 0; i < this.spinNumbers.Count; i++)
            {
                if (this.spinNumbers.Count - i <= 12)
                {
                    break;
                }

                int[] l1 =new int[0]; if(this.part1SixLines.Count - (13 + i)>=0) l1=this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (13 + i)).Take(12).ToList(), 12);
                int[] l2 =new int[0]; if(this.part1SixLines.Count - (19 + i)>=0) l2=this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (19 + i)).Take(18).ToList(), 18);
                int[] l3 =new int[0]; if(this.part1SixLines.Count - (25 + i)>=0) l3=this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (25 + i)).Take(24).ToList(), 24);
                int[] l4 =new int[0]; if(this.part1SixLines.Count - (31 + i)>=0) l4=this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (31 + i)).Take(30).ToList(), 30);
                int[] l5 = new int[0];if(this.part1SixLines.Count - (37 + i)>=0) l5=this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (37 + i)).Take(36).ToList(), 36);

                int last = this.part1SixLines[this.part1SixLines.Count - (i + 1)];
                int columnIndex = (this.part1SixLines.Count - i) % 6 == 0 ? (this.part1SixLines.Count - i) / 6 - 1 : (this.part1SixLines.Count - i) / 6;
                if (l1[0] == last)
                {
                    this.favouritessGridView.Rows[0].Cells[columnIndex].Value = 12;
                    favs.Add(12);
                }
                if (l2.Length != 0 && l2[0] == last)
                {
                    this.favouritessGridView.Rows[1].Cells[columnIndex].Value = 18;
                    favs.Add(18);

                }
                if (l3.Length != 0 && l3[0] == last)
                {
                    this.favouritessGridView.Rows[2].Cells[columnIndex].Value = 24;
                    favs.Add(24);

                }
                if (l4.Length != 0 && l4[0] == last)
                {
                    this.favouritessGridView.Rows[3].Cells[columnIndex].Value = 30;
                    favs.Add(30);

                }
                if (l5.Length != 0 && l5[0] == last)
                {
                    this.favouritessGridView.Rows[4].Cells[columnIndex].Value = 36;
                    favs.Add(36);
                }
            }
            if (favs.Count > 20)
            {
                favs = favs.Take(20).ToList();
            }
            favs.Reverse();
            this.favsSummaryDataGridItems.Clear();
            this.favsSummaryDataGridItems.Add(new object[3]);
            this.favsSummaryDataGridItems.Add(new object[3]);
            this.favsSummaryDataGridItems.Add(new object[3]);

            if (favs.Count>=10)
            {
                int[] res = this.GetLastSelaectedFavs(favs.Skip(favs.Count-10).ToList(), 10);
                this.favsSummaryDataGridItems[0][0] = res[0];
                this.favsSummaryDataGridItems[1][0] = res[1];
                this.favsSummaryDataGridItems[2][0] = 10;

            }
            if (favs.Count >= 15)
            {
                int[] res1 = this.GetLastSelaectedFavs(favs.Skip(favs.Count-15).ToList(), 15);
                this.favsSummaryDataGridItems[0][1] = res1[0];
                this.favsSummaryDataGridItems[1][1] = res1[1];
                this.favsSummaryDataGridItems[2][1] = 15;
            }
            if (favs.Count >= 20)
            {
                int[] res2 = this.GetLastSelaectedFavs(favs.Take(20).ToList(), 20);
                this.favsSummaryDataGridItems[0][2] = res2[0];
                this.favsSummaryDataGridItems[1][2] = res2[1];
                this.favsSummaryDataGridItems[2][2] = 20;
            }

            CollectionViewSource.GetDefaultView(this.favsSummaryDataGridItems).Refresh();
            this.MainWindow_OnLoaded(null, null);

            if (this.favouritessGridView.ColumnCount != 0)
            {
                this.favouritessGridView.Rows[0].Cells[0].Selected = false;
                this.favouritessGridView.FirstDisplayedScrollingColumnIndex = this.favouritessGridView.ColumnCount - 1;
            }
            favouritessGridView.ResumeLayout();

        }
        private void FillFavourites1()
        {
            int divided = (this.spinNumbers.Count) % 6;
            int columnsCount = divided == 0 ? this.spinNumbers.Count / 6 : (this.spinNumbers.Count / 6) + 1;
            if (favourites.Count < columnsCount)
            {
                for (int i = favourites.Count; i < columnsCount; i++)
                {
                    favourites.Add(new int[5]);
                }
            }
            else if (favourites.Count > columnsCount)
            {
                favourites.RemoveAt(favourites.Count - 1);
            }

            for (int i = favs1.Count; i < this.spinNumbers.Count; i++)
            {
                if (this.part1SixLines.Count - i < 12)
                {
                    break;
                }

                int[] l1 = this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (13 + i)).Take(12).ToList(), 12);
                int[] l2 = this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (19 + i)).Take(18).ToList(), 18);
                int[] l3 = this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (25 + i)).Take(24).ToList(), 24);
                int[] l4 = this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (31 + i)).Take(30).ToList(), 30);
                int[] l5 = this.GetLastSelaectedBets(this.part1SixLines.Skip(this.part1SixLines.Count - (37 + i)).Take(36).ToList(), 36);

                int last = this.part1SixLines[this.part1SixLines.Count - (i + 1)];
                int columnIndex = (this.part1SixLines.Count - i) % 6 == 0 ? (this.part1SixLines.Count - i) / 6 - 1 : (this.part1SixLines.Count - i) / 6;
                if (l1[0] == last)
                {
                    favourites[columnIndex][0] = 12;
                    favs1.Add(12);
                }
                else
                {
                    favs1.Add(0);
                }
                if (l2.Length != 0 && l2[0] == last)
                {
                    favourites[columnIndex][1] = 18;
                    favs1.Add(18);

                }
                else
                {
                    favs1.Add(0);
                }
                if (l3.Length != 0 && l3[0] == last)
                {
                    favourites[columnIndex][2] = 24;
                    favs1.Add(24);

                }
                else
                {
                    favs1.Add(0);
                }
                if (l4.Length != 0 && l4[0] == last)
                {
                    favourites[columnIndex][3] = 30;
                    favs1.Add(30);

                }
                else
                {
                    favs1.Add(0);
                }
                if (l5.Length != 0 && l5[0] == last)
                {
                    favourites[columnIndex][4] = 36;
                    favs1.Add(36);
                }
                else
                {
                    favs1.Add(0);
                }
            }
            return;
            if (favs1.Count > 20)
            {
                favs1 = favs1.Take(20).ToList();
            }
            favs1.Reverse();
            this.favsSummaryDataGridItems.Clear();
            this.favsSummaryDataGridItems.Add(new object[3]);
            this.favsSummaryDataGridItems.Add(new object[3]);
            this.favsSummaryDataGridItems.Add(new object[3]);

            if (favs1.Count >= 10)
            {
                int[] res = this.GetLastSelaectedFavs(favs1.Skip(favs1.Count - 10).ToList(), 10);
                this.favsSummaryDataGridItems[0][0] = res[0];
                this.favsSummaryDataGridItems[1][0] = res[1];
                this.favsSummaryDataGridItems[2][0] = 10;

            }
            if (favs1.Count >= 15)
            {
                int[] res1 = this.GetLastSelaectedFavs(favs1.Skip(favs1.Count - 15).ToList(), 15);
                this.favsSummaryDataGridItems[0][1] = res1[0];
                this.favsSummaryDataGridItems[1][1] = res1[1];
                this.favsSummaryDataGridItems[2][1] = 15;
            }
            if (favs1.Count >= 20)
            {
                int[] res2 = this.GetLastSelaectedFavs(favs1.Take(20).ToList(), 20);
                this.favsSummaryDataGridItems[0][2] = res2[0];
                this.favsSummaryDataGridItems[1][2] = res2[1];
                this.favsSummaryDataGridItems[2][2] = 20;
            }

            CollectionViewSource.GetDefaultView(this.favsSummaryDataGridItems).Refresh();
            this.MainWindow_OnLoaded(null, null);

            if (this.favouritessGridView.ColumnCount != 0)
            {
                this.favouritessGridView.Rows[0].Cells[0].Selected = false;
                this.favouritessGridView.FirstDisplayedScrollingColumnIndex = this.favouritessGridView.ColumnCount - 1;
            }
        }

        private void CalculateSuccessRate()
        {
            if (this.part1SixLines.Count >= 108 && this.part1SixLines.Count % 6 == 0)
            {
                int zeros = 0;

                for (int i = this.droppedInSixLinesGridView.ColumnCount - 1; i >= this.droppedInSixLinesGridView.ColumnCount - 18; i--)
                {
                    for (int j = 0; j < this.droppedInSixLinesGridView.RowCount; j++)
                    {
                        if (this.droppedInSixLinesGridView.Rows[j].Cells[i].Value == null)
                        {
                            zeros++;
                        }
                    }
                }
                float percent = zeros * (100 / (float)108);
                this.successRate.Text = Math.Round(percent, 1).ToString().Replace(",", ".") + "%";
            }
        }
        private int[] GetLastSelectedBets(int amount)
        {
            if (this.part1SixLines.Count >= amount)
            {
                IEnumerable<int> lastNeededRows = this.part1SixLines.Skip(this.part1SixLines.Count - amount);
                int[] values = new int[amount];
                for (int i = 6; i <= amount; i += 6)
                {
                    values[i - 6] = lastNeededRows.Take(i).Count(t => t == 1);
                    values[i - 5] = lastNeededRows.Take(i).Count(t => t == 2);
                    values[i - 4] = lastNeededRows.Take(i).Count(t => t == 3);
                    values[i - 3] = lastNeededRows.Take(i).Count(t => t == 4);
                    values[i - 2] = lastNeededRows.Take(i).Count(t => t == 5);
                    values[i - 1] = lastNeededRows.Take(i).Count(t => t == 6);
                }
                int[] sum = new int[6];
                for (int i = 0; i < amount; i++)
                {
                    sum[i % 6] += values[i];
                }
                var sorted = sum.Select(((i, i1) => new { index = i1, value = i })).OrderByDescending(t => t.value).ToList();
                return new int[6] { sorted[0].index + 1, sorted[1].index + 1, sorted[2].index + 1, sorted[3].index + 1, sorted[4].index + 1, sorted[5].index + 1 };
            }
            return new int[0];
        }
        private void Id_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            string text = button.Content.ToString();
            this.spinNumbers.Add(Convert.ToInt32(text));

            this.DoActions();
        }
        private void UpdateSummaryGrid()
        {
            this.firstSummaryDataGridItems.Clear();
            this.firstSummaryDataGridItems.Add(new object[5]);
            this.firstSummaryDataGridItems.Add(new object[5]);
            this.firstSummaryDataGridItems.Add(new object[5]);
            this.firstSummaryDataGridItems[2][0] = 12;
            this.firstSummaryDataGridItems[2][1] = 18;
            this.firstSummaryDataGridItems[2][2] = 24;
            this.firstSummaryDataGridItems[2][3] = 30;
            this.firstSummaryDataGridItems[2][4] = 36;

            int[] l1 = this.GetLastSelectedBets(12);
            int[] l2 = this.GetLastSelectedBets(18);
            int[] l3 = this.GetLastSelectedBets(24);
            int[] l4 = this.GetLastSelectedBets(30);
            int[] l5 = this.GetLastSelectedBets(36);
            if (this.part1SixLines.Count >= 12)
            {
                this.firstSummaryDataGridItems[0][0] = l1[0];
                this.firstSummaryDataGridItems[1][0] = l1[1];
            }
            if (this.part1SixLines.Count >= 18)
            {
                this.firstSummaryDataGridItems[0][1] = l2[0];
                this.firstSummaryDataGridItems[1][1] = l2[1];
            }
            if (this.part1SixLines.Count >= 24)
            {
                this.firstSummaryDataGridItems[0][2] = l3[0];
                this.firstSummaryDataGridItems[1][2] = l3[1];
            }
            if (this.part1SixLines.Count >= 30)
            {
                this.firstSummaryDataGridItems[0][3] = l4[0];
                this.firstSummaryDataGridItems[1][3] = l4[1];
            }
            if (this.part1SixLines.Count >= 36)
            {
                this.firstSummaryDataGridItems[0][4] = l5[0];
                this.firstSummaryDataGridItems[1][4] = l5[1];
            }


            CollectionViewSource.GetDefaultView(this.firstSummaryDataGridItems).Refresh();
            this.MainWindow_OnLoaded(null, null);
        }
        private void CanceActionButton_Click(object sender, RoutedEventArgs e)
        {
            this.MoveBackBetButton_OnClick(null, null);
            this.MoveBackBetButton2_OnClick(null, null);
            if (this.spinNumbers.Count == 0)
            {
                return;
            }

            this.spinNumbers.RemoveAt(this.spinNumbers.Count - 1);
            cancelled = true;
            this.DoActions();
        }
        private void LoadNumberButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                FileName = "result file",
                Filter = "Text file|*.txt|All Files|*.*"
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string text = File.ReadAllText(dialog.FileName);
                this.numberTextBox.Text = text;
                this.spinNumbers = text.Split(new string[] { ", " }, StringSplitOptions.None).Reverse().Select(t => Convert.ToInt32(t)).ToList();
                this.DoActions();
            }
        }
        private void SaveNumberButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                FileName = "result",
                Filter = "Text file|*.txt|All Files|*.*"
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, (string.Join(", ", this.Reverse(this.spinNumbers))));
                this.isNumbersListSaved = true;
            }
        }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            DataGridRow row = this.part1SummaryDataGrid.GetRow(2);
            Style style = new Style(typeof(DataGridRow));

            style.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(0, 2, 0, 0)));
            style.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(System.Windows.Media.Colors.Black)));

            row.Style = style;

            row = this.favsSummaryDataGrid.GetRow(2);
            row.Style = style;
            for (int i = 0; i < 2; i++)
            {
                row = this.part1SummaryDataGrid.GetRow(i);

                this.part1SummaryDataGrid.GetCell(row, 0).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.part1SummaryDataGrid.GetCell(row, 0).Foreground = Brushes.Blue;
                this.part1SummaryDataGrid.GetCell(row, 0).FontSize = 15;

                this.part1SummaryDataGrid.GetCell(row, 1).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.part1SummaryDataGrid.GetCell(row, 1).Foreground = Brushes.Blue;
                this.part1SummaryDataGrid.GetCell(row, 1).FontSize = 15;

                this.part1SummaryDataGrid.GetCell(row, 2).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.part1SummaryDataGrid.GetCell(row, 2).Foreground = Brushes.Blue;
                this.part1SummaryDataGrid.GetCell(row, 2).FontSize = 15;

                this.part1SummaryDataGrid.GetCell(row, 3).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.part1SummaryDataGrid.GetCell(row, 3).Foreground = Brushes.Blue;
                this.part1SummaryDataGrid.GetCell(row, 3).FontSize = 15;

                this.part1SummaryDataGrid.GetCell(row, 4).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.part1SummaryDataGrid.GetCell(row, 4).Foreground = Brushes.Blue;
                this.part1SummaryDataGrid.GetCell(row, 4).FontSize = 15;

                row = this.favsSummaryDataGrid.GetRow(i);
                this.favsSummaryDataGrid.GetCell(row, 0).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.favsSummaryDataGrid.GetCell(row, 0).Foreground = Brushes.Blue;
                this.favsSummaryDataGrid.GetCell(row, 0).FontSize = 15;

                this.favsSummaryDataGrid.GetCell(row, 1).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.favsSummaryDataGrid.GetCell(row, 1).Foreground = Brushes.Blue;
                this.favsSummaryDataGrid.GetCell(row, 1).FontSize = 15;

                this.favsSummaryDataGrid.GetCell(row, 2).FontWeight = FontWeight.FromOpenTypeWeight(500);
                this.favsSummaryDataGrid.GetCell(row, 2).Foreground = Brushes.Blue;
                this.favsSummaryDataGrid.GetCell(row, 2).FontSize = 15;


            }


        }
        private void PlaceBetButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.allowMoveBetFirst = true;
            this.betSelectedIndexFirst = -1;
            this.betGrid.UnselectAllCells();
            this.betGrid.GetCell(this.betGrid.GetRow(1), ++this.betSelectedIndexFirst).IsSelected = true;
        }
        private void ResetBetButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.allowMoveBetFirst = false;
            this.betSelectedIndexFirst = -1;
            this.betGrid.UnselectAllCells();
            this.betGrid.ScrollIntoView(this.betGrid.Items[1], this.betGrid.Columns[0]);

        }
        private void StopBetButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.allowMoveBetFirst = false;
        }
        private void ResumeBetButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.allowMoveBetFirst = true;
        }
        private void MoveBackBetButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.betGrid.UnselectAllCells();
            if (this.betSelectedIndexFirst <= 0)
            {
                return;
            }
            int current = this.betSelectedIndexFirst;
            this.betGrid.GetCell(this.betGrid.GetRow(1), --this.betSelectedIndexFirst).IsSelected = true;
            if (this.betSelectedIndexFirst >= 25)
            {
                this.betGrid.ScrollIntoView(this.betGrid.Items[1], this.betGrid.Columns[this.betSelectedIndexFirst - 25]);
            }
        }
        private void MoveNextBetButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.betGrid.UnselectAllCells();
            this.betGrid.GetCell(this.betGrid.GetRow(1), ++this.betSelectedIndexFirst).IsSelected = true;
            this.betGrid.ScrollIntoView(this.betGrid.Items[1], this.betGrid.Columns[this.betSelectedIndexFirst]);
        }
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!this.isNumbersListSaved)
            {
                MessageBoxResult result = MessageBox.Show("Данные не сохранены! Желаете сохранить?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    this.SaveNumberButton_Click(null, null);
                }
            }
        }
        private void betGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void PlaceBetButton2_OnClick(object sender, RoutedEventArgs e)
        {
            this.allowMoveBetSecond = true;
            this.betSelectedIndexSecond = -1;
            this.betGrid2.UnselectAllCells();
            this.betGrid2.GetCell(this.betGrid2.GetRow(1), ++this.betSelectedIndexSecond).IsSelected = true;
        }
        private void ResetBetButton2_OnClick(object sender, RoutedEventArgs e)
        {
            this.allowMoveBetSecond = false;
            this.betSelectedIndexSecond = -1;
            this.betGrid2.UnselectAllCells();
            this.betGrid2.ScrollIntoView(this.betGrid2.Items[1], this.betGrid2.Columns[0]);
        }
        private void StopBetButton2_OnClick(object sender, RoutedEventArgs e)
        {
            allowMoveBetSecond = false;
        }
        private void ResumeBetButton2_OnClick(object sender, RoutedEventArgs e)
        {
            allowMoveBetSecond = true;

        }
        private void MoveBackBetButton2_OnClick(object sender, RoutedEventArgs e)
        {
            this.betGrid2.UnselectAllCells();
            if (this.betSelectedIndexSecond <= 0)
            {
                return;
            }
            int current = this.betSelectedIndexSecond;
            this.betGrid2.GetCell(this.betGrid2.GetRow(1), --this.betSelectedIndexSecond).IsSelected = true;
            if (this.betSelectedIndexSecond >= 25)
            {
                this.betGrid2.ScrollIntoView(this.betGrid2.Items[1], this.betGrid2.Columns[this.betSelectedIndexSecond - 25]);
            }
        }
        private void MoveNextBetButton2_OnClick(object sender, RoutedEventArgs e)
        {
            this.betGrid2.UnselectAllCells();
            this.betGrid2.GetCell(this.betGrid2.GetRow(1), ++this.betSelectedIndexSecond).IsSelected = true;
            this.betGrid2.ScrollIntoView(this.betGrid2.Items[1], this.betGrid2.Columns[this.betSelectedIndexSecond]);

        }
    }
    internal static class Helper
    {
        public static System.Windows.Controls.DataGridCell GetCell(this System.Windows.Controls.DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                System.Windows.Controls.DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
    }
}