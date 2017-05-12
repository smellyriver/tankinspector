using Smellyriver.TankInspector.Modeling;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Smellyriver.TankInspector.UIComponents
{
	internal partial class HangarViewModel
    {

        private bool _isSearchingForReferenceTank;
        public bool IsSearchingForReferenceTank
        {
            get => _isSearchingForReferenceTank;
	        set
            {
                _isSearchingForReferenceTank = value;
                this.RaisePropertyChanged(() => this.IsSearchingForReferenceTank);
            }
        }


        private string _searchText;
        public string SearchText
        {
            get => _searchText;
	        set
            {
                _searchText = value;
                this.RaisePropertyChanged(() => this.SearchText);
                this.DoSearch();
            }
        }

        private SearchResultItemViewModel[] _searchResult;
        public SearchResultItemViewModel[] SearchResult
        {
            get => _searchResult;
	        private set
            {
                _searchResult = value;
                this.RaisePropertyChanged(() => this.SearchResult);
            }
        }

        private int _highlightedSearchResultItemIndex;

        private SearchResultItemViewModel _highlightedSearchResultItem;
        public SearchResultItemViewModel HighlightedSearchResultItem
        {
            get => _highlightedSearchResultItem;
	        set
            {
                _highlightedSearchResultItem = value;
                this.RaisePropertyChanged(() => this.HighlightedSearchResultItem);
                _highlightedSearchResultItemIndex = Array.IndexOf(this.SearchResult, HighlightedSearchResultItem);
            }
        }


        private void DoSearch()
        {

			if (_searchText == null || _searchText.Length < 2)
			{
				this.SearchResult = new SearchResultItemViewModel[0];
				_highlightedSearchResultItemIndex = -1;
				return;
			}

			if (!HangarViewModel.ParseSearchText(_searchText, out string keyword, out int tier, out TankClass tankClass, out _isSearchingForReferenceTank)
                || (keyword.Length < 2 && tier == -1 && tankClass == TankClass.Mixed))
            {
                this.SearchResult = new SearchResultItemViewModel[0];
                _highlightedSearchResultItemIndex = -1;
                return;
            }

            this.SearchResult = Database.Current.Search(keyword, tier, tankClass, _isSearchingForReferenceTank)
                .Select(t => new SearchResultItemViewModel(_commandBindings, t, this)).ToArray();

            if (this.SearchResult.Length > 0)
            {
                _highlightedSearchResultItemIndex = 0;
                this.HighlightedSearchResultItem = this.SearchResult.First();
            }

        }

        private static bool ParseSearchText(string searchText, out string keyword, out int tier, out TankClass tankClass, out bool searchingForReference)
        {
            tier = -1;
            tankClass = TankClass.Mixed;
            searchingForReference = false;

            searchText = searchText.Trim();

            const string pattern = @"^(?<ref>ref\:)*(?:\(([\w\d]+)(?:,\s*([\w\d]+))*\))?(?<keyword>.*)$";
            var match = Regex.Match(searchText, pattern);
            if (!match.Success)
            {
                keyword = searchText;
                return false;
            }

            searchingForReference = match.Groups["ref"].Length > 0;
            keyword = match.Groups["keyword"].Value;

            for (int i = 1; i < match.Groups.Count - 1; ++i)
            {
                var option = match.Groups[i].Value.ToLower();

                switch (option)
                {
                    case "lt":
                        tankClass = TankClass.LightTank;
                        break;
                    case "mt":
                        tankClass = TankClass.MediumTank;
                        break;
                    case "ht":
                        tankClass = TankClass.HeavyTank;
                        break;
                    case "td":
                    case "at":
                        tankClass = TankClass.TankDestroyer;
                        break;
                    case "spg":
                        tankClass = TankClass.SelfPropelledGun;
                        break;
                    default:
                        var tierMatch = Regex.Match(option, @"t(\d\d*)");
                        if (tierMatch.Success)
                            int.TryParse(tierMatch.Groups[1].Value, out tier);
                        break;

                }
            }

            return true;

        }

        internal void ClearSearch()
        {
            this.SearchText = null;
            this.SearchResult = new SearchResultItemViewModel[0];
        }


        private void SearchForReferenceTanks()
        {
            var tier = this.Tank.Tier;
            var tankClass = this.Tank.Tank.Class;
            string className;
            switch (tankClass)
            {
                case TankClass.HeavyTank: className = "ht"; break;
                case TankClass.LightTank: className = "lt"; break;
                case TankClass.MediumTank: className = "mt"; break;
                case TankClass.TankDestroyer: className = "td"; break;
                case TankClass.SelfPropelledGun: className = "spg"; break;
                default: return;
            }
            this.SearchText = $"ref:(t{tier}, {className})";
        }

        internal void SelectSearchResultTank(ITankInfo tankInfo)
        {
            if (_isSearchingForReferenceTank)
                this.SelectSearchResultTankAsReferenceTank(tankInfo);
            else
            {
                this.LoadTank((Tank)Database.Current.GetTank(tankInfo.ColonFullKey));
                this.ClearSearch();
            }
        }

        internal void SelectSearchResultTankAsReferenceTank(ITankInfo tankInfo)
        {
            var tank = Database.Current.GetTank(tankInfo.ColonFullKey);
            var tankVm = TankObjectViewModelFactory.CreateTank(_commandBindings, tank, this);

            this.SetReferenceTankDefaultState(tankVm);

            this.ReferenceTank = tankVm;
            this.ClearSearch();
        }


        internal void HighlightNextSearchResult()
        {
            if (this.SearchResult == null || this.SearchResult.Length < 2)
                return;

            ++_highlightedSearchResultItemIndex;
            if (_highlightedSearchResultItemIndex >= this.SearchResult.Length)
                _highlightedSearchResultItemIndex = 0;

            this.HighlightedSearchResultItem = this.SearchResult[_highlightedSearchResultItemIndex];
        }

        internal void HighlightPreviousSearchResult()
        {
            if (this.SearchResult == null || this.SearchResult.Length < 2)
                return;

            --_highlightedSearchResultItemIndex;
            if (_highlightedSearchResultItemIndex < 0)
                _highlightedSearchResultItemIndex = this.SearchResult.Length - 1;

            this.HighlightedSearchResultItem = this.SearchResult[_highlightedSearchResultItemIndex];
        }

        internal void SelectHighlightedSearchResultTank()
        {
            if (this.HighlightedSearchResultItem != null)
                this.SelectSearchResultTank(this.HighlightedSearchResultItem.Tank);
        }

        internal void SelectHighlightedSearchResultTankAsReferenceTank()
        {
            if (this.HighlightedSearchResultItem != null)
                this.SelectSearchResultTankAsReferenceTank(this.HighlightedSearchResultItem.Tank);
        }
    }
}
