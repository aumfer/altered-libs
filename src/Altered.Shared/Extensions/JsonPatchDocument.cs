using Altered.Shared.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altered.Shared.Extensions
{
    public static class JsonPatchExtensions
    {
        static string JsonPathToPointer(string path) => path.TrimStart('/').Replace('/', '.').Replace('[', '.').Replace(']', '.');

        public static string FindStartsWith(string path, string startsWith)
        {
            if (path.StartsWith(startsWith))
            {
                var rest = path.Substring(startsWith.Length);
                return rest;
            }
            return null;
        }

        // todo arrays
        public static JObject AlteredApplyPatch(this JsonPatchDocument jsonPatchDocument, JObject applyTo)
        {
            jsonPatchDocument.ContractResolver = new NoNullContractResolver();
            var patchedObject = jsonPatchDocument.Operations.Aggregate(applyTo, (obj, op) =>
            {
                var pointer = JsonPathToPointer(op.path);

                switch (op.OperationType)
                {
                    case OperationType.Add:
                    case OperationType.Replace:
                        var next = JToken.FromObject(op.value);
                        obj.ReplacePath(pointer, next);
                        break;
                    case OperationType.Remove:
                        obj.ReplacePath(pointer, null).Parent.Remove();
                        break;
                    case OperationType.Copy:
                        var fromPointer = JsonPathToPointer(op.from);
                        var from = obj.SelectToken(fromPointer);
                        obj.ReplacePath(pointer, from);
                        break;
                }
                return obj;
            });

            return patchedObject;
        }

        public static Dictionary<string, JsonPatchDocument> FlattenOnce(this JsonPatchDocument jsonPatchDocument)
        {
            var subPatches = jsonPatchDocument.Operations.GroupBy(op =>
            {
                var path = op.path.TrimStart('/');
                var idx = path.IndexOf('/');
                if (idx != -1)
                {
                    return path.Substring(0, idx);
                }
                return String.Empty;
            }).ToDictionary(g => g.Key, g => new JsonPatchDocument(g.ToList(), new DefaultContractResolver()));
            return subPatches;
        }

        // https://ghe.coxautoinc.com/Cox-CarVim/inventory-management-service/blob/develop/app/Domains/UIComponent/CoxAuto.Vince.InventoryManagementService.Audit/JsonPatchHelper.cs
        public static JsonPatchDocument CreatePatch(this JsonPatchDocument patch, JObject orig, JObject mod, string path = "/")
        {
            var origNames = orig.Properties().Select(x => x.Name).ToList();
            var modNames = mod.Properties().Select(x => x.Name).ToArray();

            // Names added or modified in modified
            foreach (var k in modNames)
            {
                // Add
                if (!origNames.Remove(k))
                {
                    var prop = mod.Property(k);
                    patch.Operations.Add(new Operation("add", path + prop.Name, null, prop.Value.ToObject<object>()));
                }

                // Replace
                else
                {
                    var origProp = orig.Property(k);
                    var modProp = mod.Property(k);

                    if (origProp.Value.Type != modProp.Value.Type)
                    {
                        patch.Operations.Add(new Operation("replace", path + modProp.Name, null, modProp.Value.ToObject<object>()));
                    }
                    else if (!JToken.DeepEquals(origProp.Value, modProp.Value))
                    {
                        if (origProp.Value.Type == JTokenType.Object)
                        {
                            // Recurse into objects
                            patch.CreatePatch(origProp.Value as JObject, modProp.Value as JObject, path + modProp.Name + "/");
                        }
                        else
                        {
                            // Replace values directly
                            patch.Operations.Add(new Operation("replace", path + modProp.Name, null, modProp.Value.ToObject<object>()));
                        }
                    }
                }
            }

            // Names removed in modified
            foreach (var k in origNames)
            {
                var prop = orig.Property(k);
                patch.Operations.Add(new Operation("remove", path + prop.Name, null));
            }

            return patch;
        }

        public static JsonPatchDocument WithContractResolver(this JsonPatchDocument d, IContractResolver cr)
        {
            d.ContractResolver = cr;
            return d;
        }
        public static JsonPatchDocument<T> WithContractResolver<T>(this JsonPatchDocument<T> d, IContractResolver cr)
            where T : class
        {
            d.ContractResolver = cr;
            return d;
        }
    }
}
