using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Smellyriver.TankInspector.UIComponents
{

    //specifically made for DataView, not reusable
	internal class DataViewPriorityItemsControl : ItemsControl
    {

        public int MinimumPriority
        {
            get => (int)GetValue(MinimumPriorityProperty);
	        set => SetValue(MinimumPriorityProperty, value);
        }

        public static readonly DependencyProperty MinimumPriorityProperty =
            DependencyProperty.Register("MinimumPriority", typeof(int), typeof(DataViewPriorityItemsControl), new PropertyMetadata(0, DataViewPriorityItemsControl.OnMinimumPriorityChanged));

        private static void OnMinimumPriorityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DataViewPriorityItemsControl)d;
            var oldValue = (int)e.OldValue;
            var newValue = (int)e.NewValue;
            control.OnMinimumPriorityChanged(oldValue, newValue);
        }

        private IEnumerable<DataItemViewModelBase> PrioritySortedItems
        {
            get
            {
                return this.Items.Cast<DataItemViewModelBase>()
                                 .Select((o, i) => new { index = i, value = o })
                                 .OrderByDescending(i => i.value.Priority)
                                 .ThenBy(i => i.value is ComplexDataItemViewModel ? 1 : 0)
                                 .ThenBy(i => i.index)
                                 .Select(i => i.value);
            }
        }

        private bool _isWaitingForItemContainerGenerator;

        private void OnMinimumPriorityChanged(int oldValue, int newValue)
        {
            if (oldValue != newValue)
                this.InvalidateMeasure();
        }

        protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel)
        {
            base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);
            if (!typeof(StackPanel).IsAssignableFrom(newItemsPanel.VisualTree.Type))
                throw new ArgumentException();
        }


        protected override Size MeasureOverride(Size constraint)
        {
            if (_isWaitingForItemContainerGenerator)
                return new Size(0, 0);

            if (this.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                _isWaitingForItemContainerGenerator = true;
                this.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
                return new Size(0, 0);
            }

            var desiredHeight = this.Padding.Top + this.Padding.Bottom;
            var insufficientHeight = false;

            foreach (var item in this.PrioritySortedItems)
            {
                var contentPresenter = (ContentPresenter)this.ItemContainerGenerator.ContainerFromItem(item);

                if (item.ShouldShowForCurrentTank && VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                {

                    var innerElement = (FrameworkElement)VisualTreeHelper.GetChild(contentPresenter, 0);

                    if (!insufficientHeight && item.Priority >= this.MinimumPriority)
                    {
                        var complexDataItemViewModel = item as ComplexDataItemViewModel;

                        if (item is ComplexDataItemViewModel)
                        {
                            innerElement.Height = double.NaN;
                            contentPresenter.Measure(new Size(constraint.Width, constraint.Height - desiredHeight));
                        }

                        innerElement.Height = item.DesiredHeight;

                        var increasedSize = desiredHeight + item.DesiredHeight;
                        if (increasedSize <= constraint.Height)
                        {
                            desiredHeight = increasedSize;
                            item.IsPrioritySufficient = true;
                            continue;
                        }
	                    insufficientHeight = true;
                    }
                }

                item.IsPrioritySufficient = false;
            }

            return new Size(constraint.Width, desiredHeight);

        }

	    private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;

            this.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            _isWaitingForItemContainerGenerator = false;
            this.InvalidateMeasure();

        }

    }
}
