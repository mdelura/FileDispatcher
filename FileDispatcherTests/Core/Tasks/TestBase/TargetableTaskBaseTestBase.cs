using System;
using System.IO;
using FileDispatcher;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Core.Tasks.TestBase
{
    [TestClass]
    [Serializable]
    public abstract class TargetableTaskBaseTestBase<TTargetableTask> : TaskBaseTestBase<TTargetableTask> 
        where TTargetableTask : TargetableTaskBase
    {
        public const string TargetFolder = "Target";

        /// <summary>
        /// Target folder path to be used as a target for tested <see cref="TTargetableTask"/>
        /// </summary>
        public string TargetFolderPath { get; private set; }

        /// <summary>
        /// An instance of default <see cref="TargetRouter"/>
        /// </summary>
        internal TargetRouter TargetRouter { get; private set; }

        [TestInitialize]
        public override void TestInit()
        {
            base.TestInit();
            TargetRouter = new TargetRouter();
            TargetRouter.BaseTargetPath = TargetFolderPath;
            task.TargetRouter = TargetRouter;
        }

        //TODO: Needs to be implemented for folders
        //[TestMethod]
        public void TargetExists_()
        {
            //Arrange

            //Act

            //Assert
            Assert.Fail("Test not implemented");
        }

        protected override void PrepareTestItems()
        {
            base.PrepareTestItems();
            TargetFolderPath = Directory.CreateDirectory(Path.Combine(MainTestDirectory, TargetFolder)).FullName;
        }
    }
}
