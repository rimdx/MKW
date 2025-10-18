// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using NUnit.Framework.Legacy;

namespace MKW.Common.Tests
{
    [TestFixture]
    public class TempFileTests
    {
        [Test]
        public void SimpleAccept()
        {
            string path = Path.GetTempFileName();
            using TempFile file = TempFile.Create(path);

            FileAssert.Exists(path);

            byte[] bytes = [1, 2, 3];
            file.Write(bytes);
            FileAssert.Exists(path);

            file.Accept();
            FileAssert.Exists(path);
        }

        [Test]
        public void SimpleAcceptReplace()
        {
            string path = Path.GetTempFileName();

            byte[] obytes = [1, 2, 3];
            byte[] nbytes = [4, 5, 6];

            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate))
            {
                file.Write(obytes);
            }

            using (TempFile file = TempFile.Create(path))
            {
                byte[] buffer = new byte[3];

                CollectionAssert.AreEqual(obytes, File.ReadAllBytes(path));

                file.Write(nbytes);
                CollectionAssert.AreEqual(obytes, File.ReadAllBytes(path));

                file.Accept();
                CollectionAssert.AreEqual(nbytes, File.ReadAllBytes(path));
            }
        }

        [Test]
        public void SimpleRejectReplace()
        {

            string path = Path.GetTempFileName();

            byte[] obytes = [1, 2, 3];
            byte[] nbytes = [4, 5, 6];

            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate))
            {
                file.Write(obytes);
            }

            using (TempFile file = TempFile.Create(path))
            {
                byte[] buffer = new byte[3];

                CollectionAssert.AreEqual(obytes, File.ReadAllBytes(path));

                file.Write(nbytes);
                CollectionAssert.AreEqual(obytes, File.ReadAllBytes(path));

                file.Reject();
                CollectionAssert.AreEqual(obytes, File.ReadAllBytes(path));
            }
        }
    }
}
