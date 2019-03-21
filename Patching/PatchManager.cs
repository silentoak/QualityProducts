using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using SilentOak.Patching.Exceptions;
using SilentOak.Patching.Extensions;

namespace SilentOak.Patching
{
    /// <summary>
    /// Helper for managing and applying patches.
    /// </summary>
    public static class PatchManager
    {
        /*************
         * Properties
         *************/

        /// <summary>Gets the underlying patcher.</summary>
        /// <value>The underlying patcher.</value>
        private static HarmonyInstance Patcher { get; } = HarmonyInstance.Create("silentoak.qualityproducts");


        /*****************
         * Public methods
         *****************/

        /// <summary>Applies all the given patches.</summary>
        /// <param name="patches">Patches.</param>
        public static void ApplyAll(params Type[] patches)
        {
            foreach (Type patch in patches)
            {
                Apply(patch);
            }
        }

        /// <summary>Applies the given patch</summary>
        /// <param name="patch"></param>
        public static void Apply(Type patch)
        {
            MethodInfo prefixPatch = patch.GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);
            MethodInfo postfixPatch = patch.GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

            foreach (MethodBase methodToPatch in CalculateMethod(patch))
            {
                Patcher.Patch(
                    original: methodToPatch,
                    prefix: prefixPatch != null ? new HarmonyMethod(prefixPatch) : null,
                    postfix: postfixPatch != null ? new HarmonyMethod(postfixPatch) : null,
                    transpiler: null
                );
            }
        }


        /*******************
         * Internal methods
         *******************/

        /// <summary>Retrieves the original method to be patched by the given patch.</summary>
        /// <returns>The method to be patched.</returns>
        /// <param name="patch">Patch.</param>
        internal static IEnumerable<MethodBase> CalculateMethod(Type patch)
        {
            PatchData patchData = GetPatchData(patch);

            Assembly assemblyCandidate = GetAssemblyByName(patchData.Assembly);

            Version foundAssemblyVersion = assemblyCandidate.GetName().Version;

            try
            {
                if (!foundAssemblyVersion.Match(patchData.AssemblyVersion))
                {
                    throw new DllNotFoundException($"Found version {foundAssemblyVersion}, required {patchData.AssemblyVersion}");
                }
            }
            catch (FormatException e)
            {
                throw new FormatException($"Invalid version expression in ${patch.FullName}.", e);
            }

            foreach (string typeName in patchData.Types)
            {
                Type typeToPatch = assemblyCandidate.GetType(typeName, true);

                MethodBase methodToPatch;
                if (patchData.OriginalMethod == ".ctor")
                {
                    methodToPatch = AccessTools.Constructor(typeToPatch, patchData.OriginalMethodParams);
                }
                else if (patchData.OriginalMethodParams == null)
                {
                    methodToPatch = AccessTools.Method(typeToPatch, patchData.OriginalMethod);
                }
                else
                {
                    methodToPatch = AccessTools.Method(typeToPatch, patchData.OriginalMethod, patchData.OriginalMethodParams);
                }

                if (methodToPatch == null)
                {
                    throw new MissingMethodException(typeName, patchData.OriginalMethod);
                }

                yield return methodToPatch;
            }

            yield break;
        }

        /// <summary>Gets the patch data for the given patch</summary>
        /// <returns>The patch data.</returns>
        /// <param name="patch">Patch.</param>
        internal static PatchData GetPatchData(Type patch)
        {
            PatchData patchData = patch.GetCustomAttribute<PatchData>();
            if (patchData == null)
            {
                throw new MissingAttributeException(patch.FullName, "PatchData");
            }

            return patchData;
        }

        /// <summary>Gets the loaded assembly with the given name.</summary>
        /// <returns>The assembly.</returns>
        /// <param name="assemblyName">Assembly name.</param>
        internal static Assembly GetAssemblyByName(string assemblyName)
        {
            IEnumerable<Assembly> loadedMatchingAssemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name == assemblyName);

            if (!loadedMatchingAssemblies.Any())
            {
                throw new DllNotFoundException($"Could not find {assemblyName}.");
            }

            if (loadedMatchingAssemblies.Count() > 1)
            {
                throw new AmbiguousMatchException($"Found multiple {assemblyName}.");
            }

            return loadedMatchingAssemblies.First();
        }
    }
}
