public struct OpMsgMatchStatus : BaseOpMsg
{
    #region Members   

    public float remainsMatchTime;
    public bool isMatchEnded;

    #endregion Members

    #region Properties

    public short OpId => 10003;

    #endregion Properties
}