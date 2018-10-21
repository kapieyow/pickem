using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.App
{
    public static class Consts
    {
        internal const string CLAIM_GOD = "God";
        internal const string SECRET_KEY = "FJ(&4rjiklfvawse9745r2jkifgdsJFJF"; // TODO: get this from somewhere secure
        internal const int WEB_SOCKET_BUFFER_SIZE = 8192;
        internal const int WEB_SOCKET_KEEP_ALIVE_SECONDS = 30;
    }
}
