﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Convai.RestAPI.Internal.PostProcessors
{

#nullable enable
    public static partial class RequestPostProcessor
    {
        public static bool ProcessRoomDetails(string result, out RoomDetails? roomDetails)
        {
            roomDetails = JsonConvert.DeserializeObject<RoomDetails>(result);
            return roomDetails != null;
        }
    }

}
