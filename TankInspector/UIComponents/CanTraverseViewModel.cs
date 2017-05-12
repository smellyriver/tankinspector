namespace Smellyriver.TankInspector.UIComponents
{
	internal class CanTraverseViewModel
    {

        public bool CanTraverse { get; }
        public CanTraverseViewModel(bool canTraverse)
        {
            this.CanTraverse = canTraverse;
        }

        public override string ToString()
        {
	        if (this.CanTraverse)
                return App.GetLocalizedString("CanTraverseYes");
	        return App.GetLocalizedString("CanTraverseNo");
        }

    }
}
