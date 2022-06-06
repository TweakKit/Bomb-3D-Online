public struct OpMsgKillNotify : BaseOpMsg
{
    #region Members   

    public string killerName;
    public string victimName;
    public string weaponId;

    #endregion Members

    #region Properties

    public short OpId => 10004;

    #endregion Properties
}