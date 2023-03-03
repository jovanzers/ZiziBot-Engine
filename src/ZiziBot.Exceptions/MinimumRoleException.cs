﻿using System.Runtime.Serialization;

namespace ZiziBot.Exceptions;

[Serializable()]
public class MinimumRoleException<T> : Exception where T : class
{
    protected MinimumRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public MinimumRoleException(long userId, long chatId, RoleLevel roleLevel) :
        base($"Minimum Role {roleLevel} for Request: {typeof(T)} isn't meet for {userId} in ChatId: {chatId}")
    {
    }
}