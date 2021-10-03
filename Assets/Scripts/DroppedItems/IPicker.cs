using System.Collections.Generic;

public interface IPicker
{
  List<DroppedItem> droppedItems { get; }
  void PickUp();
  void PickUp(DroppedItem droppedItem);
  void AddDroppedItem(DroppedItem item);
  void RemoveDroppedItem(DroppedItem item);
}