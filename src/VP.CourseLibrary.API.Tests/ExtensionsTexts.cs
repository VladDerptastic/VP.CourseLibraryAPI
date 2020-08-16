using Microsoft.VisualStudio.TestTools.UnitTesting;
using VP.CourseLibrary.Tools;

namespace VP.CourseLibrary.API.Tests
{
    [TestClass]
    public class ExtensionsTexts
    {
        [TestMethod]
        public void GenerateSha256Test()
        {
            var keyword = "totallyNotAdmin";
            //using https://passwordsgenerator.net/sha256-hash-generator/
            var outsideToolGeneratedHash = "4BF764B13B6361EB6FBEB56DE0A3152E02AFA040FB0E8770E9B76FAB2616CE36";

            var selfGeneratedHash = keyword.AsSha256();
            Assert.AreEqual(outsideToolGeneratedHash.ToLower(), selfGeneratedHash);
        }
    }
}
