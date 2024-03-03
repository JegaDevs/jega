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
        protected override void UpdateSlotManager(InventorySlot slotVisual, StartingItem startingItem, int slotIndex)
        {
            base.UpdateSlotManager(slotVisual, startingItem, slotIndex);
            slotVisual.UpdateAvailability();
        }
    }
}
