using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smellyriver.Wpf.Utilities;
using System.Diagnostics;
using Smellyriver.Collections;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for NationalTechTreePageView.xaml
    /// </summary>
    public partial class NationalTechTreePageView : UserControl
    {


        private static readonly BitmapImage ArrowHeadImage;
 
        static NationalTechTreePageView()
        {
            ArrowHeadImage = new BitmapImage(new Uri("pack://application:,,,/Smellyriver.TankInspector;component/Resources/Images/UIElements/ArrowHead.png"));
        }

        private static Thickness _nodeMargin;

        private bool _updateConnectorsPending = true;
        private bool _takeContentSnapshotPending = false;

        internal NationalTechTreePageViewModel ViewModel
        {
            get => this.DataContext as NationalTechTreePageViewModel;
	        set => this.DataContext = value;
        }

        private readonly Style _connectorStyle;

        public NationalTechTreePageView()
        {
            InitializeComponent();

            _connectorStyle = this.FindResource("ConnectionLine") as Style;

            this.CalculateNodeMargin();
            this.TanksContainer.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            this.TanksContainer.LayoutUpdated += TanksContainer_LayoutUpdated;          
        }

        private void CalculateNodeMargin()
        {
            var padding = (Thickness)this.FindResource("NodeButtonPadding");
            var margin = (Thickness)this.FindResource("NodeButtonMarginInTechTree");
            _nodeMargin = new Thickness(padding.Left + margin.Left,
                padding.Top + margin.Top,
                padding.Right + margin.Right,
                padding.Bottom + margin.Bottom);
        }

	    private void TanksContainer_LayoutUpdated(object sender, EventArgs e)
        {
            if (_updateConnectorsPending)
                this.UpdateConnectors();
        }


	    private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            this.ScrollViewer.ScrollToHorizontalOffset(0.0);
            this.ScrollViewer.ScrollToVerticalOffset(0.0);
            this.ScrollViewer.ReleaseMouseCapture();
            this.IsEnabled = false;

            this.TechTreeContent.Visibility = Visibility.Visible;
            this.TechTreeContentSnapshot.Visibility = Visibility.Collapsed;

            _updateConnectorsPending = true;
        }

        private void TankButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var vm = (NationalTechTreeNodeViewModel)button.DataContext;
            this.ViewModel.LoadTank(vm.Tank);
        }


        private void UpdateConnectors()
        {

            if (this.TanksContainer.Items.Count == 0)
            {
                this.LineCanvas.Children.Clear();
                _updateConnectorsPending = false;
                return;
            }
	        if (this.TanksContainer.ItemContainerGenerator.ContainerFromIndex(0) == null
	            || ((FrameworkElement)this.TanksContainer.ItemContainerGenerator.ContainerFromIndex(0)).IsLoaded)
	        {
		        _updateConnectorsPending = true;
		        return;
	        }

	        this.LineCanvas.BeginInit();
            this.LineCanvas.Children.Clear();

            var comparer = ProjectionEqualityComparer<ContentPresenter>.Create(node => ((NationalTechTreeNodeViewModel)node.Content).Tank.Key);

            var tankNodeMap = this.TanksContainer.Items.OfType<NationalTechTreeNodeViewModel>()
                .Select(item => TanksContainer.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter)
                .Distinct(comparer)
                .ToDictionary(node => ((NationalTechTreeNodeViewModel)node.Content).Tank.Key);

            foreach (var fromNode in tankNodeMap.Values)
            {
                var fromNodeVm = (NationalTechTreeNodeViewModel)fromNode.Content;
                if (fromNodeVm.UnlockTanks.Any())
                {
                    foreach (var unlockedTankKey in fromNodeVm.UnlockTanks)
                    {
						if (tankNodeMap.TryGetValue(unlockedTankKey, out ContentPresenter toNode))
							this.Connect(fromNode, toNode);
					}
                }
            }
            this.LineCanvas.EndInit();

            this.ViewModel.IsLoading = false;

            _updateConnectorsPending = false;
            this.PendContentSnapshot();

        }

        private void PendContentSnapshot()
        {
            _takeContentSnapshotPending = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

	    private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (_takeContentSnapshotPending)
            {
                this.TechTreeContentSnapshot.Source = this.TechTreeContent.RenderToBitmap();
                _takeContentSnapshotPending = false;
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                this.IsEnabled = true;
            }
        }

        private Point GetTopCenterPoint(ContentPresenter node)
        {
            return node.TranslatePoint(new Point(node.ActualWidth / 2 - (_nodeMargin.Right - _nodeMargin.Left) / 2, _nodeMargin.Top), this.LineCanvas);
        }

        private Point GetBottomCenterPoint(ContentPresenter node)
        {
            return node.TranslatePoint(new Point(node.ActualWidth / 2 - (_nodeMargin.Right - _nodeMargin.Left) / 2, node.ActualHeight - _nodeMargin.Bottom), this.LineCanvas);
        }

        private void Connect(ContentPresenter node1, ContentPresenter node2)
        {
            var from = node1;
            var to = node2;

            var fromColumn = Grid.GetColumn(from);
            var toColumn = Grid.GetColumn(to);
            var fromRow = Grid.GetRow(from);
            var toRow = Grid.GetRow(to);
            var inSameColumn = fromColumn == toColumn;

            Point fromPoint, toPoint;
            if (inSameColumn)
            {
                if (fromRow > toRow)
                {
                    fromPoint = this.GetTopCenterPoint(from);
                    toPoint = this.GetBottomCenterPoint(to);
                }
                else
                {
                    fromPoint = this.GetBottomCenterPoint(from);
                    toPoint = this.GetTopCenterPoint(to);
                }
            }
            else
            {
                Debug.Assert(fromColumn < toColumn);
                var fromX = from.ActualWidth - _nodeMargin.Right;
                var toX = _nodeMargin.Left;

                if (fromRow == toRow)
                {
                    var verticalMargin = (_nodeMargin.Bottom - _nodeMargin.Top) / 2;
                    fromPoint = from.TranslatePoint(new Point(fromX, from.ActualHeight / 2 - verticalMargin), this.LineCanvas);
                    toPoint = to.TranslatePoint(new Point(toX, to.ActualHeight / 2 - verticalMargin), this.LineCanvas);
                }
                else if (fromRow > toRow)
                {
                    fromPoint = from.TranslatePoint(new Point(fromX, _nodeMargin.Top), this.LineCanvas);
                    toPoint = to.TranslatePoint(new Point(toX, to.ActualHeight - _nodeMargin.Bottom), this.LineCanvas);
                }
                else
                {
                    fromPoint = from.TranslatePoint(new Point(fromX, from.ActualHeight - _nodeMargin.Bottom), this.LineCanvas);
                    toPoint = to.TranslatePoint(new Point(toX, _nodeMargin.Top), this.LineCanvas);
                }
            }


            var line = new Line();
            line.X1 = fromPoint.X;
            line.Y1 = fromPoint.Y;
            line.X2 = toPoint.X;
            line.Y2 = toPoint.Y;
            line.Style = _connectorStyle;
            this.LineCanvas.Children.Add(line);

            var arrowHead = new Image();
            arrowHead.Source = ArrowHeadImage;
            Canvas.SetLeft(arrowHead, toPoint.X - ArrowHeadImage.Width);
            Canvas.SetTop(arrowHead, toPoint.Y - ArrowHeadImage.Height / 2);
            this.LineCanvas.Children.Add(arrowHead);
            arrowHead.RenderTransformOrigin = new Point(1, 0.5);
            arrowHead.RenderTransform = new RotateTransform(180 * Math.Atan2(toPoint.Y - fromPoint.Y, toPoint.X - fromPoint.X) / Math.PI);

        }

        private void OnDragBegin(object sender, EventArgs e)
        {
            this.TechTreeContent.Visibility = Visibility.Collapsed;
            this.TechTreeContentSnapshot.Visibility = Visibility.Visible;
        }

        private void OnSlideEnd(object sender, EventArgs e)
        {
            this.TechTreeContent.Visibility = Visibility.Visible;
            this.TechTreeContentSnapshot.Visibility = Visibility.Collapsed;
        }
    }
}
