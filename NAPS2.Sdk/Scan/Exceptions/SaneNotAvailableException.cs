﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NAPS2.Lang.Resources;

namespace NAPS2.Scan.Exceptions
{
    public class SaneNotAvailableException : ScanDriverException
    {
        private const string PACKAGES = "\nsane\nsane-utils";

        public SaneNotAvailableException() : base(MiscResources.SaneNotAvailable + PACKAGES)
        {
        }

        public SaneNotAvailableException(Exception innerException) : base(MiscResources.SaneNotAvailable + PACKAGES, innerException)
        {
        }
    }
}
