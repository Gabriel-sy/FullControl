﻿using FullControl.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FullControl.Core
{
    public class JsonParser
    {
        private readonly JsonSerializerOptions _options;

        public JsonParser()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, 
            };
        }

        /// <summary>
        /// Converte uma string JSON na estrutura de TelaDefinicao.
        /// </summary>
        /// <param name="jsonString">A string JSON a ser parseada.</param>
        /// <returns>Um objeto TelaDefinicao ou null se o parsing falhar.</returns>
        public T? ParseJsonString<T>(string jsonString)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonString, _options);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro ao parsear JSON: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// (Opcional) Carrega e converte um arquivo JSON.
        /// </summary>
        /// <param name="filePath">O caminho para o arquivo JSON.</param>
        /// <returns>Um objeto TelaDefinicao ou null se o parsing falhar ou o arquivo não for encontrado.</returns>
        public async Task<T?> ParseJsonFileAsync<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Arquivo JSON não encontrado: {filePath}");
                    return default;
                }
                string jsonString = await File.ReadAllTextAsync(filePath);
                return ParseJsonString<T>(jsonString);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Erro ao ler arquivo JSON: {ex.Message}");
                return default;
            }
        }
    }
}

