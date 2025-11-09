// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class UserIdPacket : PgpPacketBody
    {
        public ReadOnlyMemory<byte> Content { get; }

        public UserIdPacket(ReadOnlyMemory<byte> content)
        {
            Content = content;
        }

        public UserIdPacket(string content)
        {
            Content = Encoding.UTF8.GetBytes(content);
        }

        public string GetString()
        {
            return Encoding.UTF8.GetString(Content.ToArray());
        }

        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUserIdPacket(this);
        }
    }
}
