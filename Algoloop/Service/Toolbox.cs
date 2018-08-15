﻿/*
 * Copyright 2018 Capnode AB
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Algoloop.Model;
using QuantConnect.Configuration;
using QuantConnect.Logging;
using QuantConnect.ToolBox.FxcmDownloader;
using QuantConnect.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Algoloop.Service
{
    public class Toolbox : MarshalByRefObject
    {
        public string Run(MarketModel marketModel)
        {
            if (!SetConfig(marketModel))
            {
                return "Toolbox.Run: SetConfig failed";
            }

            Log.LogHandler = Composer.Instance.GetExportedValueByTypeName<ILogHandler>(Config.Get("log-handler", "CompositeLogHandler"));
            Log.Trace("Start Toolbox");

            try
            {
                using (var writer = new StringWriter())
                {
                    Console.SetOut(writer);
                    IList<string> list = marketModel.Symbols.Where(m => m.Enabled).Select(m => m.Name).ToList();
                    FxcmDownloaderProgram.FxcmDownloader(list, "Hour", new DateTime(2018, 08, 1), new DateTime(2018, 08, 4));

                    writer.Flush(); // when you're done, make sure everything is written out
                    var console = writer.GetStringBuilder().ToString();
                    return console;
                }
            }
            catch (Exception ex)
            {
                string log = string.Format("{0}: {1}", ex.GetType(), ex.Message);
                Log.LogHandler.Error(log);
                return log;
            }
            finally
            {
                Log.LogHandler.Dispose();
            }
        }

        private bool SetConfig(MarketModel marketModel)
        {
            Config.Set("log-handler", "QuantConnect.Logging.CompositeLogHandler");
            Config.Set("data-folder", "../../../Data/");
            Config.Set("fxcm-terminal", Enum.GetName(typeof(AccountModel.AccountType), marketModel.Type));
            Config.Set("fxcm-user-name", marketModel.Login);
            Config.Set("fxcm-password", marketModel.Password);
            return true;
        }
    }
}
