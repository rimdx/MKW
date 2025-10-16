namespace MKW.Storage.Tests
{
    public static class PgpTestKeys
    {
        // RSA 2048, no passprase
        // generated with `gpg --full-generate-key`
        // export with: `gpg --export --armor`
        // for private keys: `gpg --export-secret-subkeys --armor`

        public static readonly string TestPublicKey =
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

        public static readonly string TestPrivateKey =
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

        public static readonly string PublicKeyEncryptedMessage =
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

        // passphrase: 123
        public static readonly string SymmetricallyEncryptedMessage =
            "-----BEGIN PGP MESSAGE-----\n" +
            "\n" +
            "hQEMA5ULd/u5gOShAQgAwHH6nDtY7nsRu/Do3BLDvptv7QczZvplrZnJWTItL+Aa\n" +
            "i59EAn2Srm04dVhraN5tu72Ger43RTCPjTHWc5P4HyznValyppzT70E5BdRvm5Ve\n" +
            "ZlfVXyhbNW3zlbN7J1KboTktIvuBtZBzfiKaYhZzlF/lQjBLFhpNNBZT/5dAGTuO\n" +
            "6GmWADtIeZ08fceOrNy3HKR19V338jEiZ+UHYOJ4fuyex2CiLjMjGw9b7eizW6yF\n" +
            "bCZ/iwJjLndqF3Irfu6VeEWNOeh5kcgg+ieW3LW00ctvHU66nfwuGlEFwWAZC/Qt\n" +
            "LDiRGs2QjqHNVAwU+Bm7Wibx+NBes1RdfCQO0mJSGowuBAkDAmZA5L4OHMy1/ybq\n" +
            "QHaZo//um20clWmexHVOHsQMYUpIAG4512xXV8k5j9J5AUA46xMxhLMKkT72El1E\n" +
            "v3H3OscCpc0AUD4pEQl34PuE2jC9+Jn+578P9NV9Lr3XjOEsCXmQuDn19+cijWn+\n" +
            "PqTUHQaLinE5pK78C6F0dXHYNh2J7U06Rgxj/C2TvG+sCCEMLYamR/IpIoIPXT/M\n" +
            "Ts3wT4gnZkv2fg==\n" +
            "=itNI\n" +
            "-----END PGP MESSAGE-----\n";
    }
}
