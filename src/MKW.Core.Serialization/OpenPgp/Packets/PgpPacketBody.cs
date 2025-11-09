// Copyrighpublic t (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public abstract record class PgpPacketBody
    {
        public interface IVisitor<T>
        {
            T VisitPublicKeyEncryptedSessionKeyV3(PublicKeyEncryptedSessionKeyV3 packetBody);
            T VisitPublicKeyPacketV4(PublicKeyPacketV4 publicKeyPacketV4);
            T VisitSecretKeyPacketV4(SecretKeyPacketV4 packetBody);
            T VisitSignaturePacketV4(SignaturePacketV4 packetBody);
            T VisitSymEncryptedProtectedDataV1(SymEncryptedProtectedDataV1 packetBody);
            T VisitUserIdPacket(UserIdPacket packetBody);
        }

        public abstract T Visit<T>(IVisitor<T> visitor);
    }
}
