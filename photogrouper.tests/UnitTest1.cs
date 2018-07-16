using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoGrouper;
using System.Linq;

namespace photogrouper.tests
{
    [TestClass]
    public class PhotoFileProcessorUnitTest
    {
        private void DeleteDirectory(string directory)
        {
            foreach (var dir in Directory.EnumerateDirectories(Path.Combine("./images", directory)))
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(dir))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception fe)
                        {
                            Console.WriteLine("Failed to clean up image: {0}", fe.Message);
                        }
                    }
                    Directory.Delete(dir);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to clean up folder: {0}", e.Message);
                }
            }
        }


        private static readonly string[] groupByResult = new string[]
        {
        ".\\images\\2018-02-18\\20180218_142130.jpg"
        ,".\\images\\2018-02-18\\20180218_142130.jpg"
        ,".\\images\\2018-02-18\\20180218_142132.jpg"
        ,".\\images\\2018-02-18\\20180218_142139.jpg"
        ,".\\images\\2018-02-18\\20180218_142140.jpg"
        ,".\\images\\2018-02-18\\20180218_142209.jpg"
        ,".\\images\\2018-02-22\\20180222_074357.jpg"
        ,".\\images\\2018-02-22\\20180222_074543.dng"
        ,".\\images\\2018-02-22\\20180222_074543.jpg"
        ,".\\images\\2018-02-22\\20180222_074557.dng"
        ,".\\images\\2018-02-22\\20180222_074557.jpg"
        ,".\\images\\2018-02-26\\20180226_134852.jpg"
        ,".\\images\\2018-02-27\\20180227_185321.jpg"
        ,".\\images\\2018-02-27\\20180227_185325.jpg"
        ,".\\images\\2018-02-27\\20180227_185330.jpg"
        ,".\\images\\2018-02-27\\20180227_185355.jpg"
        ,".\\images\\2018-02-27\\20180227_185401.jpg"
        ,".\\images\\2018-02-27\\20180227_185440.jpg"
        ,".\\images\\2018-02-27\\20180227_185456.jpg"
        ,".\\images\\2018-02-27\\20180227_190921.jpg"
        ,".\\images\\2018-02-27\\20180227_190935.jpg"
        ,".\\images\\2018-02-27\\20180227_190941.jpg"
        ,".\\images\\2018-03-01\\20180301_155220.jpg"
        ,".\\images\\2018-03-01\\20180301_155233.jpg"
        ,".\\images\\2018-03-01\\20180301_155240.jpg"
        ,".\\images\\2018-03-01\\20180301_155256.jpg"
        ,".\\images\\2018-03-01\\20180301_155307.jpg"
        ,".\\images\\2018-03-01\\20180301_155309.jpg"
        ,".\\images\\2018-03-01\\20180305_085331.jpg"
        ,".\\images\\2018-03-05\\20180305_130456_001.jpg"
        ,".\\images\\2018-03-05\\20180305_130457.jpg"
        ,".\\images\\2018-03-05\\20180305_130502.jpg"
        ,".\\images\\2018-03-05\\20180305_131015.jpg"
        ,".\\images\\2018-03-05\\20180305_131019.jpg"
        ,".\\images\\2018-03-07\\20180307_120107.jpg"
        ,".\\images\\2018-03-07\\20180307_120108.jpg"
        ,".\\images\\2018-03-07\\20180307_120131.dng"
        ,".\\images\\2018-03-07\\20180307_120131.jpg"
        ,".\\images\\2018-03-07\\20180307_120132.dng"
        ,".\\images\\2018-03-07\\20180307_120132.jpg"
        ,".\\images\\2018-03-07\\20180307_120134.dng"
        ,".\\images\\2018-03-07\\20180307_120134.jpg"
        ,".\\images\\2018-03-07\\20180307_120136.dng"
        ,".\\images\\2018-03-07\\20180307_120136.jpg"
        ,".\\images\\2018-03-07\\20180307_120137.dng"
        ,".\\images\\2018-03-07\\20180307_120137.jpg"
        ,".\\images\\2018-03-08\\20180308_090159.jpg"
        };

        [TestMethod]
        public void GroupByMatchsResults()
        {
            DeleteDirectory("GroupByMatchsResults");

            CopyImagesToDirectory("GroupByMatchsResults");

            var processor = new PhotoFileProcessor(new EmptyLogger());

            var files = processor.GetFilesSync("./images");

            files = files.GroupBy(x => x.Date.ToString("yyyy-MM-dd")).Confirm().Result;

            foreach (var file in groupByResult)
            {
                // Ensuring the files are moved correctly.
                Assert.IsTrue(File.Exists(file), "File not found: {0}", file);
            }

            files.UnGroup().Confirm();
        }
    }
}
