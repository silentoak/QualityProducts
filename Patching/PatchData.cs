using System;

namespace SilentOak.Patching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PatchData : Attribute
    {
        /// <summary>The assembly to patch.</summary>
        public readonly string Assembly;

        /// <summary>The version to patch.</summary>
        public readonly string AssemblyVersion;

        /// <summary>The type(s) to patch.</summary>
        public readonly string[] Types;

        /// <summary>The method to patch.</summary>
        public readonly string OriginalMethod;

        /// <summary>The method parameter types.</summary>
        public readonly Type[] OriginalMethodParams;

        public PatchData(
            string assembly,
            string assemblyVersion,
            string type,
            string originalMethod,
            params Type[] originalMethodParams
        ) : this(assembly, assemblyVersion, new string[] { type }, originalMethod, originalMethodParams )
        {
        }

        public PatchData(
            string assembly,
            string assemblyVersion,
            string[] types,
            string originalMethod,
            params Type[] originalMethodParams
        )
        {
            Assembly = assembly;
            AssemblyVersion = assemblyVersion;
            Types = types;
            OriginalMethod = originalMethod;
            OriginalMethodParams = originalMethodParams;
        }
    }
}