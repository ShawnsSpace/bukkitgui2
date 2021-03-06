﻿namespace Bukkitgui2.MinecraftServers
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Bukkitgui2.Core.Logging;
    using Bukkitgui2.MinecraftInterop.OutputHandler;
    using Bukkitgui2.MinecraftInterop.OutputHandler.PlayerActions;

    /// <summary>
    ///     The base for a minecraft server. This should contain all parsing code for a vanilla server.
    /// </summary>
    internal class MinecraftServerBase : IMinecraftServer
    {
        //new prefixes in output tags since 1.7.2
        /// <summary>
        ///     Prefix since minecraft 1.7.2 inside the message tag
        /// </summary>
        public const string RG_PRE172 = "((.*)thread(.*)/)?";

        // message tags
        /// <summary>
        ///     Info message tag at line start
        /// </summary>
        public const string RG_INFO = "^\\[" + RG_PRE172 + "info\\]";

        /// <summary>
        ///     Warning message tag at line start
        /// </summary>
        public const string RG_WARN = "^\\[" + RG_PRE172 + "(warning|warn)\\]";

        /// <summary>
        ///     Severe/Error message tag at line start
        /// </summary>
        public const string RG_SEV = "^\\[" + RG_PRE172 + "(severe|error)\\]";

        // ip's and player names
        /// <summary>
        ///     Ipv4 Ip address, without any extra's
        /// </summary>
        public const string RG_IP_NOPORT = "\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}";

        /// <summary>
        ///     Ipv4 Ip address, with optional port. this is the typical ip for a minecraft login
        /// </summary>
        public const string RG_IP = RG_IP_NOPORT + "(:\\d{2,5}|)";

        /// <summary>
        ///     Ipv4 Ip with optional port, between right brackets
        /// </summary>
        public const string RG_IP_BRACKET = "\\[/" + RG_IP + "\\]";

        /// <summary>
        ///     Player name
        /// </summary>
        public const string RG_PLAYER = "\\w{2,16}";

        // other regexes
        /// <summary>
        ///     Possible space
        /// </summary>
        public const string RG_SPACE = "\\s{0,}";

        /// <summary>
        ///     At least one space
        /// </summary>
        public const string RG_FSPACE = "\\s+";

        /// <summary>
        ///     Anything
        /// </summary>
        public const string RG_WILDCARD = "(.*)";

        /// <summary>
        ///     End of line
        /// </summary>
        public const string RG_EOL = "$";

        public virtual string Name
        {
            get
            {
                return "Default Minecraft Server";
            }
        }

        public virtual string Site
        {
            get
            {
                return "http://www.minecraft.net";
            }
        }

        public virtual Image Logo
        {
            get
            {
                return Properties.Resources.vanilla_logo;
            }
        }

        public virtual bool SupportsPlugins
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsLocal
        {
            get
            {
                return true;
            }
        }

        public virtual void PrepareLaunch()
        {
        }

        public virtual bool HasCustomAssembly
        {
            get
            {
                return false;
            }
        }

        public virtual Assembly CustomAssembly
        {
            get
            {
                return null;
            }
        }

        public virtual string GetLaunchParameters(string defaultParameters = "")
        {
            return defaultParameters;
        }

        public virtual string GetLaunchFlags(string defaultFlags = "")
        {
            return defaultFlags;
        }

        public virtual bool HasCustomSettingsControl
        {
            get
            {
                return false;
            }
        }

        public virtual UserControl CustomSettingsControl
        {
            get
            {
                return null;
            }
        }

        public virtual OutputParseResult ParseOutput(string text)
        {
            string message = this.ParseMessage(text);

            MessageType type = this.ParseMessageType(message);

            // In case of a player action, also provide the info 
            switch (type)
            {
                case MessageType.PlayerJoin:
                    return new OutputParseResult(text, message, type, this.ParsePlayerJoin(text));
                case MessageType.PlayerLeave:
                    return new OutputParseResult(text, message, type, this.ParsePlayerLeave(text));
                case MessageType.PlayerKick:
                    return new OutputParseResult(text, message, type, this.ParsePlayerActionKick(text));
                case MessageType.PlayerBan:
                    return new OutputParseResult(text, message, type, this.ParsePlayerActionBan(text));
                default:
                    return new OutputParseResult(text, message, type);
            }
        }

        public string ParseMessage(string text)
        {
            Logger.Log(LogLevel.Debug, "MinecraftServerBase", "Parsing message for \"" + text + "\"");
            text = this.RemoveTimeStamp(text); // We need to know the type, so we'll continue without the timestamp
            Logger.Log(LogLevel.Debug, "MinecraftServerBase", "Removed timestamp: \"" + text + "\"");
            text = this.FilterText(text);
            Logger.Log(LogLevel.Debug, "MinecraftServerBase", "Filtered text: \"" + text + "\"");
            return text;
        }

        public virtual MessageType ParseMessageType(string text)
        {
            MessageType type = MessageType.Unknown;

            Logger.Log(
                LogLevel.Debug,
                "MinecraftServerBase",
                "login regex",
                RG_INFO + RG_SPACE + RG_PLAYER + RG_IP_BRACKET + " logged in with entity id");
            //[WARNING]...
            if (Regex.IsMatch(text, RG_WARN + RG_WILDCARD, RegexOptions.IgnoreCase))
            {
                type = MessageType.Warning;
            }
                //[SEVERE] ...
            else if (Regex.IsMatch(text, RG_SEV + RG_WILDCARD, RegexOptions.IgnoreCase))
            {
                type = MessageType.Severe;
            }



                // LOGIN
                //[INFO] Bertware[/127.0.0.1:58189] logged in with entity id 27 at ([world] -1001.0479985318618, 2.0, 1409.300000011921)
                //[INFO] Bertware[/127.0.0.1:58260] logged in with entity id 0 at (-1001.0479985318618, 2.0, 1409.300000011921)
                //[INFO]  UUID of player Bertware is f0b27a3369394b25ab897aa4e4db83c1
                //[INFO]  Bertware[/127.0.0.1:51815] logged in with entity id 184 at ([world] 98.5, 64.0, 230.5)

            else if (Regex.IsMatch(
                text,
                RG_INFO + RG_SPACE + RG_PLAYER + RG_IP_BRACKET + " logged in with entity id",
                RegexOptions.IgnoreCase))
            {
                type = MessageType.PlayerJoin;
            }

                // Disconnect / leave
                //[INFO]  Bertware lost connection: Disconnected
                //[INFO]  Bertware left the game.
            else if (Regex.IsMatch(
                text,
                RG_INFO + RG_SPACE + RG_PLAYER + " lost connection: Disconnected",
                RegexOptions.IgnoreCase))
            {
                type = MessageType.PlayerLeave;
            }
                // Disconnect / kick
                //[INFO]  Bertware lost connection: test
                //[INFO]  Bertware left the game.
                //[INFO]  CONSOLE: Kicked player Bertware. With reason:
                //test
                //
                // [command sender]: Kicked player [player]. With reason: [newline] [Reason]
            else if (Regex.IsMatch(
                text,
                RG_INFO + RG_SPACE + RG_PLAYER + " lost connection: Disconnected",
                RegexOptions.IgnoreCase))
            {
                type = MessageType.PlayerKick;
            }
                //disconnect / ban
                //[INFO]  Bertware lost connection: Banned by admin.
                //[INFO]  Bertware left the game.
                //[INFO]  CONSOLE: Banned player bertware
            else if (Regex.IsMatch(
                text,
                RG_INFO + RG_SPACE + RG_PLAYER + " lost connection: Banned by",
                RegexOptions.IgnoreCase))
            {
                type = MessageType.PlayerBan;
            }
                // catch player left the game message
            else if (Regex.IsMatch(
                text,
                RG_INFO + RG_SPACE + RG_PLAYER + " left the game.",
                RegexOptions.IgnoreCase))
            {
                type = MessageType.PlayerLeave;
            }


                //TODO: Stacktraces and java errors

                // all other text is info text
            else if (Regex.IsMatch(text, RG_INFO + ".*", RegexOptions.IgnoreCase))
            {
                type = MessageType.Info;
            }

            return type;
        }

        public virtual string RemoveTimeStamp(string text)
        {
            //2014-01-01 00:00:00,000
            text = Regex.Replace(text, "\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}(,\\d{3}|)\\s*", "");
            //[11:36:21]
            text = Regex.Replace(text, "\\[\\d\\d:\\d\\d:\\d\\d\\]", "");
            //[11:36:21 INFO]
            text = Regex.Replace(text, "\\[\\d\\d:\\d\\d:\\d\\d ", "[");
            text = text.Trim();
            return text;
        }

        public virtual string FilterText(string text)
        {
            text = text.Replace("Server thread/", "");
            text = Regex.Replace(text, "\\[minecraft(-server|)\\]", "", RegexOptions.IgnoreCase);
            // remove [minecraft] or [minecraft-server] tags, for better parsing
            text = Regex.Replace(text, "\\]:", "]");
            text = text.Trim();
            return text;
        }

        public virtual PlayerActionJoin ParsePlayerJoin(string text)
        {
            //[INFO] Bertware[/127.0.0.1:58189] logged in with entity id 27 at ([world] -1001.0479985318618, 2.0, 1409.300000011921)
            //[INFO] Bertware[/127.0.0.1:58260] logged in with entity id 0 at (-1001.0479985318618, 2.0, 1409.300000011921)
            //[INFO]  UUID of player Bertware is f0b27a3369394b25ab897aa4e4db83c1
            //[INFO]  Bertware[/127.0.0.1:51815] logged in with entity id 184 at ([world] 98.5, 64.0, 230.5)
            PlayerActionJoin join = new PlayerActionJoin();
            join.PlayerName = Regex.Match(text, RG_FSPACE + RG_PLAYER).Value.Trim();
            join.Ip = Regex.Match(text, RG_IP_NOPORT).Value;

            return join;
        }

        public virtual PlayerActionLeave ParsePlayerLeave(string text)
        {
            //[INFO]  Bertware lost connection: Disconnected
            //[INFO]  Bertware left the game.
            PlayerActionLeave leave = new PlayerActionLeave();
            leave.PlayerName = Regex.Match(text, RG_FSPACE + RG_PLAYER).Value.Trim();
            leave.Details = Regex.Match(text, ":" + RG_WILDCARD + RG_EOL).Value.TrimStart(':').Trim();
            return leave;
        }

        public virtual PlayerActionKick ParsePlayerActionKick(string text)
        {
            //[INFO]  Bertware lost connection: test
            //[INFO]  Bertware left the game.
            //[INFO]  CONSOLE: Kicked player Bertware. With reason:
            //test
            PlayerActionKick leave = new PlayerActionKick();
            leave.PlayerName = Regex.Match(text, RG_FSPACE + RG_PLAYER).Value.Trim();
            leave.Details = Regex.Match(text, ":" + RG_WILDCARD + RG_EOL).Value.TrimStart(':').Trim();
            return leave;
        }

        public virtual PlayerActionBan ParsePlayerActionBan(string text)
        {
            //[INFO]  Bertware lost connection: Banned by admin.
            //[INFO]  Bertware left the game.
            //[INFO]  CONSOLE: Banned player bertware
            PlayerActionBan leave = new PlayerActionBan();
            leave.PlayerName = Regex.Match(text, RG_FSPACE + RG_PLAYER).Value.Trim();
            leave.Details = Regex.Match(text, ":" + RG_WILDCARD + RG_EOL).Value.TrimStart(':').Trim();
            return leave;
        }

        public virtual PlayerActionIpBan ParsePlayerActionIpBan(string text)
        {
            return new PlayerActionIpBan();
        }

        public virtual bool CanFetchRecommendedVersion
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanFetchBetaVersion
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanFetchDevVersion
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanDownloadRecommendedVersion
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanDownloadBetaVersion
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanDownloadDevVersion
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanGetCurrentVersion
        {
            get
            {
                return false;
            }
        }

        public virtual string FetchRecommendedVersion
        {
            get
            {
                return null;
            }
        }

        public virtual string FetchBetaVersion
        {
            get
            {
                return null;
            }
        }

        public virtual string FetchDevVersion
        {
            get
            {
                return null;
            }
        }

        public virtual bool DownloadRecommendedVersion(string targetfile)
        {
            return false;
        }

        public virtual bool DownloadBetaVersion(string targetfile)
        {
            return false;
        }

        public virtual bool DownloadDevVersion(string targetfile)
        {
            return false;
        }

        public virtual string GetCurrentVersion(string file)
        {
            throw new NotImplementedException();
        }
    }
}