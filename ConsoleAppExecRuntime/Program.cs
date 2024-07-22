

// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.Linq;

using System;
using System.Text.Json;
using ExtracaoCompiladorExecucao.Compiler;


//// Exemplo de string JSON
string jsonString = @"[
    {""valor"": 10},
    {""valor"": 20},
    {""valor"": 30}
]";

//// Deserializar a string JSON para uma lista de dicionários
//var objetos = JsonSerializer.Deserialize<List<Dictionary<string, double>>>(jsonString);

//// Usar LINQ para somar os valores da propriedade "valor"
//double soma = objetos.Sum(obj => obj["valor"]);

//Console.WriteLine($"A soma dos valores é: {soma}");

string code = @"

            using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Text.Json;

                    public object? ObterResultadoJson(string json)
                    {
                        var objetos = JsonSerializer.Deserialize<List<Dictionary<string, double>>>(json);
                        
                        double soma = objetos.Sum(obj => obj[""valor""]);

                        return soma;
                    }
            ";
var compiler = new Compiler();
var result = await compiler.CompileAsync(code);

var resultado = result.ObterResultadoJson(jsonString);
if (!resultado.Item2.Any())
{
    Console.WriteLine($"A soma dos valores é: {resultado}");
}
else
{
    foreach (var item in resultado.Item2)
    {
        Console.WriteLine(item);
    }
}


