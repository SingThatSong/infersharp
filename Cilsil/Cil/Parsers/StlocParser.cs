﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Cilsil.Sil;
using Cilsil.Sil.Expressions;
using Cilsil.Sil.Instructions;
using Cilsil.Sil.Types;
using Cilsil.Utils;
using Mono.Cecil.Cil;

namespace Cilsil.Cil.Parsers
{
    internal class StlocParser : InstructionParser
    {
        protected override bool ParseCilInstructionInternal(Instruction instruction,
                                                            ProgramState state)
        {
            int index;
            CfgNode node = null;
            switch (instruction.OpCode.Code)
            {
                case Code.Stloc_0:
                    index = 0;
                    break;
                case Code.Stloc_1:
                    index = 1;
                    break;
                case Code.Stloc_2:
                    index = 2;
                    break;
                case Code.Stloc_3:
                    index = 3;
                    break;
                case Code.Stloc_S:
                    index = (instruction.Operand as VariableDefinition).Index;
                    break;
                case Code.Stloc:
                    index = (int)instruction.Operand;
                    break;
                default:
                    return false;
            }

            (var value, var type) = state.Pop();

            // Records that the variable stores a boxed value.
            if (type is BoxedValueType boxedValueType)
            {
                state.VariableIndexToBoxedValueType[index] = boxedValueType;
            }
            // A non-boxed value is being stored at the location, so the corresponding record is
            // accordingly updated.
            else if (!(type is BoxedValueType) &&
                     state.VariableIndexToBoxedValueType.ContainsKey(index))
            {
                state.VariableIndexToBoxedValueType.Remove(index);
            }

            var variable = new LocalVariable(LocalName(index), state.Method);
            var storeValueIntoVariable = new Store(new LvarExpression(variable),
                                                    value,
                                                    type,
                                                    state.CurrentLocation);
            node = AddMethodBodyInstructionsToCfg(state, storeValueIntoVariable);
            RegisterLocalVariable(state, variable, type);
            state.PushInstruction(instruction.Next, node);
            return true;
        }
    }
}
