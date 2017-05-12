using Smellyriver.TankInspector.Modeling;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.UIComponents
{
    /// <summary>
    /// Interaction logic for GameClientVersionTag.xaml
    /// </summary>
    public partial class GameClientVersionTag : UserControl
    {
        private static readonly List<uint> SRevisions = new List<uint>();

        private static int GetRevisionIndex(uint revision)
        {
            var index = SRevisions.IndexOf(revision);
            if(index == -1)
            {
                index = SRevisions.Count;
                SRevisions.Add(revision);
            }

            return index;
        }


        public string GameClientVersion
        {
            get => (string)GetValue(GameClientVersionProperty);
	        set => SetValue(GameClientVersionProperty, value);
        }

        public static readonly DependencyProperty GameClientVersionProperty =
            DependencyProperty.Register("GameClientVersion", typeof(string), typeof(GameClientVersionTag), new PropertyMetadata("0.9.0 #111", GameClientVersionTag.OnGameClientVersionChanged));

        private static void OnGameClientVersionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GameClientVersionTag)d).OnGameClientVersionChanged();
        }

        public bool MiniMode
        {
            get => (bool)GetValue(MiniModeProperty);
	        set => SetValue(MiniModeProperty, value);
        }

        public static readonly DependencyProperty MiniModeProperty =
            DependencyProperty.Register("MiniMode", typeof(bool), typeof(GameClientVersionTag), new PropertyMetadata(false, GameClientVersionTag.OnMiniModeChanged));

        private static void OnMiniModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GameClientVersionTag)d).OnMiniModeChanged();
        }





        private string ShortVersion
        {
            get
            {
				if (Database.ParseVersion(this.GameClientVersion, out uint major, out uint minor, out uint build, out uint revision, out bool isCommonTest))
					return $"{minor}.{build}{(isCommonTest ? App.GetLocalizedString("CommonTest") : "")}";

				return "??";
            }
        }

        private uint Revision
        {
            get
            {
	            if (Database.ParseVersion(this.GameClientVersion, out uint major, out uint minor, out uint build, out uint revision, out bool isCommonTest))
					return revision;
	            return 0;
            }
        }

        public GameClientVersionTag()
        {
            InitializeComponent();
        }

        private void OnGameClientVersionChanged()
        {
            this.VersionDisplay.Text = this.ShortVersion;
            var index = GameClientVersionTag.GetRevisionIndex(this.Revision);
            var tagBrushes = (IList)this.TryFindResource("TagBrushes");
            this.Border.Background = this.Border.BorderBrush = (Brush)tagBrushes[index.Clamp(0, tagBrushes.Count - 1)];
            this.Border.ToolTip = this.GameClientVersion;
        }

        private void OnMiniModeChanged()
        {
            if(this.MiniMode)
            {
                this.VersionDisplay.Visibility = Visibility.Collapsed;
                this.Border.Width = this.Border.Height = 8;
            }
            else
            {
                this.VersionDisplay.Visibility = Visibility.Visible;
                this.Border.Width = this.Border.Height = double.NaN;
            }
        }

    }
}
