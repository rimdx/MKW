using MKW.Core;
using MKW.Core.Serialization.Pgp;
using MKW.Cryptography;
using MKW.Storage.MKGP;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace MKW.Storage.Tests
{
    public class MKGPLowLevelTests
    {
        [Test]
        public void SimpleEntryEncode()
        {
            using MemoryStream stream = new MemoryStream();

            using ArmoredOutputStream armor = new ArmoredOutputStream(stream);
            using PgpOutputStream pgp = new PgpOutputStream(armor);

            DatabaseEntry dbEntry = new DatabaseEntry
            {
                Id = EntryId.Create(),
                Data = new byte[128],
                Salt = new byte[16],
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            EntryObject entry = new EntryObject(dbEntry);

            entry.Encode(pgp);
            pgp.Close();

            Console.WriteLine(EncodingConverter.GetString(stream.ToArray()));
        }

        [Test]
        public void SimpleEntryDecode()
        {
            using MemoryStream stream = new MemoryStream(
                Convert.FromBase64String(
                    "hQIMA87HrhcaeKn3AQ//TtsqaTQPGbNTPPuH0832+xVobQ8HYMSJeo3jXRh8Umjo49ZE85tmG93x" +
                    "jnGrGzIygEQqUIF543hbxT1G4mXRU1gLmx0v4d0B5pH/3azKecB6PiRo4CBLRGJLhSQbRA45/9+C" +
                    "+bjzE+v4NZ7qrXAN/PsRR+rkK+SUKqQxI3fifXI61/xiY7OkdrMb6xSnAnBNhs/U1+Iqxl6+nwjl" +
                    "Dok0UXVS6meeWPS7FwS5i2LQkzCEuPcEPZxiH80uxuXGYKjYQnDzydniypukSs1P2ZXSJW59oL9W" +
                    "jCYjjnLRL3zuL8mMa9wn+iWF7qSE7Wk9q8L84FV47etUbQw+1m5iVWVWg6QRLOaUE6Q5ja4GIK1S" +
                    "fx5MLGokd82QDzFjvfu33m4GDoa+eq1T3OIe1izLsvtR6guuTruq1h/5GfkMei3O7iADQ/ZvU5Ww" +
                    "CHqFM8I6SpCbsgoOgF2YOCMBaDKx0w6vyTzIZ/EFk3zMLi6aZvi1AxcrxzHxcgTcYjbuwAMB8gRb" +
                    "mzb87rRt7l0PsXOn10m/iQYSi3HrHKulRRhNQVFOWAW51tXR2RcmtqBMwh1D1HeX5ox88LlNUQO3" +
                    "Hw6+KqAAiayEcBX8kVAKu88JSeLTmDi22JxucqQ9iJAruzeNtGHcKYnn9X0K36UdiEQA+ipsgi4h" +
                    "7gG2gFwfczFUgS9Mx37SdwEo2P/dRNKYAlOCKMFO71/y9LGCqS4Ep5t2HuB2SqZi6GAliadRxi9V" +
                    "+6mamRhDv9PcDRc/NH2V6M4ip/m7eqpTwxGVrSotn4fI/ouVGgcNO447UpbHaiav7D1CETP5vykm" +
                    "JoPWR/eBc2sju6nuENtcYFc+icy7"
            ));

            PgpObjectFactory bcpg = new PgpObjectFactory(stream);

            PgpEncryptedDataList obj = (PgpEncryptedDataList)bcpg.NextPgpObject();
        }
    }
}
