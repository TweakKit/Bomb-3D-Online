namespace ZB.Gameplay
{
    public enum MapSlotType
    {
        Empty,
        Block,
        BrickDecor,
        BrickBox,
        ChestBox,
        Bomb,
    }

    public struct MapSlot
    {
        #region Members

        public MapSlotType slotType;

        #endregion Members

        #region Properties

        public bool IsEmpty => slotType == MapSlotType.Empty;
        public bool IsBlock => slotType == MapSlotType.Block;
        public bool IsBreakable => slotType == MapSlotType.BrickDecor || slotType == MapSlotType.BrickBox || slotType == MapSlotType.ChestBox;
        public bool IsBrick => slotType == MapSlotType.BrickDecor || slotType == MapSlotType.BrickBox;
        public bool IsChestBox => slotType == MapSlotType.ChestBox;
        public bool IsBomb => slotType == MapSlotType.Bomb;

        #endregion Properties

        #region Class Methods

        public MapSlot(MapSlotType slotType)
        {
            this.slotType = slotType;
        }

        #endregion Class Methods
    }
}