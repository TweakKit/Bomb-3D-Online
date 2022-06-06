using Mirror;

public interface BaseOpMsg : NetworkMessage
{
    #region Interface Methods    

    short OpId { get; }

    #endregion Interface Methods
}