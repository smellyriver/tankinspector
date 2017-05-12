namespace Smellyriver.TankInspector.Modeling
{
	internal interface ICommodity
    {
        CurrencyType CurrencyType { get; }
        string Key { get; }
        bool NotInShop { get; }
        int Price { get; }
    }
}
