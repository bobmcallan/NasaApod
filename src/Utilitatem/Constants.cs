using System;
using System.Collections.Generic;
using System.Text;

namespace CaelumServer.Utilitatem
{
    internal class Constants
    {
        public const string HEADER_SERVICE_NAME = "x-api-service";

        public const string HEADER_SERVICE_PATH = "x-api-path";

        public const string HEADER_SERVICE_REQUEST = "x-api-request";

        public const string HEADER_AUTHORIZATION = "Authorization";

        public const string HEADER_NOMOCKHEADER = "x-no-mock";

        public const string HEADER_CLIENT_KEY = "x-client-key";

        public const string HEADER_USER_ID = "x-client-id";

        public const string HEADER_USER_ROLES = "x-client-roles";

        public const string CONTEXT_CORRELATION_KEY = "CorrelationId";

        public const string CONTEXT_CLASS_KEY = "Class";

        public const string CONTEXT_METHOD_KEY = "Method";

        public const string PRD = "Production";

        public const string STG = "Stage";

        public const string DEV = "Development";

        public const string TST = "Test";

        public const string NONE = "None";
    }
}
