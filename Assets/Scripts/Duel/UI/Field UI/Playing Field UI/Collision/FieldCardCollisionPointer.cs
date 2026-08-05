
public interface FieldCardCollisionPointer : CardCollisionPointer {
    public FieldCardUI GetFieldCardUI();

    CardUI CardCollisionPointer.GetCardUI() {
        return GetFieldCardUI();
    }
}