using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtracaoCompiladorExecucao.Compiler
{
    public class CompilationResult
    {
        private MethodInfo obterResultadoJsonMethod = null;
        private object instance = null;

        public IEnumerable<CompilationMessage> CompilationMessages { get; set; }

        public Type ScriptType { get; set; }

        public bool HasErrors => CompilationMessages.Any(m => m.IsError);

        public Tuple<object?, IEnumerable<string>> ObterResultadoJson(string json)
        {
            var messages = new List<string>();

            if (ScriptType != null && obterResultadoJsonMethod == null)
            {
                obterResultadoJsonMethod = ScriptType.GetMethod(
                     "ObterResultadoJson",
                     BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                     null,
                     new[] { typeof(string) },
                     null);

                if (obterResultadoJsonMethod == null)
                {
                    messages.Add("Unable to find entry method 'object? ObterResultadoJson(string json)'.");
                }
                else if (!obterResultadoJsonMethod.IsStatic)
                {
                    instance = Activator.CreateInstance(ScriptType, new[] { new object[2] });
                }
            }
            object? resultado = null;
            try
            {
                resultado = obterResultadoJsonMethod?.Invoke(instance, new object[] { json });
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException tiex)
                    ex = tiex.InnerException;

                messages.Add($"An error occured during execution: {ex.Message}");
            }

            return new Tuple<object?, IEnumerable<string>>(resultado, messages);
        }
    }
}
