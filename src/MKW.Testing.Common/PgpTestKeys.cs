// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Testing.Common
{
    public static class PgpTestKeys
    {
        // RSA 2048, no passprase
        // generated with `gpg --full-generate-key`
        // export with: `gpg --export --armor`
        // for private keys: `gpg --export-secret-subkeys --armor`

        public const string TestPublicKey = "pgpdata/public_key_plaintext.asc";

        public const string TestPrivateKeyEncrypted =
            "-----BEGIN PGP PRIVATE KEY BLOCK-----\n" +
            "\n" +
            "lQPGBGjxApEBCADNBaMuxtxHAxGXCr5wQYsJaQKJrnaD033TQzefmzjUK9XnIv4g\n" +
            "qeBz0AtEOYO5RPPbKavQUn8SWifCAbFAuNZg8C2C1hlF9NB68Y6d2shzk3QvZjqE\n" +
            "SUAiPqzI0RQtdIJjWHhU1OTNNpxEOXKoTaf1Bxdouf2aDuy2eAIuKBqBlHTmoQjE\n" +
            "BURK51ZOro/RImga3D4K5ihDVhmx7ca/rUV598vSAcMwDM/QfMu/pbeyxRaoA2DB\n" +
            "NdPju1XCMxRofuHUtgjGfgeklPhCCKYd84vOOjJ1gK24oNu28ouEY/UPaa9mOwWE\n" +
            "V49/ZLK6TQSlnJwgjzl6JW/6j8yNLAP8CwuBABEBAAH+BwMCL8f1wKKCTo//PzYE\n" +
            "JQO+IqmXza+iqUOQlnAV+TNCXM8X/yDYmA5aFhMo5cPif+TeWwNqgBcFNsbem5UV\n" +
            "lXzGTu4gqv4OiB6tmDOlzd81OKo1nvFOoHWy0Ov+QhwPPf0BpmhPxsTO02Hfw1KY\n" +
            "YPyCwRSxCvP7B+vD33thxLV7wuWUUAJE7SC3DXQOBYpbuE3VfmJDjMOPV1u93qxu\n" +
            "RV9yGyo6aoEH7CvXxndsw0iIYhSosX7l/oZawDF9sTH763S0/V5dPsMQjCikuJPk\n" +
            "h+2i+SKa8QQ1vU/1xDzZPu69I7UHIBxABLgL9nECD9QQB9sAE0TSvnLe2kyPjyyS\n" +
            "lPG99CgqG3sBg22l9OmL9PuI3BHXhdSsxe/7Al5ANdMB36zkMCnpV4W0f9ZBgOSC\n" +
            "ITyjYcG+zsq0FxGc3PTsHqYKlWteGdc5T66BjWOiAXIjP80NPWYNOw1gBO/YzFxF\n" +
            "EFZmB4yNhmY4UlVbm8t8VvfwYBZEn1Vy1aPUazJcLF/RNRtkKfptcJHlDnCUCibV\n" +
            "wZnDB01xOCOis86kqvZX8VMa4ca2w4WGTt3VUoLdfhSeZ57eHk2ItBnLnmJe6KeM\n" +
            "m+5p4OJgi4nW21F2LBNnl/FSnm2lGvGr/0D/V7H9WgELuJE0kPF141twmjyQL0lC\n" +
            "LwgJsAxKpUHvTtfzXZJ/Dhfus+UC1L5vO3gdiAXn2S83sAk39P4AEzS/QSp1YuW4\n" +
            "Bc0noKEYY4HxFQ7ANBsxSfFTi6vKv7bWXfCEPini1ziuIJFuz6dEOGAI/7Ix4k5u\n" +
            "g0C9J0hwSjAUtMyYwRK1jaN2IfM9o6JUBJPluQn6XntWT9JdTVStZ029MzJjHAg6\n" +
            "yvOsHhPyCHjw+MTxKTGvuWyv1YCa2p19b79+442xsXZmKkWLEtzJlWlEttNKwZrG\n" +
            "I29PFajYVbmxtDV1d3UtdGVzdC1pZCAodXNlIGZvciBta3cgdGVzdGluZykgPHRl\n" +
            "c3RAdXd1LW1haWwuY29tPokBTgQTAQoAOBYhBNCMigOB586o3zrdMy1kWd5jhplb\n" +
            "BQJo8QKRAhsDBQsJCAcCBhUKCQgLAgQWAgMBAh4BAheAAAoJEC1kWd5jhplbEXMH\n" +
            "/0LLLzX3qejlatYX1jZL0QoPT2xBvIcp5YzZUduIWDDIRX4ajURXDmnG9TbZ+9hJ\n" +
            "IZnwO4jCLzj4pgafBETjJFKJaNddtMHxihEnNwQB1FPVghDLVVDLewY6FbY4XdPJ\n" +
            "EjggHtR0E9kBxguyj5lUf7JUMPXFzgMp/EVJHYV++c+tRBy6nsGNryoYZWImZNAv\n" +
            "YAPNemwjpB/TmmZePaBDDk2hZVambz4J0IAgXsYwQdaWN4CGe/YDX31Ki+LICMQ1\n" +
            "TLjvgaEywxma5zL8K35ZuYqw2K8Z/ZBKBo9ptAZXsmN5FPawxFpMimTqq//iVwLu\n" +
            "66FkzEKP4kGIDpASBdZcXEedA8YEaPECkQEIAOpOBOfml8dSMVfRg4U3QC3Eyc+g\n" +
            "r+8+vzhlGYCmCe7lf/xHHTJO2XNxW8heu0Yni0NkSxiax2rPc/3OZqQMp9YEU76A\n" +
            "p+DNB8OhJ3l8RBN5OvJGK8YoNJywNV89ZSg2L7qvtqAqjPNEcWJOn1fHaNcDPBbv\n" +
            "y/z8RXHOfjXO843iF0NQPJGrWKBd1mrk6ByAAk21Mg2zwMphYtye8lP4xwd3UR0l\n" +
            "GMxO2NqgbFaMNJO0PJpldEl7mU6lBY/WwuMSiqOq4Fnw+b7V5YJJF0FeGCeiF62j\n" +
            "SJoabNCkwwwqgAxm7+/5rrgz8L/wxs4CZzmZhR/eP9/70ykTH20lOrX23QEAEQEA\n" +
            "Af4HAwKW6keEUSsWL/+079MJ1TQF4UxZouew7l3XARRsAMDTfC0e/kg8iSxenk26\n" +
            "0wngPK35Yd5OAZDTqxPDPU3t6Ndth8vTgickr1nze3c9Zh97nFAdxw4kGN8TJcq1\n" +
            "gHcK9kf6NeF8GyZ5YygOhcBBTxx2qq5vNsAw2kjvSVY5vKlCT7xD575JEp/wLw2z\n" +
            "kEJ5irMrx8ODoNNLkfBBp1z/sP8fnr2FQqm2j6s2H4OoVcd5g5q2CXJd89Ogj6gJ\n" +
            "P/FI/JGDNJNF5V+9KLz9VgqyLpFbD0jU78qfmDwLt9qzLnyg/xcnC/uAZUZ6R/Ua\n" +
            "YY+JL06vWHFD1vYvqmUGxHGgqoYFI6KzYH8ssfRNYtomNcheWdHD33AC30lMiCUI\n" +
            "92GH5Ecpf4XYJJTr08fz3Mw2qylecFw95ZtkiTpubaHQYTxgrPcfWJ+COtHuuN0y\n" +
            "zkv5XccVzN/JKLBXvch8/9IuRukTuc0ss3R/dgVqc6oBWhIQ2gTHpZPW3UZrGEq2\n" +
            "TDeQ0kXaftRYTbhAuWJraCaWwdEnTENC5xsNDW5qVGVuuNsS0dh2lBGW0MbuxNvg\n" +
            "/IzNWqyb1iufwaA56qhspaOvaHq6OOcV9jMKl7A07b95atwTE/DdwdnEs+CRr3Jc\n" +
            "qbQlCLpvhxbTnEjhEpgbukrdm45InFWj/PbCDGY1x+ouXTpvAgsJ2T7FSoPmwUhS\n" +
            "cKgLW5223JRKSGGL5Ik7n+/CBqfxrV75iBd6TrwFW/uYbnr8kaBHutPdsF1GvG3n\n" +
            "vXCUgPSHR3p0bBAKJQvigbgcNUZ5VN7uZLQ+TaomkdMuiUZ3Ixxn1iaKIvmMNYXE\n" +
            "cKk13dSO9zTCvYTtPJbNWrcc8OdIrzBBpOoUb8QbPi6I8MQ8l1s2oPNrMoPZbOwt\n" +
            "EuGgKM9x7ZX6nsAYsNCDF8mYLOCBxpgEbrqJATYEGAEKACAWIQTQjIoDgefOqN86\n" +
            "3TMtZFneY4aZWwUCaPECkQIbDAAKCRAtZFneY4aZW6sUB/0Urw1yHH81j8v4Qq6O\n" +
            "2qqVEwshyqwD14myhEow8qSMMbOEh8i2DIYFSEWw/SBoEqOwX8+EF0uR3SEsruj2\n" +
            "1CAWrpWZBJWZiegwPaz84kAHHzsexQMtgPk4cusdaJORqGBs6TSAo7mg1hGo6rA7\n" +
            "1dKwxdId65xaB2kmPo5ejkOJ0PQUgRPfYmAzp2AY8B+6CQ23URZxtpgF+9qIru02\n" +
            "82XQSIcxmpu81X2EhT72wazbk4OInJAAOGQYB87+zjV9ImjnDdplB8ftU6UUcKrA\n" +
            "q4y7ekh8SDjo5tW4boM9KS2ly5bGFd1n3MBcZiXDOqnpowe2sDQ31de0KLGam0TD\n" +
            "dfXU\n" +
            "=f8YI\n" +
            "-----END PGP PRIVATE KEY BLOCK-----\n";

        public const string TestPrivateKey = "pgpdata/secret_key_plaintext.asc";

        public const string PublicKeyEncryptedMessageAes256NoCompression = "pgpdata/message_pubkey_aes256_nocompression.asc";
        public const string PublicKeyEncryptedMessageAes128NoCompression = "pgpdata/message_pubkey_aes128_nocompression.asc";

        // passphrase: 123
        // string-to-key iterations: 1024
        // gpg --symmetric --armor --s2k-count 1024 test.txt
        public const string SymmetricallyEncryptedMessage = "pgpdata/secret_key_plaintext.asc";
    }
}
