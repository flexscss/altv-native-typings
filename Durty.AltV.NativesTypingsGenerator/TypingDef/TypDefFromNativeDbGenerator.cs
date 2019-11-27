﻿using System.Collections.Generic;
using System.Linq;
using Durty.AltV.NativesTypingsGenerator.Converters;
using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator.TypingDef
{
    public class TypDefFromNativeDbGenerator
    {
        private readonly TypeDef _typeDefinition;

        public TypDefFromNativeDbGenerator(List<TypeDefInterface> interfaces, List<TypeDefType> types, string nativesModuleName)
        {
            _typeDefinition = new TypeDef()
            {
                Interfaces = interfaces,
                Types = types,
                Modules = new List<TypeDefModule>()
                {
                    new TypeDefModule()
                    {
                        Name = nativesModuleName,
                        Functions = new List<TypeDefFunction>()
                    }
                }
            };
        }

        public TypeDef GetTypingDefinition()
        {
            return _typeDefinition;
        }

        public void AddFunctionsFromNativeDb(Models.NativeDb.NativeDb nativeDb)
        {
            TypeDefModule nativesModule = _typeDefinition.Modules.First(m => m.Name == "natives");

            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Graphics));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.System));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.App));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Audio));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Brain));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Cam));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Clock));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Cutscene));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Datafile));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Decorator));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Dlc));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Entity));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Event));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Files));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Fire));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Hud));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Interior));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Itemset));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Loadingscreen));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Localization));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Misc));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Mobile));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Money));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Netshopping));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Network));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Object));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Pad));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Pathfind));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Ped));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Physics));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Player));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Recording));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Replay));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Script));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Shapetest));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Socialclub));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Stats));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Streaming));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Task));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Vehicle));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Water));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Weapon));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Zone));
        }

        private List<TypeDefFunction> GetFunctionsFromNativeGroup(Dictionary<string, Native> nativeGroup)
        {
            NativeTypeToTypingConverter nativeTypeToTypingConverter = new NativeTypeToTypingConverter();
            NativeReturnTypeToTypingConverter nativeReturnTypeToTypingConverter = new NativeReturnTypeToTypingConverter();

            List<TypeDefFunction> functions = new List<TypeDefFunction>();
            foreach (Native native in nativeGroup.Values.Where(native => native.AltFunctionName != string.Empty && native.Hashes != null && native.Hashes.Count != 0))
            {
                //Prepare for docs
                List<string> nativeCommentLines = native.Comment.Split("\n").ToList();
                string foundReturnTypeDescription = string.Empty;
                if (nativeCommentLines.Any(l => l.ToLower().Contains("returns")))
                {
                    foundReturnTypeDescription = nativeCommentLines.FirstOrDefault(l => l.ToLower().Contains("returns"));
                    nativeCommentLines.Remove(foundReturnTypeDescription);
                }
                if (nativeCommentLines.Count > 10) //If native comment is really huge, cut & add NativeDB reference link to read
                {
                    nativeCommentLines = nativeCommentLines.Take(9).ToList();
                    nativeCommentLines.Add($"See NativeDB for reference: http://natives.altv.mp/#/{native.Hashes.First().Value}");
                }
                TypeDefFunction function = new TypeDefFunction()
                {
                    Name = native.AltFunctionName,
                    Description = string.Join("\n", nativeCommentLines),
                    Parameters = native.Parameters.Select(p => new TypeDefFunctionParameter()
                    {
                        Name = p.Name,
                        Type = nativeTypeToTypingConverter.Convert(native, p.NativeParamType)
                    }).ToList(),
                    ReturnType = new TypeDefFunctionReturnType()
                    {
                        Name = nativeReturnTypeToTypingConverter.Convert(native, native.ResultTypes),
                        Description = foundReturnTypeDescription
                    }
                };
                functions.Add(function);
            }

            return functions;
        }
    }
}
