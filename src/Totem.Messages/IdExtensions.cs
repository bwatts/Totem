using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Totem
{
    public static class IdExtensions
    {
        public static Id ToId(this Guid guid) =>
            Id.From(guid);

        public static bool TryToId(this Guid guid, [MaybeNullWhen(false)] out Id id) =>
            Id.TryFrom(guid, out id);

        public static Ids ToIds(this IEnumerable<Id> ids) =>
            new(ids);
    }
}