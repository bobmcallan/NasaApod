using System;
using System.ComponentModel;

namespace CaelumServer.Utilitatem
{

    public enum Scope
    {
        [Description(Constants.PRD)]
        PRD = 0,
        [Description(Constants.PRD)]
        PROD = PRD,
        [Description(Constants.PRD)]
        PRODUCTION = PRD,

        [Description(Constants.STG)]
        STG = 1,
        [Description(Constants.STG)]
        UAT = STG,
        [Description(Constants.STG)]
        STAGE = STG,

        [Description(Constants.DEV)]
        DEV = 2,
        [Description(Constants.DEV)]
        DEVELOPMENT = DEV,

        [Description(Constants.TST)]
        TST = 3,
        [Description(Constants.TST)]
        TEST = TST,

        [Description(Constants.NONE)]
        NONE = 4,
        [Description(Constants.NONE)]
        DEFAULT = NONE,
    }
}

