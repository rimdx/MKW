// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.Tests
{
    public static class PgpTestKeys
    {
        // RSA 2048, no passprase
        // generated with `gpg --full-generate-key`
        // export with: `gpg --export --armor`
        // for private keys: `gpg --export-secret-subkeys --armor`

        public const string TestPublicKey =
            "-----BEGIN PGP PUBLIC KEY BLOCK-----\n" +
            "\n" +
            "mQENBGjxApEBCADNBaMuxtxHAxGXCr5wQYsJaQKJrnaD033TQzefmzjUK9XnIv4g\n" +
            "qeBz0AtEOYO5RPPbKavQUn8SWifCAbFAuNZg8C2C1hlF9NB68Y6d2shzk3QvZjqE\n" +
            "SUAiPqzI0RQtdIJjWHhU1OTNNpxEOXKoTaf1Bxdouf2aDuy2eAIuKBqBlHTmoQjE\n" +
            "BURK51ZOro/RImga3D4K5ihDVhmx7ca/rUV598vSAcMwDM/QfMu/pbeyxRaoA2DB\n" +
            "NdPju1XCMxRofuHUtgjGfgeklPhCCKYd84vOOjJ1gK24oNu28ouEY/UPaa9mOwWE\n" +
            "V49/ZLK6TQSlnJwgjzl6JW/6j8yNLAP8CwuBABEBAAG0NXV3dS10ZXN0LWlkICh1\n" +
            "c2UgZm9yIG1rdyB0ZXN0aW5nKSA8dGVzdEB1d3UtbWFpbC5jb20+iQFOBBMBCgA4\n" +
            "FiEE0IyKA4HnzqjfOt0zLWRZ3mOGmVsFAmjxApECGwMFCwkIBwIGFQoJCAsCBBYC\n" +
            "AwECHgECF4AACgkQLWRZ3mOGmVsRcwf/QssvNfep6OVq1hfWNkvRCg9PbEG8hynl\n" +
            "jNlR24hYMMhFfhqNRFcOacb1Ntn72EkhmfA7iMIvOPimBp8EROMkUolo1120wfGK\n" +
            "ESc3BAHUU9WCEMtVUMt7BjoVtjhd08kSOCAe1HQT2QHGC7KPmVR/slQw9cXOAyn8\n" +
            "RUkdhX75z61EHLqewY2vKhhlYiZk0C9gA816bCOkH9OaZl49oEMOTaFlVqZvPgnQ\n" +
            "gCBexjBB1pY3gIZ79gNffUqL4sgIxDVMuO+BoTLDGZrnMvwrflm5irDYrxn9kEoG\n" +
            "j2m0BleyY3kU9rDEWkyKZOqr/+JXAu7roWTMQo/iQYgOkBIF1lxcR7kBDQRo8QKR\n" +
            "AQgA6k4E5+aXx1IxV9GDhTdALcTJz6Cv7z6/OGUZgKYJ7uV//EcdMk7Zc3FbyF67\n" +
            "RieLQ2RLGJrHas9z/c5mpAyn1gRTvoCn4M0Hw6EneXxEE3k68kYrxig0nLA1Xz1l\n" +
            "KDYvuq+2oCqM80RxYk6fV8do1wM8Fu/L/PxFcc5+Nc7zjeIXQ1A8katYoF3WauTo\n" +
            "HIACTbUyDbPAymFi3J7yU/jHB3dRHSUYzE7Y2qBsVow0k7Q8mmV0SXuZTqUFj9bC\n" +
            "4xKKo6rgWfD5vtXlgkkXQV4YJ6IXraNImhps0KTDDCqADGbv7/muuDPwv/DGzgJn\n" +
            "OZmFH94/3/vTKRMfbSU6tfbdAQARAQABiQE2BBgBCgAgFiEE0IyKA4HnzqjfOt0z\n" +
            "LWRZ3mOGmVsFAmjxApECGwwACgkQLWRZ3mOGmVurFAf9FK8Nchx/NY/L+EKujtqq\n" +
            "lRMLIcqsA9eJsoRKMPKkjDGzhIfItgyGBUhFsP0gaBKjsF/PhBdLkd0hLK7o9tQg\n" +
            "Fq6VmQSVmYnoMD2s/OJABx87HsUDLYD5OHLrHWiTkahgbOk0gKO5oNYRqOqwO9XS\n" +
            "sMXSHeucWgdpJj6OXo5DidD0FIET32JgM6dgGPAfugkNt1EWcbaYBfvaiK7tNvNl\n" +
            "0EiHMZqbvNV9hIU+9sGs25ODiJyQADhkGAfO/s41fSJo5w3aZQfH7VOlFHCqwKuM\n" +
            "u3pIfEg46ObVuG6DPSktpcuWxhXdZ9zAXGYlwzqp6aMHtrA0N9XXtCixmptEw3X1\n" +
            "1A==\n" +
            "=pK7/\n" +
            "-----END PGP PUBLIC KEY BLOCK-----\n";

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

        public const string TestPrivateKey =
            "-----BEGIN PGP PRIVATE KEY BLOCK-----\n" +
            "\n" +
            "lQEVBGjxApEBCADNBaMuxtxHAxGXCr5wQYsJaQKJrnaD033TQzefmzjUK9XnIv4g\n" +
            "qeBz0AtEOYO5RPPbKavQUn8SWifCAbFAuNZg8C2C1hlF9NB68Y6d2shzk3QvZjqE\n" +
            "SUAiPqzI0RQtdIJjWHhU1OTNNpxEOXKoTaf1Bxdouf2aDuy2eAIuKBqBlHTmoQjE\n" +
            "BURK51ZOro/RImga3D4K5ihDVhmx7ca/rUV598vSAcMwDM/QfMu/pbeyxRaoA2DB\n" +
            "NdPju1XCMxRofuHUtgjGfgeklPhCCKYd84vOOjJ1gK24oNu28ouEY/UPaa9mOwWE\n" +
            "V49/ZLK6TQSlnJwgjzl6JW/6j8yNLAP8CwuBABEBAAH/AGUAR05VAbQ1dXd1LXRl\n" +
            "c3QtaWQgKHVzZSBmb3IgbWt3IHRlc3RpbmcpIDx0ZXN0QHV3dS1tYWlsLmNvbT6J\n" +
            "AU4EEwEKADgWIQTQjIoDgefOqN863TMtZFneY4aZWwUCaPECkQIbAwULCQgHAgYV\n" +
            "CgkICwIEFgIDAQIeAQIXgAAKCRAtZFneY4aZWxFzB/9Cyy8196no5WrWF9Y2S9EK\n" +
            "D09sQbyHKeWM2VHbiFgwyEV+Go1EVw5pxvU22fvYSSGZ8DuIwi84+KYGnwRE4yRS\n" +
            "iWjXXbTB8YoRJzcEAdRT1YIQy1VQy3sGOhW2OF3TyRI4IB7UdBPZAcYLso+ZVH+y\n" +
            "VDD1xc4DKfxFSR2FfvnPrUQcup7Bja8qGGViJmTQL2ADzXpsI6Qf05pmXj2gQw5N\n" +
            "oWVWpm8+CdCAIF7GMEHWljeAhnv2A199SoviyAjENUy474GhMsMZmucy/Ct+WbmK\n" +
            "sNivGf2QSgaPabQGV7JjeRT2sMRaTIpk6qv/4lcC7uuhZMxCj+JBiA6QEgXWXFxH\n" +
            "nQOYBGjxApEBCADqTgTn5pfHUjFX0YOFN0AtxMnPoK/vPr84ZRmApgnu5X/8Rx0y\n" +
            "TtlzcVvIXrtGJ4tDZEsYmsdqz3P9zmakDKfWBFO+gKfgzQfDoSd5fEQTeTryRivG\n" +
            "KDScsDVfPWUoNi+6r7agKozzRHFiTp9Xx2jXAzwW78v8/EVxzn41zvON4hdDUDyR\n" +
            "q1igXdZq5OgcgAJNtTINs8DKYWLcnvJT+McHd1EdJRjMTtjaoGxWjDSTtDyaZXRJ\n" +
            "e5lOpQWP1sLjEoqjquBZ8Pm+1eWCSRdBXhgnoheto0iaGmzQpMMMKoAMZu/v+a64\n" +
            "M/C/8MbOAmc5mYUf3j/f+9MpEx9tJTq19t0BABEBAAEAB/kBmRyhdEouutnJTo2G\n" +
            "GBVg+omh1+e7SwNE4DOnU/qXXYtc7iM915nWFrzYhgTi+pwRjpqQhWW8zcNtxL2g\n" +
            "etGePRNRJlF+0Acwh2Xch0Nzmo0TX/Umedm6A92pU6Lf/lafAFnPh9rEQgA/+mdN\n" +
            "0vddGBGN2n7ar+HNX+oudcXlftTwfOt+rJZOpa9UFKe94TP8RCrYAhJaYNnEhvKz\n" +
            "L/K+NpgYhYXMfxRrNVD4zg8pVpRCrZ7lKyhmUne3itLg8YTu9kD36oOXjecYV9Hs\n" +
            "7dmu1LmyzzewzynECaJJ9UFuQUSYoGFZzGqQN6LC6S8Qs9EK1GAb93m6MXHZy5KT\n" +
            "+kv9BADzPrc4Whct3tBHpb+qI541TD/mkvhADYoGnfCz/Kw/eVs0/UgMhZmBRkQ/\n" +
            "nXEupGYJTTOu6Su7rYZdLJDX2koQWrSyOi78Yf+V4FUUzHT9J3yFlSsn3tSwiK7Z\n" +
            "Aki888dPtA8iOtDeOX4M6RH7c8dTABAYjlQYEOkAsEEom6z27wQA9pdKkMNFt4H8\n" +
            "rUIBQ2SVIqSXmB8l0c3n4p1XD9/OAE3GBt44f2EfV2ohp8kfn8PtpIzVPJ606M8D\n" +
            "JTtyWSCb4IYiUYjcCxGHenHk9yVuxLRQyw9aoiXhLrUeY6+nsjH+x8+wJUzXH+hE\n" +
            "WV3PylWjYUnyROdemX6UnlBoJvpJ6w8D/RrLJhn64wz6g2eCMJN25ApO5awH/F/d\n" +
            "EXP/k7XMZVBBRdPHB/+TPL44bMt055kiGY3NnEkgs6co0NbOZZHJL0HscPwkJ/AE\n" +
            "Fp8KxpOcJARJX1uw9txE4/FdFnVfYRhUEWT7Q+2M5KhTzos3dbK0YFJTNgbxU7NQ\n" +
            "h4/WW/4a9ifQSkKJATYEGAEKACAWIQTQjIoDgefOqN863TMtZFneY4aZWwUCaPEC\n" +
            "kQIbDAAKCRAtZFneY4aZW6sUB/0Urw1yHH81j8v4Qq6O2qqVEwshyqwD14myhEow\n" +
            "8qSMMbOEh8i2DIYFSEWw/SBoEqOwX8+EF0uR3SEsruj21CAWrpWZBJWZiegwPaz8\n" +
            "4kAHHzsexQMtgPk4cusdaJORqGBs6TSAo7mg1hGo6rA71dKwxdId65xaB2kmPo5e\n" +
            "jkOJ0PQUgRPfYmAzp2AY8B+6CQ23URZxtpgF+9qIru0282XQSIcxmpu81X2EhT72\n" +
            "wazbk4OInJAAOGQYB87+zjV9ImjnDdplB8ftU6UUcKrAq4y7ekh8SDjo5tW4boM9\n" +
            "KS2ly5bGFd1n3MBcZiXDOqnpowe2sDQ31de0KLGam0TDdfXU\n" +
            "=PJs0\n" +
            "-----END PGP PRIVATE KEY BLOCK-----\n";

        public const string PublicKeyEncryptedMessage =
            "-----BEGIN PGP MESSAGE-----\n" +
            "\n" +
            "hQEMA5ULd/u5gOShAQf+IhQ5xU/v8YFB0AbddOlakSJyRISkSD8wi+B1swbK5KZI\n" +
            "KrA10UGRTUdJtsKgel6xKokRCvp2nzZLU+/tb6vbTpbvrc9B8q8AZO7g/jsbo27Z\n" +
            "d2UfQQRm2ef1IjJ963Ej/zESqd7aZyOJ2f9Z6g7XBpHPWRfZwshA6COLXVebe816\n" +
            "+l8PO+EEoLxroEs/7qUSsvI/KrSdVTJHAy+cvAgMHxhVR55nEPbYXdg+IuLlF/xt\n" +
            "2VCm83GvyvNWnr94LdFXjnIhPw9R9Af2PpC77RKCaRbMfYm/cjkJix8yx5ZrBfo7\n" +
            "nI5IQUabxhj4C1hK9AO8IJfVC2k2FdzeCPAoM9VR89J5ARtDmRjEJkk9rlAPfUXs\n" +
            "gG8bv/yZ29C+PEsVhovUrL7tpjA1hV+49rL2IiiFpuLAUS/KiJMn/tvqNCttBqtr\n" +
            "Jmo+CDzSwb7SDYshPuywSgv+qBA+vhbaHAptN+VvI8ewEmgdbiMX8q3Mr0v+t8pM\n" +
            "LhcXBfX111MUjA==\n" +
            "=5MHI\n" +
            "-----END PGP MESSAGE-----\n";

        public const string PublicKeyEncryptedMessageAes256NoCompression =
            "-----BEGIN PGP MESSAGE-----\n" +
            "\n" +
            "hQEMA5ULd/u5gOShAQf/cnxQQsecobaLHbr/mOuy2RgzRjRlmalCoJj+tw+pb9bX\n" +
            "0fZliQis/Js5t9tQcUQSqDfEQWUOclpWqkDfBoiWOh8Obw6eH8loL2jG0Rfdlu+T\n" +
            "04+V0Fj0MNxJFvP4K3ploO2negViAIz7JlwBPY/x9VsS9IhS2NkqTZjNnN1pCRN6\n" +
            "meHdy+NfjyauAFZYqPPCtGpyrzmlWn2lUe5pCqmdBb59BPksWOTV4Y7E1wZpSMW7\n" +
            "k85CxL3fXGt+2waBS14xYq9TJWh7f067nVlMK1an8SpwR3JVCA9vRPn+M+1eL+gx\n" +
            "nymfqUpSpM6/c2c6Ee/eWF9PnZcEIqCzuf/dNePoz9J4AXebhW2llyw8zeMUpfra\n" +
            "TudSx31QW43cCPuD/2dSlX+dqFbYOkFpx/wnWXl20SXEAOEhidJqrej9z9GtmMHw\n" +
            "7kuDH3cNOwK9vf2ynynjjUxixkvw+7pZTJ3WWyKldT6iXvPYTHX+xJ9UUbvRLdcV\n" +
            "BGGodxdangH3\n" +
            "=0RGy\n" +
            "-----END PGP MESSAGE-----\n";

        public const string PublicKeyEncryptedMessageAes128NoCompression =
            "-----BEGIN PGP MESSAGE-----\n" +
            "\n" +
            "hQEMA5ULd/u5gOShAQf+Or2kT6QAFQH2s/1/hlUdK9CbMkckt/Wmw+IbNSm1HjB0\n" +
            "lmzjmt3Q+v2Lb+/sfdRTKG9fFfJNvUrT5UG5safgM4WcFGo3xcCv8Q8g56prF9fn\n" +
            "ZtmgN4MTz5vi2hMlyrMgxUHNeshYB3vC/CnBdVrUm7jxrTfwRy08F2QTR/xgZpqG\n" +
            "camTJw1EB0qsh4cbJKPdvG1U7BQiFTD3QDVY8zOFca0Ojttb76v6W5zCp66K9j3Z\n" +
            "kqbWqiSzjieXm+wS5o8OsF5xnjrkLbeHmZhyvsgZM985B1PtjLS3uTHFqF9/5COh\n" +
            "VrUxTS9Ftf4vVIPLyjPDRANWOC9hHfRyw+T5yPF+OdJ4AdUZ3a7sBgvaMqJPmUim\n" +
            "9n/3uXnYXYB7o5wyTvGlqsCGmTwztI8ReXaUGBVncQYhFAMyEGzH9saIjVtvoSRc\n" +
            "n0OprI65i9onpEDfVJJYM+LgU/KNOIB6202uaPM7EiiURCwW/ORI4DqU+qcc4lTE\n" +
            "XK09BaV8mp3L\n" +
            "=jyBb\n" +
            "-----END PGP MESSAGE-----\n";

        // passphrase: 123
        // string-to-key iterations: 1024
        // gpg --symmetric --armor --s2k-count 1024 test.txt
        public const string SymmetricallyEncryptedMessage =
            "-----BEGIN PGP MESSAGE-----\n" +
            "\n" +
            "jA0ECQMC0V4B4tm8eOv/0nMBOjcOX9A5lnnQqbKjU3XLdd2inCXVQDl4mZQAvxOY\n" +
            "19BN6KueKQE3PWF1BxSRW8j5ZTIP4+8z3dh/vFb4jKk2pFTpc/IkgV7XczIePtgm\n" +
            "GBsdLUbQ8/GRRNwGu5jDU6sGlJ95/ltjHoa+bWV2Ta/EkzfP\n" +
            "=9aJU\n" +
            "-----END PGP MESSAGE-----\n";
    }
}
