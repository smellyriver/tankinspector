using Smellyriver.TankInspector.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class NationalTechTreePageViewModel : TankGalleryPageViewModel
    {
        private const int TankTiers = 10;

        public int Rows { get; }
        public int Columns { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
	        set
            {
                _isLoading = value;
                this.RaisePropertyChanged(() => this.IsLoading);
            }
        }

        private readonly List<NationalTechTreeNodeViewModel> _nodes;
        public IEnumerable<NationalTechTreeNodeViewModel> Nodes => _nodes;

	    public NationalDatabase Nation { get; }

        public NationalTechTreePageViewModel(CommandBindingCollection commandBindings, TankGalleryViewModel owner, TechTreeLayout layout, NationalDatabase nation)
            : base(commandBindings, owner)
        {
            this.Nation = nation;
            this.IsLoading = true;

            const int columns = TankTiers;

            _nodes = new List<NationalTechTreeNodeViewModel>();

            var nodeTankMap = new Dictionary<TechTreeLayoutNode, Tank>();

            var tanksInTechTree = new HashSet<Tank>();

            var rowMap = new Dictionary<int, int>();

            int definedRows;
            if (layout == null)
                definedRows = 0;
            else
                definedRows = layout.GridDefinition.Rows;

            // 1. build a node grid, solve situations where two tanks are occupying one cell

            var occupiedCells = new List<TechTreeLayoutNode[]>();
            for (int row = 0; row < definedRows + 1; ++row)
                occupiedCells.Add(new TechTreeLayoutNode[columns]);

            if (layout != null)
            {

                var t1TankRow = 0;

                foreach (var node in layout.Nodes)
                {
					if (layout.TechTree.Nation.Tanks.TryGetValue(node.TankKey, out Tank tank))
					{
						var row = node.Row;
						if (rowMap.TryGetValue(row, out int mappedRow))
							row = mappedRow;

						while (occupiedCells[row][tank.Tier - 1] != null)
						{
							// two tanks are occupying one cell, insert a row
							++row;
							if (row == occupiedCells.Count)
								occupiedCells.Insert(row, new TechTreeLayoutNode[columns]);
						}

						rowMap[node.Row] = row;

						occupiedCells[row][tank.Tier - 1] = node;
						nodeTankMap[node] = tank;
						tanksInTechTree.Add(tank);

						if (tank.Tier == 1 && !tank.IsPremiumTank && !tank.IsObsoleted)
							t1TankRow = row;
					}
				}

                // remove empty rows
                for (int row = occupiedCells.Count - 1; row >= 0; --row)
                    if (occupiedCells[row].All(node => node == null))
                        occupiedCells.RemoveAt(row);
                    else
                        break;

                // place T1 tank in vertical center 
                var newT1TankRow = (int)Math.Round(occupiedCells.Count / 2.0);
                var sign = newT1TankRow > t1TankRow ? -1 : 1;
                for (int row = newT1TankRow; row != t1TankRow; row += sign)
                {
                    if (occupiedCells[row][0] == null)
                    {
                        var t1Node = occupiedCells[t1TankRow][0];
                        if (t1Node == null)
                            break;
                        var newT1Node = new TechTreeLayoutNode(t1Node.TankKey, row, t1Node.Column, t1Node.UnlockTanks);
                        occupiedCells[row][0] = newT1Node;
                        occupiedCells[t1TankRow][0] = null;
                        nodeTankMap[newT1Node] = nodeTankMap[t1Node];
                        nodeTankMap.Remove(t1Node);
                        break;
                    }
                }
            }

            // 2. find out tanks which are existed and valid, but not appeared in the techtree
            var techTreeRowCount = occupiedCells.Count;

            foreach (var tank in nation.Tanks.Values)
                if (!tanksInTechTree.Contains(tank) && tank.IsValid)
                {
                    var row = techTreeRowCount;
                    while (true)
                    {
                        if (occupiedCells.Count <= row)
                            occupiedCells.Insert(row, new TechTreeLayoutNode[columns]);
                        else if (occupiedCells[row][tank.Tier - 1] != null)
                            ++row;
                        else
                        {
                            var node = new TechTreeLayoutNode(tank.Key, row, tank.Tier - 1, null);
                            occupiedCells[row][tank.Tier - 1] = node;
                            nodeTankMap[node] = tank;
                            break;
                        }
                    }
                }


            this.Rows = occupiedCells.Count;
            this.Columns = columns;

            // 3. create node viewmodels
            for (int row = 1; row < occupiedCells.Count; ++row)
                for (int column = 0; column < columns; ++column)
                {
                    var node = occupiedCells[row][column];
                    if (node != null)
                        _nodes.Add(new NationalTechTreeNodeViewModel(commandBindings, nodeTankMap[node], row, column, node.UnlockTanks));
                }


            // IsLoading will be set by the view, after all controls are populated
            //this.IsLoading = false;

        }

    }
}
