﻿namespace Bukkitgui2.MinecraftInterop.OutputHandler
{
    /// <summary>
    /// The type of output from a minecraft server
    /// </summary>
	public enum MessageType
	{
	    /// <summary>
	    /// Informative
	    /// </summary>
	    Info=10,

        /// <summary>
        /// Warning
        /// </summary>
		Warning=11,

        /// <summary>
        /// Severe/Error
        /// </summary>
		Severe=12,

        /// <summary>
        /// Player joined
        /// </summary>
		PlayerJoin=20,

        /// <summary>
        /// Player left or disconnected
        /// </summary>
		PlayerLeave=21,

        /// <summary>
        /// Player kicked
        /// </summary>
		PlayerKick=22,

        /// <summary>
        /// Player banned
        /// </summary>
		PlayerBan=23,

        /// <summary>
        /// Ip banned
        /// </summary>
		PlayerIpBan=24,

        /// <summary>
        /// Java stacktrace
        /// </summary>
        JavaStackTrace = 30,

        /// <summary>
        /// Java VM messages
        /// </summary>
        JavaStatus = 31,

        /// <summary>
        /// Output from /list command
        /// </summary>
		PlayerList=40,

        /// <summary>
        /// Uknown text
        /// </summary>
		Unknown=0
	}
}
