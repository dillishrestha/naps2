﻿using System;
using System.Collections.Generic;
using System.Linq;
using NAPS2.Ocr;

namespace NAPS2.Config.Experimental
{
    public static class ConfigExtensions
    {
        public static T Get<T>(this ConfigScopes configScopes, Func<CommonConfig, T> func) where T : class => configScopes.Provider.Get(func);

        public static T Get<T>(this ConfigScopes configScopes, Func<CommonConfig, T?> func) where T : struct => configScopes.Provider.Get(func);

        public static OcrParams DefaultOcrParams(this ConfigProvider<CommonConfig> provider)
        {
            if (!provider.Get(c => c.EnableOcr))
            {
                return new OcrParams();
            }
            return new OcrParams(provider.Get(c => c.OcrLanguageCode), provider.Get(c => c.OcrMode), provider.Get(c => c.OcrTimeoutInSeconds));
        }

        public static TransactionConfigScope<T> BeginTransaction<T>(this ConfigScope<T> scope) where T : new() => 
            new TransactionConfigScope<T>(scope, () => new T());

        public static ConfigProvider<CommonConfig> WithTransactions(this ConfigScopes configScopes, TransactionConfigScope<CommonConfig> userTransact, TransactionConfigScope<CommonConfig> runTransact) =>
            new ScopeSetConfigProvider<CommonConfig>(configScopes.AppLocked, runTransact, userTransact, configScopes.AppDefault, configScopes.InternalDefault);
    }
}