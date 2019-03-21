using System;
using System.Reflection;
using NUnit.Framework;
using SilentOak.Patching.Exceptions;

namespace SilentOak.Patching.Tests
{
    /// <summary>
    /// Patch manager tests.
    /// </summary>
    [TestFixture]
    public class PatchManagerTests
    {
        /// <summary>
        /// Ensures that <see cref="PatchManager.GetAssemblyByName(string)"/> returns this assembly when given its name. 
        /// </summary>
        [TestCase]
        public void Test_GetAssemblyByName_ThisAssembly()
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            Assembly foundAssembly = PatchManager.GetAssemblyByName(thisAssembly.GetName().Name);
            Assert.AreEqual(thisAssembly, foundAssembly);
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.GetAssemblyByName(string)"/> throws on non-existent assembly.
        /// </summary>
        [TestCase]
        public void Test_GetAssemblyByName_NoAssembly()
        {
            Assert.Throws<DllNotFoundException>(() => PatchManager.GetAssemblyByName("null"));
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.GetPatchData(Type)"/> throws on invalid patch class.
        /// </summary>
        [TestCase]
        public void Test_GetPatchData_NoPatchData()
        {
            Assert.Throws<MissingAttributeException>(() => PatchManager.GetPatchData(typeof(int)));
        }


        [PatchData(
            assembly: "PatchTest",
            assemblyVersion: "1.0.0",
            type: "PatchTest.Test",
            originalMethod: "NoTest"
            )]
        public static class PatchTest1
        {
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.GetPatchData(Type)"/> retrieves correctly the PatchData for a patch class.
        /// </summary>
        [TestCase]
        public void Test_GetPatchData_HadPatchData()
        {
            PatchData patchData = new PatchData(
                assembly: "PatchTest",
                assemblyVersion: "1.0.0",
                type: "PatchTest.Test",
                originalMethod: "NoTest"
                );

            PatchData testPatchData = typeof(PatchTest1).GetCustomAttribute<PatchData>();

            PatchData retrievedPatchData = PatchManager.GetPatchData(typeof(PatchTest1));

            Assert.AreEqual(retrievedPatchData, testPatchData);
            Assert.AreEqual(patchData.Assembly, retrievedPatchData.Assembly);
            Assert.AreEqual(patchData.AssemblyVersion, retrievedPatchData.AssemblyVersion);
            Assert.AreEqual(patchData.Type, retrievedPatchData.Type);
            Assert.AreEqual(patchData.OriginalMethod, retrievedPatchData.OriginalMethod);
            Assert.AreEqual(patchData.OriginalMethodParams, retrievedPatchData.OriginalMethodParams);
        }


        [PatchData(
            assembly: "NoAssembly",
            assemblyVersion: "*",
            type: "Anything",
            originalMethod: "Anything"
            )]
        public static class PatchTest2
        {
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.CalculateMethod(Type)"/> throws when the assembly is not found.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_DllMissing()
        {
            Assert.Throws<DllNotFoundException>(() => PatchManager.CalculateMethod(typeof(PatchTest2)));
        }


        [PatchData(
            assembly: "Patching",
            assemblyVersion: "0.1.0",
            type: "Anything",
            originalMethod: "Anything"
            )]
        public static class PatchTest3
        {
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.CalculateMethod(Type)"/> throws when the assembly has the wrong version.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_DllWrongVersion()
        {
            Assert.Throws<DllNotFoundException>(() => PatchManager.CalculateMethod(typeof(PatchTest3)));
        }


        [PatchData(
            assembly: "Patching",
            assemblyVersion: "*",
            type: "NoType",
            originalMethod: "Anything"
            )]
        public static class PatchTest4
        {
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.CalculateMethod(Type)"/> throws when the type is not found.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_TypeMissing()
        {
            Assert.Throws<TypeLoadException>(() => PatchManager.CalculateMethod(typeof(PatchTest4)));
        }


        [PatchData(
            assembly: "Patching",
            assemblyVersion: "*",
            type: "SilentOak.Patching.Tests.PatchManagerTests+PatchTest5",
            originalMethod: "NoMethod"
            )]
        public static class PatchTest5
        {
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.CalculateMethod(Type)"/> throws when the method is not found.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_MethodMissing()
        {
            Assert.Throws<MissingMethodException>(() => PatchManager.CalculateMethod(typeof(PatchTest5)));
        }


        [PatchData(
            assembly: "Patching",
            assemblyVersion: "*",
            type: "SilentOak.Patching.Tests.PatchManagerTests+PatchTest6",
            originalMethod: "Test",
            originalMethodParams: new Type[] { typeof(int) }
            )]
        public static class PatchTest6
        {
            public static void Test(int dummy)
            {
                return;
            }
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.CalculateMethod(Type)"/> successfully retrieves the correct method.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_Success()
        {
            MethodInfo method = typeof(PatchTest6).GetMethod("Test", BindingFlags.Public | BindingFlags.Static);

            Assert.AreEqual(method, PatchManager.CalculateMethod(typeof(PatchTest6)));
        }
    }
}