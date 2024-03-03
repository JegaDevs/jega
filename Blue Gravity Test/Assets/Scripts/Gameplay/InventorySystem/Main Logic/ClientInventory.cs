namespace Jega.BlueGravity.InventorySystem
{
    public class ClientInventory : Inventory
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            sessionService.RegisterActiveClientInventory(this);
        }
        private void OnDisable()
        {
            sessionService.UnregisterClientShopInventory();
        }
    }
}
