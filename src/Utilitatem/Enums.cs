using System;
using System.ComponentModel;

namespace NasaApod.Utilitatem
{

    internal class Constants
    {
        public const string PRD = "Production";
        public const string STG = "Stage";
        public const string DEV = "Development";
        public const string TST = "Test";
        public const string NONE = "None";
    }

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

