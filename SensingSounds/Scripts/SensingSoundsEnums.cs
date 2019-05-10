namespace CATAHL {

    /// <summary>
    /// Enums for packettypes to send.
    /// </summary>
    public enum PacketType : byte {
        Transform,
        Sounds,
        Stop
    }

    /// <summary>
    /// Enum for different sounds.
    /// </summary>
    public enum Sounds {
        Trumpet = 1,
        Violin = 2,
        Orchestra = 3,
        Other = 4
    }
}
